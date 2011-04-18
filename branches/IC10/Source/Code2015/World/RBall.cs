using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Code2015.Logic;
using Apoc3D.MathLib;
using Apoc3D.Graphics;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Apoc3D.Graphics.Animation;

namespace Code2015.World
{
    enum RBallState 
    {
        Standby,
        Gathering,
        Attack,
        Gathered,
        Free,        
    }

    class RGatheredBall 
    {
        const float GBallRadius = 30;

        public enum State
        {
            WaitingGathering,
            WaitingThrow,
            Flying,
            WaitingPass
        }

        List<City> goPath;
        State state;
        List<RBall> subBalls;
        Vector3 position;

        public Vector3 Position 
        {
            get { return position; }
        }
        public State CurrentState 
        {
            get { return state; }
        }

        public RGatheredBall(List<RBall> balls, City dockCity, List<City> path)
        {
            state = State.WaitingGathering;
            subBalls = balls;
            goPath = path;


            Vector4 dir = Vector3.Transform(Vector3.UnitZ, dockCity.CurrentFacing);

            Vector3 offset = new Vector3(dir.X, dir.Y, dir.Z);
            offset *= City.CityRadius;
            offset.Y += 100;

            offset = Vector3.TransformNormal(offset, dockCity.Transformation);

            int lineCount = (int)Math.Ceiling(Math.Sqrt(balls.Count));
            float span = (MathEx.PIf * 2) / lineCount;

            for (int i = 0; i < balls.Count; i++)
            {
                int row = i / lineCount;
                int col = i % lineCount;

                float radLng = col * span;
                float radLat = row * span;

                Vector3 positionInGBall = PlanetEarth.GetPosition(radLng, radLat, GBallRadius);
                Quaternion oriInGBall = Quaternion.RotationMatrix(PlanetEarth.GetOrientation(radLng, radLat));
                balls[i].Gather(this, positionInGBall, oriInGBall);
            }
        }

        void NotifyThrow() 
        {

        }

        void ChangeState(State newState) 
        {
            state = newState;
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
        const float PositioningSpeed = 10;
        

        public RBallType Type { get; private set; }
        
        private City dockCity;
        public City DockCity { get { return this.dockCity; } }


        RBallState state;


        #region State_Positioning
        
        float positioningProgress;
        float positioningDistance;
        Vector3 positioningEndPosition;
        Vector3 positioningStartPosition;
        Quaternion positioningStartOri;
        Quaternion positioningEndOri;
        #endregion

        RGatheredBall gatheredParent;



        /// <summary>
        /// 在城=城市间移动的速度
        /// </summary>
        private Vector3 velocity;




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
            this.dockCity = city;
            this.Type = type;
            //设置血量
            this.Health = city.Development * 0.15f;

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
            if (positioningProgress < 0.5f)
            {
                Position = Vector3.Lerp(positioningStartPosition, positioningEndPosition, positioningProgress * 2);

                positioningProgress += (PositioningSpeed / positioningDistance) * dt;
            }
            else if (positioningProgress < 1)
            {
                Orientation = Matrix.RotationQuaternion(
                    Quaternion.Slerp(positioningStartOri, positioningEndOri, (positioningProgress - 0.5f) * 2));
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
            positioningDistance = Vector3.Distance(ref positioningStartPosition, ref positioningEndPosition);
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
            }
            base.Update(gameTime);

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
