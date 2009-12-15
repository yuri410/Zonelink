using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Code2015.BalanceSystem
{
    class FarmLand:NaturalResource
    {
        /// <summary>
        /// 农场的面积
        /// </summary>
        public float FarmArea
        {
            get;
            set;
        }
       
        /// <summary>
        /// 农场土壤的等级
        /// </summary>
        public enum OilGrade { fine, medium, bad };
        
        /// <summary>
        /// 得到区域内的粮食的产量
        /// </summary>
        /// <returns></returns>
        public float GetFoodAmount(OilGrade oilGrade)
        {
            switch (oilGrade)
            { 
                case OilGrade.fine:
                    return FarmArea * 1000;
                case OilGrade.medium:
                    return FarmArea * 800;
                case OilGrade.bad:
                    return FarmArea * 600;
            }

            return 0;
        }

    }
}
