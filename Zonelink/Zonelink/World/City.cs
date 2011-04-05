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


    /// <summary>
    ///  表示游戏世界中的城市
    /// </summary>
    class City : Entity
    {
        //public static readonly float CityRadius = RulesTable.CityRadius;
       
        //public static readonly float MaxDevelopment = RulesTable.CityMaxDevelopment;

        BattleField battleField;

        //城市类型
        public CityType Type { get; private set; }

        //城市名称
        public string Name { get; set; } 

        public float HealthValue { get; private set; }
        public float Development { get; private set; }
        public float HPRate
        {
            get { return HealthValue / Development; }
        }


        //城市附近的资源球 
        private List<RBall> nearbyBallList = new List<RBall>();
        
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
        }

        int Camparision(NatureResource a, NatureResource b)
        {
            float da = Vector3.DistanceSquared(a.Position, this.Position);
            float db = Vector3.DistanceSquared(b.Position, this.Position);
            return da.CompareTo(db);
        }

        //// 注：在子类中实现
        ////发展到一定程度时产生资源
        //public void ProduceResourceBall()
        //{
        //    nearbyBallList.Add( Technology.Instance.CreateRBall(this) ) ;
        //}


        public void Develop(float v)
        {
            float healthRate = (this.HealthValue / this.Development);

            this.Development += v;
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
            
            UpdateLocation();
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


        public override void Render()
        {
           
        }

        public override void Update(GameTime dt)
        {
           //更新Transform

            
        }
    }
}
