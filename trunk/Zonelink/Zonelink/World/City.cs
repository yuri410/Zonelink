using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Code2015.EngineEx;
using Code2015.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Zonelink.Graphics;
using Apoc3D.Collections;
using Apoc3D;

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

    enum CityAnimationState
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
        protected BattleField battleField;

        CityAnimationState animationType = CityAnimationState.ReceiveBall;

        string[] linkableCityName;
        FastList<City> linkableCity = new FastList<City>();

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
        public CityAnimationState AnimationType { get { return animationType; } }

        //城市类型
        public CityType Type { get; protected set; }

        //城市名称
        public string Name { get; set; }

        //动画状态，由状态机改变

        public float HealthValue { get { return healthValue; } }
        public float Development { get { return development; } }

        public float HPRate
        {
            get { return healthValue / development; }
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

            this.healthValue = 1000;  
        }  


        int Camparision(NatureResource a, NatureResource b)
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

        public void HookAnimationEvent(RigidModel model)
        {
            model.Completed += Animation_Complete;
        }
        private void Animation_Complete(object sender, EventArgs e)
        {
            switch (animationType)
            {

            }
            ((RigidModel)sender).Play();
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

        public void ChangeOwner(Player belong)
        {
            this.Owner = belong;
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
        public override void Parse(GameConfigurationSection sect)
        {
            base.Parse(sect);
            this.Name = sect.GetString("Name", string.Empty);

            StartUp = sect.GetInt("StartUp", -1);

            linkableCityName = sect.GetStringArray("Linkable", null);

            development = sect.GetSingle("InitialDevelopment", RulesTable.CityInitialDevelopment);


            //设置城市类型
            string type = sect.GetString("Type", string.Empty).ToLowerInvariant();

            if (type == CityType.Oil.ToString().ToLowerInvariant())
            {
                this.Type = CityType.Oil;
                developStep = RulesTable.OilDevelopStep;
                //ProduceBallInterval = 0;
                
            }
            else if (type == CityType.Green.ToString().ToLowerInvariant())
            {
                this.Type = CityType.Green;
                developStep = RulesTable.GreenDevelopStep;
                //ProduceBallInterval = 0;
            }
            else if (type == CityType.Neutral.ToString().ToLowerInvariant())
            {
                this.Type = CityType.Neutral;
                developStep = 20;
                //ProduceBallInterval = 20;
            }
            else if (type == CityType.Volience.ToString().ToLowerInvariant())
            {
                this.Type = CityType.Volience;
                developStep = RulesTable.VolienceDevelopStep;
                //ProduceBallInterval = RulesTable.VolienceBallInterval;
            }
            else if (type == CityType.Health.ToString().ToLowerInvariant())
            {
                this.Type = CityType.Health;
                developStep = RulesTable.HealthDevelopStep;
                //ProduceBallInterval = RulesTable.HealthBallInterval;
            }
            else if (type == CityType.Disease.ToString().ToLowerInvariant())
            {
                this.Type = CityType.Disease;
                developStep = RulesTable.DiseaseDevelopStep;
                //ProduceBallInterval = RulesTable.DiseaseBallInterval;
            }
            else if (type == CityType.Education.ToString().ToLowerInvariant())
            {
                this.Type = CityType.Education;
                developStep = RulesTable.EducationDevelopStep;
                //ProduceBallInterval = RulesTable.EducationBallInterval;
            }

            //this.interval = this.ProduceBallInterval;
            UpdateLocation();

            //进去默认状态a
            //this.fsmMachine.CurrentState = new CityDevelopmentState(); 

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

        public override void Update(GameTime dt)
        {
            float ddt = (float)dt.ElapsedGameTime.TotalSeconds;

            if (Owner != null)
            {
                NaturalDevelop(ddt);

                float devIncr = 0;
                // 计算附近同阵营资源球贡献发展量
                for (int i = 0; i < nearbyBallList.Count; i++)
                {
                    devIncr += ddt * Utils.GetRBallContribution(nearbyBallList[i].Type);
                }         
            }
        }

        #region 发展状态

        ///// <summary>
        /////  普通城市类型，当时间间隔小于0时，产生资源球
        /////  Gather City 根据Buffer产生资源球
        /////  在更新资源状态中调用，true则切换状态到产生球状态，播放动画
        ///// </summary>
        ///// <returns></returns>
        //virtual bool CanProduceRBall()
        //{
        //    return this.interval < 0;
        //}

        #endregion

        //#region 产生球状态
        public virtual void ProduceBall()
        {
            //this.battleField.CreateResourceBall(this);
        }

        //#endregion



        public override void Render()
        {
           
        }

       
    }
}
