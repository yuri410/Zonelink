using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Code2015.BalanceSystem
{
    public class CityPlugin
    {


        public string Name
        {
            get;
            set;
        }
        public CityPlugin(string name)
        {
            this.Name = name;
        }

        public string GetCityPluginName()
        {
            return this.Name;
        }

        /// <summary>
        /// 开始建造一个所需费用
        /// </summary>
        public float CostMoney
        {
            get;
            set;
        }
        /// <summary>
        /// 升级所需费用
        /// </summary>
        public float ImproveCost
        {
            get;
            set;
        }
        /// <summary>
        /// 高能产生的速度
        /// </summary>
        public float HighPowerSpeed
        {
            get;
            set;
        }
        /// <summary>
        /// 低能产生的速度
        /// </summary>
        public float LowPower
        {
            get;
            set;
        }
        /// <summary>
        /// 产生的C
        /// </summary>
        public float CarbonWeight
        {
            get;
            set;
        }
        /// <summary>
        /// 消耗的能量
        /// </summary>
        public float ConsumePower
        {
            get;
            set;
        }       
        public void NotifyAdded(City city)
        {
           city.NotifyAdded(city);   
        }
        public void NotifyRemoved(City city)
        { 
            
        }



    }
}
