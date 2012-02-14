using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Animation;
using Apoc3D.Graphics.Effects;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.Effects;
using Code2015.EngineEx;
using Code2015.Logic;

namespace Code2015.World
{
    enum RBallState
    {
        /// <summary>
        ///  The ball is ready, moving around the city
        /// </summary>
        Standby,
        /// <summary>
        /// [Not used]
        /// </summary>
        Born,
        /// <summary>
        ///  The ball is going to a RGatheredBall
        /// </summary>
        Gathering,
        /// <summary>
        ///  The state when the ball going closer to the center city before attacking it.
        /// </summary>
        BeginingAttackCity,
        /// <summary>
        ///  The state when the ball attack other enemy balls
        /// </summary>
        Attack,
        /// <summary>
        ///  The state when the ball attacks the city,
        ///  accelerating moving around close to the city
        /// </summary>
        AttackCity, 
        /// <summary>
        ///  The state when the ball defend the home city and attack other enemy balls
        /// </summary>
        Defend,
        /// <summary>
        ///  The state when the ball has arrived the RGatheredBall
        /// </summary>
        Gathered,
        /// <summary>
        ///  The state when the ball is changing to the Standby state, 
        ///  adjusting its position to the orbit
        /// </summary>
        Free,
        /// <summary>
        ///  The state when the ball is fleeing or be calling back from an enemy city
        /// </summary>
        Float
    }

    /// <summary>
    ///  The RGatheredBall, which is a virtual big ball with all ball gathered created in a City.Throw_State,
    ///  staying in air until the next city has received it.
    ///  
    ///  The lifetime of RGatheredBall lasts only throwing between 2 cities. Continuously passing balls will
    ///  create new ones for the latter steps.
    /// </summary>
    class RGatheredBall
    {
        const float GatherPositionRadius = City.CityRadius * 0.75f;
        const float GatherPositionHeight = 250;

        const float GBallRadius = 75;

        public enum State
        {
            /// <summary>
            ///  State when the RBalls are gathering to the RGatheredBall's location
            /// </summary>
            WaitingGathering,
            /// <summary>
            ///  State when waiting the NotifyThrow() call from city. 
            ///  Usually city are turning or playing animation at the time.
            /// </summary>
            WaitingThrow,
            /// <summary>
            ///  State when the ball is in the air
            /// </summary>
            Flying,
            /// <summary>
            ///  State when the ball has reached the target city
            /// </summary>
            Finished
        }

        Normal3DSoundObject throwSound;
        Normal3DSoundObject catchSound;

        List<City> goPath;
        State state;
        List<Vector3> ballOffsets;
        List<RBall> subBalls;
        Vector3 position;
        City sourceCity;

        #region FLY
        // The trace of RGatheredBall in the air is a arc
        // Constant velocity at horizontal direction(relative to the surface)
        // and a quadric trace at vertical direction
        Vector3 normal;
        Vector3 srcPosition;
        Vector3 dstPosition;
        /// <summary>
        ///  The interpolation amount, increases over time
        /// </summary>
        float flyProgress;
        bool notifyedDest;
        City destCity;

        bool catchSoundPlayed;
        #endregion

        public List<RBall> Balls { get { return subBalls; } }
        public City SourceCity 
        {
            get { return sourceCity; }
        }
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
        ///  Releases all RBalls from this GatheredBall when it have arrived 
        ///  at the destination city
        /// </summary>
        void ReleaseBalls() 
        {
            City cc = goPath[0];

            for (int i = 0; i < subBalls.Count; i++)
            {
                subBalls[i].Free(cc);
            }
        }
        /// <summary>
        ///  Releases all RBalls from this GatheredBall when gathering
        /// </summary>
        void ReleaseBallGathering() 
        {
            
            for (int i = 0; i < subBalls.Count; i++)
            {
                subBalls[i].Free(sourceCity);
            }
        }

        /// <summary>
        ///  Calculate the position of the RBall in the city's hand at the given direction
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

            float lineCount = (float)Math.Sqrt(balls.Count);
            float span = (MathEx.PIf * 2) / lineCount;

            ballOffsets = new List<Vector3>(balls.Count);
            for (int i = 0; i < balls.Count; i++)
            {
                float row = (float)i / lineCount;
                float col = (float)Math.IEEERemainder(i, lineCount);

                float radLng = col * span;
                float radLat = (2 * row - lineCount) * span;

                Vector3 positionInGBall = PlanetEarth.GetPosition(radLng, radLat, GBallRadius);
                Quaternion oriInGBall = Quaternion.RotationMatrix(PlanetEarth.GetOrientation(radLng, radLat));
                ballOffsets.Add(positionInGBall);
                balls[i].Gather(this, positionInGBall + position, oriInGBall);
            }

            throwSound = (Normal3DSoundObject)SoundManager.Instance.MakeSoundObjcet("throw", null, 1500);
            catchSound = (Normal3DSoundObject)SoundManager.Instance.MakeSoundObjcet("catch", null, 1500);
            
        }

        /// <summary>
        ///  Cancel the gathering
        /// </summary>
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
        /// <summary>
        ///  Notified by city, ask the RGatheredBall to go flying
        /// </summary>
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


                Quaternion newDstOri = dstCity.GetOrientation(srcPosition);

                normal = sourceCity.Transformation.Up + dstCity.Transformation.Up;
                normal.Normalize();

                dstPosition = GetRGBallPosition(dstCity, newDstOri);


                notifyedDest = false;
                flyProgress = 0;

                throwSound.Fire();
            }
            else if (newState == State.Finished)
            {
                ReleaseBalls();
            }

            state = newState;
        }


        /// <summary>
        ///  Update called for the Flying state
        /// </summary>
        /// <param name="time"></param>
        void UpdateFly(GameTime time)
        {
            // linear lerp
            position = Vector3.Lerp(srcPosition, dstPosition, flyProgress);

            flyProgress += time.ElapsedGameTimeSeconds * 0.5f;

            // estimate the remaining time to check if the target city's catch
            // animation should be played to make it in time
            float timeStamp = destCity.CatchPreserveTime;

            timeStamp = 1.5f - timeStamp;

            if ((flyProgress / 0.5f) > timeStamp)
            {
                if (!notifyedDest)
                {
                    destCity.NotifyIncomingBall(this);

                    // only notify once
                    notifyedDest = true;
                }
            }

            // estimate the remaining time to check if the receive sound should be played
            timeStamp += 0.5f;
            if ((flyProgress / 0.5f) > timeStamp)
            {
                if (!catchSoundPlayed)
                {
                    catchSound.Fire();
                    catchSoundPlayed = true;
                }
            }

            // now adds height to the position
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

            // finishes as long as the progress reaches 1
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
                            if (!subBalls[i].IsDied && subBalls[i].State != RBallState.Gathered)
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

                        // keep the RBalls inside the same position as this GatheredBall
                        for (int i = 0; i < subBalls.Count; i++) 
                        {
                            subBalls[i].Position = ballOffsets[i] + position;
                        }
                        break;
                    }

            }

            catchSound.Position = position;
            catchSound.Update(time);
            throwSound.Position = position;
            throwSound.Update(time);
        }

        
    }

    /// <summary>
    ///  Represents resource balls.
    /// </summary>
    /// <remarks>
    ///  Basically there are 2 types of locomotion in RBalls,
    ///  orbit and targeted moving.
    ///  
    ///  AttackCity, Standby are orbit moving.
    ///  The reset moves toward a target.
    /// </remarks>
    class RBall : WorldDynamicObject
    {
        /// <summary>
        ///  The inborn properties of the RBall, varies from type to type
        /// </summary>
        public struct Props
        {
            public float Damage;
            public float Contribution;
            public float Heal;
            public float HealthIncr;
            public float MaxHealth;
            public float BaseMaxHealth;
        }

        const float EnemyCheckTime = 0.75f;
        const float WeaponCoolDownTime = 1;

        const float MinRadius = City.CityRadius * 0.8f;
        const float MaxRadius = City.CityRadius * 1.5f;
        const float EduMaxRadius = City.CityRadius * 1.9f;

        const float FloatingSpeedMod = 0.5f;
        const float MinSpeedMod = 1;
        const float GatherSpeedMod = 3.5f;
        const float AttackCitySpeedMod = 7.0f;
        const float DefenceSpeedMod = 2.5f;
        const float AttackSpeedMod = 3.5f;

        /// <summary>
        ///  Minimum linear velocity to limit refrenceLinSpeed
        /// </summary>
        const float MaxLinSpeed = 233;
        /// <summary>
        ///  Maximum linear velocity to limit refrenceLinSpeed
        /// </summary>
        const float MinLinSpeed = 190;

        /// <summary>
        ///  The radius of the impassable barier at the center of the city, stopping
        ///  balls in normal cases
        /// </summary>
        const float CitySafeRadius = 193;
        /// <summary>
        ///  The radius when the balls are attcking the docking city
        /// </summary>
        const float CityAttackRadius = 150;


        /// <summary>
        ///  The amount of distance to assume the destination has arrive when the remaining
        ///  distance is smaller than this. Used in moving.
        /// </summary>
        const float MoveDistanceThreshold = 50;
        /// <summary>
        ///  The amount of distance for a RBall can attack a enemy RBall within this distance.
        /// </summary>
        const float AttackRange = 150;


        public RBallType Type { get; private set; }

        private City dockCity;
        public City DockCity { get { return this.dockCity; } }

        RBallState state;

        readonly Props props;

        /// <summary>
        ///  The remaining time for the next attack(on enemy RBall)
        /// </summary>
        float weaponCoolDown;
        /// <summary>
        ///  The remaining time to check if any balls to attack in the city
        /// </summary>
        float enemyCheckTime;

        
        /// <summary>
        ///  The current velocity multipler
        /// </summary>
        float speedModifier = 1;
        /// <summary>
        ///  The velocity multipler that the speedModifier is approching to
        /// </summary>
        float speedModifierTarget = 1;

        Matrix displayScale;

        #region State_NewMove
        // This State_NewMove, a sub-state, is used inside and as a part of a variety of states, 
        // to let the RBall move to a given location
        bool isMoving;

        Vector3 moveTarget;

        #endregion

        /// <summary>
        ///  Keep a reference if the RBall is gathering
        /// </summary>
        RGatheredBall gatheredParent;

        #region State_Float

        /// <summary>
        ///  The destination city when in float state
        ///  Ordinarily, balls flee back to their the nearest adjancent home city
        /// </summary>
        City floatingTarget;

        #endregion

        #region State_Attack

        RBall target;

        #endregion


        #region Standby
        /// <summary>
        /// The radius when the RBall orbits around the city in 
        /// Standby state.
        /// </summary>
        private float currentRadius;
        /// <summary>
        ///  The height of the orbit
        /// </summary>
        private float currentHeight;

        /// <summary>
        ///  The current orbit angle to the center of the city
        /// </summary>
        private float roundAngle;
        #endregion

        FastList<RenderOperation> opBuffer = new FastList<RenderOperation>();
        /// <summary>
        ///  The tail is presented by a horizontal transparent plane with a arc in the texture
        ///  To make the final look, the plane is position at the center of the city, rotating
        ///  to keep after its RBall
        /// </summary>
        Model red_tail;
        Model green_tail;
        
        /// <summary>
        ///  A random value between MinLinSpeed and MaxLinSpeed, used as the normal
        ///  linear velocity when standing by
        /// </summary>
        float refrenceLinSpeed;


        Normal3DSoundObject soundObject;
        bool shouldPlayDeathSonds;

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

        /// <summary>
        ///  Get a random radius for the RBall's orbit
        /// </summary>
        float NextRadius()
        {
            if (Type == RBallType.Disease || Type == RBallType.Health) 
            {
                return CitySafeRadius;
            }
            else if (Type == RBallType.Education || Type == RBallType.Volience)
            {
                return Randomizer.GetRandomSingle() * (EduMaxRadius - MaxRadius) + MaxRadius; 
            }
            return Randomizer.GetRandomSingle() * (MaxRadius - MinRadius) + MinRadius;
        }
        /// <summary>
        ///  Get a random height
        /// </summary>
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
                        props.BaseMaxHealth = RulesTable.OilBallBaseHealth;
                        props.MaxHealth = props.BaseMaxHealth * (city.Level / 10.0f + 1);
                        props.Contribution = RulesTable.OilBallContribution;
                        props.Heal = RulesTable.OilBallBaseHeal;
                        props.Damage = RulesTable.OilBallBaseDamage;
                        
                        break;
                    }
                case RBallType.Green:
                    {
                        props.BaseMaxHealth = RulesTable.GreenBallBaseHealth;
                        props.MaxHealth = props.BaseMaxHealth * (city.Level / 10.0f + 1);
                        props.Contribution = RulesTable.GreenBallContribution;
                        props.Heal = RulesTable.GreenBallBaseHeal;
                        props.Damage = RulesTable.GreenBallBaseDamage;

                        break;
                    }
                case RBallType.Disease:
                    {
                        props.BaseMaxHealth = RulesTable.DiseaseBallBaseHealth;
                        props.MaxHealth = props.BaseMaxHealth * (city.Level / 10.0f + 1);
                        props.Contribution = RulesTable.DiseaseBallContribution;
                        props.Heal = RulesTable.DiseaseBallBaseHeal;
                        props.Damage = RulesTable.DiseaseBallBaseDamage;
                        props.HealthIncr = 0.0015f;
                        break;
                    }
                case RBallType.Health:
                    {
                        props.BaseMaxHealth = RulesTable.DiseaseBallBaseHealth;
                        props.MaxHealth = props.BaseMaxHealth * (city.Level / 10.0f + 1);
                        props.Contribution = RulesTable.HealthBallContribution;
                        props.Heal = RulesTable.HealthBallBaseHeal;
                        props.Damage = RulesTable.HealthBallBaseDamage;
                        props.HealthIncr = 0.0015f;
                        break;
                    }
                case RBallType.Education:
                    {
                        props.BaseMaxHealth = RulesTable.EducationBallBaseHealth;
                        props.MaxHealth = props.BaseMaxHealth * (city.Level / 10.0f + 1);
                        props.Contribution = RulesTable.EducationBallContribution;
                        props.Heal = RulesTable.EducationBallBaseHeal;
                        props.Damage = RulesTable.EducationBallBaseDamage;

                        break;
                    }
                case RBallType.Volience:
                    {
                        props.BaseMaxHealth = RulesTable.VolienceBallBaseHealth;
                        props.MaxHealth = props.BaseMaxHealth + city.Development * RulesTable.CityDevRBallHealthRate;
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
            else if (Type == RBallType.Volience)
            {
                ModelL0.CurrentAnimation.Add(new NoAnimaionPlayer(Matrix.Scaling(2.0f, 2.0f, 2.0f) * Matrix.RotationX(-MathEx.PiOver2)));
            }
            else if (Type == RBallType.Education)
            {
                ModelL0.CurrentAnimation.Add(new NoAnimaionPlayer(Matrix.Scaling(2.25f, 2.25f, 2.25f) * Matrix.RotationX(-MathEx.PiOver2)));
            }
            else if (Type == RBallType.Disease)
            {
                ModelL0.CurrentAnimation.Add(new NoAnimaionPlayer(Matrix.Scaling(3.75f, 3.75f, 3.75f)
                    * Matrix.RotationX(-MathEx.PiOver2) * Matrix.RotationY(MathEx.PiOver2)));
            }
            else
            {
                ModelL0.CurrentAnimation.Add(new NoAnimaionPlayer(Matrix.Scaling(1.4f, 1.4f, 1.4f) * Matrix.RotationX(-MathEx.PiOver2)));
            }

            UpdateDisplayScale();

            fl = FileSystem.Instance.Locate("rball_tail_red.mesh", GameFileLocs.Model);
            red_tail = new Model(ModelManager.Instance.CreateInstance(rs, fl));
            red_tail.CurrentAnimation.Add(new NoAnimaionPlayer(Matrix.RotationX(-MathEx.PiOver2) ));

            fl = FileSystem.Instance.Locate("rball_tail_green.mesh", GameFileLocs.Model);
            green_tail = new Model(ModelManager.Instance.CreateInstance(rs, fl));
            green_tail.CurrentAnimation.Add(new NoAnimaionPlayer(Matrix.RotationX(-MathEx.PiOver2)));

            soundObject = (Normal3DSoundObject)SoundManager.Instance.MakeSoundObjcet("rball_die", null, 1800);
        }

        /// <summary>
        ///  Damage this RBall's health by a given value
        /// </summary>
        public void Damage(float v)
        {
            if (Health > 0)
            {
                Health -= v;
                if (Health <= 0 && Owner != null)
                {
                    this.Owner = null;
                    shouldPlayDeathSonds = true;
                }
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

            state = newState;
        }

        /// <summary>
        ///  Called to enter the move sub-state, 
        /// </summary>
        /// <param name="pos"></param>
        void NewMove(Vector3 pos)
        {
            moveTarget = pos;
            isMoving = true;
        }
        /// <summary>
        ///  Called when moving for the Update
        /// </summary>
        /// <param name="dt"></param>
        void NewMoveUpdate(float dt)
        {

            Vector3 direction = moveTarget - position;
            // Is it close enough to assume arrived?
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
                        // At the city's center, a cylinder with a radius of CitySafeRadius, is impassable.
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

                    orientation = newOri;

                    Position += newOri.Forward * (refrenceLinSpeed * speedModifier * dt);
                }

            }
            else
            {
                isMoving = false;
            }
        }

        /// <summary>
        ///  Calculate the orbit transfrom for balls
        /// </summary>
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
        /// <summary>
        ///  Same as above
        /// </summary>
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

        
        /// <summary>
        ///  Try attack other ememy balls in the docking city
        /// </summary>
        void Attack()
        {
            if (dockCity.NearbyOwnedBallCount > 0)
            {
                // The enemy from the same side as the city's
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
                // The enemy from other sides than the city's
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
        /// <summary>
        ///  Enters defend state when the docking city is owned
        /// </summary>
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
        /// <summary>
        ///  Enters the attack city state.
        ///  First the balls will move close to the center. Then accelerating to a enough speed before
        ///  makeing damaged on the city. When the capturing has done, the balls decrease and move to stand by
        ///  position.
        /// </summary>
        void AttackCity()
        {
            Vector3 targetPos;
            Quaternion dockOri;
            CalculateRoundTransform(CityAttackRadius, true, out targetPos, out dockOri);
            NewMove(targetPos);

            //tail.Reset();
            ChangeState(RBallState.BeginingAttackCity);            
        }
        /// <summary>
        ///  Flee from a city to a given target city
        /// </summary>
        public void Float(City target) 
        {
            floatingTarget = target;
            NewMove(target.Position);
            ChangeState(RBallState.Float);
        }
        
        /// <summary>
        ///  Changes the state to Free, making the ball back to their stand by position
        /// </summary>
        public void Reposition() 
        {
            Vector3 targetPos;
            Quaternion dockOri;
            CalculateRoundTransform(currentRadius, false, out targetPos, out dockOri);
            NewMove(targetPos);

            ChangeState(RBallState.Free);

        }
        /// <summary>
        ///  Called when the ball has arrived a different city than its previous one.
        ///  Changes the state to Free, making the ball back to their stand by position.
        /// </summary>
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
            // bigger ball has more health
            float s = (0.3f + (Health / MaxHealth) * 0.7f) *
                (props.MaxHealth / props.BaseMaxHealth);
            s *= 1.10f;

            Matrix.Scaling(s, s, s, out displayScale);
        }
        public override RenderOperation[] GetRenderOperation()
        {
            // not drawing anything when in fog
            if (dockCity != null)
            {
                if (!dockCity.IsInVisibleRange)
                {
                    return null;
                }
            }

            // normal drawing
            if (state != RBallState.AttackCity || (state == RBallState.AttackCity && speedModifier < AttackCitySpeedMod - 0.1f))
            {
                RenderOperation[] ops = base.GetRenderOperation();
                if (ops != null)
                {
                    for (int i = 0; i < ops.Length; i++)
                    {
                        //ops[i].Sender = this;
                        ops[i].Transformation = displayScale * ops[i].Transformation;
                    }
                }

                return ops;
            }

            // add the tails to the render operation when needed, beside the normal model
            {

                opBuffer.FastClear();
                RenderOperation[] ops = base.GetRenderOperation();
                if (ops != null)
                {
                    for (int i = 0; i < ops.Length; i++)
                    {
                        //ops[i].Sender = this;
                        ops[i].Transformation = displayScale * ops[i].Transformation;
                    }
                    opBuffer.Add(ops);
                }


                Matrix trans = Transformation;
                trans.TranslationValue = dockCity.Position + Transformation.Up * currentHeight;


                Matrix invTrans = Transformation;
                invTrans.Invert();

                Matrix.Multiply(ref  trans, ref invTrans, out trans);

                if (dockCity.Owner == null)
                {
                    ops = green_tail.GetRenderOperation();
                    for (int i = 0; i < ops.Length; i++)
                    {
                        ops[i].Transformation = trans * ops[i].Transformation;
                    } 
                    if (ops != null)
                    {
                        opBuffer.Add(ops);
                    }
                }
                else
                {
                    ops = red_tail.GetRenderOperation();
                    if (ops != null)
                    {
                        for (int i = 0; i < ops.Length; i++)
                        {
                            ops[i].Transformation = trans * ops[i].Transformation;
                        } 
                        opBuffer.Add(ops);
                    }
                }


                opBuffer.Trim();
                return opBuffer.Elements;
            }
        }
        public override RenderOperation[] GetRenderOperation(int level)
        {
            return GetRenderOperation();
        }

        /// <summary>
        ///  Used to calculate orbit moving angle
        /// </summary>
        void IntegrateRoundAngle(float dt)
        {
            float mod = Type == RBallType.Education || Type == RBallType.Volience ? 2 : 1;
            float sign = state == RBallState.AttackCity ? -1 : 1;
            roundAngle += mod * sign * dt * speedModifier * refrenceLinSpeed / currentRadius;
            if (roundAngle > MathEx.PIf * 2)
                roundAngle -= MathEx.PIf * 2;
        }





        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTimeSeconds;

            if (weaponCoolDown > 0)
                weaponCoolDown -= dt;

            // accelerating and decelerating
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

                        // stand by balls can contribute their docking city
                        if (dockCity.Owner != null)
                        {
                            if (dockCity.Owner == Owner)
                            {
                                dockCity.Heal(dt * props.Heal, props.HealthIncr * dt);
                                dockCity.Develop(props.Contribution, dt);
                            }
                            else
                            {
                                dockCity.Develop(-props.Contribution * 5, dt);
                            }
                        }
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
                        // attack when city not owned
                        if (dockCity.Owner != Owner)
                        {
                            IntegrateRoundAngle(dt);

                            Vector3 pos;
                            CalculateRoundTransform(CityAttackRadius, true, out pos, out orientation);
                            Position = pos;

                            //tail.Update(gameTime, position, orientation.Forward);

                            if (speedModifier >= AttackCitySpeedMod - 0.5f)
                            {
                                if (weaponCoolDown <= 0)
                                {
                                    float dmg = props.Damage;
                                    dockCity.Damage(dmg, Owner);
                                    Damage(dmg * 0.075f);
                                    weaponCoolDown = WeaponCoolDownTime;
                                }
                            }
                        }
                        else
                        {
                            // when owned, check if the speed has decreased low enough 
                            if (speedModifier < AttackCitySpeedMod * 0.3f || speedModifier < MinSpeedMod)
                            {
                                // if so, go back to normal standby position
                                Reposition();
                            }
                            else
                            {
                                // otherwise, keep moveing, decelerating
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
                                    float dmg = props.Damage;
                                    target.Damage(dmg);
                                    weaponCoolDown = WeaponCoolDownTime;
                                }
                            }
                        }
                        break;
                    }
                case RBallState.Float:
                    {
                        NewMoveUpdate(dt);

                        float dist = Vector3.DistanceSquared(position, floatingTarget.Position);
                       
                        if (dist < City.CityOutterRadius * City.CityOutterRadius)
                        {
                            Free(floatingTarget);                            
                        }

                        break;
                    }
            }
            base.Update(gameTime);

            // try attack if any attackable, and when in a suitable state
            if (dockCity != null &&
                (state != RBallState.Gathered && state != RBallState.Gathering && state != RBallState.Free && state != RBallState.Float))
            {
                if (dockCity.Owner != Owner )
                {
                    // In an enemy city
                    enemyCheckTime -= dt;
                    if (enemyCheckTime <= 0 && props.Damage > 10)
                    {
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
                            if (props.Damage > 10)
                            {
                                // defend if any
                                if (dockCity.NearbyEnemyBallCount > 0)
                                {
                                    if (state != RBallState.Attack)
                                        Defend();
                                }
                            }                            
                            break;
                        case RBallType.Health:
                        case RBallType.Disease:
                            // These type of balls
                            // defend if able to attack and the city is in good condition
                            if (dockCity.HPRate > 1 - float.Epsilon && props.Damage > 10)
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


            if (shouldPlayDeathSonds) 
            {
                soundObject.Position = position;
                soundObject.Update(gameTime);
                soundObject.Fire();
                shouldPlayDeathSonds = false;
            }


        }
    }
}
