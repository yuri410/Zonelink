using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;

namespace Code2015.BalanceSystem
{
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
        [SLGValueAttribute()]
        const int TownPluginCount = 1;
        [SLGValueAttribute()]
        const int NormalPluginCount = 3;
        [SLGValueAttribute()]
        const int LargePluginCount = 4;

        #region 能源流通速度
        [SLGValueAttribute()]
        const int SmallCityHPTranportSpeed = 30;
        [SLGValueAttribute()]
        const int MediumCityHPTranportSpeed = 50;
        [SLGValueAttribute()]
        const int LargeCityHPTranportSpeed = 100;

        [SLGValueAttribute()]
        const int SmallCityLPTranportSpeed = 15;
        [SLGValueAttribute()]
        const int MediumCityLPTranportSpeed = 25;
        [SLGValueAttribute()]
        const int LargeCityLPTranportSpeed = 50;
        #endregion

        #region 能源使用速度
        [SLGValueAttribute()]
        const int SmallCityLPSpeed = -30;
        [SLGValueAttribute()]
        const int MediumCityLPSpeed = -50;
        [SLGValueAttribute()]
        const int LargeCityLPSpeed = -100;

        [SLGValueAttribute()]
        const int SmallCityHPSpeed = -30;
        [SLGValueAttribute()]
        const int MediumCityHPSpeed = -50;
        [SLGValueAttribute()]
        const int LargeCityHPSpeed = -100;
        #endregion

        static readonly int[] LPSpeed = { SmallCityLPSpeed, MediumCityLPSpeed, LargeCityLPSpeed };
        static readonly int[] HPSpeed = { SmallCityHPSpeed, MediumCityHPSpeed, LargeCityHPSpeed };

        public City()
        {
            UpdateCity();
        }
        public City(UrbanSize size)
        {
            this.Size = size;
            UpdateCity();
        }
        public City(string name)
        {
            this.Name = name;
            UpdateCity();
        }

        /// <summary>
        ///  表示城市的附加设施
        /// </summary>
        FastList<CityPlugin> plugins;



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
            get
            {
                switch (Size)
                {
                    case UrbanSize.Small:
                        return 20000;
                    case UrbanSize.Normal:
                        return 500000;
                    case UrbanSize.Large:
                        return 3000000;
                }
            }
        }
        public float Disease
        {
            get;
            set;
        }
        /// <summary>
        /// 城市所需的能量阈值
        /// </summary>
        public float LimitedHPEnergy
        {
            get;
            set;
        }
        public float LimitedLPEnergy
        {
            get;
            set;
        }


        protected float HPChange
        {
            get;
            set;
        }


        public float SelfHPProductionSpeed
        {
            get { return HPSpeed[(int)Size]; }
        }
        public float GetHPChange()
        {
            float r = HPChange;
            HPChange = 0;
            return r;
        }

        protected float LPChange
        {
            get;
            set;
        }
        public float SelfLPProductionSpeed
        {
            get { return LPSpeed[(int)Size]; }
        }
        public float GetLPChange()
        {
            float r = LPChange;
            LPChange = 0;
            return r;
        }

        protected float FoodChange
        {
            get;
            set;
        }
        public float SelfFoodCostSpeed
        {
            get;
            protected set;
        }
        public float GetFoodChange()
        {
            float r = FoodChange;
            FoodChange = 0;
            return r;
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
        /// 若有Plugin，获取Plugin的消耗速度
        /// </summary>
        public float PluginHPProductionSpeed
        {
            get;
            set;
        }

        public float PluginLPProductionSpeed
        {
            get;
            set;
        }
        public float PluginFoodCostSpeed
        {
            get;
            set;
        }
        public float PluginCarbonProduceSpeed
        {
            get;
            set;
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
                    this.LimitedHPEnergy = 5000;
                    this.LimitedLPEnergy = 5000;
                    this.SelfFoodCostSpeed = 50;
                    this.CarbonProduceSpeed = 500;
                    break;
                case UrbanSize.Normal:
                    this.LimitedHPEnergy = 3000;
                    this.LimitedLPEnergy = 3000;
                    this.SelfFoodCostSpeed = 30;
                    this.CarbonProduceSpeed = 300;
                    break;
                case UrbanSize.Small:
                    this.LimitedHPEnergy = 2000;
                    this.LimitedLPEnergy = 2000;
                    this.SelfFoodCostSpeed = 10;
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
            return PluginHPProductionSpeed-PluginFoodCostSpeed;//获得由炼油厂产生的高能速度
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
            float hours = (float)time.ElapsedGameTime.TotalHours;

            this.HPChange += ProduceHPSpeed * hours;
            this.LPChange += ProduceLPSpeed * hours;

            this.CarbonChange += (CarbonProduceSpeed +PluginCarbonProduceSpeed)* hours;
            this.FoodChange += FoodCostSpeed * hours;
                    // 计算发展度
        }
    }
}
