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
    //public enum CityPluginType { }
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
            UpgradeCostBase = sect.GetSingle("UpgradeCostBase");

            FullHRPSpeed = sect.GetSingle("FullHPProductionSpeed");
            FullLRPSpeed = sect.GetSingle("FullLPProductionSpeed");
            FullCarbonProduceSpeed = sect.GetSingle("FullCarbonProduceSpeed");

            NaturalHRCostSpeed = sect.GetSingle("HRCostSpeed");
            NaturalLRCostSpeed = sect.GetSingle("LRCostSpeed");
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

        /// <summary>
        /// 升级所需费用
        /// </summary>
        public float UpgradeCostBase
        {
            get;
            protected set;
        }
        public virtual float GetUpgradeCost()
        {
            UpgradeCostBase = Cost * 0.5f;
            float upgradecost = UpgradeCostBase;
            return UpgradeCostBase = 0;
        }

        public float GatherRadius
        {
            get;
            protected set;
        }
        public float NaturalHRCostSpeed
        {
            get;
            private set;
        }
        public float NaturalLRCostSpeed
        {
            get;
            private set;
        }
        public float FoodCostSpeed
        {
            get;
            protected set;
        }


        ///// <summary>
        /////  输入资源充足时的高能生产速度
        ///// </summary>
        //public float FullHRPSpeed
        //{
        //    get;
        //    private set;
        //}
        ///// <summary>
        /////  输入资源充足时的低能生产速度
        ///// </summary>
        //public float FullLRPSpeed
        //{
        //    get;
        //    private set;
        //}

        public float HRConvertionRate
        {
            get;
            private set;
        }
        public float LRConvertionRate
        {
            get;
            private set;
        }
        public float FoodConvertionRate
        {
            get;
            private set;
        }
        //public float FullCarbonProduceSpeed
        //{
        //    get;
        //    private set;
        //}

        /// <summary>
        ///  在当前状况下的高能产生的速度,速度为正表示产生能量，为负值表示消耗能量
        /// </summary>
        public float HRPSpeed
        {
            get;
            protected set;
        }
        /// <summary>
        ///  在当前状况下的低能产生的速度
        /// </summary>
        public float LRPSpeed
        {
            get;
            protected set;
        }

        /// <summary>
        ///  在当前状况下的碳排放速度
        /// </summary>
        public float CarbonProduceSpeed
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
            // 处理采集自然资源
            int index = Randomizer.GetRandomInt(resource.Count);

            float food = FoodCostSpeed * hours;
            float hpResource = NaturalHRCostSpeed * hours;
            float lpResource = NaturalLRCostSpeed * hours;

            int tries = 0;
            bool finished = false;
            CarbonProduceSpeed = 0;
             

            if (hpResource > float.Epsilon ||
                lpResource > float.Epsilon)
            {
                while (tries < resource.Count && !finished)
                {
                    NaturalResource res = resource[index % resource.Count];

                    if (hpResource > 0)
                    {
                        if (res.Type == NaturalResourceType.Oil)
                        {
                            //采集资源

                            float act = res.Exploit(hpResource);
                            float speed = act / hours;

                            HRPSpeed = speed * HRConvertionRate;
                            CarbonProduceSpeed += speed * Math.Max(0, 1 - HRConvertionRate);
                            hpResource = 0;
                        }
                    }
                    if (lpResource > 0)
                    {
                        if (res.Type == NaturalResourceType.Wood)
                        {
                            float act = res.Exploit(lpResource);
                            float speed = act / hours;

                            LRPSpeed = speed * LRConvertionRate;
                            CarbonProduceSpeed += speed * Math.Max(0, 1 - LRConvertionRate);
                            lpResource = 0;
                        }
                    }


                    index++;
                    tries++;

                }
            }
            if (food > 0)
            {
                float act = parent.LocalFood.Apply(-food);

                float speed = act / hours;

                HRPSpeed = speed * FoodConvertionRate;
                CarbonProduceSpeed += Math.Max(0, 1 - FoodConvertionRate) * speed;
            }




        }

        #endregion
    }
}
