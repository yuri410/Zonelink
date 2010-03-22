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
        const float SafeLimitRate = 0.5f;
        [SLGValue()]
        const float StandardStorageBallanceRate = 1;

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

    /// <summary>
    ///  表示城市占领的状态
    /// </summary>
    public class CaptureState
    {
        public bool IsCapturing
        {
            get { return NewOwner1 != null || NewOwner2 != null || NewOwner3 != null || NewOwner4 != null; }
        }

        public Player CheckCapture()
        {
            if (CaputreProgress1 >= 1)
            {
                return NewOwner1;
            }
            if (CaputreProgress2 >= 1)
            {
                return NewOwner2;
            }
            if (CaputreProgress3 >= 1)
            {
                return NewOwner3;
            }
            if (CaputreProgress4 >= 1)
            {
                return NewOwner4;
            }
            return null;
        }


        public City NearbyCity1
        {
            get;
            private set;
        }

        public City NearbyCity2
        {
            get;
            private set;
        }
        public City NearbyCity3
        {
            get;
            private set;
        }
        public City NearbyCity4
        {
            get;
            private set;

        }



        /// <summary>
        ///  即将占领的玩家
        /// </summary>
        public Player NewOwner1
        {
            get;
            private set;
        }
        public Player NewOwner2
        {
            get;
            private set;
        }
        public Player NewOwner3
        {
            get;
            private set;
        }
        public Player NewOwner4
        {
            get;
            private set;
        }
        /// <summary>
        ///  占领进度
        /// </summary>
        public float CaputreProgress1
        {
            get;
            private set;
        }
        public float CaputreProgress2
        {
            get;
            private set;
        }
        public float CaputreProgress3
        {
            get;
            private set;
        }
        public float CaputreProgress4
        {
            get;
            private set;
        }

        public void CancelCapture(Player player)
        {
            if (player == NewOwner1)
            {
                NewOwner1 = null;
                CaputreProgress1 = 0;
            }
            if (player == NewOwner2)
            {
                NewOwner2 = null;
                CaputreProgress2 = 0;
            }
            if (player == NewOwner3)
            {
                NewOwner3 = null;
                CaputreProgress3 = 0;
            }
            if (player == NewOwner4)
            {
                NewOwner4 = null;
                CaputreProgress4 = 0;
            }
        }

        public bool IsPlayerCapturing(Player player)
        {
            if (player == NewOwner1)
            {
                return true;
            }
            if (player == NewOwner2)
            {
                return true;
            }
            if (player == NewOwner3)
            {
                return true;
            }
            if (player == NewOwner4)
            {
                return true;
            }
            return false;
        }
        public bool CanCapture(Player player)
        {
            if (!IsCapturing)
                return true;
            if (player == NewOwner1)
            {
                return false;
            }
            if (player == NewOwner2)
            {
                return false;
            }
            if (player == NewOwner3)
            {
                return false;
            }
            if (player == NewOwner4)
            {
                return false;
            }
            return true;
        }
        public void SetCapture(Player player, City nearby)
        {
            if (object.ReferenceEquals(NewOwner1, null))
            {
                NewOwner1 = player;
                NearbyCity1 = nearby;
            }
            else if (object.ReferenceEquals(NewOwner2, null))
            {
                NewOwner2 = player;
                NearbyCity2 = nearby;
            }
            else if (object.ReferenceEquals(NewOwner3, null))
            {
                NewOwner3 = player;
                NearbyCity3 = nearby;
            }
            else if (object.ReferenceEquals(NewOwner4, null))
            {
                NewOwner4 = player;
                NearbyCity4 = nearby;
            }
        }

        public void ReceiveGood(Player player, float hrAmount, float lrAmount)
        {
            if (player == NewOwner1)
            {
                CaputreProgress1 += hrAmount + lrAmount * 0.5f;
            }
            else if (player == NewOwner2)
            {
                CaputreProgress2 += hrAmount + lrAmount * 0.5f;
            }
            else if (player == NewOwner3)
            {
                CaputreProgress3 += hrAmount + lrAmount * 0.5f;
            }
            else if (player == NewOwner4)
            {
                CaputreProgress4 += hrAmount + lrAmount * 0.5f;
            }
        }
    }

    public delegate void CitypluginEventHandle(City city, CityPlugin plugin);
    public delegate void NearbyCityAddedHandler(City city, City srcCity);
    public delegate void CityOwnerChanged(Player newOwner);

    public class City : SimulateObject, IConfigurable, IUpdatable
    {
        [SLGValue()]
        const float SatThreshold = 0.1f;

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
        const float DevBias = -0.01f;

        float recoverCooldown;
        /// <summary>
        ///  表示城市的附加设施
        /// </summary>
        FastList<CityPlugin> plugins = new FastList<CityPlugin>();

        ResourceStorage localLr;
        ResourceStorage localHr;
        ResourceStorage localFood;
        UrbanSize size;
        Player owner;
        CultureId culture;

        FastList<City> nearbyCity = new FastList<City>();
        CityObject parent;


        [SLGValue()]
        const int RecentCarbonLength = 100;

        FastQueue<float> recentCarbon = new FastQueue<float>(RecentCarbonLength);
        int carbonAddCounter;

        public float RecentCarbonAmount
        {
            get
            {
                float r = 0;
                for (int i = 0; i < recentCarbon.Count; i++)
                {
                    r += recentCarbon.GetElement(i);
                }
                return r;
            }
        }

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

            Capture = new CaptureState();
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
        /// <summary>
        ///  获取该城市市民满意度
        /// </summary>
        public float Satisfaction
        {
            get
            {
                switch (Size)
                {
                    case UrbanSize.Small:
                        float v = Population < CityGrade.SmallRefPop ?
                            Development * Population : Development * (2 * CityGrade.SmallRefPop - Population);
                        return MathEx.Saturate(v / CityGrade.SmallRefSat);
                    case UrbanSize.Medium:
                        v = Population < CityGrade.MediumRefPop ?
                           Development * Population : Development * (2 * CityGrade.MediumRefPop - Population);
                        return MathEx.Saturate(v / CityGrade.MediumRefSat);
                    case UrbanSize.Large:
                        v = Population < CityGrade.LargeRefPop ?
                           Development * Population : Development * (2 * CityGrade.LargeRefPop - Population);
                        return MathEx.Saturate(v / CityGrade.LargeCityRefSat);
                }
                return 0;
            }
        }
        public bool IsSatisfactionLow
        {
            get { return Satisfaction < SatThreshold; }
        }

        public Player CoolDownPlayer
        {
            get;
            private set;
        }
        public float RecoverCoolDown
        {
            get { return recoverCooldown; }
        }

        public bool IsRecovering
        {
            get;
            private set;
        }

        public CaptureState Capture
        {
            get;
            private set;
        }

        public bool IsCaptured
        {
            get { return !object.ReferenceEquals(Owner, null); }
        }

        public Player Owner
        {
            get { return owner; }
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
            float res = Population * CityGrade.FoodCostPerPeople;
            if (res < 0)
                res = 0;
            return res;
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

        public void Damage(float pop, float dev)
        {
            Population -= pop;
            Development -= dev;
        }

        /// <summary>
        ///  给无家可归的城市一个主
        /// </summary>
        /// <param name="player"></param>
        public void ChangeOwner(Player player)
        {
            if (Owner != null)
            {
                Owner.Area.NotifyLostCity(this);
            }
            owner = player;

            if (player != null)
            {
                Owner.Area.NotifyNewCity(this);
            }

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


        public void AddNearbyCity(City city)
        {
            nearbyCity.Add(city);
            if (NearbyCityAdded != null)
                NearbyCityAdded(this, city);

        }

        public override void Update(GameTime time)
        {
            float hours = (float)time.ElapsedGameTime.TotalHours;

            CarbonProduceSpeed = 0;


            if (Capture.IsCapturing && !IsCaptured)
            {
                if (!object.ReferenceEquals(Capture.NearbyCity1, null))
                {
                    if (object.ReferenceEquals(Capture.NewOwner1, Capture.NearbyCity1.owner))
                    {
                        float capreq = Capture.NearbyCity1.LocalHR.Apply(100 * hours);
                        float capreq2 = Capture.NearbyCity1.LocalLR.Apply(100 * hours);
                        Capture.ReceiveGood(Capture.NewOwner1, capreq / CityGrade.GetCapturePoint(size), capreq2 / CityGrade.GetCapturePoint(size));
                    }
                    else
                    {
                        Capture.CancelCapture(Capture.NewOwner1);
                    }
                }



                Player player = Capture.CheckCapture();
                if (!object.ReferenceEquals(player, null))
                {
                    ChangeOwner(player);
                }
            }

            if (IsRecovering)
            {
                recoverCooldown -= hours;
                if (recoverCooldown < 0)
                {
                    IsRecovering = false;
                }
            }
            else if (Satisfaction < float.Epsilon && !object.ReferenceEquals(Owner, null))
            {
                CoolDownPlayer = Owner;
                ChangeOwner(null);
                IsRecovering = true;
                recoverCooldown = CityGrade.GetRecoverCoolDown(Size);
            }

            if (object.ReferenceEquals(Owner, null))
            {
                return;
            }



            #region 城市自动级别调整

            if (Size != UrbanSize.Large)
            {
                float points = Development;

                UrbanSize newSize;
                if (points < CityGrade.MediumCityPointThreshold)
                {
                    if (points < CityGrade.SmallCityPointThreshold)
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


            #region 补缺储备，物流

            for (int i = 0; i < nearbyCity.Count; i++)
            {
                float transSpeed;
                City sourceCity = nearbyCity[i];
                {
                    float requirement = localLr.StandardStorageBalance - localLr.Current;
                    transSpeed = Math.Min(CityGrade.GetLPTransportSpeed(Size), CityGrade.GetLPTransportSpeed(sourceCity.Size));

                    if (requirement > 0)
                    {
                        bool passed = true;
                        if (sourceCity.IsLRLow)
                            passed ^= Randomizer.GetRandomBool();
                        if (passed)
                        {
                            float applyAmount = Math.Min(requirement * hours, transSpeed * hours);
                            applyAmount = sourceCity.LocalLR.ApplyFar(applyAmount);
                            localLr.Commit(applyAmount);
                        }
                    }
                    //else
                    //{
                    //    float commitAmount = Math.Min(-requirement * hours, CityGrade.GetLPTransportSpeed(Size) * hours);
                    //    commitAmount = sourceCity.LocalLR.Commit(commitAmount);
                    //    localLr.Apply(commitAmount);
                    //}
                }
                {
                    float requirement = localHr.StandardStorageBalance - localHr.Current;
                    transSpeed = Math.Min(CityGrade.GetHPTransportSpeed(Size), CityGrade.GetHPTransportSpeed(sourceCity.Size));

                    if (requirement > 0)
                    {
                        bool passed = true;
                        if (sourceCity.IsHRLow)
                            passed ^= Randomizer.GetRandomBool();
                        if (passed)
                        {

                            float applyAmount = Math.Min(requirement * hours, transSpeed * hours);
                            applyAmount = sourceCity.LocalHR.ApplyFar(applyAmount);
                            localHr.Commit(applyAmount);
                        }
                    }
                    //else
                    //{
                    //    float commitAmount = Math.Min(-requirement * hours, CityGrade.GetHPTransportSpeed(Size) * hours);
                    //    commitAmount = sourceCity.LocalHR.Commit(commitAmount);
                    //    localHr.Apply(commitAmount);
                    //}
                }
                {
                    float requirement = localFood.StandardStorageBalance - localFood.Current;
                    transSpeed = Math.Min(CityGrade.GetFoodTransportSpeed(Size), CityGrade.GetFoodTransportSpeed(sourceCity.Size));

                    if (requirement > 0)
                    {
                        bool passed = true;
                        if (sourceCity.IsFoodLow)
                            passed ^= Randomizer.GetRandomBool();
                        if (passed)
                        {

                            float applyAmount = Math.Min(requirement * hours, transSpeed * hours);
                            applyAmount = sourceCity.LocalFood.ApplyFar(applyAmount);
                            localFood.Commit(applyAmount);
                        }
                    }
                    //else
                    //{
                    //    float commitAmount = Math.Min(-requirement * hours, CityGrade.GetFoodTransportSpeed(Size) * hours);
                    //    commitAmount = sourceCity.LocalFood.Commit(commitAmount);
                    //    localFood.Apply(commitAmount);
                    //}
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

                if (actHrChange < hrChange) // 资源过剩，转为碳
                {
                    CarbonProduceSpeed += (hrChange - actHrChange) / hours;
                    //float actCmt = localHr.Commit(Math.Min(hrChange - actHrChange, CityGrade.GetHPTransportSpeed(Size) * hours));
                }



                lrChange = plugins[i].LRPSpeed * hours;
                float actLrChange = localLr.Commit(lrChange);

                if (actLrChange < lrChange)// 资源过剩，转为碳
                {
                    CarbonProduceSpeed += (hrChange - actHrChange) / hours;

                    //localLr.Commit(Math.Min(lrChange - actLrChange, CityGrade.GetLPTransportSpeed(Size) * hours));
                }


                CarbonProduceSpeed += plugins[i].CarbonProduceSpeed;
            }


            float foodLack = 0;
            // 计算自身
            {
                // 高能资源消耗量
                float hrChange = CityGrade.GetSelfHRCSpeed(Size) * hours;
                if (hrChange < -float.Epsilon)
                {
                    float actHrChange = localHr.Apply(-hrChange);
                    SelfHRCRatio = -actHrChange / hrChange;
                    hrDev += actHrChange * CityGrade.GetDevelopmentMult(Size);
                }
                else
                {
                    SelfHRCRatio = 0;
                }

                // 低能资源消耗量
                float lrChange = CityGrade.GetSelfLRCSpeed(Size) * hours;
                if (lrChange < -float.Epsilon)
                {
                    float actLrChange = localLr.Apply(-lrChange);
                    SelfLRCRatio = -actLrChange / lrChange;
                    lrDev += actLrChange * CityGrade.GetDevelopmentMult(Size);
                }
                else
                {
                    SelfLRCRatio = 0;
                }

                float foodSpeedFull = GetSelfFoodCostSpeedFull();

#warning 实现采集食物
                // 仅仅测试
                localFood.Commit(CityGrade.GetSelfFoodGatheringSpeed(Size) * hours);


                float foodChange = (-foodSpeedFull) * hours;

                float actFood = localFood.Apply(-foodChange);

                if (foodChange < -float.Epsilon)
                {
                    SelfFoodCostRatio = actFood / -foodChange;
                }
                else
                {
                    SelfFoodCostRatio = 0;
                }

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
                    else
                    {
                        Disease -= foodLack * 0.005f;
                    }
                }
                else
                {
                    Disease -= actFood * 0.033f;
                }


            }
            #endregion


            // 疾病发展传播计算
            if (Disease > 0)
            {
                Disease += Disease * (float)Math.Log(Population, 1000) * 0.001f;
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

            CarbonProduceSpeed += lrDev / hours + hrDev / hours;

            float devIncr = popDevAdj * (lrDev * 0.5f + hrDev + DevBias / CityGrade.GetDevelopmentMult(Size));
            Development += devIncr + foodLack;
            if (Development < 0)
            {
                Development = 0;
            }
            if (devIncr > 0)
            {
                Population += (devIncr + foodLack) * 0.01f;
            }

            if (carbonAddCounter++ == 8)
            {
                recentCarbon.Enqueue(CarbonProduceSpeed * hours);
                while (recentCarbon.Count > RecentCarbonLength)
                    recentCarbon.Dequeue();
                carbonAddCounter = 0;
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

            switch (Size)
            {
                case UrbanSize.Small:
                    Development = 200;
                    break;
                case UrbanSize.Medium:
                    Development = 2000;
                    break;
                case UrbanSize.Large:
                    Development = 20000;
                    break;
            }
            UpgradeUpdate();
        }

        #endregion
    }
}
