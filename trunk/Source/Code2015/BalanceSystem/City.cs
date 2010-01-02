using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;

namespace Code2015.BalanceSystem
{
  

   /// <summary>
    ///  表示城市的大小
    /// </summary>
    public  enum UrbanSize
    {
        Town,
        Normal,
        Large
    }

    /// <summary>
    ///  表示一座城市
    /// </summary>
   public  class City : Simulateobject,IUpdatable
    {
        [SLGValueAttribute()]
        const int TownPluginCount = 1;
        [SLGValueAttribute()]
        const int NormalPluginCount = 3;
        [SLGValueAttribute()]
        const int LargePluginCount = 4;

        public City()
        {
            InitialCity();
        }
        public City(UrbanSize size)
        {
            this.Size = size;
            InitialCity();
        }
        public City(string name)
        {
            this.Name = name;
            InitialCity();
        }

        /// <summary>
        ///  表示城市的附加设施
        /// </summary>
       FastList<CityPlugin> plugins = new FastList<CityPlugin>();

       
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
            set;
        }
        public float FoodCostSpeed
        {
            get;
            set;
        }
        public float Disease
        {
            get;
            set;
        }
       /// <summary>
       /// 城市自身的消耗高能和低能的速度，为负值，表示消耗
       /// </summary>
        public float ProduceHPSpeed
        {
            get;
            set;
        }

        public float ProduceLPSpeed
        {
            get;
            set;
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
                    case UrbanSize.Town:
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
            InitialCity();
            plugin.NotifyAdded(this);
        }

        public void Remove(CityPlugin plugin) 
        {
            plugins.Remove(plugin);
            InitialCity();
            plugin.NotifyRemoved(this);
        }
       /// <summary>
       ///  初始化城市的属性设置
       /// </summary>
        public void InitialCity()
        {
            float pluginLPspeed = 0;
            float pluginHPspeed = 0;
            float FoodCostSpeed = 0;
            switch (Size)
            {
                case UrbanSize.Large:
                    
                        this.ProduceHPSpeed = -100;
                        this.ProduceLPSpeed = -100;
                        this.Population = 100000;
                        this.FoodCostSpeed = 50;
                        this.CarbonSpeed = 500;
                        break;
                case UrbanSize.Normal:
                        this.ProduceHPSpeed = -80;
                        this.ProduceLPSpeed = -80;
                        this.Population = 50000;
                        this.FoodCostSpeed = 30;
                        this.CarbonSpeed = 300;
                        break;
                case UrbanSize.Town:
                        this.ProduceHPSpeed = -50;
                        this.ProduceLPSpeed = -50;
                        this.Population = 20000;
                        this.FoodCostSpeed = 10;
                        this.CarbonSpeed = 100;
                        break;
            }
            if (plugins.Count != 0)
            {
                for (int i = 0; i < plugins.Count; i++)
                {
                    pluginHPspeed += plugins[i].ProduceHLSpeed;
                    pluginLPspeed += plugins[i].ProduceLPSpeed;
                    if (plugins[i].Name == "BioEnergyFactory")
                        FoodCostSpeed += plugins[i].ProduceHLSpeed * 1.5f;
                }
                this.ProduceHPSpeed += pluginHPspeed;
                this.ProduceLPSpeed += pluginLPspeed;
                this.FoodCostSpeed += FoodCostSpeed;
            }
        }
       /// <summary>
       /// 得到当前城市拥有的Plugins
       /// </summary>
       /// <returns></returns>
        public FastList<CityPlugin> GetPlugins()
        {
            if (plugins.Count != 0)

                return plugins;
            else
                return new FastList<CityPlugin>();
            
        }
     
        public override void Update(GameTime time)
        {         
                this.CarbonWeight += this.CarbonSpeed * time.ElapsedGameTime.Days;
        }
    }
}
