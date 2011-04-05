using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zonelink.State;
using Microsoft.Xna.Framework;
using Code2015.World;
using Code2015.EngineEx;

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
        
        public const float CityRadius = 3 * 100;
        public const float GatherDistance = 150;
        public const float MaxDevelopment = 10000;

        //城市类型
        public CityType CityType { get; private set; }

        //城市名称
        public string Name { get; set; }
        
        public float HealthValue { get; private set; }
        public float Development { get; private set; }
        public float HPRate
        {
            get { return HealthValue / Development; }
        }

        //周围自然资源
        private List<NatureResource> nearWood = new List<NatureResource>();
        private List<NatureResource> nearOil = new List<NatureResource>();

        //城市创造出来的资源球 
        private List<RBall> ResourceBallList = new List<RBall>();

        //是否被玩家占领
        public bool IsCaptured
        {
            get { return Owner != null; }
        }


        int woodIndex = 0;
        int oilIndex = 0;
        float oilBuffer;
        float woodBuffer;

        public NatureResource ExWood
        {
            get
            {
                if (nearWood.Count == 0)
                    return null;
                return nearWood[woodIndex];
            }
        }

        public NatureResource ExOil
        {
            get
            {
                if (nearOil.Count == 0)
                    return null;
                return nearOil[oilIndex];
            }
        }  

        public City(Player owner)
            : base(owner)
        {

        }

        int Camparision(NatureResource a, NatureResource b)
        {
            float da = Vector3.DistanceSquared(a.Position, this.Position);
            float db = Vector3.DistanceSquared(b.Position, this.Position);
            return da.CompareTo(db);
        }

        public void FindResources(List<NatureResource> resList)
        {
            for (int i = 0; i < resList.Count; i++)
            {
                float d = Vector3.Distance(resList[i].Position, this.Position);
                if (d < GatherDistance)
                {
                    if (resList[i].Type == NatureResourceType.Wood)
                    {
                        nearWood.Add(resList[i]);
                    }
                    if (resList[i].Type == NatureResourceType.Oil)
                    {
                        nearOil.Add(resList[i]);
                    }

                }
            }
            nearWood.Sort(Camparision);
            nearOil.Sort(Camparision);
        }

        void FindNewOil()
        {
            for (int i = 0; i < nearOil.Count; i++)
            {
                if (nearOil[i].CurrentAmount > 0)
                    oilIndex = i;
            }
        }

        void FindNewWood()
        {
            for (int i = 0; i < nearWood.Count; i++)
            {
                if (nearWood[i].CurrentAmount > 0)
                    woodIndex = i;
            }
        }



        //发展到一定程度时产生资源
        public void ProduceResourceBall()
        {
            ResourceBallList.Add( Technology.Instance.CreateRBall(GetResourceType(), this.Owner, this) ) ;
        }


        public void Develop(float v)
        {
            float healthRate = (this.HealthValue / this.Development);

            this.Development += v;
            if (this.Development > MaxDevelopment)
                this.Development = MaxDevelopment;
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

        //城市产生的资源球类型
        private RBallTypeID GetResourceType()
        {
            switch (this.CityType)
            {
                case CityType.Disease:
                    return RBallTypeID.Disease;

                case CityType.Education:
                    return RBallTypeID.Education;

                case CityType.Green:
                    return RBallTypeID.Green;

                case CityType.Health:
                    return RBallTypeID.Health;

                case CityType.Oil:
                    return RBallTypeID.Oil;

                case CityType.Volience:
                    return RBallTypeID.Volience;
                
                default:
                    //Error
                    return RBallTypeID.Disease;
            }
        }

        public override void Parse(GameConfigurationSection sect)
        {
            base.Parse(sect);
            this.Name = sect.GetString("Name", string.Empty);

            //Type 设置
            //this.CityType = 

        }


        public void UpdateLocation()
        {
            float radLong = MathHelper.ToRadians(this.Longitude);
            float radLat = MathHelper.ToRadians(this.Latitude);

            float altitude = TerrainData.Instance.QueryHeight(radLong, radLat);
            this.Position = PlanetEarth.GetPosition(radLong, radLat, PlanetEarth.PlanetRadius + TerrainMeshManager.PostHeightScale * altitude + 5);

            this.Transformation = PlanetEarth.GetOrientation(radLong, radLat);
            this.InvTransformation = Matrix.Invert(Transformation);

            this.Transformation.Translation = this.Position; // TranslationValue = pos;

            BoundingSphere.Radius = CityRadius;
            BoundingSphere.Center = this.Position;
        }

    }
}
