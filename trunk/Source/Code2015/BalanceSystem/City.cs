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
   public  class City : IUpdatable
    {
        [SLGValueAttribute()]
        const int TownPluginCount = 1;
        [SLGValueAttribute()]
        const int NormalPluginCount = 3;
        [SLGValueAttribute()]
        const int LargePluginCount = 4;

        public City()
        {
            GetProHPSpeed();
            GetProLPSpeed();
        }
        public City(UrbanSize size)
        {
            this.Size = size;
            GetProHPSpeed();
            GetProLPSpeed();
        }
        public City(string name)
        {
            this.Name = name;
            GetProHPSpeed();
            GetProLPSpeed();
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
        public float FoodCost
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
            GetProLPSpeed();
            GetProHPSpeed();
            plugin.NotifyAdded(this);
        }

        public void Remove(CityPlugin plugin) 
        {
            plugins.Remove(plugin);
            GetProLPSpeed();
            GetProHPSpeed();
            plugin.NotifyRemoved(this);
        }
       /// <summary>
       /// 得到城市添加或移除CityPlugin低能的消耗速度
       /// </summary>
        public void GetProLPSpeed()
        {
            float pluginspeed = 0;
            switch (Size)
            { 
                case UrbanSize.Large:
                    this.ProduceHPSpeed = -100;
                    this.ProduceLPSpeed = -100;
                    break;
                case UrbanSize.Normal:
                    this.ProduceHPSpeed = -80;
                    this.ProduceLPSpeed = -80;
                    break;
                case UrbanSize.Town:
                    this.ProduceHPSpeed = -50;
                    this.ProduceLPSpeed = -50;
                    break;
            }
           
            if (plugins.Count != 0)
            {
                for (int i = 0; i < plugins.Count; i++)
                {
                    pluginspeed += plugins[i].ProduceLPSpeed;   
                }
            }
            this.ProduceLPSpeed += pluginspeed;
        }
       /// <summary>
        /// 得到城市添加或移除CityPlugin高能的消耗速度
       /// </summary>
        public void GetProHPSpeed()
        {
            float pluginspeed = 0;
            switch (Size)
            {
                case UrbanSize.Large:
                    this.ProduceHPSpeed = -100;
                    this.ProduceLPSpeed = -100;
                    break;
                case UrbanSize.Normal:
                    this.ProduceHPSpeed = -80;
                    this.ProduceLPSpeed = -80;
                    break;
                case UrbanSize.Town:
                    this.ProduceHPSpeed = -50;
                    this.ProduceLPSpeed = -50;
                    break;
            }
           
            if (plugins.Count != 0)
            {
                for (int i = 0; i < plugins.Count; i++)
                {
                    pluginspeed += plugins[i].ProduceHLSpeed;
                }

                this.ProduceHPSpeed += pluginspeed;
            }
           
        }

        public void Update(GameTime time)
        {
          
        }
    }
}
