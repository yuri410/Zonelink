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


        #region  属性
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
        /// 高能产生的速度,速度为正表示产生能量，为负值表示消耗能量
        /// </summary>
        public float ProduceHLSpeed
        {
            get;
            set;
        }
        /// <summary>
        /// 低能产生的速度
        /// </summary>
        public float ProduceLPSpeed
        {
            get;
            set;
        }
        /// <summary>
        /// 产生的C，为正值表示释放C，为负值表示吸收碳
        /// </summary>
        public float CarbonWeight
        {
            get;
            set;
        }
        #endregion

        public void NotifyAdded(City city)
        {
           
        }

        public void NotifyRemoved(City city)
        {

        }


    }
}
