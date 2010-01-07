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
    class ResourceStorage
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
        const float SmallRefPop = 20;
        [SLGValue]
        const float MediumRefPop = 400;
        [SLGValue]
        const float LargeRefPop = 1000;




        FastList<NaturalResource> farms = new FastList<NaturalResource>();


        /// <summary>
        ///  发展增量的偏移值。无任何附加条件下的发展量。
        /// </summary>
        [SLGValue]
        const float DevBias = -10;

        static readonly float[] DevMult = { SmallDevMult, MediumDevMult, LargeDevMult };
        static readonly float[] LPSpeed = { SmallCityLPSpeed, MediumCityLPSpeed, LargeCityLPSpeed };
        static readonly float[] HPSpeed = { SmallCityHPSpeed, MediumCityHPSpeed, LargeCityHPSpeed };

        static readonly float[] LPTSpeed = { SmallCityLPTranportSpeed, MediumCityLPTranportSpeed, LargeCityLPTranportSpeed };
        static readonly float[] HPTSpeed = { SmallCityHPTranportSpeed, MediumCityHPTranportSpeed, LargeCityHPTranportSpeed };
        static readonly float[] FoodTSpeed = { SmallFoodTranportSpeed, MediumFoodTranportSpeed, LargeFoodTranportSpeed };

        public City(EnergyStatus energyStat)
            : base(energyStat.Region)
        {
            this.energyStat = energyStat;
            localLp = new ResourceStorage(SmallMaxLPStorage, float.MaxValue);
            localHp = new ResourceStorage(SmallMaxHPStorage, float.MaxValue);
            localFood = new ResourceStorage(SmallMaxFoodStorage, float.MaxValue);
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

        ResourceStorage localLp;
        ResourceStorage localHp;
        ResourceStorage localFood;

        UrbanSize size;

        #region  属性


        /// <summary>
        ///  获取城市已存储（已缓存）的高能资源数量
        /// </summary>
        public float LocalHP
        {
            get { return localHp.Current; }
        }

        /// <summary>
        ///  获取城市已存储（已缓存）的低能资源数量
        /// </summary>
        public float LocalLP
        {
            get { return localLp.Current; }
        }

        /// <summary>
        ///  获取城市已存储（已缓存）的食物数量
        /// </summary>
        public float LocalFood
        {
            get { return localFood.Current; }
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

        /// <summary>
        ///  获取在当前城市规模下的参考人口（标准人口）
        /// </summary>
        public float RefPopulation
        {
            get
            {
                {
                    switch (Size)
                    {
                        case UrbanSize.Small:
                            return SmallRefPop;
                        case UrbanSize.Medium:
                            return MediumRefPop;
                        case UrbanSize.Large:
                            return LargeRefPop;
                    }
                    return 0;
                }
            }
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
        ///  若有Plugin，获取Plugin的对高能资源的消耗速度
        /// </summary>
        [Obsolete()]
        public float PluginHPProductionSpeed
        {
            get;
            private set;
        }
        /// <summary>
        ///   若有Plugin，获取Plugin的对低能资源的消耗速度
        /// </summary>
        [Obsolete()]
        public float PluginLPProductionSpeed
        {
            get;
            private set;
        }
        /// <summary>
        ///  若有Plugin，获取Plugin的对食物的消耗速度
        /// </summary>
        [Obsolete()]
        public float PluginFoodCostSpeed
        {
            get;
            private set;
        }

        /// <summary>
        ///   若有Plugin，获取Plugin的碳排放速度
        /// </summary>
        [Obsolete()]
        public float PluginCarbonProduceSpeed
        {
            get;
            private set;
        }

        /// <summary>
        ///  城市自身的产生高能和低能的速度，初始为负值，表示消耗
        /// </summary>
        [Obsolete()]
        public float ProduceHPSpeed
        {
            get { return PluginHPProductionSpeed + SelfHPProductionSpeed; }
        }
        [Obsolete()]
        public float ProduceLPSpeed
        {
            get { return PluginLPProductionSpeed + SelfLPProductionSpeed; }
        }
        [Obsolete()]
        public float FoodCostSpeed
        {
            get { return SelfFoodCostSpeed + PluginFoodCostSpeed; }
        }

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
                    case UrbanSize.Medium:
                        return NormalPluginCount;
                    case UrbanSize.Large:
                        return LargePluginCount;
                }
                return 0;
            }
        }

        /// <summary>
        ///  添加一个<see cref="CityPlugin"/>到当前城市中，会用CityPlugin.NotifyAdded告知CityPlugin被添加了
        /// </summary>
        /// <param name="plugin"></param>
        public void Add(CityPlugin plugin)
        {
            plugins.Add(plugin);
            plugin.NotifyAdded(this);
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
            //plugins = new FastList<CityPlugin>();
            switch (Size)
            {
                case UrbanSize.Large:
                    localLp.MaxLimit = LargeMaxLPStorage;
                    localHp.MaxLimit = LargeMaxHPStorage;
                    localFood.MaxLimit = LargeMaxFoodStorage;
                    break;
                case UrbanSize.Medium:

                    localLp.MaxLimit = MediumMaxLPStorage;
                    localHp.MaxLimit = MediumMaxHPStorage;
                    localFood.MaxLimit = MediumMaxFoodStorage;
                    break;
                case UrbanSize.Small:
                    localLp.MaxLimit = SmallMaxLPStorage;
                    localHp.MaxLimit = SmallMaxHPStorage;
                    localFood.MaxLimit = SmallMaxFoodStorage;
                    break;
            }

            //PluginFoodCostSpeed = 0;
            //PluginHPProductionSpeed = 0;
            //PluginLPProductionSpeed = 0;
            //PluginCarbonProduceSpeed = 0;
            //for (int i = 0; i < plugins.Count; i++)
            //{
            //    CarbonProduceSpeed += plugins[i].CarbonProduceSpeed;
            //    PluginFoodCostSpeed += plugins[i].FoodCostSpeed;
            //    PluginHPProductionSpeed += plugins[i].HPProductionSpeed;
            //    PluginLPProductionSpeed += plugins[i].LPProductionSpeed;
            //    PluginCarbonProduceSpeed += plugins[i].CarbonProduceSpeed;
            //}
        }

        #region unk

        #endregion

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
            SelfFoodCostSpeed = Population * 0.05f;

            //CarbonProduceSpeed = Population * 0.02f + 



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


            for (int i = 0; i < plugins.Count; i++)
            {
                plugins[i].Update(time);
            }


            //CarbonProduceSpeed = SelfFoodCostSpeed * 0.1f;


            float hours = (float)time.ElapsedGameTime.TotalHours;
            //this.CarbonChange += PluginCarbonProduceSpeed * hours;

            {
                // 严禁使用旧的模式，属性泛滥

                #region 资源消耗计算
                // 计算自身

                // 高能资源消耗量
                float hrChange = SelfHPProductionSpeed * hours;

                // 低能资源消耗量
                float lrChange = SelfLPProductionSpeed * hours;





                // 计算插件
                for (int i = 0; i < plugins.Count; i++)
                {

                }

                #endregion
            }

            {
                float hpChange = ProduceHPSpeed * hours;
                float lpChange = ProduceLPSpeed * hours;

                float foodChange = -FoodCostSpeed * hours;

#warning TODO: 分开计算
#warning ISSUE: 发展倒退过快
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

                #region 消耗资源计算发展度
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
                    float act = localLp.Commit(lpChange);

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


                float foodLack = 0;


                //if (foodChange > 0)
                //{
                //    //this.CarbonChange += CarbonMult * foodLack;


                //    // 如果有疾病，那么先将食物用于控制疾病
                //    if (Disease > 0)
                //    {
                //        Disease -= foodChange;
                //    }
                //    else
                //    {
                //        float actFood = localFood.Commit(foodChange);
                //        if (actFood < foodChange)
                //        {
                //            energyStat.CommitHPEnergy(Math.Min(foodChange - actFood, FoodTransportSpeed * hours));
                //        }
                //    }
                //}
                //else
                {
                    float actFood = localFood.Apply(-foodChange);
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

                    //this.CarbonChange += CarbonMult * foodLack;
                }

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
                float devIncr = popDevAdj * (lpnewDev * 0.5f + hpnewDev);
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
