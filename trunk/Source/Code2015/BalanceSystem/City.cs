using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.MathLib;

namespace Code2015.BalanceSystem
{
    class ResourceType
    {
        float amount;
        float limit;

        public ResourceType(float a, float limit)
        {
            this.amount = a;
            this.limit = limit;
        }

        public float Current
        {
            get { return amount; }
            private set { amount = value; }
        }

        public float MaxLimit
        {
            get { return limit; }
            set { limit = value; }
        }


        /// <summary>
        ///  申请获得资源
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>实际申请到的能源量</returns>
        public float Apply(float amount)
        {
            if (Current >= amount)
            {
                Current -= amount;
                return amount;
            }
            float r = Current;
            Current = 0;
            return r;
        }

        public float Commit(float amount)
        {
            float r = Current + amount;
            if (r > MaxLimit)
            {
                r = MaxLimit - Current;
                Current = MaxLimit;
                return r;
            }
            Current = r;
            return amount;
        }
    }

    public enum UrbanSize
    {
        Small = 0,
        Normal = 1,
        Large = 2
    }

    /// <summary>
    ///  表示一座城市
    /// </summary>
    public class City : SimulateObject, IUpdatable
    {
        [SLGValue]
        const int TownPluginCount = 1;
        [SLGValue]
        const int NormalPluginCount = 3;
        [SLGValue]
        const int LargePluginCount = 4;

        #region 能源流通速度
        [SLGValueAttribute()]
        const float SmallCityHPTranportSpeed = 30;
        [SLGValueAttribute()]
        const float MediumCityHPTranportSpeed = 50;
        [SLGValueAttribute()]
        const float LargeCityHPTranportSpeed = 100;

        [SLGValueAttribute()]
        const float SmallCityLPTranportSpeed = 15;
        [SLGValueAttribute()]
        const float MediumCityLPTranportSpeed = 25;
        [SLGValueAttribute()]
        const float LargeCityLPTranportSpeed = 50;

        [SLGValue]
        const float SmallFoodTranportSpeed = 20;
        [SLGValue]
        const float MediumFoodTranportSpeed = 30;
        [SLGValue]
        const float LargeFoodTranportSpeed = 50;

        #endregion

        #region 能源使用速度
        [SLGValueAttribute()]
        const float SmallCityLPSpeed = -30;
        [SLGValueAttribute()]
        const float MediumCityLPSpeed = -50;
        [SLGValueAttribute()]
        const float LargeCityLPSpeed = -100;

        [SLGValueAttribute()]
        const float SmallCityHPSpeed = -30;
        [SLGValueAttribute()]
        const float MediumCityHPSpeed = -50;
        [SLGValueAttribute()]
        const float LargeCityHPSpeed = -100;

        #endregion

        #region 存储 最大量
        [SLGValue]
        const int SmallMaxLPStorage = 100;
        [SLGValue]
        const int MediumMaxLPStorage = 1000;
        [SLGValue]
        const int LargeMaxLPStorage = 3000;

        [SLGValue]
        const int SmallMaxHPStorage = 100;
        [SLGValue]
        const int MediumMaxHPStorage = 1000;
        [SLGValue]
        const int LargeMaxHPStorage = 3000;

        [SLGValue]
        const int SmallMaxFoodStorage = 100;
        [SLGValue]
        const int MediumMaxFoodStorage = 1000;
        [SLGValue]
        const int LargeMaxFoodStorage = 3000;
        #endregion

        [SLGValue]
        const float SmallDevMult = 1;
        [SLGValue]
        const float MediumDevMult = 0.33f;
        [SLGValue]
        const float LargeDevMult = 0.1f;

        [SLGValue]
        const float DevBias = -10;

        static readonly float[] DevMult = { SmallDevMult, MediumDevMult, LargeDevMult };
        static readonly float[] LPSpeed = { SmallCityLPSpeed, MediumCityLPSpeed, LargeCityLPSpeed };
        static readonly float[] HPSpeed = { SmallCityHPSpeed, MediumCityHPSpeed, LargeCityHPSpeed };

        static readonly float[] LPTSpeed = { SmallCityLPTranportSpeed, MediumCityLPTranportSpeed, LargeCityLPTranportSpeed };
        static readonly float[] HPTSpeed = { SmallCityHPTranportSpeed, MediumCityHPTranportSpeed, LargeCityHPTranportSpeed };
        static readonly float[] FoodTSpeed = { SmallFoodTranportSpeed, MediumFoodTranportSpeed, LargeFoodTranportSpeed };

        public City(EnergyStatus energyStat)
        {
            this.energyStat = energyStat;
            localLp = new ResourceType(SmallMaxLPStorage, float.MaxValue);
            localHp = new ResourceType(SmallMaxHPStorage, float.MaxValue);
            localFood = new ResourceType(SmallMaxFoodStorage, float.MaxValue);
            UpdateCity();
        }
        public City(EnergyStatus energyStat, UrbanSize size)
            : this(energyStat)
        {
            this.Size = size;
        }
        public City(EnergyStatus energyStat, string name)
            : this(energyStat)
        {
            this.Name = name;
        }


        EnergyStatus energyStat;
        /// <summary>
        ///  表示城市的附加设施
        /// </summary>
        FastList<CityPlugin> plugins;

        ResourceType localLp;
        ResourceType localHp;
        ResourceType localFood;

        #region  属性
        public string Name
        {
            get;
            set;
        }
        public float Development
        {
            get;
            set;
        }
        public float Population
        {
            get;
            private set;
            //{
            //    switch (Size)
            //    {
            //        case UrbanSize.Small:
            //            return 20000;
            //        case UrbanSize.Normal:
            //            return 500000;
            //        case UrbanSize.Large:
            //            return 3000000;
            //    }
            //    return 0;
            //}
        }
        public float Disease
        {
            get;
            set;
        }

        public float DevelopmentMult
        {
            get { return DevMult[(int)Size]; }
        }
        public float HPTransportSpeed
        {
            get { return HPTSpeed[(int)Size]; }
        }
        public float LPTransportSpeed
        {
            get { return LPTSpeed[(int)Size]; }
        }
        public float FoodTransportSpeed
        {
            get { return FoodTSpeed[(int)Size]; }
        }
        #region 产出/投入
        public float SelfHPProductionSpeed
        {
            get { return HPSpeed[(int)Size]; }
        }
        public float SelfLPProductionSpeed
        {
            get { return LPSpeed[(int)Size]; }
        }
        public float SelfFoodCostSpeed
        {
            get;
            protected set;
        }

        /// <summary>
        /// 若有Plugin，获取Plugin的消耗速度
        /// </summary>
        public float PluginHPProductionSpeed
        {
            get;
            private set;
        }
        public float PluginLPProductionSpeed
        {
            get;
            private set;
        }
        public float PluginFoodCostSpeed
        {
            get;
            private set;
        }
        public float PluginCarbonProduceSpeed
        {
            get;
            private set;
        }

        /// <summary>
        ///  城市自身的产生高能和低能的速度，初始为负值，表示消耗
        /// </summary>
        public float ProduceHPSpeed
        {
            get { return PluginHPProductionSpeed + SelfHPProductionSpeed; }
        }
        public float ProduceLPSpeed
        {
            get { return PluginLPProductionSpeed + SelfLPProductionSpeed; }
        }
        public float FoodCostSpeed
        {
            get { return SelfFoodCostSpeed + PluginFoodCostSpeed; }
        }

        #endregion



        #endregion

        public UrbanSize Size
        {
            get;
            private set;
        }

        /// <summary>
        ///  获取目前城市最多可以添加的附加设施数量
        /// </summary>
        public int MaxPlugins
        {
            get
            {
                switch (Size)
                {
                    case UrbanSize.Small:
                        return TownPluginCount;
                    case UrbanSize.Normal:
                        return NormalPluginCount;
                    case UrbanSize.Large:
                        return LargePluginCount;
                }
                return 0;
            }
        }

        public void Add(CityPlugin plugin)
        {
            plugins.Add(plugin);
            UpdateCity();
            plugin.NotifyAdded(this);
        }

        public void Remove(CityPlugin plugin)
        {
            plugins.Remove(plugin);
            UpdateCity();
            plugin.NotifyRemoved(this);
        }


        /// <summary>
        ///  更新城市的属性设置
        /// </summary>
        public void UpdateCity()
        {
            plugins = new FastList<CityPlugin>();
            switch (Size)
            {
                case UrbanSize.Large:
                    this.SelfFoodCostSpeed = 50;
                    this.CarbonProduceSpeed = 500;
                    localLp.MaxLimit = LargeMaxLPStorage;
                    localHp.MaxLimit = LargeMaxHPStorage;
                    localFood.MaxLimit = LargeMaxFoodStorage;
                    break;
                case UrbanSize.Normal:
                    this.SelfFoodCostSpeed = 30;
                    this.CarbonProduceSpeed = 300;
                    localLp.MaxLimit = MediumMaxLPStorage;
                    localHp.MaxLimit = MediumMaxHPStorage;
                    localFood.MaxLimit = MediumMaxFoodStorage;
                    break;
                case UrbanSize.Small:
                    this.SelfFoodCostSpeed = 10;
                    localLp.MaxLimit = SmallMaxLPStorage;
                    localHp.MaxLimit = SmallMaxHPStorage;
                    localFood.MaxLimit = SmallMaxFoodStorage;
                    this.CarbonProduceSpeed = 100;
                    break;
            }

            PluginFoodCostSpeed = 0;
            PluginHPProductionSpeed = 0;
            PluginLPProductionSpeed = 0;
            PluginCarbonProduceSpeed = 0;
            for (int i = 0; i < plugins.Count; i++)
            {
                CarbonProduceSpeed += plugins[i].CarbonProduceSpeed;
                PluginFoodCostSpeed += plugins[i].FoodCostSpeed;
                PluginHPProductionSpeed += plugins[i].HPProductionSpeed;
                PluginLPProductionSpeed += plugins[i].LPProductionSpeed;
                PluginCarbonProduceSpeed += plugins[i].CarbonProduceSpeed;
            }
        }

        /// <summary>
        /// 高能由石油厂和生态工厂产生
        /// </summary>
        /// <returns></returns>
        public float GetPluginHPProductionSpeed()
        {
            return PluginHPProductionSpeed - PluginFoodCostSpeed;//获得由炼油厂产生的高能速度
        }
        public float GetPluginLPProductionSpeed()
        {
            return PluginLPProductionSpeed;
        }
        public float GetPluginFoodCostSpeed()
        {
            return PluginFoodCostSpeed;//获得由生态工厂产生的高能速度
        }


        public int PluginCount
        {
            get { return plugins.Count; }
        }

        public CityPlugin this[int i]
        {
            get { return plugins[i]; }
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
            float hours = (float)time.ElapsedGameTime.TotalHours;
            this.CarbonChange += PluginCarbonProduceSpeed * hours;


            float hpChange = ProduceHPSpeed * hours;
            float lpChange = ProduceLPSpeed * hours;

            float foodChange = FoodCostSpeed * hours;

            #region 补缺储备
            {
                float requirement = localLp.MaxLimit - localLp.Current;

                if (requirement > 0)
                {
                    bool passed = true;
                    if (energyStat.IsLPLow)
                        passed ^= Randomizer.GetRandomBool();
                    if (passed)
                    {
                        float applyAmount = Math.Min(requirement * hours, LPTransportSpeed * hours);
                        applyAmount = energyStat.ApplyLPEnergy(applyAmount);
                        localLp.Commit(applyAmount);
                    }
                }
            }
            {
                float requirement = localHp.MaxLimit - localHp.Current;

                if (requirement > 0)
                {
                    bool passed = true;
                    if (energyStat.IsHPLow)
                        passed ^= Randomizer.GetRandomBool();
                    if (passed)
                    {
                        float applyAmount = Math.Min(requirement * hours, HPTransportSpeed * hours);
                        applyAmount = energyStat.ApplyHPEnergy(applyAmount);
                        localHp.Commit(applyAmount);
                    }
                }
            }
            {
                float requirement = localFood.MaxLimit - localFood.Current;

                if (requirement > 0)
                {
                    bool passed = true;
                    if (energyStat.IsFoodLow)
                        passed ^= Randomizer.GetRandomBool();
                    if (passed)
                    {
                        float applyAmount = Math.Min(requirement * hours, FoodTransportSpeed * hours);
                        applyAmount = energyStat.ApplyFood(applyAmount);
                        localFood.Commit(applyAmount);
                    }
                }
            }
            #endregion

            // 计算发展度
            float hpnewDev = DevBias;

            #region 
            if (hpChange > 0)
            {
                float act = localHp.Commit(hpChange);

                if (act < hpChange)
                {
                    energyStat.CommitHPEnergy(Math.Min(hpChange - act, HPTransportSpeed * hours));
                }
                for (int i = 0; i < plugins.Count; i++)
                {
                    if (plugins[i].HPProductionSpeed < 0)
                    {
                        hpnewDev -= plugins[i].HPProductionSpeed * hours * DevelopmentMult;
                    }
                }
                if (SelfHPProductionSpeed < 0)
                    hpnewDev -= SelfHPProductionSpeed * hours * DevelopmentMult;
            }
            else
            {
                float act = localHp.Apply(-hpChange);

                for (int i = 0; i < plugins.Count; i++)
                {
                    if (plugins[i].HPProductionSpeed < 0)
                    {
                        hpnewDev -= plugins[i].HPProductionSpeed * hours * DevelopmentMult;
                    }
                }
                if (SelfHPProductionSpeed < 0)
                    hpnewDev -= SelfHPProductionSpeed * hours * DevelopmentMult;

                hpnewDev += (act + hpChange) * DevelopmentMult;
            }

            float lpnewDev = 0;
            if (hpChange > 0)
            {
                float act = localLp.Apply(lpChange);

                if (act < lpChange)
                {
                    energyStat.CommitHPEnergy(Math.Min(hpChange - act, LPTransportSpeed * hours));
                }
                for (int i = 0; i < plugins.Count; i++)
                {
                    if (plugins[i].LPProductionSpeed < 0)
                    {
                        lpnewDev -= plugins[i].LPProductionSpeed * hours * DevelopmentMult;
                    }
                }
                if (SelfLPProductionSpeed < 0)
                    lpnewDev -= SelfLPProductionSpeed * hours * DevelopmentMult;
            }
            else
            {
                float act = localLp.Apply(-lpChange);

                for (int i = 0; i < plugins.Count; i++)
                {
                    if (plugins[i].LPProductionSpeed < 0)
                    {
                        lpnewDev -= plugins[i].LPProductionSpeed * hours * DevelopmentMult;
                    }
                }
                if (SelfLPProductionSpeed < 0)
                    lpnewDev -= SelfLPProductionSpeed * hours * DevelopmentMult;

                lpnewDev += (act + lpChange) * DevelopmentMult;
            }
            #endregion


            float actFood = localFood.Apply(foodChange);

            float foodLack = actFood - foodChange;
            if (foodLack < 0)
            {
                if (Disease < float.Epsilon)
                {
                    Disease = 1;
                }
            }
            else
            {
                Disease -= foodLack * 0.1f;
            }

            if (Disease > 0)
            {
                Disease += Disease * 0.01f;
            }
            else
            {
                Disease = 0;
            }


            float popChange = 0;
            if (Disease > 0)
            {
                popChange -= Disease * 0.1f;
            }
            Population += popChange;

            if (Population < 0)
            {
                Population = 0;
            }

            // TODO：对数（人口参照）作为发展的调整值
            Development += lpnewDev * 0.5f + hpnewDev + foodLack;
            if (Development < 0)
            {
                Development = 0;
            }
        }
    }
}
