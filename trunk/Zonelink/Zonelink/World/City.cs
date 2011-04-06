using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zonelink.State;
using Microsoft.Xna.Framework;
using Code2015.World;
using Code2015.EngineEx;
using Microsoft.Xna.Framework.Graphics;

namespace Zonelink.World
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

    enum CityAnimationType
    {
        Stopped,
        SendBall,
        ReceiveBall,
        Idle,
    }


    /// <summary>
    ///  表示游戏世界中的城市
    ///  是特殊的类型才继承，比如那些带矿车的
    /// </summary>
    class City : Entity
    {
        //public static readonly float CityRadius = RulesTable.CityRadius;
       
        //public static readonly float MaxDevelopment = RulesTable.CityMaxDevelopment;

        protected BattleField battleField;

        //城市类型
        public CityType Type { get; protected set; }

        //城市发展速度
        private float DevelopStep;

        //每隔多少时间产生资源球,对oil, green没用
        private float ProduceBallInterval;
        private float interval;
        

        //城市名称
        public string Name { get; set; }

        //动画状态，由状态机改变
        public CityAnimationType AnimationType { get; set; }
        public bool animationPlayOver { get; set; }

        public float HealthValue { get; set; }
        public float Development { get; set; }

        public float HPRate
        {
            get { return HealthValue / Development; }
        }


        //城市附近的资源球 
        protected List<RBall> nearbyBallList = new List<RBall>();
        
        /// <summary>
        ///  是否有被玩家占领
        /// </summary>
        public bool IsCaptured
        {
            get { return Owner != null; }
        }


        /// <summary>
        ///  待选起始点
        /// </summary>
        public int StartUp 
        {
            get;
            private set;
        }


        public City(BattleField btfld, Player owner)
            : base(owner)
        {
            this.battleField = btfld;

            this.HealthValue = 1000;  
        }  


        int Camparision(NatureResource a, NatureResource b)
        {
            float da = Vector3.DistanceSquared(a.Position, this.Position);
            float db = Vector3.DistanceSquared(b.Position, this.Position);
            return da.CompareTo(db);
        }


        //根据dt，增加发展量
        public void Develop(float dt)
        {
            float healthRate = (this.HealthValue / this.Development);

            this.Development += dt * DevelopStep;
            if (this.Development > RulesTable.CityMaxDevelopment)
                this.Development = RulesTable.CityMaxDevelopment;
            this.HealthValue = healthRate * this.Development;
        }

        public void Damage(float v, Player owener)
        {
            this.HealthValue -= v;
            if (this.HealthValue < 0)
            {
                this.HealthValue = this.Development;
                this.Owner = owener;    
            }
        }

        public void ChangeOwner(Player belong)
        {
            this.Owner = belong;
        }


        public override void Parse(GameConfigurationSection sect)
        {
            base.Parse(sect);
            this.Name = sect.GetString("Name", string.Empty);

            StartUp = sect.GetInt("StartUp", -1);

            Development = sect.GetSingle("InitialDevelopment", RulesTable.CityInitialDevelopment);


            //设置城市类型
            string type = sect.GetString("Type", string.Empty);

            if (type == "oil")
            {
                this.Type = CityType.Oil;
                DevelopStep = RulesTable.OilDevelopStep;
                ProduceBallInterval = 0;
                
            }
            else if (type == "green")
            {
                this.Type = CityType.Green;
                DevelopStep = RulesTable.GreenDevelopStep;
                ProduceBallInterval = 0;
            }
            else if (type == "Neutral")
            {
                this.Type = CityType.Neutral;
                DevelopStep = 20;
                ProduceBallInterval = 20;
            }
            else if (type == "Volience")
            {
                this.Type = CityType.Volience;
                DevelopStep = RulesTable.VolienceDevelopStep;
                ProduceBallInterval = RulesTable.VolienceBallInterval;
            }
            else if (type == "Health")
            {
                this.Type = CityType.Health;
                DevelopStep = RulesTable.HealthDevelopStep;
                ProduceBallInterval = RulesTable.HealthBallInterval;
            }
            else if (type == "Disease")
            {
                this.Type = CityType.Disease;
                DevelopStep = RulesTable.DiseaseDevelopStep;
                ProduceBallInterval = RulesTable.DiseaseBallInterval;
            }
            else if (type == "Education")
            {
                this.Type = CityType.Education;
                DevelopStep = RulesTable.EducationDevelopStep;
                ProduceBallInterval = RulesTable.EducationBallInterval;
            }

            this.interval = this.ProduceBallInterval;
            UpdateLocation();

            //进去默认状态a
            this.fsmMachine.CurrentState = new CityDevelopmentState(); 

        }


        private void UpdateLocation()
        {
            float radLong = MathHelper.ToRadians(this.Longitude);
            float radLat = MathHelper.ToRadians(this.Latitude);

            float altitude = TerrainData.Instance.QueryHeight(radLong, radLat);
            this.Position = PlanetEarth.GetPosition(radLong, radLat, PlanetEarth.PlanetRadius + TerrainMeshManager.PostHeightScale * altitude + 5);

            this.Transformation = PlanetEarth.GetOrientation(radLong, radLat);
            this.InvTransformation = Matrix.Invert(Transformation);

            this.Transformation.Translation = this.Position; // TranslationValue = pos;

            BoundingSphere.Radius = RulesTable.CityRadius;
            BoundingSphere.Center = this.Position;
        }


        #region 发展状态

        /// <summary>
        ///  普通城市类型，当时间间隔小于0时，产生资源球
        ///  Gather City 根据Buffer产生资源球
        ///  在更新资源状态中调用，true则切换状态到产生球状态，播放动画
        /// </summary>
        /// <returns></returns>
        public virtual bool CanProduceRBall()
        {
            return this.interval < 0;
        }


        //更新资源状态，在状态机中调用
        //更新资源,  对一般城市来说，每隔一段时间产生资源球, Oil跟Green根据buffer产生
        public virtual void UpdateResource(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (this.interval < 0)
                this.interval = this.ProduceBallInterval;
            this.interval -= dt * (float)5;
            this.Develop(dt);     
        }

        #endregion

        #region 产生球状态
        public virtual void ProduceBall()
        {
            this.battleField.CreateResourceBall(this);
        }

        #endregion



        public override void Render()
        {
           
        }

       
    }
}
