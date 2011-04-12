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

    enum CityAnimationState
    {
        Stopped,
        SendBall,
        ReceiveBall,
        Idle,

    }


    delegate void CityVisibleHander(City obj);    

    /// <summary>
    ///  表示游戏世界中的城市
    ///  是特殊的类型才继承，比如那些带矿车的
    /// </summary>
    class City : WorldObject
    {
        const float RingRadius = 100;

        public const float CityRadiusDeg = 3.5f;

        /// <summary>
        ///  城市底座所占圆的半径
        /// </summary>
        public const float CityRadius = Game.ObjectScale * 100;

        /// <summary>
        ///  城市所属圈的半径（过时）
        /// </summary>
        public const float CityOutterRadius = CityRadius + Game.ObjectScale * 15;

        /// <summary>
        ///  城市选择圈的半径
        /// </summary>
        public const float CitySelRingScale = 2.6f;


        protected BattleField battleField;

        CityAnimationState animationType = CityAnimationState.ReceiveBall;


        SoundObject sound;





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

        public event CityVisibleHander CityVisible;


        public City(BattleField btfld, Player owner)
            : base(owner)
        {
            this.battleField = btfld;

            this.healthValue = 1000;  
        }

        public override void InitalizeGraphics(RenderSystem rs) 
        {
            switch (Type) 
            {
                case CityType.Oil:
                    break;
                case CityType.Neutral:
                    break;
            }
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

        //public void HookAnimationEvent(RigidModel model)
        //{
        //    model.Completed += Animation_Complete;
        //}


        public void Damage(float v, Player owener)
        {
            healthValue -= v;
            if (healthValue < 0)
            {
                healthValue = this.Development;
                this.Owner = owener;
            }
        }

        public void ChangeOwner(Player player)
        {
            if (IsCaptured && player == null)
            {
                Owner.Area.NotifyLostCity(this);
            }
            this.Owner = player;

            if (player != null)
            {
                Owner.Area.NotifyNewCity(this);
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
            float radLong = MathEx.Degree2Radian(this.Longitude);
            float radLat = MathEx.Degree2Radian(this.Latitude);

            float altitude = TerrainData.Instance.QueryHeight(radLong, radLat);
            this.Position = PlanetEarth.GetPosition(radLong, radLat, PlanetEarth.PlanetRadius + TerrainMeshManager.PostHeightScale * altitude + 5);

            this.Transformation = PlanetEarth.GetOrientation(radLong, radLat);
            //this = Matrix.Invert(Transformation);

            this.Transformation.TranslationValue = this.Position; // TranslationValue = pos;

            BoundingSphere.Radius = RulesTable.CityRadius;
            BoundingSphere.Center = this.Position;
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
        }




        public virtual void ProduceBall()
        {
            //this.battleField.CreateResourceBall(this);
        }



        public override RenderOperation[] GetRenderOperation()
        {
            //isVisible = true;
            //opBuffer.FastClear();
            if (CityVisible != null)
            {
                CityVisible(this);
            }
            return null;
            // TODO: Render City here
            throw new NotImplementedException();
        }
       
    }
}
