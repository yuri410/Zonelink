using System;
using System.Collections.Generic;
using System.Text;
using Code2015.EngineEx;
using Code2015.World;
using Apoc3D.Collections;
using Apoc3D;
using Apoc3D.Scene;
using Apoc3D.MathLib;
using Code2015.Logic;
using Apoc3D.Config;
using Apoc3D.Graphics;
using Apoc3D.Vfs;
using Apoc3D.Graphics.Animation;

namespace Code2015.World
{
    /// <summary>
    /// 城市的类型的标识
    /// </summary>
    enum CityType
    {
        /// <summary>
        /// 中性城市，无功能
        /// </summary>
        Neutral,

        Oil,
        Disease,
        Volience,
        Green,
        Health,
        Education
    }

    enum CityState
    {
        Stopped,
        Throw,
        Catch,
        Idle,
        WakeingUp,
        Laugh,
        Fear,
        Sleeping,
        Rotate,
        WaitingGather
    }

    enum CityRotationPurpose 
    {
        None,
        Throw,
        Receive
    }
    delegate void CityVisibleHander(City obj);    

    /// <summary>
    ///  表示游戏世界中的城市
    ///  是特殊的类型才继承，比如那些带矿车的
    /// </summary>
    class City : WorldObject, ISelectableObject
    {

        public const float CityRadiusDeg = 3.5f;

        /// <summary>
        ///  城市底座所占圆的半径
        /// </summary>
        public const float CityRadius = Game.ObjectScale * 100;

        /// <summary>
        ///  城市所属圈的半径
        /// </summary>
        public const float CityOutterRadius = CityRadius + Game.ObjectScale * 15;

        /// <summary>
        ///  城市选择圈的半径
        /// </summary>
        public const float CitySelRingScale = 2.35f;


        protected BattleField battleField;

        FastList<RenderOperation> opBuffer = new FastList<RenderOperation>();

        bool isVisible;
        int visibleCountDown = 10;

        Model cityBase;

        Model stopped;
        Model throwing;
        Model catching;
        Model idle;
        Model wakeingUp;
        Model laugh;
        Model fear;
        Model sleeping;

        /// <summary>
        ///  是否正在等待当前动画完成
        /// </summary>
        bool isWaitingAnimation;

        #region Idle_State
        float nextIdleAnimationCD;

        #endregion

        #region Throw_State

        RGatheredBall rgball;
        RBallType typeToThrow;
        bool isTypedThrow;
        List<City> throwPath;

        #endregion

        Quaternion currentFacing;
        Quaternion rotationSrc;
        Quaternion rotationTarget;
        float rotationTime;
        float remainingRotationTime;

        #region RotationPurpose
        CityRotationPurpose rotationPurpose;
        #endregion

        CityState currentState;



        string[] linkableCityName;
        FastList<City> linkableCity = new FastList<City>();


        SoundObject sound;

        //城市发展速度
        float developStep;
        float development;
        float healthValue;


        public City GetLinkableCity(int i)
        {
            return linkableCity[i];
        }
        public int LinkableCityCount
        {
            get { return linkableCity.Count; }
        }
        public CityState CityState
        { 
            get { return currentState; }
        }

        public float CatchPreserveTime 
        {
            get { return catching.SkinAnimDuration; }
        }

        public Quaternion CurrentFacing 
        {
            get { return currentFacing; }
        }

        //城市类型
        public CityType Type { get; protected set; }


        /// <summary>
        ///  获取城市的名称
        /// </summary>
        public string Name { get; set; }


        public float HealthValue { get { return healthValue; } }

        /// <summary>
        ///  获取城市的发展度
        /// </summary>
        public float Development { get { return development; } }

        public float HPRate
        {
            get { return healthValue / (development * RulesTable.CityDevHealthRate); }
        }
    
        
        /// <summary>
        ///  城市附近敌我的资源球
        /// </summary>
        protected List<RBall> nearbyBallList = new List<RBall>();
        
        /// <summary>
        ///  是否有被玩家占领
        /// </summary>
        public bool IsCaptured
        {
            get { return Owner != null; }
        }


        public bool IsHomeCity
        {
            get;
            set;
        }
        /// <summary>
        ///  待选起始点
        /// </summary>
        public int StartUp 
        {
            get;
            private set;
        }
        /// <summary>
        ///  友好城市之间判断城市是否有空
        /// </summary>
        public bool IsIdle
        {
            get { return currentState == CityState.Stopped; }
        }

        public event CityVisibleHander CityVisible;


        public City(BattleField btfld, Player owner, CityType type)
            : base(owner)
        {
            this.battleField = btfld;
            this.Type = type;

            
            BoundingSphere.Radius = CityRadius;
        }

        public override void InitalizeGraphics(RenderSystem rs) 
        {
            FileLocation fl = FileSystem.Instance.Locate("citybase.mesh", GameFileLocs.Model);
            cityBase = new Model(ModelManager.Instance.CreateInstance(rs, fl));
            cityBase.CurrentAnimation.Clear();
            cityBase.CurrentAnimation.Add(new NoAnimaionPlayer(Matrix.Scaling(2, 2, 0.25f) * Matrix.RotationX(-MathEx.PiOver2)));

            switch (Type) 
            {
                case CityType.Oil:
                    break;
                case CityType.Neutral:
                    break;
            }

            NoAnimaionPlayer scaling = new NoAnimaionPlayer(Matrix.Scaling(0.67f, 0.67f, 0.67f) ) ;
            fl = FileSystem.Instance.Locate("oil_catch.mesh", GameFileLocs.Model);
            catching = new Model(ModelManager.Instance.CreateInstance(rs, fl));
            catching.AnimationCompeleted += Animation_Completed;
            catching.CurrentAnimation.Insert(0, scaling);
          
            fl = FileSystem.Instance.Locate("oil_throw.mesh", GameFileLocs.Model);
            throwing = new Model(ModelManager.Instance.CreateInstance(rs, fl));
            throwing.AnimationCompeleted += Animation_Completed;
            throwing.CurrentAnimation.Insert(0, scaling);

            fl = FileSystem.Instance.Locate("oil_fear.mesh", GameFileLocs.Model);
            fear = new Model(ModelManager.Instance.CreateInstance(rs, fl));
            fear.AnimationCompeleted += Animation_Completed;
            fear.CurrentAnimation.Insert(0, scaling);

            fl = FileSystem.Instance.Locate("oil_idle.mesh", GameFileLocs.Model);
            idle = new Model(ModelManager.Instance.CreateInstance(rs, fl));
            idle.AnimationCompeleted += Animation_Completed;
            idle.CurrentAnimation.Insert(0, scaling);

            fl = FileSystem.Instance.Locate("oil_laugh.mesh", GameFileLocs.Model);
            laugh = new Model(ModelManager.Instance.CreateInstance(rs, fl));
            laugh.AnimationCompeleted += Animation_Completed;
            laugh.CurrentAnimation.Insert(0, scaling);

            fl = FileSystem.Instance.Locate("oil_sleeping.mesh", GameFileLocs.Model);
            sleeping = new Model(ModelManager.Instance.CreateInstance(rs, fl));
            sleeping.AnimationCompeleted += Animation_Completed;
            sleeping.CurrentAnimation.Insert(0, scaling);
            sleeping.AutoLoop = true;

            fl = FileSystem.Instance.Locate("oil_stopped.mesh", GameFileLocs.Model);
            stopped = new Model(ModelManager.Instance.CreateInstance(rs, fl));
            stopped.AnimationCompeleted += Animation_Completed;
            stopped.CurrentAnimation.Insert(0, scaling);
            stopped.AutoLoop = true;

            fl = FileSystem.Instance.Locate("oil_wakeup.mesh", GameFileLocs.Model);
            wakeingUp = new Model(ModelManager.Instance.CreateInstance(rs, fl));
            wakeingUp.AnimationCompeleted += Animation_Completed;
            wakeingUp.CurrentAnimation.Insert(0, scaling);

            ChangeState(CityState.Sleeping);

            sound = SoundManager.Instance.MakeSoundObjcet("city", null, CityRadius * 2);
            sound.Position = Position;
        }

        int Camparision(NaturalResource a, NaturalResource b)
        {
            float da = Vector3.DistanceSquared(a.Position, this.Position);
            float db = Vector3.DistanceSquared(b.Position, this.Position);
            return da.CompareTo(db);
        }

        /// <summary>
        ///  城市自然发展。根据dt，增加发展量
        /// </summary>
        /// <param name="dt"></param>
        private void NaturalDevelop(float dt)
        {
            float healthRate = (this.HealthValue / this.Development);

            development += dt * developStep;
            if (development > RulesTable.CityMaxDevelopment)
                development = RulesTable.CityMaxDevelopment;
            healthValue = healthRate * this.Development;
        }
        private void Develop(float amount, float dt)
        {
            float healthRate = HPRate;


            development += amount * dt;
            if (development > RulesTable.CityMaxDevelopment)
                development = RulesTable.CityMaxDevelopment;
            healthValue = healthRate * development;
        }

        public void Damage(float v, Player owener)
        {
            healthValue -= v;
            if (healthValue < 0)
            {
                healthValue = this.Development;
                this.Owner = owener;
            }
        }

        public virtual void ChangeOwner(Player player)
        {
            if (IsCaptured && player == null)
            {
                Owner.Area.NotifyLostCity(this);
            }
            this.Owner = player;

            if (player != null)
            {
                Owner.Area.NotifyNewCity(this);
                ChangeState(CityState.WakeingUp);
            }            
        }


        public void ResolveCities(Dictionary<string, City> table)
        {
            if (linkableCityName == null)
                return;
            for (int i = 0; i < linkableCityName.Length; i++)
            {
                City city = table[linkableCityName[i]];
                linkableCity.Add(city);
            }
        }

        public static CityType ParseType(string typeString)
        {
            string typestr = typeString.ToLowerInvariant();

            CityType type = CityType.Neutral;
            if (typestr == CityType.Oil.ToString().ToLowerInvariant())
            {
                type = CityType.Oil;

            }
            else if (typestr == CityType.Green.ToString().ToLowerInvariant())
            {
                type = CityType.Green;
            }
            else if (typestr == CityType.Neutral.ToString().ToLowerInvariant())
            {
                type = CityType.Neutral;
            }
            else if (typestr == CityType.Volience.ToString().ToLowerInvariant())
            {
                type = CityType.Volience;
            }
            else if (typestr == CityType.Health.ToString().ToLowerInvariant())
            {
                type = CityType.Health;
            }
            else if (typestr == CityType.Disease.ToString().ToLowerInvariant())
            {
                type = CityType.Disease;
            }
            else if (typestr == CityType.Education.ToString().ToLowerInvariant())
            {
                type = CityType.Education;
            }
            return type;
        }

        public override void Parse(GameConfigurationSection sect)
        {
            base.Parse(sect);
            this.Name = sect.GetString("Name", string.Empty);

            StartUp = sect.GetInt("StartUp", -1);

            linkableCityName = sect.GetStringArray("Linkable", null);

            development = sect.GetSingle("InitialDevelopment", RulesTable.CityInitialDevelopment);
            healthValue = development * RulesTable.CityDevHealthRate;

            //设置城市类型
            switch (Type) 
            {
                case CityType.Neutral:
                    developStep = 20;
                    break;
                case CityType.Health:
                    developStep = RulesTable.HealthDevelopStep;
                    break;
                case CityType.Green:
                    developStep = RulesTable.GreenDevelopStep;
                    break;
                case CityType.Education:
                    developStep = RulesTable.EducationDevelopStep;
                    break;
                case CityType.Disease:
                    developStep = RulesTable.DiseaseDevelopStep;
                    break;
                case CityType.Oil:
                    developStep = RulesTable.OilDevelopStep;
                    break;
                case CityType.Volience:
                    developStep = RulesTable.VolienceDevelopStep;
                    break;
            }


            float facing = MathEx.PIf + (Randomizer.GetRandomSingle() - 0.5f) * MathEx.PiOver2;
            //currentFacing =  MathEx.PIf * (5f / 6 + Randomizer.GetRandomSingle() / 3);
            currentFacing = Quaternion.RotationAxis(Vector3.UnitY, facing);

            UpdateLocation();
        }

        private void UpdateLocation()
        {
            float radLong = MathEx.Degree2Radian(this.Longitude);
            float radLat = MathEx.Degree2Radian(this.Latitude);

            float altitude = TerrainData.Instance.QueryHeight(radLong, radLat);
            this.Position = PlanetEarth.GetPosition(radLong, radLat, PlanetEarth.PlanetRadius + TerrainMeshManager.PostHeightScale * altitude + 5);

            this.Transformation = PlanetEarth.GetOrientation(radLong, radLat);
            //this = Matrix.Invert(Transformation);

            this.Transformation.TranslationValue = this.Position; // TranslationValue = pos;

            BoundingSphere.Radius = CityRadius;
            BoundingSphere.Center = this.Position;
        }
        
        void RotateTo(Quaternion angle, float time)
        {
            ChangeState(CityState.Rotate);
            rotationSrc = currentFacing;
            rotationTarget = angle;
            rotationTime = time;
            remainingRotationTime = time;
        }
        void RotateTo(float angle, float time) 
        {
            ChangeState(CityState.Rotate);
            rotationSrc = currentFacing;
            rotationTarget = Quaternion.RotationAxis(Vector3.UnitY, angle);
            rotationTime = time;
            remainingRotationTime = time;
        }
        void SetRotationPurpose(CityRotationPurpose porpose) { rotationPurpose = porpose; }
        
        void Animation_Completed(object sender, AnimationCompletedEventArgs e)
        {

            switch (currentState)
            {
                case CityState.Catch:
                    ChangeState(CityState.Stopped);
                    break;
                case CityState.Fear:
                    ChangeState(CityState.Stopped); 
                    break;
                case CityState.Idle: // 每次Idle动画播放完后都先转到Stopped
                    ChangeState(CityState.Stopped);
                    break;
                case CityState.Laugh:
                    ChangeState(CityState.Stopped);
                    break;
                case CityState.Throw:
                    ChangeState(CityState.Stopped);
                    break;
                case CityState.WakeingUp:
                    ChangeState(CityState.Stopped);
                    break;
            }

            //currentStateEnded = true;
        }


        public Quaternion GetOrientation(Vector3 pb) 
        {
            Matrix invTransform;
            Matrix.Invert(ref Transformation, out invTransform);


            Matrix worldFacing = Matrix.Identity;
            worldFacing.Right = Vector3.Normalize(position - pb);
            worldFacing.Up = Transformation.Up;
            worldFacing.Forward = Vector3.Normalize(Vector3.Cross(Transformation.Up, worldFacing.Right));
            worldFacing.Right = Vector3.Cross(worldFacing.Forward, Transformation.Up);

            worldFacing = Matrix.RotationY(-MathEx.PiOver2) * worldFacing;

            Matrix.Multiply(ref worldFacing, ref invTransform, out worldFacing);

            Quaternion targetRot = Quaternion.RotationMatrix(worldFacing);
            return targetRot;
        }


        public void NotifyResourceBallMoveIn(RBall ball) 
        {
            nearbyBallList.Add(ball);
        }
        public void NotifyResourceBallMoveOut(RBall ball)
        {
            nearbyBallList.Remove(ball);
        }

        public void NotifyIncomingBall(RGatheredBall rgball)
        {
            if (currentState == CityState.Sleeping)
            {
                ChangeState(CityState.WakeingUp);
            }
            else
            {
                Quaternion targetRot = GetOrientation(rgball.Position);
                RotateTo(targetRot, 0.5f);
                SetRotationPurpose(CityRotationPurpose.Receive);
            }
        }

        void ReceiveNow() 
        {
            ChangeState(CityState.Catch);
        }

        void ThrowNow()
        {
            if (throwPath == null)
            {
                throw new InvalidOperationException();
            }
            List<RBall> toThrow = new List<RBall>(nearbyBallList.Count);
            if (isTypedThrow)
            {
                for (int i = 0; i < nearbyBallList.Count; i++)
                {
                    if (nearbyBallList[i].Owner == Owner && nearbyBallList[i].Type == typeToThrow)
                    {
                        toThrow.Add(nearbyBallList[i]);
                    }
                }
            }
            else
            {

                for (int i = 0; i < nearbyBallList.Count; i++)
                {
                    if (nearbyBallList[i].Owner == Owner)
                    {
                        toThrow.Add(nearbyBallList[i]);
                    }
                }
            }

            if (toThrow.Count == 0) 
            {
                return;
            }
            rgball = battleField.CreateRGatherBall(toThrow, this, throwPath);
            throwPath = null;
            ChangeState(CityState.WaitingGather);
        }
        public void Throw(City target)
        {            
            battleField.BallPathFinder.Reset();
            BallPathFinderResult result = battleField.BallPathFinder.FindPath(this, target);

            if (result != null)
            {
                throwPath = new List<City>(result.NodeCount);
                for (int i = 0; i < result.NodeCount; i++) 
                {
                    throwPath.Add(result[i]);
                }                

                Quaternion targetRot = GetOrientation(result[0].Position); 

                RotateTo(targetRot, 0.5f);
                SetRotationPurpose(CityRotationPurpose.Throw);
                isTypedThrow = false;
            }
        }
        public void Throw(City target, RBallType type)
        {
            battleField.BallPathFinder.Reset();
            BallPathFinderResult result = battleField.BallPathFinder.FindPath(this, target);

            if (result != null)
            {
                throwPath = new List<City>(result.NodeCount);
                for (int i = 0; i < result.NodeCount; i++)
                {
                    throwPath.Add(result[i]);
                }

                Quaternion targetRot = GetOrientation(result[0].Position); 

                RotateTo(targetRot, 0.5f);
                SetRotationPurpose(CityRotationPurpose.Throw);
                isTypedThrow = true;
                typeToThrow = type;
            }
        }

        private void ChangeState(CityState state) 
        {
            switch (state) 
            {
                case CityState.Catch:
                    catching.PlayAnimation();
                    break;
                case CityState.Fear:
                    break;
                case CityState.Idle:
                    idle.PlayAnimation();
                    break;
                case CityState.Laugh:
                    break;
                case CityState.Stopped:
                    stopped.PlayAnimation();

                    if (currentState == CityState.WakeingUp)
                    {
                        wakeingUp.PauseAnimation();
                    }
                    break;
                case CityState.Throw:
                    throwing.PlayAnimation();
                    rgball.NotifyThrow();
                    rgball = null;
                    break;
                case CityState.WakeingUp:
                    wakeingUp.PlayAnimation();

                    if (currentState == CityState.Sleeping)
                    {
                        sleeping.PauseAnimation();
                    }
                    break;
                case CityState.Sleeping:
                    sleeping.PlayAnimation();
                    break;
                case CityState.WaitingGather:
                    catching.PlayAnimation();
                    break;
            }
            currentState = state;
        }
        private void UpdateState(GameTime time)
        {
            float dt = time.ElapsedGameTimeSeconds;


            switch (currentState)
            {
                case CityState.Rotate:

                    remainingRotationTime -= dt;
                    if (remainingRotationTime < 0)
                    {
                        if (rotationPurpose == CityRotationPurpose.Throw)
                        {
                            ThrowNow();
                        }
                        else if (rotationPurpose == CityRotationPurpose.Receive) 
                        {
                            ReceiveNow();
                        }
                        else
                        {
                            ChangeState(CityState.Stopped);
                        }
                        rotationPurpose = CityRotationPurpose.None;
                    }
                    float progress = MathEx.Saturate(1 - remainingRotationTime / rotationTime);

                    currentFacing = Quaternion.Slerp(rotationSrc, rotationTarget, progress);

                    if (isVisible) stopped.Update(time);
                    break;
                case CityState.Catch:
                    catching.Update(time); 
                    break;
                case CityState.Fear:
                    if (isVisible) fear.Update(time); 
                    break;
                case CityState.Idle:
                    if (isVisible) idle.Update(time);
                    break;
                case CityState.Laugh:
                    if (isVisible) laugh.Update(time); 
                    break;
                case CityState.Stopped:
                    if (nextIdleAnimationCD > 0)
                    {
                        nextIdleAnimationCD -= dt;
                    }
                    else
                    {
                        nextIdleAnimationCD = Randomizer.GetRandomSingle() * 3 + 2.5f;

                        bool goIdle = Randomizer.GetRandomBool();
                        
                        if (goIdle)
                        {
                            ChangeState(CityState.Idle);
                        }
                        else 
                        {
                            float rotTime = Randomizer.GetRandomSingle() * 0.5f + 0.5f;

                            float facingChange = Randomizer.GetRandomSingle() * MathEx.PIf - MathEx.PiOver2;
                            Quaternion nextFacing = currentFacing * Quaternion.RotationAxis(Vector3.UnitY, facingChange);

                            RotateTo(nextFacing, rotTime);
                        }

                    }
                    if (isVisible) stopped.Update(time);
                    break;
                case CityState.Throw:

                    throwing.Update(time);
                    break;
                case CityState.WakeingUp:
                    wakeingUp.Update(time);
                    break;
                case CityState.Sleeping:
                    if (isVisible) sleeping.Update(time);
                    break;
                case CityState.WaitingGather:
                    if (rgball.CurrentState == RGatheredBall.State.WaitingThrow) 
                    {
                        ChangeState(CityState.Throw);
                    }
                    catching.Update(time);
                    break;
            }

        }

        public override void Update(GameTime dt)
        {
            float ddt = (float)dt.ElapsedGameTimeSeconds;

            if (Owner != null)
            {
                NaturalDevelop(ddt);

                float devIncr = 0;
                // 计算附近同阵营资源球贡献发展量
                for (int i = 0; i < nearbyBallList.Count; i++)
                {
                    devIncr += ddt;// *Utils.GetRBallContribution(nearbyBallList[i].Type);
                }
            }

            if (isVisible)
            {
                visibleCountDown--;
                if (visibleCountDown < 0)
                {
                    visibleCountDown = 10;
                    isVisible = false;
                }
            }

            UpdateState(dt);

            // 城内资源球AI
        }




        public virtual void ProduceBall()
        {
            
        }



        public override RenderOperation[] GetRenderOperation()
        {
            isVisible = true;
            opBuffer.FastClear();
            if (CityVisible != null)
            {
                CityVisible(this);
            }


            RenderOperation[] ops = cityBase.GetRenderOperation();
            if (ops != null)
            {
                opBuffer.Add(ops);
            }

            ops = null;
            switch (currentState)
            {
                case CityState.Catch:
                    ops = catching.GetRenderOperation();
                    break;
                case CityState.Fear:
                    ops = fear.GetRenderOperation();
                    break;
                case CityState.Idle:
                    ops = idle.GetRenderOperation();
                    break;
                case CityState.Laugh:
                    ops = laugh.GetRenderOperation();
                    break;
                case CityState.Rotate:
                    if (rgball != null  && rotationPurpose != CityRotationPurpose.Receive)
                        ops = catching.GetRenderOperation();
                    else
                        ops = stopped.GetRenderOperation();
                    break;
                case CityState.Stopped:
                case CityState.WaitingGather:
                    ops = stopped.GetRenderOperation();
                    break;
                case CityState.Throw:
                    ops = throwing.GetRenderOperation();
                    break;
                case CityState.WakeingUp:
                    ops = wakeingUp.GetRenderOperation();
                    break;
                case CityState.Sleeping:
                    ops = sleeping.GetRenderOperation();
                    break;                        
            }

            Matrix currentFacingMatrix = Matrix.RotationQuaternion(currentFacing);
            
            if (ops != null)
            {
                for (int i = 0; i < ops.Length; i++)
                {
                    ops[i].Transformation *= currentFacingMatrix;                    
                }
                opBuffer.Add(ops);
            }

            opBuffer.Trim();
            return opBuffer.Elements;              
        }


        #region ISelectableObject 成员

        public bool IsSelected
        {
            get;
            set;
        }

        #endregion
    }
}
