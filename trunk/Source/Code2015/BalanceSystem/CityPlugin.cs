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

        public CityPlugin(CityPluginType type, CityPluginTypeId typeId)
        {
            this.Name = type.TypeName;
            this.type = type;
            this.TypeId = typeId;
            this.HRPConvRate = type.HRPConvRate;
            this.LRPConvRate = type.LRPConvRate;
            this.FoodConvRate = type.FoodConvRate;
        }
        #region  属性

        public NaturalResource CurrentResource
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

        public CityPluginType Type 
        {
            get { return type; }
        }

        public CityPluginTypeId TypeId
        {
            get;
            private set;
        }

        /// <summary>
        ///  0..1之间的建造进度
        /// </summary>
        public float BuildProgress
        {
            get;
            private  set;
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
            SelectResource();
        }

        public void NotifyRemoved(City city)
        {
            parent = null;
            resource.Clear();
        }

        void SelectResource()
        {
            if (CurrentResource == null || CurrentResource.CurrentAmount < float.Epsilon)
            {
                bool finished = false;
                int tries = 0;

                while (!finished && tries < resource.Count)
                {
                    tries++;

                    int index = Randomizer.GetRandomInt(int.MaxValue);
                    CurrentResource = resource[index % resource.Count];

                    switch (TypeId)
                    {
                        case CityPluginTypeId.WoodFactory:
                            finished = CurrentResource.Type == NaturalResourceType.Wood && CurrentResource.CurrentAmount > float.Epsilon;
                            break;
                        case CityPluginTypeId.OilRefinary:
                            finished = CurrentResource.Type == NaturalResourceType.Petro && CurrentResource.CurrentAmount > float.Epsilon;
                            break;
                    }
                }

                if (!finished)
                    CurrentResource = null;
            }
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

                    // 如果没有采集目标，定下
                    SelectResource();

                    #region 处理采集自然资源

                    float food = type.FoodCostSpeed * hours;
                    float hpResource = type.HRCSpeed * hours;
                    float lpResource = type.LRCSpeed * hours;

                    CarbonProduceSpeed = 0;
                    if (CurrentResource != null)
                    {
                        if (hpResource > float.Epsilon ||
                            lpResource > float.Epsilon)
                        {

                            NaturalResource res = CurrentResource;

                            if (hpResource > 0)
                            {
                                if (res.Type == NaturalResourceType.Petro)
                                {
                                    //采集资源

                                    float act = res.Exploit(hpResource);
                                    float speed = act / hours;

                                    HRPSpeed = speed * HRPConvRate;
                                    CarbonProduceSpeed += speed * Math.Max(0, 1 - HRPConvRate);
                                    hpResource -= act;
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
                                    lpResource -= act;
                                }
                            }

                        }
                    }

                    if (food > float.Epsilon)
                    {
                        float act = parent.LocalFood.Apply(food);

                        float speed = act / hours;

                        HRPSpeed = speed * FoodConvRate;
                        CarbonProduceSpeed += Math.Max(0, 1 - FoodConvRate) * speed;
                    }
                    #endregion
                    break;
            }

        }

        #endregion

    }
}
