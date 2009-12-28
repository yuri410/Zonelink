using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Code2015.BalanceSystem
{
    class OilField :Simulateobject
    {
        public float CurrentOilWeight
        {
            get;
            set;
        }

        public float RemainedOilWeight
        {
            get;
            set;
        }
        /// <summary>
        /// 得到开采到的石油
        /// </summary>
        /// <returns></returns>
        public float GetOilWeight()
        {
            return CurrentOilWeight - RemainedOilWeight;
        }
       /// <summary>
       /// 开采石油要产生的碳量
       /// </summary>
       /// <returns></returns>
        public override float GetCarbonWeight()
        {
            return this.GetCarbonWeight() * 200;
        }
    }
}
