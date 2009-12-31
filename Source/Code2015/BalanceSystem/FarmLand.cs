using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Collections;

namespace Code2015.BalanceSystem
{
    /// <summary>
    /// 农场土壤的等级
    /// </summary>
    public enum SoilGrade { fine, medium, bad };
    
    /// <summary>
    /// 农作物的类
    /// </summary>
   
    public class FarmLand : NaturalResource
    {

        public string Name
        {
            get;
            set;
        }
        public FarmLand(string name)
        {
            this.Name = name;
        }

        public FarmLand()
        {
            InitFarm();
        }

        FastList<PlantSpecies> farm = new FastList<PlantSpecies>();
        public FastList<PlantSpecies> InitFarm()
        {
            farm.Add(new PlantSpecies("Wheat"));//小麦
            farm.Add(new PlantSpecies("Rice"));//水稻
            return farm;
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
       
    }
}
