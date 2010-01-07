using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Config;
using Apoc3D.MathLib;

namespace Code2015.BalanceSystem
{
    public class CityPlugin : IConfigurable, IUpdatable
    {
        City parent;
        FastList<NaturalResource> resource = new FastList<NaturalResource>();

        public string Name
        {
            get;
            protected set;
        }
        public CityPlugin(string name)
        {
            this.Name = name;
        }
        public CityPlugin()
        { }
       
        #region IConfigurable 成员

        public void Parse(ConfigurationSection sect)
        {

            Cost = sect.GetSingle("Cost");
            UpgradeCost = sect.GetSingle("UpgradeCost");

            HPProductionSpeed = sect.GetSingle("HPProductionSpeed");
            LPProductionSpeed = sect.GetSingle("LPProductionSpeed");
            CarbonProduceSpeed = sect.GetSingle("CarbonProduceSpeed");
            FoodCostSpeed = sect.GetSingle("FoodCostSpeed");
            GatherRadius = sect.GetSingle("GatherRadius");

        }
      

        #endregion

        #region  属性
        /// <summary>
        /// 开始建造一个所需费用
        /// </summary>
        public float Cost
        {
            get;
            protected set;
        }

#warning 不同的升级费用
        /// <summary>
        /// 升级所需费用
        /// </summary>
        public float UpgradeCost
        {
            get;
            protected set;
        }
        public virtual float GetUpgradeCost()
        {
            UpgradeCost = Cost * 0.5f;
            float upgradecost = UpgradeCost;
            return UpgradeCost = 0;
        }

        public float GatherRadius
        {
            get;
            protected set;
        }



        /// <summary>
        ///  输入资源充足时的高能生产速度
        /// </summary>
        public float FullHPProductionSpeed
        {
            get;
            private set;
        }
        /// <summary>
        ///  输入资源充足时的低能生产速度
        /// </summary>
        public float FullLPProductionSpeed
        {
            get;
            private set;
        }

        
        /// <summary>
        /// 高能产生的速度,速度为正表示产生能量，为负值表示消耗能量
        /// </summary>
        public float HPProductionSpeed
        {
            get;
            protected set;
        }
        /// <summary>
        /// 低能产生的速度
        /// </summary>
        public float LPProductionSpeed
        {
            get;
            protected set;
        }

        public float CarbonProduceSpeed
        {
            get;
            protected set;
        }

        public float FoodCostSpeed
        {
            get;
            protected set;
        }

        #endregion

        void FindResources()
        {
            SimulateRegion region = parent.Region;

            Vector2 myPos = new Vector2(parent.Latitude, parent.Longitude);
            for (int i = 0; i < region.Count; i++)
            {
                if (!object.ReferenceEquals(region[i], parent))
                {
                    NaturalResource res = region[i] as NaturalResource;
                    if (res != null)
                    {
                        Vector2 pos = new Vector2(res.Latitude, res.Longitude);
                        float dist = Vector2.Distance(pos, myPos);

                        if (dist < GatherRadius)
                        {
                            resource.Add(res);
                        }
                    }
                }
            }
        }

     
        public void NotifyAdded(City city)
        {
            if (city != null)
            {
                throw new InvalidOperationException();
            }
            parent = city;

            FindResources();
        }

        public void NotifyRemoved(City city)
        {
            parent = null;
            resource.Clear();
        }



        #region IUpdatable 成员
       
        public void Update(GameTime time)
        {
            float hours = (float)time.ElapsedGameTime.TotalHours;
            // 采集资源

            int index = Randomizer.GetRandomInt(resource.Count);


            float hpResource = HPProductionSpeed * hours*1.5f;
            float lpResource = LPProductionSpeed * hours*1.5f;

            int tries = 0;
            bool finished = false;
            while (tries < resource.Count && !finished)
            {
                NaturalResource res = resource[index % resource.Count];

                if (hpResource > 0)
                {
                    if (res.Type == NaturalResourceType.Oil)
                    {
                        //采集资源
                        parent.RemainingHPAmount= res.Exploit(hpResource)/1.5f;
                        hpResource = 0;        
                    }
                }
                if (lpResource > 0)
                {
                    if (res.Type == NaturalResourceType.Wood)
                    {
                       parent.RemainingLPAmount= res.Exploit(lpResource)/1.5f;
                        lpResource = 0;
                    }
                }

                index++;
                tries++;
               
            }
        }

        #endregion
    }
}
