using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Config;
using Apoc3D.MathLib;
using Code2015.Logic;
using Code2015.World;

namespace Code2015.BalanceSystem
{
    /// <summary>
    ///  表示一种资源的存储器。
    ///  可以储存一定量的资源。并且从这批资源中申请获得一定数量，以及将一定数量的资源存储进来
    /// </summary>
    /// <remarks>
    ///  生产有限，使用无限
    ///  
    ///  生产限制为能源转化率
    ///  使用限制使用“上限”实现
    /// </remarks>
    public class ResourceStorage
    {
        [SLGValue()]
        const float SafeLimitRate = 0.33f;
        [SLGValue()]
        const float StandardStorageBallanceRate = 0.67f;

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

        public float StandardStorageBalance 
        {
            get { return limit * StandardStorageBallanceRate; }
        }
        public float SafeLimit
        {
            get { return limit * SafeLimitRate; }
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

        public float ApplyFar(float amount)
        {
            if (Current > SafeLimit)
            {
                return Apply(amount);
            }
            return 0;
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

    public enum CultureId
    {
        Asia = 0,
        Europe = 1,
        Africa = 2,
        Russia = 3,
        American = 4,
        Invariant = 5,
        Count = 6
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


    public delegate void CitypluginEventHandle(City city, CityPlugin plugin);
    public delegate void NearbyCityAddedHandler(City city,City srcCity);
    public delegate void CityOwnerChanged(Player newOwner);

    public class City : SimulateObject, IConfigurable, IUpdatable
    {

        [SLGValue()]
        const float HPLowThreshold = 0.15f;
        [SLGValue()]
        const float LPLowThreshold = 0.15f;
        [SLGValue()]
        const float FoodLowThreshold = 0.15f;


        /// <summary>
        ///  发展增量的偏移值。无任何附加条件下的发展量。
        /// </summary>
        [SLGValue]
        const float DevBias = -3;

        /// <summary>
        ///  表示城市的附加设施
        /// </summary>
        FastList<CityPlugin> plugins = new FastList<CityPlugin>();

        ResourceStorage localLr;
        ResourceStorage localHr;
        ResourceStorage localFood;
        UrbanSize size;

        CultureId culture;

        FastList<City> nearbyCity = new FastList<City>();
        CityObject parent;

        public CityObject Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public City(SimulationRegion sim)
            : base(sim)
        {
            localLr = new ResourceStorage(CityGrade.SmallMaxLPStorage, float.MaxValue);
            localHr = new ResourceStorage(CityGrade.SmallMaxHPStorage, float.MaxValue);
            localFood = new ResourceStorage(CityGrade.SmallMaxFoodStorage, float.MaxValue);
            culture = CultureId.Asia;
            UpgradeUpdate();
        }
        public City(SimulationRegion sim, UrbanSize size)
            : this(sim)
        {
            this.Size = size;
        }
        public City(SimulationRegion sim, string name)
            : this(sim)
        {
            this.Name = name;
        }


        #region  属性
        public Player Owner
        {
            get;
            private set;
        }
        public bool IsDead
        {
            get { return Population < 0.1f; }
        }

        public CultureId Culture 
        {
            get { return culture; }
        }

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

        /// <summary>
        ///  获取一个浮点数，反应使用HR时，相应HR的供应率
        /// </summary>
        public float SelfHRCRatio
        {
            get;
            private set;
        }
        /// <summary>
        ///  获取一个浮点数，反应使用LR时，相应LR的供应率
        /// </summary>
        public float SelfLRCRatio
        {
            get;
            private set;
        }
        /// <summary>
        ///  获取一个浮点数，反应使用食物时，相应食物的供应率
        /// </summary>
        public float SelfFoodCostRatio
        {
            get;
            private set;
        }

        /// <summary>
        ///  计算在当前供应率的情况下，使用食物的速度
        /// </summary>
        /// <returns></returns>
        public float GetSelfFoodCostSpeed()
        {
            return GetSelfFoodCostSpeedFull() * SelfFoodCostRatio;
        }

        /// <summary>
        ///  计算在当前供应率的情况下，使用HR的速度
        /// </summary>
        /// <returns></returns>
        public float GetSelfHRCSpeed()
        {
            return CityGrade.GetSelfHRCSpeed(Size) * SelfHRCRatio;
        }
        /// <summary>
        ///  计算在当前供应率的情况下，使用LR的速度
        /// </summary>
        /// <returns></returns>
        public float GetSelfLRCSpeed()
        {
            return CityGrade.GetSelfLRCSpeed(Size) * SelfLRCRatio;
        }

        /// <summary>
        ///  获取是否可以添加Plugin
        /// </summary>
        public bool CanAddPlugins
        {
            get { return plugins.Count < CityGrade.GetMaxPlugins(Size); }
        }

        #region 产出/投入


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
        ///  获取一个布尔值，表示当前已储备的高能资源量是否过低
        /// </summary>
        public bool IsHRLow
        {
            get { return localHr.Current < HPLowThreshold; }
        }
        public bool IsLRLow
        {
            get { return localLr.Current < LPLowThreshold; }
        }
        public bool IsFoodLow
        {
            get { return localFood.Current < FoodLowThreshold; }
        }
        #endregion

        public event CitypluginEventHandle PluginAdded;
        public event CitypluginEventHandle PluginRemoved;
        public event NearbyCityAddedHandler NearbyCityAdded;
        public event CityOwnerChanged CityOwnerChanged;

        /// <summary>
        ///  给无家可归的城市一个主
        /// </summary>
        /// <param name="player"></param>
        public void ChangeOwner(Player player)
        {
            if (Owner != null) 
            {
                throw new InvalidOperationException("目前不能抢夺别人的城市");
            }
            Owner = player;
            Owner.Area.NotifyNewCity(this);

            if (CityOwnerChanged != null)
                CityOwnerChanged(player);
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

                if (PluginAdded != null)
                {
                    PluginAdded(this, plugin);
                }
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

            if (PluginRemoved != null)
            {
                PluginRemoved(this, plugin);
            }
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
            return dev;
        }

        public void AddNearbyCity(City city)
        {
            nearbyCity.Add(city);
            if (NearbyCityAdded != null)
                NearbyCityAdded(this, city);

        }


        public override void Update(GameTime time)
        {
            CarbonProduceSpeed = 0;

            if (IsDead) 
            {
                return;
            }

            #region 城市自动级别调整

            if (Size != UrbanSize.Large)
            {
                float points = GetCityPoints(Development, Population);

                UrbanSize newSize;
                if (points < MediumCityPointThreshold)
                {
                    if (points < SmallCityPointThreshold)
                    {
                        newSize = UrbanSize.Small;
                    }
                    else
                    {
                        newSize = UrbanSize.Medium;
                    }
                }
                else
                {
                    newSize = UrbanSize.Large;
                }
                if (newSize > Size)
                    Size = newSize;
            }

            #endregion

            for (int i = 0; i < plugins.Count; i++)
            {
                plugins[i].Update(time);
            }

            float hours = (float)time.ElapsedGameTime.TotalHours;

            #region 补缺储备，物流
            
            for (int i = 0; i < nearbyCity.Count; i++)
            {
                City sourceCity = nearbyCity[i];
                {
                    float requirement = localLr.StandardStorageBalance - localLr.Current;

                    if (requirement > 0)
                    {
                        bool passed = true;
                        if (sourceCity.IsLRLow)
                            passed ^= Randomizer.GetRandomBool();
                        if (passed)
                        {
                            float applyAmount = Math.Min(requirement * hours, CityGrade.GetLPTransportSpeed(Size) * hours);
                            applyAmount = sourceCity.LocalLR.ApplyFar(applyAmount);// energyStat.ApplyLPEnergy(applyAmount);
                            localLr.Commit(applyAmount);
                        }
                    }
                    else
                    {
                        float commitAmount = Math.Min(-requirement * hours, CityGrade.GetLPTransportSpeed(Size) * hours);
                        commitAmount = sourceCity.LocalLR.Commit(commitAmount);
                        localLr.Apply(commitAmount);
                    }
                }
                {
                    float requirement = localHr.StandardStorageBalance - localHr.Current;

                    if (requirement > 0)
                    {
                        bool passed = true;
                        if (sourceCity.IsHRLow)
                            passed ^= Randomizer.GetRandomBool();
                        if (passed)
                        {
                            float applyAmount = Math.Min(requirement * hours, CityGrade.GetHPTransportSpeed(Size) * hours);
                            applyAmount = sourceCity.LocalHR.ApplyFar(applyAmount);// energyStat.ApplyHPEnergy(applyAmount);
                            localHr.Commit(applyAmount);
                        }
                    }
                    else
                    {
                        float commitAmount = Math.Min(-requirement * hours, CityGrade.GetHPTransportSpeed(Size) * hours);
                        commitAmount = sourceCity.LocalHR.Commit(commitAmount);
                        localHr.Apply(commitAmount);
                    }
                }
                {
                    float requirement = localFood.StandardStorageBalance - localFood.Current;

                    if (requirement > 0)
                    {
                        bool passed = true;
                        if (sourceCity.IsFoodLow)
                            passed ^= Randomizer.GetRandomBool();
                        if (passed)
                        {
                            float applyAmount = Math.Min(requirement * hours, CityGrade.GetFoodTransportSpeed(Size) * hours);
                            applyAmount = sourceCity.LocalFood.ApplyFar(applyAmount);// energyStat.ApplyFood(applyAmount);
                            localFood.Commit(applyAmount);
                        }
                    }
                    else
                    {
                        float commitAmount = Math.Min(-requirement * hours, CityGrade.GetFoodTransportSpeed(Size) * hours);
                        commitAmount = sourceCity.LocalFood.Commit(commitAmount);
                        localFood.Apply(commitAmount);
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
                // 高能资源消耗量，消耗的其他计算在Plugin中
                float hrChange = plugins[i].HRCSpeed * hours;
                hrDev += hrChange * CityGrade.GetDevelopmentMult(Size);

                // 低能资源消耗量
                float lrChange = plugins[i].LRCSpeed * hours;
                lrDev += lrDev * CityGrade.GetDevelopmentMult(Size);


                hrChange = plugins[i].HRPSpeed * hours;
                float actHrChange = localHr.Commit(hrChange);

                if (actHrChange < hrChange)
                {
                    localHr.Commit(Math.Min(hrChange - actHrChange, CityGrade.GetHPTransportSpeed(Size) * hours));
                }

                lrChange = plugins[i].LRPSpeed * hours;
                float actLrChange = localLr.Commit(lrChange);

                if (actLrChange < lrChange)
                {
                    localLr.Commit(Math.Min(lrChange - actLrChange, CityGrade.GetLPTransportSpeed(Size) * hours));
                }


                CarbonProduceSpeed += plugins[i].CarbonProduceSpeed;
            }


            float foodLack = 0;
            // 计算自身
            {
                // 高能资源消耗量
                float hrChange = CityGrade.GetSelfHRCSpeed(Size) * hours;
                if (hrChange < float.Epsilon)
                {
                    float actHrChange = localHr.Apply(-hrChange);
                    SelfHRCRatio = -actHrChange / hrChange;
                    hrDev += actHrChange * CityGrade.GetDevelopmentMult(Size);
                }

                // 低能资源消耗量
                float lrChange = CityGrade.GetSelfLRCSpeed(Size) * hours;
                if (lrChange < float.Epsilon)
                {
                    float actLrChange = localLr.Apply(-lrChange);
                    SelfLRCRatio = -actLrChange / lrChange;
                    lrDev += actLrChange * CityGrade.GetDevelopmentMult(Size);
                }

                float foodSpeedFull = GetSelfFoodCostSpeedFull();

#warning 实现采集食物
                float foodChange = (-foodSpeedFull + CityGrade.GetSelfFoodGatheringSpeed(Size)) * hours;

                float actFood = localFood.Apply(-foodChange);

                SelfFoodCostRatio = actFood / -foodChange;

                // 食物 碳排量计算
                CarbonProduceSpeed += foodSpeedFull * SelfFoodCostRatio;

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
