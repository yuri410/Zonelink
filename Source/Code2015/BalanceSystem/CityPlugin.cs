using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Config;
using Apoc3D.MathLib;

namespace Code2015.BalanceSystem
{
    public class CityPlugin : IUpdatable
    {
        City parent;
        CityPluginType type;
        FastList<NaturalResource> resource = new FastList<NaturalResource>();

        public CityPlugin(CityPluginType type)
        {
            this.Name = type.TypeName;
            this.type = type;
        }
        #region  属性

        public CityPluginType Type 
        {
            get { return type; }
        }

        public float UpgradeProgress
        {
            get;
            set;
        }

        public string Name
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
            SimulationRegion region = parent.Region;

            Vector2 myPos = new Vector2(parent.Latitude, parent.Longitude);
            float r = CityGrade.GetGatherRadius(parent.Size);
            for (int i = 0; i < region.Count; i++)
            {
                if (!object.ReferenceEquals(region[i], parent))
                {
                    NaturalResource res = region[i] as NaturalResource;
                    if (res != null)
                    {
                        Vector2 pos = new Vector2(res.Latitude, res.Longitude);
                        float dist = Vector2.Distance(pos, myPos);

                        if (dist < r)
                        {
                            resource.Add(res);
                        }
                    }
                }
            }
        }


        public void NotifyAdded(City city)
        {
            if (city == null)
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

            switch (type.Behaviour)
            {
                case CityPluginBehaviourType.Hospital:
                case CityPluginBehaviourType.Education:
                    #region 处理消耗资源

                    // 高能资源消耗量

                    float hrChange = type.HRCSpeed * hours;


                    if (hrChange < -float.Epsilon)
                    {
                        float actHrChange = parent.LocalHR.Apply(-hrChange);

                        HRCSpeed = actHrChange / hours;
                    }

                    // 低能资源消耗量

                    float lrChange = type.LRCSpeed * hours;


                    if (lrChange < -float.Epsilon)
                    {
                        float actLrChange = parent.LocalLR.Apply(-lrChange);

                        LRCSpeed = actLrChange / hours;
                    }

                    CarbonProduceSpeed += -HRCSpeed - LRCSpeed;
                    #endregion

                    break;
                case CityPluginBehaviourType.CollectorFactory:
                    #region 处理采集自然资源
                    int index = Randomizer.GetRandomInt(resource.Count);

                    float food = type.FoodCostSpeed * hours;
                    float hpResource = type.HRCSpeed * hours;
                    float lpResource = type.LRCSpeed * hours;

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
                                if (res.Type == NaturalResourceType.Petro)
                                {
                                    //采集资源

                                    float act = res.Exploit(hpResource);
                                    float speed = act / hours;

                                    HRPSpeed = speed * type.HRPConvRate;
                                    CarbonProduceSpeed += speed * Math.Max(0, 1 - type.HRPConvRate);
                                    hpResource -= act;
                                }
                            }
                            if (lpResource > 0)
                            {
                                if (res.Type == NaturalResourceType.Wood)
                                {
                                    float act = res.Exploit(lpResource);
                                    float speed = act / hours;

                                    LRPSpeed = speed * type.LRPConvRate;
                                    CarbonProduceSpeed += speed * Math.Max(0, 1 - type.LRPConvRate);
                                    lpResource -= act;
                                }
                            }

                            finished = (lpResource < float.Epsilon && hpResource < float.Epsilon);

                            index++;
                            tries++;

                        }
                    }
                    if (food > float.Epsilon)
                    {
                        float act = parent.LocalFood.Apply(food);

                        float speed = act / hours;

                        HRPSpeed = speed * type.FoodConvRate;
                        CarbonProduceSpeed += Math.Max(0, 1 - type.FoodConvRate) * speed;
                    }
                    #endregion
                    break;
            }
            
        }

        #endregion

    }
}
