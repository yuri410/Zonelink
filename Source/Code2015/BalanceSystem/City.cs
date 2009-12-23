using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;

namespace Code2015.BalanceSystem
{
    /// <summary>
    ///  表示城市的附加设施
    /// </summary>
    abstract class CityPlugin 
    {
        public abstract void NotifyAdded(City city);
        public abstract void NotifyRemoved(City city);        
    }

    /// <summary>
    ///  表示城市的大小
    /// </summary>
    enum UrbanSize
    {
        Town,
        Normal,
        Large
    }

    /// <summary>
    ///  表示一座城市
    /// </summary>
    class City : IUpdatable
    {
        [SLGValueAttribute()]
        const int TownPluginCount = 1;
        [SLGValueAttribute()]
        const int NormalPluginCount = 3;
        [SLGValueAttribute()]
        const int LargePluginCount = 4;

        private string name;
        private float population, development, food, disease;


        FastList<CityPlugin> plugins = new FastList<CityPlugin>();

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public float Development
        {
            get { return development; }
            set { development = value; }
        }
        public float Population
        {
            get { return population; }
            set { population = value; }
        }
        public float Food
        {
            get { return food; }
            set { food = value; }
        }
        public float Disease
        {
            get { return disease; }
            set { disease = value; }
        }

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

        public void Update(GameTime time)
        {
        }

        

      
    }
}
