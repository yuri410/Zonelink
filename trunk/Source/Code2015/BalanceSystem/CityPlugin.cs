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
        public const float UpgradeAmount = 0.25f;

        City parent;
        CityPluginType type;
        FastList<NaturalResource> resource = new FastList<NaturalResource>();

        CityPluginFactory factory;

        public CityPlugin(CityPluginFactory fac, CityPluginType type, CityPluginTypeId typeId)
        {
            this.Name = type.TypeName;
            this.factory = fac;
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
        public bool IsBuilding 
        {
            get { return BuildProgress < 1; }
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

        public void Upgrade(float amount)
        {
            if (HRPConvRate > 1 - float.Epsilon && TypeId == CityPluginTypeId.OilRefinary)
            {
                CityPlugin plugin = factory.MakeBioEnergeFactory();
                HRPConvRate = plugin.HRPConvRate;
                LRPConvRate = plugin.LRPConvRate;
                FoodConvRate = plugin.FoodConvRate;
                
                Name = plugin.Name;
                TypeId = plugin.TypeId;

                HRCSpeed = plugin.HRCSpeed;
                LRCSpeed = plugin.LRCSpeed;
                HRPSpeed = plugin.HRPSpeed;
                LRPSpeed = plugin.LRPSpeed;

               

                resource.Clear();
            }

            HRPConvRate = MathEx.Saturate(amount + HRPConvRate);
            LRPConvRate = MathEx.Saturate(amount + LRPConvRate);
            FoodConvRate = MathEx.Saturate(amount + FoodConvRate);

            
        }

        void FindResources()
        {
            if (TypeId != CityPluginTypeId.WoodFactory && TypeId != CityPluginTypeId.OilRefinary)
                return;

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
                            switch (TypeId)
                            {
                                case CityPluginTypeId.OilRefinary:
                                    if (res is OilField)
                                    {
                                        resource.Add(res);
                                    }
                                    break;
                                case CityPluginTypeId.WoodFactory:
                                    if (res is Forest)
                                    {
                                        resource.Add(res);
                                    }
                                    break;
                            }
                            
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
            SelectResourceStandard();
        }

        public void NotifyRemoved(City city)
        {
            parent = null;
            resource.Clear();
        }
        void SelectResourceStandard()
        {
            if (CurrentResource == null || CurrentResource.CurrentAmount < float.Epsilon)
            {
                if (CurrentResource != null)
                    CurrentResource.OutputTarget = null;

                for (int i = 0; i < resource.Count; i++)
                {
                    NaturalResource selRes = resource[i];

                    if (selRes.OutputTarget == null)
                    {
                        if (!selRes.IsLow)
                        {
                            selRes.OutputTarget = this;
                            CurrentResource = selRes;
                            return;
                        }
                    }
                }

                int index = Randomizer.GetRandomInt(resource.Count);
                CurrentResource = resource[index];
            }
        }
        void SelectResource()
        {
            if (CurrentResource == null || CurrentResource.CurrentAmount < float.Epsilon)
            {
                if (CurrentResource != null)
                    CurrentResource.OutputTarget = null;

                int tries = 0;
                NaturalResource selRes = null;

                while (selRes != null && tries < resource.Count)
                {
                    tries++;

                    int index = Randomizer.GetRandomInt(int.MaxValue);
                    selRes = resource[index % resource.Count];

                    if (selRes.IsLow)
                    {
                        selRes = null;
                    }

                }

                if (selRes != null)
                {
                    selRes.OutputTarget = this;
                    CurrentResource = selRes;
                }
            }
        }
     

        #region IUpdatable 成员

        public void Update(GameTime time)
        {
            float hours = (float)time.ElapsedGameTime.TotalHours;

            if (IsBuilding) 
            {
                BuildProgress += 0.03f;
                return;
            }

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
