using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Code2015.EngineEx;
using Code2015.Logic;
using Code2015.World;

namespace Code2015.World
{
    public enum NaturalResourceType
    {
        Wood,
        Oil
    }

    abstract class NaturalResource : WorldObject
    {
        const float OTimesMaxAmount = 2;
        //const float ORecoverBias = 1;


        //const float FRecoverRate = 0.0015f;
        //const float FRecoverBias = 1;
        const float FTimesMaxAmount = 2;

        public const int OilFrameCount = 30;
        //public const float OilScale = 6.6f;

        /// <summary>
        ///  获取自然资源的类型
        /// </summary>
        public NaturalResourceType Type
        {
            get;
            private set;
        }


        public float MaxAmount
        {
            get;
            private set;
        }

        //当前资源数量
        public float CurrentAmount
        {
            get;
            protected set;
        }

        public object OutputTarget
        {
            get;
            set;
        }

        public bool IsLow
        {
            get { return CurrentAmount < 1000; }
        }


        public NaturalResource()
            
        {
      
        }

        private void Reset(float current) 
        {
            CurrentAmount = current;

            switch (Type)
            {
                case NaturalResourceType.Oil:
                    MaxAmount = CurrentAmount * OTimesMaxAmount;
                    break;
                case NaturalResourceType.Wood:
                    MaxAmount = CurrentAmount * FTimesMaxAmount;
                    break;
            }
            
        }

        /// <summary>
        ///  开采一定数量的自然资源
        /// </summary>
        /// <param name="amount">申请值</param>
        /// <returns>实际得到的</returns>
        public float Exploit(float amount)
        {
            if (amount < CurrentAmount)
            {
                CurrentAmount -= amount;
                return amount;
            }

            float r = CurrentAmount;
            CurrentAmount = 0;
            return r;

        } 

        public override void Update(GameTime time)
        {
            base.Update(time);
        }


        public override void Parse(GameConfigurationSection sect)
        {
            base.Parse(sect);

            string type = sect.GetString("Type", string.Empty).ToLowerInvariant();
            if (type == "wood")
            {
                this.Type = NaturalResourceType.Wood;
            }
            else if (type == "petro")
            {
                this.Type = NaturalResourceType.Oil;
            }


            CurrentAmount = sect.GetSingle("Amount", 0);
            //MaxAmount = CurrentAmount * FTimesMaxAmount;

            UpdateLocation();

            Reset(CurrentAmount);
        }

        private void UpdateLocation()
        {
            float radLong = MathEx.Degree2Radian(this.Longitude);
            float radLat = MathEx.Degree2Radian(this.Latitude);

            float altitude = TerrainData.Instance.QueryHeight(radLong, radLat);

            //IsInOcean = false;
            //if (altitude < 0)
            //{
            //    altitude = 0;
            //    IsInOcean = true;
            //}
            
            this.Position = PlanetEarth.GetPosition(radLong, radLat, PlanetEarth.PlanetRadius + TerrainMeshManager.PostHeightScale * altitude);

            this.Transformation = PlanetEarth.GetOrientation(radLong, radLat);
            //this.InvTransformation = Matrix.Invert(Transformation);

            this.Transformation.TranslationValue = this.Position; // TranslationValue = pos;

            BoundingSphere.Radius = RulesTable.CityRadius;
            BoundingSphere.Center = this.Position;




        }
      
    }
}
