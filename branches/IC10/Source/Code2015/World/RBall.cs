using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Animation;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.Logic;

namespace Code2015.World
{
    enum RBallState
    {
        Standby,
        Born,
        Gathering,
        Attack,
        AttackCity,
        Gathered,
        Free,
    }

    /// <summary>
    ///  大球只进行一次跳跃
    /// </summary>
    class RGatheredBall
    {
        const float GatherPositionRadius = City.CityRadius * 0.5f;
        const float GatherPositionHeight = 250;

        const float GBallRadius = 100;

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
        const float MinRadius = City.CityRadius * 0.8f;
        const float MaxRadius = City.CityRadius * 1.5f;
        const float GoRoundVel = 0.35f;
        const float MaxSpeed = 1.2f;
        const float MinSpeed = 0.8f;
        
        const float MaxLinSpeed = MaxRadius * MaxSpeed;
        //const float PositioningSpeed = 200;
        

        public RBallType Type { get; private set; }
        
        private City dockCity;
        public City DockCity { get { return this.dockCity; } }


        RBallState state;


        #region State_Positioning
        
        float positioningProgress;
        //float positioningDistance;
        Vector3 positioningEndPosition;
        Vector3 positioningStartPosition;
        Quaternion positioningStartOri;
        Quaternion positioningEndOri;
        #endregion

        RGatheredBall gatheredParent;



        ///// <summary>
        ///// 在城=城市间移动的速度
        ///// </summary>
        //private Vector3 velocity;




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

        /// <summary>
        /// 旋转速度
        /// </summary>
        private float roundSpeed;
        #endregion

        public RBallState State 
        {
            get { return state; }
        }
        //是否被消灭
        public bool IsDied { get { return Health <= 0; } }

        //血条
        public float Health { get; private set; }
        public float MaxHealth { get; private set; }

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
        
        static float NextRadius()
        {
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
            //设置血量
            this.Health = city.Development * 0.15f;
            SetDockCity(city);

            currentRadius = NextRadius();
            currentHeight = NextHeight();

            roundSpeed = Randomizer.GetRandomSingle() * (MaxSpeed - MinSpeed) + MinSpeed;
            BoundingSphere.Radius = 15;
        }

        public void InitializeGraphics(RenderSystem rs) 
        {
            FileLocation fl = FileSystem.Instance.Locate("oil_ball.mesh", GameFileLocs.Model);
            ModelL0 = new Model(ModelManager.Instance.CreateInstance(rs, fl));
            ModelL0.CurrentAnimation.Clear();
            ModelL0.CurrentAnimation.Add(new NoAnimaionPlayer(Matrix.Scaling(1.4f, 1.4f, 1.4f) * Matrix.RotationX(-MathEx.PiOver2)));

            switch (Type)
            {
                case RBallType.Disease:
                    
                    break;
                case RBallType.Education:
                    break;
                case RBallType.Green:
                    break;
                case RBallType.Health:
                    break;
                case RBallType.Oil:
                    break;
                case RBallType.Volience:
                    break;
            }
            
            
        }

        public void Damage(float v)
        {
            Health -= v;
            if (Health < 0)
            {
                this.Owner = null;
            }
        }


        //public void SetTarget(City target)
        //{
        //    this.target = target;
        //}

        void ChangeState(RBallState newState) 
        {
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


        void MoveLerpUpdate(float dt)
        {
            if (positioningProgress < 0.8f)
            {
                Position = Vector3.Lerp(positioningStartPosition, positioningEndPosition, positioningProgress / 0.8f);

                positioningProgress += dt * 1.5f;// (PositioningSpeed / positioningDistance) * dt;
            }
            else if (positioningProgress < 1 && positioningProgress > 0.8f)
            {
                Orientation = Matrix.RotationQuaternion(
                    Quaternion.Slerp(positioningStartOri, positioningEndOri, (positioningProgress - 0.8f) * 5));
                positioningProgress += dt * 0.5f;
            }
        }
        void Move(Vector3 pos, Quaternion ori)
        {
            positioningProgress = 0;

            positioningEndOri = ori;
            positioningStartOri = Quaternion.RotationMatrix(orientation);

            positioningEndPosition = pos;
            positioningStartPosition = position;
            //positioningDistance = Vector3.Distance(ref positioningStartPosition, ref positioningEndPosition);
        }

        public void Free(City city)
        {
            gatheredParent = null; 
            
            SetDockCity(city);

            Matrix dockOri = dockCity.Transformation;
            dockOri.TranslationValue = Vector3.Zero;
            orientation = Matrix.RotationY(roundAngle) * dockOri;
          
            Vector3 y = dockOri.Forward;
            Vector3 x = dockOri.Right;
            Vector3 z = dockOri.Up;

            Vector3 dir = x * (float)Math.Cos(roundAngle) + y * (float)Math.Sin(roundAngle);
            Vector3 ofs = dir * currentRadius + z * currentHeight;

            Move(dockCity.Position + ofs, Quaternion.RotationMatrix(Matrix.RotationY(roundAngle) * dockOri));
            ChangeState(RBallState.Free);
        }
        public void Gather(RGatheredBall ball, Vector3 endGatherPosition, Quaternion endGatherOrient)
        {            
            gatheredParent = ball;
            Move(endGatherPosition, endGatherOrient);
            ChangeState(RBallState.Gathering);
        }

        public override RenderOperation[] GetRenderOperation()
        {
            return base.GetRenderOperation();
        }
        public override RenderOperation[] GetRenderOperation(int level)
        {
            return base.GetRenderOperation(level);
        }
        
        public override void Update(GameTime gameTime)
        {

            float dt = (float)gameTime.ElapsedGameTimeSeconds;

            switch (state) 
            {
                case RBallState.Standby:
                    {
                        roundAngle += dt * GoRoundVel * roundSpeed;
                        if (roundAngle > MathEx.PIf * 2)
                            roundAngle -= MathEx.PIf * 2;


                        Matrix dockOri = dockCity.Transformation;

                        Vector3 y = dockOri.Forward;
                        Vector3 x = dockOri.Right;
                        Vector3 z = dockOri.Up;

                        Vector3 dir = x * (float)Math.Cos(roundAngle) + y * (float)Math.Sin(roundAngle);
                        Vector3 ofs = dir * currentRadius + z * currentHeight;
                        dockOri.TranslationValue = Vector3.Zero;

                        orientation = Matrix.RotationY(roundAngle) *  dockOri;
                        Position = dockCity.Position + ofs;
                        break;
                    }
                case RBallState.Gathering:
                    {
                        MoveLerpUpdate(dt);

                        if (positioningProgress > 1)
                        {
                            ChangeState(RBallState.Gathered);
                        }
                        break;
                    }
                case RBallState.Free:
                    {
                        MoveLerpUpdate(dt);
                        if (positioningProgress > 1)
                        {
                            ChangeState(RBallState.Standby);
                        }
                        break;
                    }
            }
            base.Update(gameTime);

            if (dockCity != null)
            {
                switch (Type)
                {
                    case RBallType.Green:
                    case RBallType.Oil:
                        {
                            // 攻击

                            if (dockCity.Owner != Owner)
                            {
                                // 在别人城里
                                //if ( state != RBallState.AttackCity)
                            }
                            else
                            {
                                //在自己城里
                                
                            }
                            break;
                        }
                    case RBallType.Disease:
                        {
                            break;
                        }
                    case RBallType.Health:
                        {
                            break;
                        }
                    case RBallType.Education:
                        {
                            break;
                        }
                    case RBallType.Volience:
                        {
                            break;
                        }
                }
            }
            
            //else if (this.target != null)
            //{
                ////攻击城市
                //Vector3 dir = target.Position - Position;

                //if (this.Parent != null)
                //{

                //}

            //}

            
        }
    }
}
