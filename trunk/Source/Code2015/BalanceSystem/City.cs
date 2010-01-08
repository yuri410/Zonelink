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
    /// <summary>
    ///  表示一种资源的存储器。
    ///  可以储存一定量的资源。并且从这批资源中申请获得一定数量，以及将一定数量的资源存储进来
    /// </summary>
    /// <remarks>生产有限，使用无限</remarks>
    public class ResourceStorage
    {
        float amount;
        float limit;

        public ResourceStorage(float a, float limit)
        {
            this.amount = a;
            this.limit = limit;
        }

        /// <summary>
        ///  获取或设置当前资源数量
        /// </summary>
        public float Current
        {
            get { return amount; }
            private set { amount = value; }
        }

        /// <summary>
        ///  获取或设置资源存储量的上限
        /// </summary>
        public float MaxLimit
        {
            get { return limit; }
            set { limit = value; }
        }


        /// <summary>
        ///  申请获得能源
        /// </summary>
        /// <param name="amount">要求的能源量</param>
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

        /// <summary>
        ///  将过剩资源提交，存储起来
        /// </summary>
        /// <param name="amount">提交的数量</param>
        /// <returns>实际接受提交的数量</returns>
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

    /// <summary>
    ///  表示城市的大小
    /// </summary>
    public enum UrbanSize
    {
        /// <summary>
        ///  小城
        /// </summary>
        Small = 0,
        /// <summary>
        ///  中型城市
        /// </summary>
        Medium = 1,
        /// <summary>
        ///  大型城市
        /// </summary>
        Large = 2
    }

    public class City : SimulateObject, IConfigurable, IUpdatable
    {
        FastList<NaturalResource> farms = new FastList<NaturalResource>();

        /// <summary>
        ///  发展增量的偏移值。无任何附加条件下的发展量。
        /// </summary>
        [SLGValue]
        const float DevBias = -3;


        public City(EnergyStatus energyStat)
            : base(energyStat.Region)
        {
            this.energyStat = energyStat;
            localLr = new ResourceStorage(CityGrade.SmallMaxLPStorage, float.MaxValue);
            localHr = new ResourceStorage(CityGrade.SmallMaxHPStorage, float.MaxValue);
            localFood = new ResourceStorage(CityGrade.SmallMaxFoodStorage, float.MaxValue);
            UpgradeUpdate();
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
        FastList<CityPlugin> plugins = new FastList<CityPlugin>();

        ResourceStorage localLr;
        ResourceStorage localHr;
        ResourceStorage localFood;
        UrbanSize size;

        #region  属性


        /// <summary>
        ///  获取城市已存储（已缓存）的高能资源
        /// </summary>
        public ResourceStorage LocalHR
        {
            get { return localHr; }
        }

        /// <summary>
        ///  获取城市已存储（已缓存）的低能资源
        /// </summary>
        public ResourceStorage LocalLR
        {
            get { return localLr; }
        }

        /// <summary>
        ///  获取城市已存储（已缓存）的食物数量
        /// </summary>
        public ResourceStorage LocalFood
        {
            get { return localFood; }
        }


        /// <summary>
        ///  获取城市的名称
        /// </summary>
        public string Name
        {
            get;
            private set;
        }
        /// <summary>
        ///  获取城市的发展度
        /// </summary>
        public float Development
        {
            get;
            private set;
        }
        /// <summary>
        ///  获取城市的人口数量
        /// </summary>
        public float Population
        {
            get;
            private set;
        }
        /// <summary>
        ///  获取城市的疾病因数
        /// </summary>
        public float Disease
        {
            get;
            private set;
        }




        public float GetSelfFoodCostSpeedFull()
        {
            return Population * 0.05f;
        }


        public float SelfHRCRatio
        {
            get;
            private set;
        }
        public float SelfLRCRatio
        {
            get;
            private set;
        }
        public float SelfFoodCostRatio
        {
            get;
            private set;
        }

        public float GetSelfFoodCostSpeed()
        {
            return GetSelfFoodCostSpeedFull() * SelfFoodCostRatio;
        }
        public float GetSelfHPCSpeed()
        {
            return CityGrade.GetSelfHRCSpeed(Size) * SelfHRCRatio;
        }
        public float GetSelfLPCSpeed()
        {
            return CityGrade.GetSelfLRCSpeed(Size) * SelfLRCRatio;
        }

        public bool CanAddPlugins
        {
            get { return plugins.Count < CityGrade.GetMaxPlugins(Size); }
        }

        #region 产出/投入


        #endregion

        #endregion

        /// <summary>
        ///  获取城市的大小
        /// </summary>
        public UrbanSize Size
        {
            get { return size; }
            private set
            {
                if (value != size)
                {
                    size = value;
                    UpgradeUpdate();
                }
            }
        }


        /// <summary>
        ///  添加一个<see cref="CityPlugin"/>到当前城市中，会用CityPlugin.NotifyAdded告知CityPlugin被添加了
        /// </summary>
        /// <param name="plugin"></param>
        public void Add(CityPlugin plugin)
        {
            if (CanAddPlugins)
            {
                plugins.Add(plugin);
                plugin.NotifyAdded(this);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        ///  从当前城市中删除一个<see cref="CityPlugin"/>，会用CityPlugin.NotifyRemoved告知CityPlugin被删除了
        /// </summary>
        /// <param name="plugin"></param>
        public void Remove(CityPlugin plugin)
        {
            plugins.Remove(plugin);
            plugin.NotifyRemoved(this);
        }


        /// <summary>
        ///  更新城市 所有与等级相关的属性
        /// </summary>
        void UpgradeUpdate()
        {
            switch (Size)
            {
                case UrbanSize.Large:
                    localLr.MaxLimit = CityGrade.LargeMaxLPStorage;
                    localHr.MaxLimit = CityGrade.LargeMaxHPStorage;
                    localFood.MaxLimit = CityGrade.LargeMaxFoodStorage;
                    break;
                case UrbanSize.Medium:

                    localLr.MaxLimit = CityGrade.MediumMaxLPStorage;
                    localHr.MaxLimit = CityGrade.MediumMaxHPStorage;
                    localFood.MaxLimit = CityGrade.MediumMaxFoodStorage;
                    break;
                case UrbanSize.Small:
                    localLr.MaxLimit = CityGrade.SmallMaxLPStorage;
                    localHr.MaxLimit = CityGrade.SmallMaxHPStorage;
                    localFood.MaxLimit = CityGrade.SmallMaxFoodStorage;
                    break;
            }
        }


        /// <summary>
        ///  获取这个城市的Plugin数量
        /// </summary>
        public int PluginCount
        {
            get { return plugins.Count; }
        }

        /// <summary>
        ///  通过索引获取城市的Plugin
        /// </summary>
        /// <param name="i">在容器中的索引</param>
        /// <returns></returns>
        public CityPlugin this[int i]
        {
            get { return plugins[i]; }
        }


        const float SmallCityPointThreshold = 10000;
        const float MediumCityPointThreshold = 100000;
        //const float LargeCityPointThreshold = 1000000;

        /// <summary>
        ///  计算城市的分数，用于评估城市的等级
        /// </summary>
        /// <param name="dev"></param>
        /// <param name="pop"></param>
        /// <returns></returns>
        static float GetCityPoints(float dev, float pop)
        {
            return dev * pop;
        }


        public override void Update(GameTime time)
        {
            CarbonProduceSpeed = 0;


            #region 城市自动级别调整
            float points = GetCityPoints(Development, Population);

            if (points < MediumCityPointThreshold)
            {
                if (points < SmallCityPointThreshold)
                {
                    Size = UrbanSize.Small;
                }
                else
                {
                    Size = UrbanSize.Medium;
                }
            }
            else
            {
                Size = UrbanSize.Large;
            }
            #endregion

            for (int i = 0; i < plugins.Count; i++)
            {
                plugins[i].Update(time);
            }

            float hours = (float)time.ElapsedGameTime.TotalHours;

            #region 补缺储备，物流
            {
                float requirement = localLr.MaxLimit - localLr.Current;

                if (requirement > 0)
                {
                    bool passed = true;
                    if (energyStat.IsLPLow)
                        passed ^= Randomizer.GetRandomBool();
                    if (passed)
                    {
                        float applyAmount = Math.Min(requirement * hours, CityGrade.GetLPTransportSpeed(Size) * hours);
                        applyAmount = energyStat.ApplyLPEnergy(applyAmount);
                        localLr.Commit(applyAmount);
                    }
                }
            }
            {
                float requirement = localHr.MaxLimit - localHr.Current;

                if (requirement > 0)
                {
                    bool passed = true;
                    if (energyStat.IsHPLow)
                        passed ^= Randomizer.GetRandomBool();
                    if (passed)
                    {
                        float applyAmount = Math.Min(requirement * hours, CityGrade.GetHPTransportSpeed(Size) * hours);
                        applyAmount = energyStat.ApplyHPEnergy(applyAmount);
                        localHr.Commit(applyAmount);
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
                        float applyAmount = Math.Min(requirement * hours, CityGrade.GetFoodTransportSpeed(Size) * hours);
                        applyAmount = energyStat.ApplyFood(applyAmount);
                        localFood.Commit(applyAmount);
                    }
                }
            }
            #endregion


            float hrDev = 0;
            float lrDev = 0;

            // 严禁使用旧的模式，属性泛滥
            #region 资源消耗计算

            // 计算插件
            for (int i = 0; i < plugins.Count; i++)
            {
                // 高能资源消耗量
                float hrChange = plugins[i].HRCSpeed * hours;
                hrDev += hrChange * CityGrade.GetDevelopmentMult(Size);

                // 低能资源消耗量
                float lrChange = plugins[i].LRCSpeed * hours;
                lrDev += lrDev * CityGrade.GetDevelopmentMult(Size);


                hrChange = plugins[i].HRPSpeed * hours;
                float actHrChange = localHr.Commit(hrChange);

                if (actHrChange < hrChange)
                {
                    energyStat.CommitHPEnergy(Math.Min(hrChange - actHrChange, CityGrade.GetHPTransportSpeed(Size) * hours));
                }

                lrChange = plugins[i].LRPSpeed * hours;
                float actLrChange = localLr.Commit(lrChange);

                if (actLrChange < lrChange)
                {
                    energyStat.CommitHPEnergy(Math.Min(lrChange - actLrChange, CityGrade.GetLPTransportSpeed(Size) * hours));
                }


                CarbonProduceSpeed += plugins[i].CarbonProduceSpeed;
            }


            float foodLack = 0;
            // 计算自身
            {
                // 高能资源消耗量
                float hrChange = CityGrade.GetSelfHRCSpeed(Size) * hours;
                if (hrChange > float.Epsilon ||
                    hrChange < -float.Epsilon)
                {
                    float actHrChange = localHr.Apply(-hrChange);
                    hrDev += actHrChange * CityGrade.GetDevelopmentMult(Size);
                }

                // 低能资源消耗量
                float lrChange = CityGrade.GetSelfLRCSpeed(Size) * hours;
                if (lrChange > float.Epsilon ||
                    lrChange < -float.Epsilon)
                {
                    float actLrChange = localLr.Apply(-lrChange);
                    lrDev += actLrChange * CityGrade.GetDevelopmentMult(Size);
                }

                float foodSpeedFull = GetSelfFoodCostSpeedFull();

                float foodChange = (-foodSpeedFull + CityGrade.GetSelfFoodGatheringSpeed(Size)) * hours;

                float actFood = localFood.Apply(-foodChange);


                // 食物 碳排量计算
                CarbonProduceSpeed += foodSpeedFull * actFood / -foodChange;

                // 计算疾病发生情况
                foodLack = actFood + foodChange;

                if (foodLack < 0)
                {
                    if (Disease < float.Epsilon)
                    {
                        Disease = 0.01f;
                    }
                }
                else
                {
                    Disease -= actFood;
                }


            }
            #endregion


            // 疾病发展传播计算
            if (Disease > 0)
            {
                Disease += Disease * (float)Math.Log(Population, 100) * 0.001f;
            }
            else
            {
                Disease = 0;
            }

            // 计算人口变化情况
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


            float popDevAdj = 1;
            if (Population > 0)
            {
                //if (Population < 2 * RefPopulation)
                //{
                //    popDevAdj = Population <= RefPopulation ?
                //        (float)Math.Log(Population, RefPopulation) : (float)Math.Log(2 * RefPopulation - Population, RefPopulation);


                //    if (devIncr < 0)
                //    {
                //        if (popDevAdj < 0)
                //        {
                //            popDevAdj = -popDevAdj;
                //        }
                //    }

                //    //devIncr = devIncr < 0 ? 1000 : -1000;
                //    devIncr *= popDevAdj;
                //}
                //else
                //{
                //    popDevAdj = devIncr < 0 ? 1000 : -1000;
                //    devIncr *= popDevAdj;
                //}


            }
#warning sign check
            float devIncr = popDevAdj * (lrDev * 0.5f + lrDev - DevBias);
            Development += devIncr + foodLack;
            if (Development < 0)
            {
                Development = 0;
            }
            if (devIncr > 0)
            {
                Population += (devIncr + foodLack) * 0.01f;
            }

            base.Update(time);
        }

        #region IConfigurable 成员

        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);
            Name = sect.GetString("Name", string.Empty);
            Population = sect.GetSingle("Population");
            Size = (UrbanSize)Enum.Parse(typeof(UrbanSize), sect.GetString("Size", string.Empty));

            UpgradeUpdate();
        }

        #endregion
    }
}
