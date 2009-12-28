using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Code2015.BalanceSystem
{
    /// <summary>
    /// 农场土壤的等级
    /// </summary>
    public enum SoilGrade { fine, medium, bad };

    class FarmLand:Simulateobject
    {

        PlantSpecies foofPlant = new PlantSpecies("foodPlant");
       /// <summary>
       /// 粮食的总产量
       /// </summary>
        public float FoodWeight
        {
            get;
            set;
        }
        /// <summary>
        /// 土壤的等级
        /// </summary>
        public SoilGrade soilGrade
        {
            get;
            private set;
        }
        /// <summary>
        /// 单位的产量
        /// </summary>
        public float UnitProduct
        {
            get
            {
                switch (soilGrade)
                { 
                    case SoilGrade.fine:
                        return 1000;
                    case SoilGrade.medium:
                        return 700;
                    case SoilGrade.bad:
                        return 300;

                }
                return 0;
            }
        }
        /// <summary>
        /// 得到粮食的总产量
        /// </summary>
        /// <returns></returns>
        public float GetFoodWeight()
        {
            return this.FoodWeight= UnitProduct * foofPlant.Amount;
        }
        /// <summary>
        /// 获得粮食固碳量
        /// </summary>
        /// <returns></returns>
        public override float GetCarbonWeight()
        {
            return foofPlant.Amount * (UnitProduct + 200);
        }
       
    }
}
