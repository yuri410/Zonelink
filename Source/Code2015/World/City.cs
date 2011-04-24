using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Config;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Animation;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.Logic;
using Code2015.World;

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
        ThrowContinued,
        Catch,
        PostCatch,
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
        Receive,
        ThrowContinued,
        Fear
    }
    delegate void CityVisibleHander(City obj);

    /// <summary>
    ///  表示游戏世界中的城市
    ///  是特殊的类型才继承，比如那些带矿车的
    /// </summary>
    class City : WorldObject, ISelectableObject
    {
        struct Yangbing
        {
            public const float SchoolPreseveTime = (62.0f / 30.0f) * (30.0f / 62.0f);
            public const float GunPreseveTime = (68.0f / 30.0f) * (29.0f / 68.0f);
            public const float HospitalPreseveTime = (29.0f / 30.0f) * (15.0f / 29.0f);
            public const float FactoryPreseveTime = (44.0f / 30.0f) * (26.0f / 44.0f);
        }

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

        int currentForm;

        Smokes smoke;

        Model cityBase;

        Model[] stopped;
        Model[] throwing;
        Model[] throwingPrepare;
        Model[] catching;
        Model[] catchingRelease;
        Model[] idle;
        Model[] wakeingUp;
        Model[] laugh;
        Model[] fear;
        Model[] sleeping;

        CityType cityType;


        #region Idle_State
        float nextIdleAnimationCD;

        #endregion

        float reThrowDelay;

        #region Throw_State

        RGatheredBall rgball;
        RBallType typeToThrow;
        bool isTypedThrow;
        List<City> throwPath;

        #endregion
        
        #region ThrowCont_State
        List<RBall> ballsToThrowCont;
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

        //SoundObject 


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
            get { return catching[currentForm].SkinAnimDuration; }
        }

        public Quaternion CurrentFacing
        {
            get { return currentFacing; }
        }

        //城市类型
        public CityType Type
        {
            get { return cityType; }
            protected set
            {
                cityType = value;
            }
        }


        /// <summary>
        ///  获取城市的名称
        /// </summary>
        public string Name { get; set; }


        public float HealthValue { get { return healthValue; } }


        /// <summary>
        ///  获取城市的发展等级
        /// </summary>
        public int Level 
        {
            get
            {
                float ratio = development / RulesTable.CityMaxDevelopment;

                return (int)(Math.Floor(ratio * 10));
            }
        }
        /// <summary>
        ///  获取城市经验值，在[0, 1]区间
        /// </summary>
        public float LevelEP
        {
            get
            {
                float ratio = development / RulesTable.CityMaxDevelopment;
                ratio *= 10;
                ratio = (float)(ratio - Math.Floor(ratio));

                return ratio;
            }
        }

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
        FastList<RBall> nearbyEnemyBalls = new FastList<RBall>();
        FastList<RBall> nearbyOwnedBalls = new FastList<RBall>();

        public int NearbyEnemyBallCount { get { return nearbyEnemyBalls.Count; } }
        public int NearbyOwnedBallCount { get { return nearbyOwnedBalls.Count; } }

        public RBall GetNearbyEnemyBall(int i)
        {
            return nearbyEnemyBalls[i];
        }
        public RBall GetNearbyOwnedBall(int i)
        {
            return nearbyOwnedBalls[i];
        }
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

        public bool CanProduceProduction()
        {
            return Type != CityType.Neutral;
        }

        public RBallType GetProductionType()
        {
            switch (Type)
            {
                case CityType.Oil:
                    return RBallType.Oil;
                case CityType.Health:
                    return RBallType.Health;
                case CityType.Disease:
                    return RBallType.Disease;
                case CityType.Education:
                    return RBallType.Education;
                case CityType.Volience:
                    return RBallType.Volience;
                case CityType.Green:
                    return RBallType.Green;
            }
            throw new InvalidOperationException();
        }

        public virtual float GetProductionProgress() 
        {
            throw new NotImplementedException();
        }


        public event CityVisibleHander CityVisible;


        public City(BattleField btfld, Player owner, CityType type)
            : base(owner)
        {
            this.battleField = btfld;
            this.Type = type;


            BoundingSphere.Radius = CityRadius;
        }

        public float GetDevelopmentStep()
        { 
            //设置城市类型
            switch (Type)
            {
                case CityType.Neutral:
                    return  20;                    
                case CityType.Health:
                    return RulesTable.HealthDevelopStep;
                case CityType.Green:
                    return RulesTable.GreenDevelopStep;                    
                case CityType.Education:
                    return RulesTable.EducationDevelopStep;                    
                case CityType.Disease:
                    return RulesTable.DiseaseDevelopStep;                    
                case CityType.Oil:
                    return RulesTable.OilDevelopStep;
                case CityType.Volience:
                    return RulesTable.VolienceDevelopStep;
            }
            throw new InvalidOperationException();
        }
        public override void InitalizeGraphics(RenderSystem rs)
        {
            FileLocation fl = FileSystem.Instance.Locate("citybase.mesh", GameFileLocs.Model);
            cityBase = new Model(ModelManager.Instance.CreateInstance(rs, fl));
            cityBase.CurrentAnimation.Clear();
            cityBase.CurrentAnimation.Add(new NoAnimaionPlayer(Matrix.Scaling(2, 2, 0.25f) * Matrix.RotationX(-MathEx.PiOver2)));


            catching = new Model[2];
            catchingRelease = new Model[2];
            throwing = new Model[2];
            throwingPrepare = new Model[2];
            fear = new Model[2];
            idle = new Model[2];
            laugh = new Model[2];
            sleeping = new Model[2];
            stopped = new Model[2];
            wakeingUp = new Model[2];

            

            switch (Type)
            {
                case CityType.Oil:
                case CityType.Green:
                    {
                        NoAnimaionPlayer scaling = new NoAnimaionPlayer(Matrix.Scaling(0.67f, 0.67f, 0.67f));
                        #region Oil
                        fl = FileSystem.Instance.Locate("ch_oil_catch.mesh", GameFileLocs.Model);
                        catching[0] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        catching[0].AnimationCompeleted += Animation_Completed;
                        catching[0].CurrentAnimation.Insert(0, scaling);

                        fl = FileSystem.Instance.Locate("ch_oil_catchrelease.mesh", GameFileLocs.Model);
                        catchingRelease[0] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        catchingRelease[0].AnimationCompeleted += Animation_Completed;
                        catchingRelease[0].CurrentAnimation.Insert(0, scaling);

                        fl = FileSystem.Instance.Locate("ch_oil_throwrelease.mesh", GameFileLocs.Model);
                        throwing[0] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        throwing[0].AnimationCompeleted += Animation_Completed;
                        throwing[0].CurrentAnimation.Insert(0, scaling);

                        fl = FileSystem.Instance.Locate("ch_oil_throw.mesh", GameFileLocs.Model);
                        throwingPrepare[0] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        throwingPrepare[0].AnimationCompeleted += Animation_Completed;
                        throwingPrepare[0].CurrentAnimation.Insert(0, scaling);


                        fl = FileSystem.Instance.Locate("ch_oil_fear.mesh", GameFileLocs.Model);
                        fear[0] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        fear[0].AnimationCompeleted += Animation_Completed;
                        fear[0].CurrentAnimation.Insert(0, scaling);

                        fl = FileSystem.Instance.Locate("ch_oil_idle.mesh", GameFileLocs.Model);
                        idle[0] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        idle[0].AnimationCompeleted += Animation_Completed;
                        idle[0].CurrentAnimation.Insert(0, scaling);

                        fl = FileSystem.Instance.Locate("ch_oil_laugh.mesh", GameFileLocs.Model);
                        laugh[0] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        laugh[0].AnimationCompeleted += Animation_Completed;
                        laugh[0].CurrentAnimation.Insert(0, scaling);

                        fl = FileSystem.Instance.Locate("ch_oil_sleeping.mesh", GameFileLocs.Model);
                        sleeping[0] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        sleeping[0].AnimationCompeleted += Animation_Completed;
                        sleeping[0].CurrentAnimation.Insert(0, scaling);
                        sleeping[0].AutoLoop = true;

                        fl = FileSystem.Instance.Locate("ch_oil_stopped.mesh", GameFileLocs.Model);
                        stopped[0] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        stopped[0].AnimationCompeleted += Animation_Completed;
                        stopped[0].CurrentAnimation.Insert(0, scaling);
                        stopped[0].AutoLoop = true;

                        fl = FileSystem.Instance.Locate("ch_oil_wakeup.mesh", GameFileLocs.Model);
                        wakeingUp[0] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        wakeingUp[0].AnimationCompeleted += Animation_Completed;
                        wakeingUp[0].CurrentAnimation.Insert(0, scaling);
                        #endregion

                        #region Green

                        fl = FileSystem.Instance.Locate("ch_green_catch.mesh", GameFileLocs.Model);
                        catching[1] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        catching[1].AnimationCompeleted += Animation_Completed;
                        catching[1].CurrentAnimation.Insert(0, scaling);
                        
                        fl = FileSystem.Instance.Locate("ch_green_catchrelease.mesh", GameFileLocs.Model);
                        catchingRelease[1] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        catchingRelease[1].AnimationCompeleted += Animation_Completed;
                        catchingRelease[1].CurrentAnimation.Insert(0, scaling);

                        fl = FileSystem.Instance.Locate("ch_green_throwrelease.mesh", GameFileLocs.Model);
                        throwing[1] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        throwing[1].AnimationCompeleted += Animation_Completed;
                        throwing[1].CurrentAnimation.Insert(0, scaling);

                        fl = FileSystem.Instance.Locate("ch_green_throw.mesh", GameFileLocs.Model);
                        throwingPrepare[1] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        throwingPrepare[1].AnimationCompeleted += Animation_Completed;
                        throwingPrepare[1].CurrentAnimation.Insert(0, scaling);

                        fl = FileSystem.Instance.Locate("ch_green_fear.mesh", GameFileLocs.Model);
                        fear[1] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        fear[1].AnimationCompeleted += Animation_Completed;
                        fear[1].CurrentAnimation.Insert(0, scaling);

                        fl = FileSystem.Instance.Locate("ch_green_idle.mesh", GameFileLocs.Model);
                        idle[1] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        idle[1].AnimationCompeleted += Animation_Completed;
                        idle[1].CurrentAnimation.Insert(0, scaling);

                        fl = FileSystem.Instance.Locate("ch_green_laugh.mesh", GameFileLocs.Model);
                        laugh[1] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        laugh[1].AnimationCompeleted += Animation_Completed;
                        laugh[1].CurrentAnimation.Insert(0, scaling);

                        fl = FileSystem.Instance.Locate("ch_green_sleeping.mesh", GameFileLocs.Model);
                        sleeping[1] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        sleeping[1].AnimationCompeleted += Animation_Completed;
                        sleeping[1].CurrentAnimation.Insert(0, scaling);
                        sleeping[1].AutoLoop = true;

                        fl = FileSystem.Instance.Locate("ch_green_stopped.mesh", GameFileLocs.Model);
                        stopped[1] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        stopped[1].AnimationCompeleted += Animation_Completed;
                        stopped[1].CurrentAnimation.Insert(0, scaling);
                        stopped[1].AutoLoop = true;

                        fl = FileSystem.Instance.Locate("ch_green_wakeup.mesh", GameFileLocs.Model);
                        wakeingUp[1] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        wakeingUp[1].AnimationCompeleted += Animation_Completed;
                        wakeingUp[1].CurrentAnimation.Insert(0, scaling);
                        #endregion
                        break;
                    }
                case CityType.Education:
                case CityType.Volience:
                case CityType.Disease:
                case CityType.Health:
                    {
                        NoAnimaionPlayer scaling = new NoAnimaionPlayer(Matrix.Scaling(50, 50, 50));
                        #region Disease
                        fl = FileSystem.Instance.Locate("ch_virus_catch.mesh", GameFileLocs.Model);
                        catching[0] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        catching[0].AnimationCompeleted += Animation_Completed;
                        catching[0].CurrentAnimation.Insert(0, scaling);

                        fl = FileSystem.Instance.Locate("ch_virus_throw.mesh", GameFileLocs.Model);
                        throwing[0] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        throwing[0].AnimationCompeleted += Animation_Completed;
                        throwing[0].CurrentAnimation.Insert(0, scaling);

                        fl = FileSystem.Instance.Locate("ch_virus_fear.mesh", GameFileLocs.Model);
                        fear[0] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        fear[0].AnimationCompeleted += Animation_Completed;
                        fear[0].CurrentAnimation.Insert(0, scaling);

                        fl = FileSystem.Instance.Locate("ch_virus_idle.mesh", GameFileLocs.Model);
                        idle[0] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        idle[0].AnimationCompeleted += Animation_Completed;
                        idle[0].CurrentAnimation.Insert(0, scaling);

                        fl = FileSystem.Instance.Locate("ch_virus_laugh.mesh", GameFileLocs.Model);
                        laugh[0] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        laugh[0].AnimationCompeleted += Animation_Completed;
                        laugh[0].CurrentAnimation.Insert(0, scaling);

                        fl = FileSystem.Instance.Locate("ch_virus_sleeping.mesh", GameFileLocs.Model);
                        sleeping[0] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        sleeping[0].AnimationCompeleted += Animation_Completed;
                        sleeping[0].CurrentAnimation.Insert(0, scaling);
                        sleeping[0].AutoLoop = true;

                        fl = FileSystem.Instance.Locate("ch_virus_stopped.mesh", GameFileLocs.Model);
                        stopped[0] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        stopped[0].AnimationCompeleted += Animation_Completed;
                        stopped[0].CurrentAnimation.Insert(0, scaling);
                        stopped[0].AutoLoop = true;

                        fl = FileSystem.Instance.Locate("ch_virus_wakeup.mesh", GameFileLocs.Model);
                        wakeingUp[0] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        wakeingUp[0].AnimationCompeleted += Animation_Completed;
                        wakeingUp[0].CurrentAnimation.Insert(0, scaling);
                        #endregion

                        #region Health

                        fl = FileSystem.Instance.Locate("ch_hospital_catch.mesh", GameFileLocs.Model);
                        catching[1] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        catching[1].AnimationCompeleted += Animation_Completed;
                        catching[1].CurrentAnimation.Insert(0, scaling);

                        fl = FileSystem.Instance.Locate("ch_hospital_throw.mesh", GameFileLocs.Model);
                        throwing[1] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        throwing[1].AnimationCompeleted += Animation_Completed;
                        throwing[1].CurrentAnimation.Insert(0, scaling);

                        fl = FileSystem.Instance.Locate("ch_hospital_fear.mesh", GameFileLocs.Model);
                        fear[1] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        fear[1].AnimationCompeleted += Animation_Completed;
                        fear[1].CurrentAnimation.Insert(0, scaling);

                        fl = FileSystem.Instance.Locate("ch_hospital_idle.mesh", GameFileLocs.Model);
                        idle[1] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        idle[1].AnimationCompeleted += Animation_Completed;
                        idle[1].CurrentAnimation.Insert(0, scaling);

                        fl = FileSystem.Instance.Locate("ch_hospital_laugh.mesh", GameFileLocs.Model);
                        laugh[1] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        laugh[1].AnimationCompeleted += Animation_Completed;
                        laugh[1].CurrentAnimation.Insert(0, scaling);

                        fl = FileSystem.Instance.Locate("ch_hospital_sleeping.mesh", GameFileLocs.Model);
                        sleeping[1] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        sleeping[1].AnimationCompeleted += Animation_Completed;
                        sleeping[1].CurrentAnimation.Insert(0, scaling);
                        sleeping[1].AutoLoop = true;

                        fl = FileSystem.Instance.Locate("ch_hospital_stopped.mesh", GameFileLocs.Model);
                        stopped[1] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        stopped[1].AnimationCompeleted += Animation_Completed;
                        stopped[1].CurrentAnimation.Insert(0, scaling);
                        stopped[1].AutoLoop = true;

                        fl = FileSystem.Instance.Locate("ch_hospital_wakeup.mesh", GameFileLocs.Model);
                        wakeingUp[1] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        wakeingUp[1].AnimationCompeleted += Animation_Completed;
                        wakeingUp[1].CurrentAnimation.Insert(0, scaling);
                        #endregion
                        break;
                    }
                case CityType.Neutral:
                    {
                        NoAnimaionPlayer scaling = new NoAnimaionPlayer(Matrix.Scaling(0.67f, 0.67f, 0.67f));

                        #region Neutral
                        fl = FileSystem.Instance.Locate("ch_neutral_catch.mesh", GameFileLocs.Model);
                        catching[0] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        catching[0].AnimationCompeleted += Animation_Completed;
                        catching[0].CurrentAnimation.Insert(0, scaling);

                        fl = FileSystem.Instance.Locate("ch_neutral_catchrelease.mesh", GameFileLocs.Model);
                        catchingRelease[0] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        catchingRelease[0].AnimationCompeleted += Animation_Completed;
                        catchingRelease[0].CurrentAnimation.Insert(0, scaling);

                        fl = FileSystem.Instance.Locate("ch_neutral_throwrelease.mesh", GameFileLocs.Model);
                        throwing[0] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        throwing[0].AnimationCompeleted += Animation_Completed;
                        throwing[0].CurrentAnimation.Insert(0, scaling);

                        fl = FileSystem.Instance.Locate("ch_neutral_throw.mesh", GameFileLocs.Model);
                        throwingPrepare[0] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        throwingPrepare[0].AnimationCompeleted += Animation_Completed;
                        throwingPrepare[0].CurrentAnimation.Insert(0, scaling);


                        fl = FileSystem.Instance.Locate("ch_neutral_fear.mesh", GameFileLocs.Model);
                        fear[0] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        fear[0].AnimationCompeleted += Animation_Completed;
                        fear[0].CurrentAnimation.Insert(0, scaling);

                        fl = FileSystem.Instance.Locate("ch_neutral_idle.mesh", GameFileLocs.Model);
                        idle[0] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        idle[0].AnimationCompeleted += Animation_Completed;
                        idle[0].CurrentAnimation.Insert(0, scaling);

                        fl = FileSystem.Instance.Locate("ch_neutral_laugh.mesh", GameFileLocs.Model);
                        laugh[0] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        laugh[0].AnimationCompeleted += Animation_Completed;
                        laugh[0].CurrentAnimation.Insert(0, scaling);

                        fl = FileSystem.Instance.Locate("ch_neutral_sleeping.mesh", GameFileLocs.Model);
                        sleeping[0] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        sleeping[0].AnimationCompeleted += Animation_Completed;
                        sleeping[0].CurrentAnimation.Insert(0, scaling);
                        sleeping[0].AutoLoop = true;

                        fl = FileSystem.Instance.Locate("ch_neutral_stopped.mesh", GameFileLocs.Model);
                        stopped[0] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        stopped[0].AnimationCompeleted += Animation_Completed;
                        stopped[0].CurrentAnimation.Insert(0, scaling);
                        stopped[0].AutoLoop = true;

                        fl = FileSystem.Instance.Locate("ch_neutral_wakeup.mesh", GameFileLocs.Model);
                        wakeingUp[0] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                        wakeingUp[0].AnimationCompeleted += Animation_Completed;
                        wakeingUp[0].CurrentAnimation.Insert(0, scaling);
                        #endregion

                        catching[1] = catching[0];
                        catchingRelease[1] = catchingRelease[0];
                        throwing[1] = throwing[0];
                        throwingPrepare[1] = throwingPrepare[0];
                        fear[1] = fear[0];
                        idle[1] = idle[0];
                        laugh[1] = laugh[0];
                        sleeping[1] = sleeping[0];
                        stopped[1] = stopped[0];
                        wakeingUp[1] = wakeingUp[0];

                        break;
                    }
            }
            smoke = new Smokes(this, rs); 
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
            float healthRate = HPRate;

            development += dt * GetDevelopmentStep();
            if (development > RulesTable.CityMaxDevelopment)
                development = RulesTable.CityMaxDevelopment;
            healthValue = healthRate * this.Development * RulesTable.CityDevHealthRate;
        }
        public void Develop(float amount, float dt)
        {
            float healthRate = HPRate;


            development += amount * dt;
            if (development > RulesTable.CityMaxDevelopment)
                development = RulesTable.CityMaxDevelopment;
            healthValue = healthRate * development * RulesTable.CityDevHealthRate;
        }

        public void Damage(float v, Player owener)
        {
            healthValue -= v * RulesTable.CityArmor;
            if (healthValue < 0)
            {
                healthValue =  development * RulesTable.CityDevHealthRate;
                ChangeOwner(owener);
            }
        }
        public void Heal(float v) 
        {
            healthValue += v;
            if (healthValue > development * RulesTable.CityDevHealthRate)
                healthValue = development * RulesTable.CityDevHealthRate;
        }

        void RefreshNearbyBalls() 
        {
            nearbyEnemyBalls.FastClear();
            nearbyOwnedBalls.FastClear();

            for (int i = 0; i < nearbyBallList.Count; i++) 
            {
                if (nearbyBallList[i].Owner == Owner)
                {
                    nearbyOwnedBalls.Add(nearbyBallList[i]);
                }
                else 
                {
                    nearbyEnemyBalls.Add(nearbyBallList[i]);
                }
            }
        }
        protected virtual void ChangeType()
        {
            if (IsCaptured)
            {
                if (Owner.Type == PlayerType.LocalHuman)
                {
                    switch (Type)
                    {
                        case CityType.Disease:
                            Type = CityType.Health;
                            break;
                        case CityType.Volience:
                            Type = CityType.Education;
                            break;
                        case CityType.Oil:
                            Type = CityType.Green;
                            break;
                    }
                    currentForm = 1;
                }
                else
                {
                    switch (Type)
                    {
                        case CityType.Health:
                            Type = CityType.Disease;
                            break;
                        case CityType.Education:
                            Type = CityType.Volience;
                            break;
                        case CityType.Green:
                            Type = CityType.Oil;
                            break;
                    }
                    currentForm = 0;
                }
            }
            stopped[currentForm].PlayAnimation();                           
        }
        public virtual void ChangeOwner(Player player)
        {
            if (Owner != player)
            {
                if (IsCaptured)
                {
                    Owner.Area.NotifyLostCity(this);
                }
                this.Owner = player;

                if (IsCaptured)
                {
                    Owner.Area.NotifyNewCity(this);
                }
                ChangeType();
                RefreshNearbyBalls();

                if (player != null)
                {
                    ChangeState(CityState.WakeingUp);
                }
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

        public bool HasOwnedBalls(Player player)
        {
            for (int i = 0; i < nearbyEnemyBalls.Count; i++)
            {
                if (nearbyEnemyBalls[i].Owner == player)
                {
                    return true;
                }
            }
            return false;
        }
        public bool HasMultipleTypeRBalls()
        {
            if (nearbyOwnedBalls.Count > 0)
            {
                RBallType detected = nearbyOwnedBalls[0].Type;
                
                for (int i = 1; i < nearbyOwnedBalls.Count; i++)
                {
                    if (nearbyOwnedBalls[i].Type != detected)
                    {
                        return true;
                    }
                }
            }
            return false;
            
        }
        public bool CanHandleCommand()
        {
            return currentState != CityState.Catch && currentState != CityState.Throw && currentState != CityState.ThrowContinued;
        }
        public void CancelCurrentCommand()
        {
            switch (currentState)
            {
                case CityState.WaitingGather:
                    {
                        rgball.Cancel();
                        break;
                    }
                case CityState.Rotate:
                    {
                        if (rotationPurpose != CityRotationPurpose.None)
                        {
                            rotationPurpose = CityRotationPurpose.None;
                        }
                        break;
                    }
            }
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
                    ChangeState(CityState.PostCatch);

                    if (!rgball.IsPathFinished)
                    {
                        reThrowDelay = 0.5f;
                    }
                    break;
                case CityState.PostCatch:
                    ChangeState(CityState.Stopped);
                    break;
                case CityState.Fear:
                    ChangeState(CityState.Stopped);
                    break;
                case CityState.Idle:
                    ChangeState(CityState.Stopped);
                    break;
                case CityState.Laugh:
                    ChangeState(CityState.Stopped);
                    break;
                case CityState.Throw:
                case CityState.ThrowContinued:
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
            if (ball.Owner == Owner)
            {
                nearbyOwnedBalls.Add(ball);
            }
            else
            {
                nearbyEnemyBalls.Add(ball);
            }

        }
        public void NotifyResourceBallMoveOut(RBall ball)
        {
            nearbyBallList.Remove(ball);
            if (ball.Owner == Owner)
            {
                nearbyOwnedBalls.Remove(ball);
            }
            else
            {
                nearbyEnemyBalls.Remove(ball);
            }
        }

        public void NotifyIncomingBall(RGatheredBall rgball)
        {
            bool flag = false;
            if (currentState == CityState.Rotate)
            {
                if (rotationPurpose != CityRotationPurpose.None)
                {
                    flag = true;
                }
            }

            bool flag2 = currentState == CityState.PostCatch || currentState == CityState.WaitingGather;


            if (currentState != CityState.Sleeping && CanHandleCommand() && !flag && !flag2)
            {
                if (rgball.SourceCity != null && rgball.SourceCity.Owner != Owner)
                {
                    Quaternion targetRot = GetOrientation(rgball.Position);
                    RotateTo(targetRot, 0.5f);
                    SetRotationPurpose(CityRotationPurpose.Fear);
                }
                else
                {
                    Quaternion targetRot = GetOrientation(rgball.Position);
                    RotateTo(targetRot, 0.5f);
                    SetRotationPurpose(CityRotationPurpose.Receive);
                    this.rgball = rgball;
                }
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
        void ThrowNowContinued()
        {
            List<RBall> toThrow = new List<RBall>(ballsToThrowCont.Count);

            for (int i = 0; i < ballsToThrowCont.Count; i++)
            {
                if (ballsToThrowCont[i].Owner == Owner &&
                    ballsToThrowCont[i].DockCity == this &&
                    !ballsToThrowCont[i].IsDied)
                {
                    toThrow.Add(ballsToThrowCont[i]);
                }
            }
            rgball = battleField.CreateRGatherBall(toThrow, this, throwPath);
            throwPath = null;
            ChangeState(CityState.WaitingGather);
        }

        /// <summary>
        ///  人类直接命令发球
        /// </summary>
        /// <param name="target"></param>
        public void Throw(City target)
        {
            if (!CanHandleCommand())
                return;

            CancelCurrentCommand();

            reThrowDelay = 0;
            rgball = null;
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
        /// <summary>
        ///  人类直接命令发球
        /// </summary>
        /// <param name="target"></param>
        public void Throw(City target, RBallType type)
        {
            if (!CanHandleCommand())
                return;

            CancelCurrentCommand();

            reThrowDelay = 0;
            rgball = null;
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

        /// <summary>
        ///  自动续传球
        /// </summary>
        /// <param name="next"></param>
        /// <param name="follow"></param>
        void ThrowContinued(City next, List<City> follow, List<RBall> balls)
        {
            if (!CanHandleCommand())
            {
                // 重设延时，到时候重新尝试
                reThrowDelay = 0.5f;
                return;
            }

            // 重新寻路，考虑断路可能
            battleField.BallPathFinder.Reset();
            BallPathFinderResult result = battleField.BallPathFinder.FindPath(this, follow[follow.Count - 1]);

            if (result != null)
            {
                throwPath = new List<City>(result.NodeCount);
                for (int i = 0; i < result.NodeCount; i++)
                {
                    throwPath.Add(result[i]);
                }

                ballsToThrowCont = balls;
                Quaternion targetRot = GetOrientation(follow[0].Position);

                RotateTo(targetRot, 0.5f);
                SetRotationPurpose(CityRotationPurpose.ThrowContinued);
            }
        }
        private void ChangeState(CityState state)
        {
            switch (state)
            {
                case CityState.Catch:
                    catching[currentForm].PlayAnimation();
                    break;
                case CityState.PostCatch:
                    if (catchingRelease[currentForm] != null)
                        catchingRelease[currentForm].PlayAnimation();
                    else
                        catching[currentForm].ResumeAnimation();
                    break;
                case CityState.Fear:
                    fear[currentForm].PlayAnimation();
                    break;
                case CityState.Idle:
                    idle[currentForm].PlayAnimation();
                    break;
                case CityState.Laugh:
                    break;
                case CityState.Stopped:
                    stopped[currentForm].PlayAnimation();

                    if (currentState == CityState.WakeingUp)
                    {
                        wakeingUp[currentForm].PauseAnimation();
                    }
                    break;
                case CityState.Throw:
                    throwing[currentForm].PlayAnimation();
                    rgball.NotifyThrow();
                    rgball = null;
                    break;
                case CityState.WakeingUp:
                    wakeingUp[currentForm].PlayAnimation();

                    if (currentState == CityState.Sleeping)
                    {
                        sleeping[currentForm].PauseAnimation();
                    }
                    break;
                case CityState.Rotate:
                case CityState.Sleeping:
                    sleeping[currentForm].PlayAnimation();
                    break;
                case CityState.WaitingGather:
                    if (throwingPrepare[currentForm] != null)
                        throwingPrepare[currentForm].PlayAnimation();
                    else
                        throwing[currentForm].ResumeAnimation();
                    break;
            }
            currentState = state;
        }
        private void UpdateState(GameTime time)
        {
            float dt = time.ElapsedGameTimeSeconds;

            smoke.EmitEnabled = false;
            switch (currentState)
            {
                case CityState.Rotate:

                    remainingRotationTime -= dt;
                    if (remainingRotationTime < 0)
                    {
                        switch (rotationPurpose)
                        {
                            case CityRotationPurpose.Throw:
                                ThrowNow();
                                break;
                            case CityRotationPurpose.Receive:
                                ReceiveNow();
                                break;
                            case CityRotationPurpose.ThrowContinued:
                                ThrowNowContinued();
                                break;
                            case CityRotationPurpose.Fear:
                                ChangeState(CityState.Fear);
                                break;
                            default:
                                ChangeState(CityState.Stopped);
                                break;

                        }
                        
                        rotationPurpose = CityRotationPurpose.None;
                    }
                    float progress = MathEx.Saturate(1 - remainingRotationTime / rotationTime);

                    currentFacing = Quaternion.Slerp(rotationSrc, rotationTarget, progress);

                    if (isVisible) stopped[currentForm].Update(time);
                    break;
                case CityState.PostCatch:
                    if (catchingRelease[currentForm] != null)
                        catchingRelease[currentForm].Update(time);
                    else
                        catching[currentForm].Update(time);
                    break;
                case CityState.Catch:
                    catching[currentForm].Update(time);
                    break;
                case CityState.Fear:
                    if (isVisible) fear[currentForm].Update(time);
                    break;
                case CityState.Idle:
                    if (isVisible) idle[currentForm].Update(time);
                    break;
                case CityState.Laugh:
                    if (isVisible) laugh[currentForm].Update(time);
                    break;
                case CityState.Stopped:
                    if (reThrowDelay > 0)
                    {
                        reThrowDelay -= dt;
                        if (reThrowDelay < 0)
                        {
                            ThrowContinued(rgball.FollowingCity, rgball.GetRemainingPath(), rgball.Balls);
                        }
                    }
                    
                    smoke.EmitEnabled = true;

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
                    if (isVisible) stopped[currentForm].Update(time);
                    break;
                case CityState.Throw:

                    throwing[currentForm].Update(time);
                    break;
                case CityState.ThrowContinued:

                    throwing[currentForm].Update(time);
                    break;
                case CityState.WakeingUp:
                    wakeingUp[currentForm].Update(time);
                    break;
                case CityState.Sleeping:
                    if (isVisible) sleeping[currentForm].Update(time);
                    break;
                case CityState.WaitingGather:
                    if (rgball.CurrentState == RGatheredBall.State.WaitingThrow)
                    {
                        ChangeState(CityState.Throw);
                    }
                    if (throwingPrepare[currentForm] != null)
                        throwingPrepare[currentForm].Update(time);
                    else
                        throwing[currentForm].Update(time);
                    break;
            }

        }
        private void UpdateAI(GameTime time)
        {
            // 检查资源球死亡
            for (int i = nearbyBallList.Count - 1; i >= 0; i--)
            {
                if (nearbyBallList[i].IsDied)
                {
                    RBall ball = nearbyBallList[i];
                    nearbyBallList.RemoveAt(i);
                    battleField.DestroyResourceBall(ball);
                }
            }
            for (int i = nearbyOwnedBalls.Count - 1; i >= 0; i--)
            {
                if (nearbyOwnedBalls[i].IsDied)
                {
                    RBall ball = nearbyBallList[i];
                    nearbyOwnedBalls.RemoveAt(i);
                }
            } 
            for (int i = nearbyEnemyBalls.Count - 1; i >= 0; i--)
            {
                if (nearbyEnemyBalls[i].IsDied)
                {
                    RBall ball = nearbyEnemyBalls[i];
                    nearbyEnemyBalls.RemoveAt(i);
                }
            }
        }

        public override void Update(GameTime dt)
        {
            float ddt = (float)dt.ElapsedGameTimeSeconds;

            if (Owner != null)
            {
                NaturalDevelop(ddt);

                //float devIncr = 0;
                //// 计算附近同阵营资源球贡献发展量
                //for (int i = 0; i < nearbyBallList.Count; i++)
                //{
                //    devIncr += ddt;// *Utils.GetRBallContribution(nearbyBallList[i].Type);
                //}
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

            if (isVisible)
            {
                if (smoke != null)
                    smoke.Update(dt);
            }
            
            UpdateState(dt);

            UpdateAI(dt);
        }

        public void TestBalls()
        {
            for (int i = 0; i < 5; i++)
            {
                battleField.CreateResourceBall(Owner, this, RBallType.Health);
                battleField.CreateResourceBall(Owner, this, RBallType.Education);

            }
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


            RenderOperation[] ops = null;


            ops = cityBase.GetRenderOperation();
            if (ops != null)
            {
                for (int i = 0; i < ops.Length; i++)
                {
                    ops[i].Sender = this;
                }
                opBuffer.Add(ops);
            }

            if (Type == CityType.Oil && smoke != null)
            {
                ops = smoke.GetRenderOperation();
                if (ops != null)
                {
                    opBuffer.Add(ops);
                }
            }

            
            ops = null;
            switch (currentState)
            {
                case CityState.Catch:
                    ops = catching[currentForm].GetRenderOperation();
                    break;
                case CityState.Fear:
                    ops = fear[currentForm].GetRenderOperation();
                    break;
                case CityState.Idle:
                    ops = idle[currentForm].GetRenderOperation();
                    break;
                case CityState.Laugh:
                    ops = laugh[currentForm].GetRenderOperation();
                    break;
                case CityState.Rotate:
                    ops = stopped[currentForm].GetRenderOperation();
                    break;
                case CityState.Stopped:
                    ops = stopped[currentForm].GetRenderOperation();
                    break;
                case CityState.Throw:
                    ops = throwing[currentForm].GetRenderOperation();
                    break;
                case CityState.WakeingUp:
                    ops = wakeingUp[currentForm].GetRenderOperation();
                    break;
                case CityState.Sleeping:
                    ops = sleeping[currentForm].GetRenderOperation();
                    break;
                case CityState.PostCatch:
                    if (catchingRelease[currentForm] != null)
                    {
                        ops = catchingRelease[currentForm].GetRenderOperation();
                    }
                    else 
                    {
                        ops = catching[currentForm].GetRenderOperation();
                    }
                    break;
                case CityState.WaitingGather:
                    if (throwingPrepare[currentForm] != null)
                    {
                        ops = throwingPrepare[currentForm].GetRenderOperation();
                    }
                    else
                    {
                        ops = throwing[currentForm].GetRenderOperation();
                    }
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
