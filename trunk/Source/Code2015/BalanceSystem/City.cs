using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Config;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.Logic;
using Code2015.Network;
using Code2015.World;

namespace Code2015.BalanceSystem
{
    public enum CultureId
    {
        Asia = 0,
        Europe = 1,
        Africa = 2,
        //Russia = 3,
        American = 3,
        //Invariant = 5,
        Count = 4
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
    public delegate void NearbyCityAddedHandler(City city, City srcCity);
    public delegate void NearbyCityRemovedHandler(City city, City srcCity);
    public delegate void CityOwnerChanged(Player newOwner);
    public delegate void CityGrowHandler();

    public struct PieceCategoryProbability
    {
        public float Health;
        public float Environment;
        public float Education;
        public float Hunger;
    }

    public class City : SimulationObject, IConfigurable, IUpdatable
    {
        [SLGValue()]
        const float SatThreshold = 0.1f;

        [SLGValue()]
        const float HPLowThreshold = 0.15f;
        [SLGValue()]
        const float LPLowThreshold = 0.15f;
        [SLGValue()]
        const float FoodLowThreshold = 0.15f;

        [SLGValue()]
        const float ProbabilityDecr = 0.75f;
        [SLGValue]
        const float MinProbability = 0.1f;

        [SLGValue]
        public const float DefaultProblemWeight = 0.5f;

        /// <summary>
        ///  发展增量的偏移值。无任何附加条件下的发展量。
        /// </summary>
        [SLGValue]
        const float DevBias = -0.001f;

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

        string[] linkableCityName;
        FastList<City> linkableCity = new FastList<City>();
        FastList<CityLink> nearbyCity = new FastList<CityLink>();
        CityObject parent;
        FastList<FarmLand> farms = new FastList<FarmLand>();

        [SLGValue()]
        const int RecentCarbonLength = 5;

        [SLGValue]
        public const int MaxFarmLand = 4;

        FastQueue<float> recentCarbon = new FastQueue<float>(RecentCarbonLength);

        int recentCounter = 0;
        ValueSmoother recentOil = new ValueSmoother(5);
        ValueSmoother recentFood = new ValueSmoother(5);
        ValueSmoother recentWood = new ValueSmoother(5);

        int carbonAddCounter;


        bool stateChanged;

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

        public City(SimulationWorld sim)
            : base(sim)
        {
            localLr = new ResourceStorage(CityGrade.SmallMaxLPStorage, float.MaxValue);
            localHr = new ResourceStorage(CityGrade.SmallMaxHPStorage, float.MaxValue);
            localFood = new ResourceStorage(CityGrade.SmallMaxFoodStorage, float.MaxValue);
            culture = CultureId.Asia;

            Capture = new CaptureState();
            UpgradeUpdate();
        }
        public City(SimulationWorld sim, UrbanSize size)
            : this(sim)
        {
            this.Size = size;
        }
        public City(SimulationWorld sim, string name)
            : this(sim)
        {
            this.Name = name;
        }


        #region  属性

        public string[] LinkableCityName
        {
            get
            {
                string[] result = new string[linkableCityName.Length];
                Array.Copy(linkableCityName, result, linkableCityName.Length);
                return result;
            }
        }
        public int LinkableCityCount
        {
            get { return linkableCity.Count; }
        }

        public City GetLinkableCity(int i)
        {
            return linkableCity[i];
        }

        public MdgType MajorProblem
        {
            get;
            private set;
        }

        public bool IsFarmFull
        {
            get { return farms.Count < MaxFarmLand; }
        }

        public int FarmLandCount
        {
            get { return farms.Count; }
        }
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
                        //float v = Population < CityGrade.SmallRefPop ?
                        //    Development * MathEx.Saturate(Population / CityGrade.SmallRefPop) : 
                        //    Development * MathEx.Saturate((2 * CityGrade.SmallRefPop - Population) / CityGrade.SmallRefPop);

                        return MathEx.Saturate(Development / CityGrade.SmallCityPointThreshold);
                    case UrbanSize.Medium:
                        //v = Population < CityGrade.MediumRefPop ?
                        //   Development * MathEx.Saturate(Population / CityGrade.MediumRefPop) :
                        //   Development * MathEx.Saturate((2 * CityGrade.MediumRefPop - Population) / CityGrade.MediumRefPop);

                        return MathEx.Saturate(Development / CityGrade.MediumCityPointThreshold);
                    case UrbanSize.Large:
                        //v = Population < CityGrade.LargeRefPop ?
                        //   Development * MathEx.Saturate(Population / CityGrade.LargeRefPop) :
                        //   Development * MathEx.Saturate((2 * CityGrade.LargeRefPop - Population) / CityGrade.LargeRefPop);
                        return MathEx.Saturate(Development / CityGrade.LargeCityPointThreshold);
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
            get { return Owner != null; }
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

        public bool IsHomeCity
        {
            get;
            set;
        }

        public int StartUp
        {
            get;
            private set;
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
        public float InitialDeveleopment
        {
            get;
            private set;
        }
        public float InitialPopulation
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


        public float AdditionalDevMult
        {
            get;
            private set;
        }

        public float HealthCare
        {
            get;
            private set;
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
        public event NearbyCityAddedHandler CaptureSet;
        public event NearbyCityRemovedHandler CaptureCancel;
        public event NearbyCityAddedHandler NearbyCityAdded;
        public event NearbyCityRemovedHandler NearbyCityRemoved;
        public event CityOwnerChanged CityOwnerChanged;

        public event CityGrowHandler CityGrow;

        public PieceCategoryProbability GetProbability()
        {
            PieceCategoryProbability r;
            r.Education = 1;
            r.Environment = 1;
            r.Health = 1;
            r.Hunger = 1;

            for (int i = 0; i < plugins.Count; i++)
            {
                switch (plugins[i].TypeId)
                {
                    case CityPluginTypeId.BiofuelFactory:
                        r.Hunger -= ProbabilityDecr;
                        break;
                    case CityPluginTypeId.OilRefinary:
                        r.Environment -= ProbabilityDecr * 0.3f;
                        break;
                    case CityPluginTypeId.WoodFactory:

                        break;
                    case CityPluginTypeId.Hospital:
                        r.Health -= ProbabilityDecr;
                        break;
                    case CityPluginTypeId.EducationOrg:
                        r.Education -= ProbabilityDecr;
                        break;
                }
            }

            {
                if (r.Education < MinProbability)
                    r.Education = MinProbability;
                if (r.Hunger < MinProbability)
                    r.Hunger = MinProbability;
                if (r.Environment < MinProbability)
                    r.Environment = MinProbability;
                if (r.Health < MinProbability)
                    r.Health = MinProbability;
                float len = r.Environment + r.Education + r.Hunger + r.Health;
                if (len > float.Epsilon)
                {
                    len = 1.0f / len;
                    r.Education *= len;
                    r.Hunger *= len;
                    r.Environment *= len;
                    r.Health *= len;
                }

            }

            return r;
        }

        public bool IsInCaptureRange(City city)
        {
            return linkableCity.IndexOf(city) != -1;
        }
        public bool CanCapture(Player pl)
        {
            if (IsCaptured)
            {
                return false;
            }
            if (IsRecovering && CoolDownPlayer == pl)
            {
                return false;
            }
            return Capture.CanCapture(pl);
        }
        public void Damage(float pop, float dev)
        {
            Disease += pop;
            //Population -= pop;
            Development -= dev / HealthCare;
        }

        /// <summary>
        ///  给无家可归的城市一个主
        /// </summary>
        /// <param name="player"></param>
        public void ChangeOwner(Player player)
        {
            if (IsCaptured && player == null)
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

            if (player == null)
            {
                for (int i = 0; i < nearbyCity.Count; i++)
                {
                    nearbyCity[i].Target.RemoveNearbyCity(this);
                    //nearbyCity[i].Disable();
                }
                nearbyCity.Clear();
            }
            stateChanged = true;
        }

        public void AddFarm()
        {
            farms.Add(new FarmLand(Region, this));
            stateChanged = true;
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
            stateChanged = true;
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
            stateChanged = true;
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
            stateChanged = true;
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

        public CityLink GetLink(City city)
        {
            for (int i = 0; i < nearbyCity.Count; i++)
            {
                if (nearbyCity[i].Target == city)
                    return nearbyCity[i];
            }
            return null;
        }

        public void RemoveNearbyCity(City city)
        {
            for (int i = 0; i < nearbyCity.Count; i++)
            {
                if (nearbyCity[i].Target == city)
                {
                    //nearbyCity[i].Disable();
                    nearbyCity.RemoveAt(i);

                    if (NearbyCityRemoved != null)
                        NearbyCityRemoved(this, city);
                    return;
                }
            }
            stateChanged = true;
        }
        public void AddNearbyCity(City city)
        {
            CityLink link = new CityLink(city);
            nearbyCity.Add(link);
            if (NearbyCityAdded != null)
                NearbyCityAdded(this, city);

            stateChanged = true;
        }

        public override void Update(GameTime time)
        {
            float hours = (float)time.ElapsedGameTime.TotalHours;

            CarbonProduceSpeed = 0;

            


            if (Capture.IsCapturing && !IsCaptured)
            {
                float lra = 100 * hours;
                float hra = 100 * hours;
                if (Capture.NearbyCity1 != null)
                {
                    if (Capture.NewOwner1 == Capture.NearbyCity1.owner)
                    {
                        float capreq = Capture.NearbyCity1.LocalHR.Apply(hra);
                        float capreq2 = Capture.NearbyCity1.LocalLR.Apply(lra);
                        Capture.ReceiveGood(Capture.NewOwner1, capreq / CityGrade.GetCapturePoint(size), capreq2 / CityGrade.GetCapturePoint(size));
                    }
                    else
                    {
                        CancelCapture(Capture.NearbyCity1);
                    }
                }
                if (Capture.NearbyCity2 != null)
                {
                    if (Capture.NewOwner2 == Capture.NearbyCity2.owner)
                    {
                        float capreq = Capture.NearbyCity2.LocalHR.Apply(hra);
                        float capreq2 = Capture.NearbyCity2.LocalLR.Apply(lra);
                        Capture.ReceiveGood(Capture.NewOwner2, capreq / CityGrade.GetCapturePoint(size), capreq2 / CityGrade.GetCapturePoint(size));
                    }
                    else
                    {
                        CancelCapture(Capture.NearbyCity2);
                    }
                }
                if (Capture.NearbyCity3 != null)
                {
                    if (Capture.NewOwner3 == Capture.NearbyCity3.owner)
                    {
                        float capreq = Capture.NearbyCity3.LocalHR.Apply(hra);
                        float capreq2 = Capture.NearbyCity3.LocalLR.Apply(lra);
                        Capture.ReceiveGood(Capture.NewOwner3, capreq / CityGrade.GetCapturePoint(size), capreq2 / CityGrade.GetCapturePoint(size));
                    }
                    else
                    {
                        CancelCapture(Capture.NearbyCity3);
                    }
                }
                if (Capture.NearbyCity4 != null)
                {
                    if (Capture.NewOwner4 == Capture.NearbyCity4.owner)
                    {
                        float capreq = Capture.NearbyCity4.LocalHR.Apply(hra);
                        float capreq2 = Capture.NearbyCity4.LocalLR.Apply(lra);
                        Capture.ReceiveGood(Capture.NewOwner4, capreq / CityGrade.GetCapturePoint(size), capreq2 / CityGrade.GetCapturePoint(size));
                    }
                    else
                    {
                        CancelCapture(Capture.NearbyCity4);
                    }
                }



                Player player = Capture.CheckCapture();
                if (player != null)
                {
                    Capture.CancelCapture(player);

                    if (Capture.NewOwner1 != null && Capture.NewOwner1 != player)
                    {
                        CancelCapture(Capture.NearbyCity1);
                    }
                    if (Capture.NewOwner2 != null && Capture.NewOwner2 != player)
                    {
                        CancelCapture(Capture.NearbyCity2);
                    }
                    if (Capture.NewOwner3 != null && Capture.NewOwner3 != player)
                    {
                        CancelCapture(Capture.NearbyCity3);
                    }
                    if (Capture.NewOwner4 != null && Capture.NewOwner4 != player)
                    {
                        CancelCapture(Capture.NearbyCity4);
                    }
                    ChangeOwner(player);
                }
            }

            if (IsRecovering)
            {
                recoverCooldown -= hours;
                if (recoverCooldown < 0)
                {
                    IsRecovering = false;
                    CoolDownPlayer = null;
                }
            }
            else if (!IsHomeCity && Satisfaction < CityGrade.GetMinSatRatio(Size) && IsCaptured)
            {
                CoolDownPlayer = Owner;
                ChangeOwner(null);

                Development = InitialDeveleopment;
                Population = InitialPopulation;

                IsRecovering = true;
                recoverCooldown = CityGrade.GetRecoverCoolDown(Size);
            }

            if (!IsCaptured)
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
                {
                    Size = newSize;
                    if (CityGrow != null)
                        CityGrow();
                }
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
                City sourceCity = nearbyCity[i].Target;
                nearbyCity[i].HR = 0;
                nearbyCity[i].LR = 0;
                nearbyCity[i].Food = 0;

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
                            nearbyCity[i].LR += applyAmount;
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
                            nearbyCity[i].HR += applyAmount;
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
                            nearbyCity[i].Food += applyAmount;
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
            AdditionalDevMult = 1;
            HealthCare = 1;
            
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
                if (hrChange > float.Epsilon)
                    localHr.Commit(hrChange);

                //if (actHrChange < hrChange) // 资源过剩，转为碳
                //{
                //    CarbonProduceSpeed += (hrChange - actHrChange) / hours;
                //float actCmt = localHr.Commit(Math.Min(hrChange - actHrChange, CityGrade.GetHPTransportSpeed(Size) * hours));
                //}



                lrChange = plugins[i].LRPSpeed * hours;

                if (lrChange > float.Epsilon)
                    localLr.Commit(lrChange);

                //if (actLrChange < lrChange)// 资源过剩，转为碳
                //{
                //    CarbonProduceSpeed += (lrChange - actLrChange) / hours;

                //localLr.Commit(Math.Min(lrChange - actLrChange, CityGrade.GetLPTransportSpeed(Size) * hours));
                //}


                CarbonProduceSpeed += plugins[i].CarbonProduceSpeed;

                if (plugins[i].TypeId == CityPluginTypeId.EducationOrg)
                {
                    AdditionalDevMult *= (1.5f + plugins[i].UpgradePoint);
                }
                else if (plugins[i].TypeId == CityPluginTypeId.Hospital)
                {
                    HealthCare *= (1.5f + plugins[i].UpgradePoint);
                }
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

                for (int i = 0; i < farms.Count; i++)
                {
                    farms[i].Update(time);
                }

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
                        EventLogger.Instance.Log(EventType.Food, this);
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
                if (HealthCare > float.Epsilon)
                    Disease /= HealthCare;
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
            CarbonProduceSpeed += hrDev / hours;
            if (Owner.Type != PlayerType.LocalHuman)
            {
                CarbonProduceSpeed *= 0.5f;
            }
            float devIncr = popDevAdj * (lrDev * 0.5f + hrDev + DevBias / CityGrade.GetDevelopmentMult(Size));
            Development += AdditionalDevMult * (devIncr + foodLack);
            if (Development < 0)
            {
                Development = 0;
            }
            if (devIncr > 0)
            {
                Population += (devIncr + foodLack) * 0.05f;
            }
            if (Population > CityGrade.GetRefPopulation(Size))
            {
                Population = CityGrade.GetRefPopulation(Size);
            }

            if (carbonAddCounter++ == 60)
            {
                recentCarbon.Enqueue(CarbonProduceSpeed * hours);
                while (recentCarbon.Count > RecentCarbonLength)
                    recentCarbon.Dequeue();
                carbonAddCounter = 0;
            }

            if (owner.Type == PlayerType.LocalHuman)
            {
                if (recentCounter++ == 120)
                {
                    recentFood.Add(LocalFood.Current);
                    recentWood.Add(LocalLR.Current);
                    recentOil.Add(localHr.Current);
                    recentCounter = 0;

                    if (recentFood.Result < float.Epsilon) 
                    {
                        EventLogger.Instance.Log(EventType.Food, this);
                    }
                    if (recentWood.Result < float.Epsilon)
                    {
                        EventLogger.Instance.Log(EventType.Wood, this);
                    }
                    if (recentOil.Result < float.Epsilon)
                    {
                        EventLogger.Instance.Log(EventType.Oil, this);
                    }
                }
            }
            base.Update(time);
        }

        public void ResolveCities(Dictionary<string, City> table)
        {
            for (int i = 0; i < linkableCityName.Length; i++)
            {
                City city = table[linkableCityName[i]];
                linkableCity.Add(city);
            }
        }


        public float ProblemEnvironment
        {
            get;
            private set;
        }
        public float ProblemDisease
        {
            get;
            private set;
        }
        public float ProblemGender
        {
            get;
            private set;
        }
        public float ProblemHunger
        {
            get;
            private set;
        }
        public float ProblemMaternal
        {
            get;
            private set;
        }
        public float ProblemChild
        {
            get;
            private set;
        }
        public float ProblemEducation
        {
            get;
            private set;
        }

        public virtual void SetCapture(City city)
        {
            Capture.SetCapture(city.owner, city);

            if (CaptureSet != null)
                CaptureSet(this, city);
        }
        public virtual void CancelCapture(City city) 
        {
            Capture.CancelCapture(city.owner);

            if (CaptureCancel != null)
                CaptureCancel(this, city);
        }

        public override void Serialize(StateDataBuffer data)
        {
            ContentBinaryWriter bw = data.Writer;

            bw.Write(Development);
            bw.Write(Population);
            bw.Write(plugins.Count);
            for (int i = 0; i < plugins.Count; i++)
            {
                plugins[i].Serialize(bw);
            }
            bw.Write(FarmLandCount);

            Capture.Serialize(bw);

            data.EndWrite();
            stateChanged = false; 
        }
        public override void Deserialize(StateDataBuffer data)
        {
            ContentBinaryReader br = data.Reader;

            Development = br.ReadSingle();
            Population = br.ReadSingle();

            int plugCount = br.ReadInt32();

            if (plugCount > plugins.Count)
            {
                for (int i = 0; i < plugins.Count; i++) 
                {
                    plugins[i].Deserialize(br, Region);
                }

                int newCount = plugCount - plugins.Count;

                CityPluginFactory factory = new CityPluginFactory();
                for (int i = 0; i < newCount; i++)
                {
                    CityPlugin plugin = new CityPlugin(factory, factory.EducationOrgType, CityPluginTypeId.EducationOrg);
                    plugin.Deserialize(br, Region);
                    Add(plugin);
                }
            }
            else if (plugCount < plugins.Count) 
            {
                int nc = plugins.Count - plugCount;
                for (int i = 0; i < nc; i++)
                {
                    Remove(plugins[plugins.Count - 1]);
                }

                for (int i = 0; i < plugins.Count; i++) 
                {
                    plugins[i].Deserialize(br, Region);
                }
            }

            int farmCount = br.ReadInt32();

            if (farmCount > FarmLandCount) 
            {
                int nc = farmCount - FarmLandCount;
                for (int i = 0; i < nc; i++) 
                {
                    AddFarm();
                }
            }
            else if (farmCount < FarmLandCount)
            {
                int nc = FarmLandCount - farmCount;
                for (int i = 0; i < nc; i++)
                {
                    farms.RemoveAt(farms.Count - 1);
                }
            }

            Capture.Deserialize(br);
        }
        public override bool Changed
        {
            get
            {
                bool result = stateChanged | Capture.Changed;
                if (result)
                    return true;

                for (int i = 0; i < plugins.Count; i++)
                {
                    result |= plugins[i].Changed;
                    if (result)
                        break;
                }
                return result;
            }
        }

        #region IConfigurable 成员

        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);
            Name = sect.GetString("Name", string.Empty);
            //Population = sect.GetSingle("Population");
            Size = (UrbanSize)Enum.Parse(typeof(UrbanSize), sect.GetString("Size", UrbanSize.Small.ToString()));
            culture = (CultureId)Enum.Parse(typeof(CultureId), sect.GetString("Culture", CultureId.Asia.ToString()));
            StartUp = sect.GetInt("StartUp", -1);

            switch (Size)
            {
                case UrbanSize.Small:
                    InitialDeveleopment = (Randomizer.GetRandomSingle() * 0.3f + 0.7f) * CityGrade.SmallCityPointThreshold;
                    InitialPopulation = (Randomizer.GetRandomSingle() * 0.3f + 0.7f) * CityGrade.SmallRefPop;
                    break;
                case UrbanSize.Medium:
                    InitialDeveleopment = (Randomizer.GetRandomSingle() * 0.3f + 0.7f) * CityGrade.MediumCityPointThreshold;
                    InitialPopulation = (Randomizer.GetRandomSingle() * 0.3f + 0.7f) * CityGrade.MediumRefPop;
                    break;
                case UrbanSize.Large:
                    InitialDeveleopment = (Randomizer.GetRandomSingle() * 0.3f + 0.7f) * CityGrade.LargeCityPointThreshold;
                    InitialPopulation = (Randomizer.GetRandomSingle() * 0.3f + 0.7f) * CityGrade.LargeRefPop;
                    break;
            }

            Development = InitialDeveleopment;
            Population = InitialPopulation;


            int farmCount = Math.Min(MaxFarmLand, sect.GetInt("Farm", 0));
            for (int i = 0; i < farmCount; i++)
            {
                farms.Add(new FarmLand(base.Region, this));
            }

            linkableCityName = sect.GetStringArray("Linkable", null);

            float environment = sect.GetSingle("Environment", DefaultProblemWeight);
            float disease = sect.GetSingle("Disease", DefaultProblemWeight);
            float gender = sect.GetSingle("Gender", DefaultProblemWeight);
            float hunger = sect.GetSingle("Hunger", DefaultProblemWeight);
            float maternal = sect.GetSingle("Maternal", DefaultProblemWeight);
            float child = sect.GetSingle("Child", DefaultProblemWeight);
            float education = sect.GetSingle("Education", DefaultProblemWeight);


            ProblemChild = child;
            ProblemDisease = disease;
            ProblemEducation = education;
            ProblemEnvironment = environment;
            ProblemGender = gender;
            ProblemHunger = hunger;
            ProblemMaternal = maternal;

            float total = environment + disease + gender + hunger + maternal + child + education;

            total = 1 / total;

            environment *= total;
            disease *= total;
            gender *= total;
            hunger *= total;
            maternal *= total;
            child *= total;
            education *= total;

            disease += environment;
            gender += disease;
            hunger += gender;
            maternal += hunger;
            child += maternal;
            education += child;



            float p = Randomizer.GetRandomSingle();
            if (p < environment)
            {
                MajorProblem = MdgType.Environment;
            }
            else if (p < disease)
            {
                MajorProblem = MdgType.Diseases;
            }
            else if (p < gender)
            {
                MajorProblem = MdgType.GenderEquality;
            }
            else if (p < hunger)
            {
                MajorProblem = MdgType.Hunger;
            }
            else if (p < maternal)
            {
                MajorProblem = MdgType.MaternalHealth;
            }
            else if (p < child)
            {
                MajorProblem = MdgType.ChildMortality;
            }
            else
            {
                MajorProblem = MdgType.Education;
            }


            UpgradeUpdate();
        }

        #endregion
    }
}
