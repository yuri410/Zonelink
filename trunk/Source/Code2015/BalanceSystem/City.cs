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

        private string name;
        private float population, development, food, disease;

        /// <summary>
        ///  表示城市的附加设施
        /// </summary>
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
        public float FoodCost
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

       /// <summary>
       ///获得用户所选择的添加附加物，用于可以和界面进行交互
       /// </summary>
       /// <returns></returns>
        public FastList<CityPlugin> ChoosedCityPlugin()
        { 
            FastList<CityPlugin> choosedplugins=new FastList<CityPlugin>();
            CityPluginFactory factory=new CityPluginFactory();
            choosedplugins.Add(factory.MakeCollege());
            //得到用户选择添加的附加物
            return choosedplugins;
        }
        public void NotifyAdded(City city)
        {
            city.plugins.Add(ChoosedCityPlugin());
           
        }

        public void NotifyRemoved(City city)
        {
            for (int i = 0; i < ChoosedCityPlugin().Count; i++)
            {
                if (city.plugins[i].Name == ChoosedCityPlugin()[i].Name)
                {
                    city.plugins.RemoveAt(i);
                }
            }
                
           
        }

        public void Out()
        {
            for (int i = 0; i < plugins.Count; i++)
            {
                Console.WriteLine(plugins[i].Name);
            }
        }
        public void Update(GameTime time)
        {
          
        }

        

      
    }
}
