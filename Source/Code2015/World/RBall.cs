﻿using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Animation;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.Logic;
using Apoc3D.Collections;

namespace Code2015.World
{
    enum RBallState
    {
        Standby,
        Born,
        Gathering,
        BeginingAttackCity,
        Attack,
        AttackCity,        
        Defend,
        Gathered,
        Free,
    }

    /// <summary>
    ///  大球只进行一次跳跃
    /// </summary>
    class RGatheredBall
    {
        const float GatherPositionRadius = City.CityRadius * 0.75f;
        const float GatherPositionHeight = 250;

        const float GBallRadius = 75;

        public enum State
        {
            WaitingGathering,
            WaitingThrow,
            Flying,
            Finished
        }


        List<City> goPath;
        State state;
        List<Vector3> ballOffsets;
        List<RBall> subBalls;
        Vector3 position;
        City sourceCity;

        Vector3 normal;

        float targetAngle;

        #region FLY
        Vector3 srcPosition;
        Vector3 dstPosition;
        float flyProgress;
        bool notifyedDest;
        City destCity;
        
        Vector3 flyRoundCenter;
        #endregion

        public List<RBall> Balls { get { return subBalls; } }

        public City FollowingCity 
        {
            get
            {
                if (goPath.Count >= 2)
                    return goPath[1];
                return null;
            }
        }
        public List<City> GetRemainingPath() 
        {
            if (goPath.Count >= 2)
            {
                List<City> result = new List<City>(goPath.Count - 1);
                for (int i = 1; i < goPath.Count; i++) 
                {
                    result.Add(goPath[i]);
                }
                return result;
            }
            return null;
        }
        public Vector3 Position
        {
            get { return position; }
        }
        public State CurrentState
        {
            get { return state; }
        }

        public bool IsOver 
        {
            get { return state == State.Finished; }
        }
        public bool IsPathFinished
        {
            get { return goPath.Count <= 1; }
        }

        /// <summary>
        ///  释放所有资源球
        /// </summary>
        void ReleaseBalls() 
        {
            City cc = goPath[0];

            for (int i = 0; i < subBalls.Count; i++)
            {
                subBalls[i].Free(cc);
            }
        }

        void ReleaseBallGathering() 
        {
            
            for (int i = 0; i < subBalls.Count; i++)
            {
                subBalls[i].Free(sourceCity);
            }
        }

        /// <summary>
        ///  计算在一定方向上，城市手中的RG球位置
        /// </summary>
        /// <param name="city"></param>
        /// <param name="ori"></param>
        /// <returns></returns>
        static Vector3 GetRGBallPosition(City city, Quaternion ori)
        {
            Vector4 dir = Vector3.Transform(Vector3.UnitZ, ori);

            Vector3 offset = new Vector3(dir.X, dir.Y, dir.Z);
            offset *= GatherPositionRadius;
            offset.Y += GatherPositionHeight;

            offset = Vector3.TransformNormal(offset, city.Transformation);
            
            return city.Position + offset;
        }

        public RGatheredBall(List<RBall> balls, City dockCity, List<City> path)
        {
            state = State.WaitingGathering;
            subBalls = balls;
            goPath = path;
            sourceCity = dockCity;


            position = GetRGBallPosition(dockCity, dockCity.CurrentFacing);
            
            int lineCount = (int)Math.Ceiling(Math.Sqrt(balls.Count));
            float span = (MathEx.PIf * 2) / lineCount;

            ballOffsets = new List<Vector3>(balls.Count);
            for (int i = 0; i < balls.Count; i++)
            {
                int row = i / lineCount;
                int col = i % lineCount;

                float radLng = col * span;
                float radLat = row * span;

                Vector3 positionInGBall = PlanetEarth.GetPosition(radLng, radLat, GBallRadius);
                Quaternion oriInGBall = Quaternion.RotationMatrix(PlanetEarth.GetOrientation(radLng, radLat));
                ballOffsets.Add(positionInGBall);
                balls[i].Gather(this, positionInGBall + position, oriInGBall);
            }
        }

        public void Cancel()
        {
            switch (state)
            {
                case State.WaitingGathering:
                    {
                        ReleaseBallGathering();
                        state = State.Finished;
                        goPath.Clear(); 
                        break;
                    }                
            }
         
        }
        public void NotifyThrow()
        {
            ChangeState(State.Flying);
        }

        void ChangeState(State newState)
        {
            if (newState == State.Flying)
            {
                for (int i = 0; i < subBalls.Count; i++)
                {
                    subBalls[i].SetDockCity(null);
                }

                srcPosition = position;


                City dstCity = goPath[0];
                this.destCity = dstCity;


                if (dstCity.Type == CityType.Health)
                {
                    targetAngle = MathEx.PiOver4;
                }
                else
                {
                    targetAngle = MathEx.PIf / 6.0f;
                }

                Quaternion newDstOri = dstCity.GetOrientation(srcPosition);

                normal = sourceCity.Transformation.Up + dstCity.Transformation.Up;
                normal.Normalize();

                dstPosition = GetRGBallPosition(dstCity, newDstOri);

                flyRoundCenter = GetFlyCenter(dstCity.Transformation.Up);
                notifyedDest = false;
                flyProgress = 0;
            }
            else if (newState == State.Finished) 
            {
                ReleaseBalls();
            }

            state = newState;
        }


        Vector3 GetFlyCenter(Vector3 n)
        {
            Vector3 d = dstPosition - srcPosition;
            
            Vector3 r = Vector3.Cross(d, n);
            r = Vector3.Cross(r, d);
            r.Normalize();
            Vector3 cp = dstPosition + srcPosition;
            cp *= 0.5f;
            return cp - r * (float)Math.Tan(MathEx.PiOver2 - targetAngle) * d.Length() * 0.5f;
        }
        void UpdateFly(GameTime time)
        {
            //Vector3 r = Vector3.Lerp(srcPosition, dstPosition, flyProgress) - flyRoundCenter;
            //r.Normalize();
            //r *= (dstPosition - srcPosition).Length() * 0.5f;

            position = Vector3.Lerp(srcPosition, dstPosition, flyProgress);

            flyProgress += time.ElapsedGameTimeSeconds * 0.5f;


            float timeStamp = destCity.CatchPreserveTime;

            timeStamp = 1.5f - timeStamp;

            if ((flyProgress / 0.5f) > timeStamp)
            {
                if (!notifyedDest)
                {
                    destCity.NotifyIncomingBall(this);
                    notifyedDest = true;
                }
            }

            {
                float t = flyProgress * 2;
                const float Gr = 1000;
                float h;
                if (t < 1)
                {
                    h = Gr * t - 0.5f * (Gr * t * t);
                }
                else
                {
                    h = 0.5f * Gr - 0.5f * Gr * MathEx.Sqr(t - 1);
                }
                
                position += normal * h;
            }

            if (flyProgress > 1)
            {
                ChangeState(State.Finished);
            }

        }
        public void Update(GameTime time)
        {
            switch (state)
            {
                case State.WaitingGathering:
                    {
                        bool passed = true;
                        for (int i = 0; i < subBalls.Count; i++)
                        {
                            if (subBalls[i].State != RBallState.Gathered)
                            {
                                passed = false;
                                break;
                            }
                        }

                        if (passed)
                        {
                            ChangeState(State.WaitingThrow);
                        }
                        break;
                    }
                case State.Flying:
                    {
                        UpdateFly(time);
                        for (int i = 0; i < subBalls.Count; i++) 
                        {
                            subBalls[i].Position = ballOffsets[i] + position;
                        }
                        break;
                    }

            }

        }

        
    }

    /// <summary>
    ///  表示资源球
    /// </summary>
    class RBall : WorldDynamicObject
    {
        public struct Props
        {
            public float Damage;
            public float Contribution;
            public float Heal;

            public float MaxHealth;
        }

        const float EnemyCheckTime = 0.75f;
        const float WeaponCoolDownTime = 1;

        const float MinRadius = City.CityRadius * 0.8f;
        const float MaxRadius = City.CityRadius * 1.5f;
        const float EduMaxRadius = City.CityRadius * 1.9f;

        const float MinSpeedMod = 1;
        const float GatherSpeedMod = 3.5f;
        const float AttackCitySpeedMod = 7.0f;
        const float DefenceSpeedMod = 3.5f;
        const float AttackSpeedMod = 2.5f;

        const float MaxLinSpeed = 233;
        const float MinLinSpeed = 190;

        const float CitySafeRadius = 193;
        const float CityAttackRadius = 150;



        const float MoveDistanceThreshold = 50;

        const float AttackRange = 100;

        /// <summary>
        ///  高生命值球的现实比例系数
        /// </summary>
        const float RBallHealthScale = 0.01f;

        public RBallType Type { get; private set; }

        private City dockCity;
        public City DockCity { get { return this.dockCity; } }

        RBallState state;

        readonly Props props;

        float weaponCoolDown;
        float enemyCheckTime;

        float speedModifierTarget = 1;
        float speedModifier = 1;

        Matrix displayScale;

        #region State_NewMove
        
        bool isMoving;

        Vector3 moveTarget;

        float rotationLerp;
        #endregion


        RGatheredBall gatheredParent;

        
        #region State_Attack

        RBall target;

        #endregion


        #region Standby
        /// <summary>
        /// 在城市上方旋转半径
        /// </summary>
        private float currentRadius;
        private float currentHeight;

        /// <summary>
        /// 旋转角度
        /// </summary>
        private float roundAngle;
        #endregion


        float refrenceLinSpeed;

        public RBallState State
        {
            get { return state; }
        }
        //是否被消灭
        public bool IsDied { get { return Health <= 0; } }

        //血条
        public float Health { get; private set; }
        public float MaxHealth { get { return props.MaxHealth; } }

        public void SetDockCity(City c)
        {
            if (c != dockCity)
            {
                if (c != null)
                {
                    c.NotifyResourceBallMoveIn(this);
                }
                if (dockCity != null)
                {
                    dockCity.NotifyResourceBallMoveOut(this);
                }
                dockCity = c;
            }
        }

        float NextRadius()
        {
            if (Type == RBallType.Disease || Type == RBallType.Disease) 
            {
                return CitySafeRadius;
            }
            else if (Type == RBallType.Education || Type == RBallType.Volience)
            {
                return Randomizer.GetRandomSingle() * (EduMaxRadius - MaxRadius) + MaxRadius; 
            }
            return Randomizer.GetRandomSingle() * (MaxRadius - MinRadius) + MinRadius;
        }
        static float NextHeight()
        {
            return Randomizer.GetRandomSingle() * 150 + 150;
        }

        public RBall(Player owner, City city, RBallType type)
            : base(owner)
        {
            this.Owner = owner;
            this.Type = type;


            switch (type) 
            {
                case RBallType.Oil:
                    {
                        props.MaxHealth = RulesTable.OilBallBaseHealth + city.Development * RulesTable.CityDevRBallHealthRate;
                        props.Contribution = RulesTable.OilBallContribution;
                        props.Heal = RulesTable.OilBallBaseHeal;
                        props.Damage = RulesTable.OilBallBaseDamage;
                        
                        break;
                    }
                case RBallType.Green:
                    {
                        props.MaxHealth = RulesTable.GreenBallBaseHealth + city.Development * RulesTable.CityDevRBallHealthRate;
                        props.Contribution = RulesTable.GreenBallContribution;
                        props.Heal = RulesTable.GreenBallBaseHeal;
                        props.Damage = RulesTable.GreenBallBaseDamage;

                        break;
                    }
                case RBallType.Disease:
                    {
                        props.MaxHealth = RulesTable.DiseaseBallBaseHealth + city.Development * RulesTable.CityDevRBallHealthRate;
                        props.Contribution = RulesTable.DiseaseBallContribution;
                        props.Heal = RulesTable.DiseaseBallBaseHeal;
                        props.Damage = RulesTable.DiseaseBallBaseDamage;

                        break;
                    }
                case RBallType.Health:
                    {
                        props.MaxHealth = RulesTable.HealthBallBaseHealth + city.Development * RulesTable.CityDevRBallHealthRate;
                        props.Contribution = RulesTable.HealthBallContribution;
                        props.Heal = RulesTable.HealthBallBaseHeal;
                        props.Damage = RulesTable.HealthBallBaseDamage;

                        break;
                    }
                case RBallType.Education:
                    {
                        props.MaxHealth = RulesTable.EducationBallBaseHealth + city.Development * RulesTable.CityDevRBallHealthRate;
                        props.Contribution = RulesTable.EducationBallContribution;
                        props.Heal = RulesTable.EducationBallBaseHeal;
                        props.Damage = RulesTable.EducationBallBaseDamage;

                        break;
                    }
                case RBallType.Volience:
                    {
                        props.MaxHealth = RulesTable.VolienceBallBaseHealth + city.Development * RulesTable.CityDevRBallHealthRate;
                        props.Contribution = RulesTable.VolienceBallContribution;
                        props.Heal = RulesTable.VolienceBallBaseHeal;
                        props.Damage = RulesTable.VolienceBallBaseDamage;

                        break;
                    }            
            }
            Health = props.MaxHealth;

            SetDockCity(city);

            currentRadius = NextRadius();
            currentHeight = NextHeight();


            refrenceLinSpeed = Randomizer.GetRandomSingle() * (MaxLinSpeed - MinLinSpeed) + MinLinSpeed;
            BoundingSphere.Radius = 15;
        }

        public void InitializeGraphics(RenderSystem rs)
        {
            FileLocation fl = null;
            switch (Type)
            {
                case RBallType.Disease:
                    fl = FileSystem.Instance.Locate("rb_virus_ball.mesh", GameFileLocs.Model);   
                    break;
                case RBallType.Education:
                    fl = FileSystem.Instance.Locate("rb_school_ball.mesh", GameFileLocs.Model);   
                    break;
                case RBallType.Green:
                    fl = FileSystem.Instance.Locate("rb_green_ball.mesh", GameFileLocs.Model);   
                    break;
                case RBallType.Health:
                    fl = FileSystem.Instance.Locate("rb_hospital_ball.mesh", GameFileLocs.Model);   
                    break;
                case RBallType.Oil:
                    fl = FileSystem.Instance.Locate("rb_oil_ball.mesh", GameFileLocs.Model);   
                    break;
                case RBallType.Volience:
                    fl = FileSystem.Instance.Locate("rb_volient_ball.mesh", GameFileLocs.Model);   
                    break;
            }
            ModelL0 = new Model(ModelManager.Instance.CreateInstance(rs, fl));
            ModelL0.CurrentAnimation.Clear();

            if (Type == RBallType.Oil)
            {
                ModelL0.CurrentAnimation.Add(new NoAnimaionPlayer(Matrix.Scaling(1.6f, 1.6f, 1.6f) * Matrix.RotationX(-MathEx.PiOver2)));
            }
            else 
            {
                ModelL0.CurrentAnimation.Add(new NoAnimaionPlayer(Matrix.Scaling(1.4f, 1.4f, 1.4f) * Matrix.RotationX(-MathEx.PiOver2)));
            }

            UpdateDisplayScale();
        }

        public void Damage(float v)
        {
            Health -= v;
            if (Health < 0)
            {
                this.Owner = null;
            }
        }

        void ChangeState(RBallState newState)
        {
            switch (newState) 
            {
                case RBallState.AttackCity:                
                    speedModifierTarget = (AttackCitySpeedMod);
                    break;
                case RBallState.BeginingAttackCity:
                case RBallState.Free:
                case RBallState.Standby:
                    speedModifierTarget = (MinSpeedMod);
                    break;
                case RBallState.Gathering:
                    speedModifierTarget = (GatherSpeedMod);
                    break;
                case RBallState.Defend:
                    speedModifierTarget = DefenceSpeedMod;
                    break;
                case RBallState.Attack:
                    speedModifierTarget = AttackSpeedMod;
                    break;
            }
            //switch (newState) 
            //{
            //    case RBallState.Positioning:
            //        {                        

            //            //positioningStartPosition = position;
            //            //positioningEndPosition = dockCity.Position + offset;
            //            positioningProgress = 0;
            //            positioningDistance = Vector3.Distance(ref positioningStartPosition, ref positioningEndPosition);
            //            break;
            //        }
            //}
            state = newState;
        }

        void NewMove(Vector3 pos)
        {
            moveTarget = pos;
            isMoving = true;
        }
        void NewMoveUpdate(float dt)
        {

            Vector3 direction = moveTarget - position;
            if (direction.LengthSquared() > MathEx.Sqr(MoveDistanceThreshold * (float)Math.Sqrt(speedModifier)))
            {

                direction.Normalize();

                if (dockCity != null)
                {
                    Vector3 cityNormal = dockCity.Transformation.Up;
                    Ray ra = new Ray(dockCity.Position, cityNormal);
                    float dist = MathEx.Distance(ref ra, ref position);

                    if (state != RBallState.BeginingAttackCity && (Type != RBallType.Disease || Type != RBallType.Health))
                    {
                        if (dist < CitySafeRadius)
                        {
                            Vector3 newDir;
                            Vector3.Cross(ref direction, ref cityNormal, out newDir);
                            newDir.Normalize();

                            direction = newDir;
                        }
                    }
                    Vector3 currentDir = orientation.Forward;

                    currentDir += 8 * direction * dt;
                    currentDir.Normalize();

                    Matrix newOri = orientation;
                    newOri.Forward = currentDir;
                    newOri.Up = cityNormal;
                    newOri.Right = Vector3.Normalize(Vector3.Cross(currentDir, cityNormal));
                    newOri.Up = Vector3.Cross(newOri.Right, currentDir);
                    //Vector3 currentDir = orientation.Forward;

                    //Quaternion rotmod = shortestArcQuat(currentDir, Vector3.Lerp(currentDir, direction, 0.5f), cityNormal);


                    //orientation *= Matrix.RotationQuaternion(rotmod);
                    orientation = newOri;

                    //currentDir = orientation.Forward;
                    Position += newOri.Forward * (refrenceLinSpeed * speedModifier * dt);
                }
                rotationLerp += dt;

            }
            else
            {
                isMoving = false;
                rotationLerp = 0;
            }
        }

        void CalculateRoundTransform(float radius, bool clockwise, out Vector3 pos, out Matrix rot)
        {
            Matrix dockOri = dockCity.Transformation;
            dockOri.TranslationValue = Vector3.Zero;
            orientation = Matrix.RotationY(roundAngle) * dockOri;

            Vector3 y = dockOri.Forward;
            Vector3 x = dockOri.Right;
            Vector3 z = dockOri.Up;

            Vector3 dir = x * (float)Math.Cos(roundAngle) + y * (float)Math.Sin(roundAngle);
            Vector3 ofs = dir * radius + z * currentHeight;

            float rotYAngle = clockwise ? (roundAngle + MathEx.PIf) : roundAngle;

            pos = dockCity.Position + ofs;
            rot = Matrix.RotationY(rotYAngle) * dockOri;
        }

        void CalculateRoundTransform(float radius, bool clockwise, out Vector3 pos, out Quaternion rot)
        {
            Matrix dockOri = dockCity.Transformation;
            dockOri.TranslationValue = Vector3.Zero;
            orientation = Matrix.RotationY(roundAngle) * dockOri;

            Vector3 y = dockOri.Forward;
            Vector3 x = dockOri.Right;
            Vector3 z = dockOri.Up;

            Vector3 dir = x * (float)Math.Cos(roundAngle) + y * (float)Math.Sin(roundAngle);
            Vector3 ofs = dir * radius + z * currentHeight;

            float rotYAngle = clockwise ? (roundAngle + MathEx.PIf) : roundAngle;

            pos = dockCity.Position + ofs;
            rot = Quaternion.RotationMatrix(Matrix.RotationY(rotYAngle) * dockOri);
        }

        

        void Attack()
        {
            if (dockCity.NearbyOwnedBallCount > 0)
            {
                int tries = 0;
                int idx = Randomizer.GetRandomInt(dockCity.NearbyOwnedBallCount);

                while (tries < dockCity.NearbyOwnedBallCount && dockCity.GetNearbyOwnedBall(idx).IsDied)
                {
                    tries++;
                    idx++;
                    idx %= dockCity.NearbyOwnedBallCount;
                }

                if (tries < dockCity.NearbyOwnedBallCount)
                {
                    target = dockCity.GetNearbyOwnedBall(idx);

                    if (target != null)
                    {
                        ChangeState(RBallState.Attack);
                    }
                }
            }
            else if (dockCity.NearbyEnemyBallCount > 0)
            {
                // 其他阵营的敌人
                int tries = 0;
                int idx = Randomizer.GetRandomInt(dockCity.NearbyEnemyBallCount);

                while (tries < dockCity.NearbyEnemyBallCount
                    && dockCity.GetNearbyEnemyBall(idx).IsDied 
                    && dockCity.GetNearbyEnemyBall(idx).Owner == Owner)
                {
                    idx++;
                    idx %= dockCity.NearbyEnemyBallCount;
                }

                if (tries < dockCity.NearbyEnemyBallCount)
                {
                    target = dockCity.GetNearbyEnemyBall(idx);

                    if (target != null)
                    {
                        ChangeState(RBallState.Attack);
                    }
                }
            }
        }
        void Defend()
        {
            if (dockCity.NearbyEnemyBallCount > 0)
            {
                int tries = 0;
                int idx = Randomizer.GetRandomInt(dockCity.NearbyEnemyBallCount);

                while (tries<dockCity.NearbyEnemyBallCount && dockCity.GetNearbyEnemyBall(idx).IsDied)
                {
                    tries++;
                    idx++;
                    idx %=dockCity.NearbyEnemyBallCount;
                }

                if (tries < dockCity.NearbyEnemyBallCount)
                {
                    target = dockCity.GetNearbyEnemyBall(idx);

                    if (target != null)
                    {
                        ChangeState(RBallState.Defend);
                    }
                }
            }
        }
        void AttackCity()
        {
            Vector3 targetPos;
            Quaternion dockOri;
            CalculateRoundTransform(CityAttackRadius, true, out targetPos, out dockOri);
            NewMove(targetPos);
            ChangeState(RBallState.BeginingAttackCity);
        }
        public void Reposition() 
        {
            Vector3 targetPos;
            Quaternion dockOri;
            CalculateRoundTransform(currentRadius, false, out targetPos, out dockOri);
            NewMove(targetPos);

            ChangeState(RBallState.Free);

        }
        public void Free(City city)
        {
            gatheredParent = null;

            SetDockCity(city);

            Vector3 targetPos;
            Quaternion dockOri;
            CalculateRoundTransform(currentRadius, false, out targetPos, out dockOri);
            NewMove(targetPos);

            ChangeState(RBallState.Free);

            enemyCheckTime = EnemyCheckTime;
        }
        public void Gather(RGatheredBall ball, Vector3 endGatherPosition, Quaternion endGatherOrient)
        {
            gatheredParent = ball;
            NewMove(endGatherPosition);
            ChangeState(RBallState.Gathering);
        }

        void UpdateDisplayScale() 
        {
            float s = 1 + Health * RBallHealthScale;
            Matrix.Scaling(s,s,s, out displayScale);
        }
        public override RenderOperation[] GetRenderOperation()
        {
            RenderOperation[] ops = base.GetRenderOperation();
            for (int i = 0; i < ops.Length; i++) 
            {
                ops[i].Transformation = displayScale * ops[i].Transformation;
            }
            return ops;
        }
        public override RenderOperation[] GetRenderOperation(int level)
        {
            return GetRenderOperation();
        }


        void IntegrateRoundAngle(float dt)
        {
            
            float sign = state == RBallState.AttackCity ? -1 : 1;
            roundAngle += sign * dt * speedModifier * refrenceLinSpeed / currentRadius;
            if (roundAngle > MathEx.PIf * 2)
                roundAngle -= MathEx.PIf * 2;
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTimeSeconds;

            if (weaponCoolDown > 0)
                weaponCoolDown -= dt;

            if (speedModifier < speedModifierTarget)
            {
                speedModifier += dt;
            }
            else if (speedModifier > speedModifierTarget)
            {
                speedModifier -= dt;
            }

            switch (state)
            {
                case RBallState.Standby:
                    {
                        IntegrateRoundAngle(dt);

                        Vector3 pos;
                        CalculateRoundTransform(currentRadius, false, out pos, out orientation);
                        Position = pos;

                        dockCity.Heal(dt * props.Heal);
                        dockCity.Develop(props.Contribution, dt);
                        break;
                    }
                case RBallState.Gathering:
                    {
                        NewMoveUpdate(dt);

                        if (!isMoving)
                        {
                            ChangeState(RBallState.Gathered);
                        }
                        break;
                    }
                case RBallState.Free:
                    {
                        NewMoveUpdate(dt);
                        if (!isMoving)
                        {
                            ChangeState(RBallState.Standby);
                        }
                        break;
                    }
                case RBallState.BeginingAttackCity:
                    {
                        NewMoveUpdate(dt);
                        if (!isMoving)
                        {
                            ChangeState(RBallState.AttackCity);
                        }
                        break;
                    }
                case RBallState.AttackCity:
                    {
                        if (dockCity.Owner != Owner)
                        {
                            IntegrateRoundAngle(dt);

                            Vector3 pos;
                            CalculateRoundTransform(CityAttackRadius, true, out pos, out orientation);
                            Position = pos;

                            if (speedModifier >= AttackCitySpeedMod - 0.5f)
                            {
                                if (weaponCoolDown <= 0)
                                {
                                    float dmg = Type == RBallType.Oil ? RulesTable.OilBallBaseDamage : RulesTable.GreenBallBaseDamage;
                                    dockCity.Damage(dmg, Owner);
                                    Damage(dmg * 0.005f);
                                    weaponCoolDown = WeaponCoolDownTime;
                                }
                            }
                        }
                        else
                        {
                            if (speedModifier < AttackCitySpeedMod * 0.3f || speedModifier < MinSpeedMod)
                            {
                                Reposition();
                            }
                            else
                            {
                                IntegrateRoundAngle(dt);

                                Vector3 pos;
                                CalculateRoundTransform(CityAttackRadius, true, out pos, out orientation);
                                Position = pos;

                                speedModifierTarget = MinSpeedMod;
                            }
                        }
                        break;
                    }
                case RBallState.Defend:
                case RBallState.Attack:
                    {
                        if (target == null || target.Owner == Owner || target.IsDied)
                        {
                            Reposition();
                        }
                        else
                        {
                            NewMove(target.position);
                            NewMoveUpdate(dt);

                            float dist = Vector3.DistanceSquared(ref position, ref target.position);
                            if (dist < AttackRange * AttackRange)
                            {
                                if (weaponCoolDown <= 0)
                                {
                                    float dmg = Type == RBallType.Oil ? RulesTable.OilBallBaseDamage : RulesTable.GreenBallBaseDamage;
                                    target.Damage(dmg);
                                    weaponCoolDown = WeaponCoolDownTime;
                                }
                            }
                        }
                        break;
                    }
            }
            base.Update(gameTime);

            
            if (dockCity != null &&
                (state != RBallState.Gathered || state != RBallState.Gathering || state != RBallState.Free))
            {

                // 攻击
                if (dockCity.Owner != Owner)
                {
                    enemyCheckTime -= dt;
                    if (enemyCheckTime <= 0)
                    {
                        // 在别人城里
                        if (dockCity.NearbyOwnedBallCount == 0)
                        {
                            if (state != RBallState.AttackCity && state != RBallState.BeginingAttackCity)
                                AttackCity();
                        }
                        else
                        {
                            if (state != RBallState.Attack)
                                Attack();
                        }
                        enemyCheckTime = EnemyCheckTime;
                    }

                }
                else
                {
                    switch (Type)
                    {
                        case RBallType.Oil:
                        case RBallType.Green:
                        case RBallType.Education:
                        case RBallType.Volience:
                            if (dockCity.NearbyEnemyBallCount > 0)
                            {
                                if (state != RBallState.Attack)
                                    Defend();
                            }
                            break;
                        case RBallType.Health:
                        case RBallType.Disease:
                            if (dockCity.HPRate > 1 - float.Epsilon)                             
                            {
                                if (dockCity.NearbyEnemyBallCount > 0)
                                {
                                    if (state != RBallState.Attack)
                                        Defend();
                                }
                            }
                            break;
                    }
                   
                }

                
            }





        }
    }
}
