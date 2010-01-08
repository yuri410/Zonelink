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
    class CityPluginType : IConfigurable
    {

        #region IConfigurable 成员

        public void Parse(ConfigurationSection sect)
        {
            throw new NotImplementedException();
        }

        #endregion



    }
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

        //#region IConfigurable 成员

        //public void Parse(ConfigurationSection sect)
        //{

        //    Cost = sect.GetSingle("Cost");
        //    UpgradeCostBase = sect.GetSingle("UpgradeCostBase");

        //    //FullHRPSpeed = sect.GetSingle("FullHPProductionSpeed");
        //    //FullLRPSpeed = sect.GetSingle("FullLPProductionSpeed");
        //    //FullCarbonProduceSpeed = sect.GetSingle("FullCarbonProduceSpeed");

        //    //NaturalHRCSpeed = sect.GetSingle("HRCostSpeed");
        //    //NaturalLRCSpeed = sect.GetSingle("LRCostSpeed");
        //    FoodCostSpeed = sect.GetSingle("FoodCostSpeed");
        //    GatherRadius = sect.GetSingle("GatherRadius");

        //}


        //#endregion

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

        /// <summary>
        ///  获取在资源充足的条件下，高能资源消耗的速度
        /// </summary>
        public float HRCSpeedFull
        {
            get;
            private set;
        }
        /// <summary>
        ///  获取在资源充足的条件下，低能资源消耗的速度
        /// </summary>
        public float LRCSpeedFull
        {
            get;
            private set;
        }

        public float FoodCostSpeed
        {
            get;
            protected set;
        }

        /// <summary>
        ///  获取在当前状况下，高能资源消耗的速度
        /// </summary>
        public float HRCSpeed
        {
            get;
            private set;
        }
        /// <summary>
        ///  获取在当期情况下，低能资源消耗的速度
        /// </summary>
        public float LRCSpeed
        {
            get;
            private set;
        }




        public float HRPConvRate
        {
            get;
            private set;
        }

        public float LRPConvRate
        {
            get;
            private set;
        }

        public float FoodConvRate
        {
            get;
            private set;
        }

        /// <summary>
        ///  获取在当前状况下，高能资源产生的速度
        /// </summary>
        public float HRPSpeed
        {
            get;
            protected set;
        }
        /// <summary>
        ///  获取在当前状况下，低能资源产生的速度
        /// </summary>
        public float LRPSpeed
        {
            get;
            protected set;
        }

        /// <summary>
        ///  获取在当前状况下的自身总碳排放量
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

            #region 处理采集自然资源
            int index = Randomizer.GetRandomInt(resource.Count);

            float food = FoodCostSpeed * hours;
            float hpResource = HRCSpeedFull * hours;
            float lpResource = LRCSpeedFull * hours;

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

                            HRPSpeed = speed * HRPConvRate;
                            CarbonProduceSpeed += speed * Math.Max(0, 1 - HRPConvRate);
                            hpResource = 0;
                        }
                    }
                    if (lpResource > 0)
                    {
                        if (res.Type == NaturalResourceType.Wood)
                        {
                            float act = res.Exploit(lpResource);
                            float speed = act / hours;

                            LRPSpeed = speed * LRPConvRate;
                            CarbonProduceSpeed += speed * Math.Max(0, 1 - LRPConvRate);
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

                HRPSpeed = speed * FoodConvRate;
                CarbonProduceSpeed += Math.Max(0, 1 - FoodConvRate) * speed;
            }
            #endregion

            #region 处理消耗资源

            // 高能资源消耗量
            float hrChange = HRCSpeedFull * hours;

            if (hrChange > float.Epsilon ||
                hrChange < -float.Epsilon)
            {
                float actHrChange = parent.LocalHR.Apply(-hrChange);

                HRCSpeed = actHrChange / hours;
            }

            // 低能资源消耗量
            float lrChange = LRCSpeedFull * hours;

            if (lrChange > float.Epsilon ||
                lrChange < -float.Epsilon)
            {
                float actLrChange = parent.LocalLR.Apply(-lrChange);

                LRCSpeed = actLrChange / hours;
            }

            CarbonProduceSpeed += -HRCSpeed - LRCSpeed;
            #endregion
        }

        #endregion
    }
}
