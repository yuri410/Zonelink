using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;

namespace Code2015.BalanceSystem
{
    class LocalEcoSystem : IUpdatable, ICarbon
    {
        private float age, humidity, fertility, desertification;
        
        private bool balanced;
        /// <summary>
        /// 局部生态系统的存在时间
        /// </summary>
        public float Age
        {
            get { return age; }
            set { age = value; }
        }
        /// <summary>
        /// 局部生态系统湿度调整系数
        /// </summary>
        public float Humidity
        {
            get { return humidity; }
            set { humidity = value; }
        }
        /// <summary>
        /// 动物使土壤肥沃程度
        /// </summary>
        public float Fertility
        {
            get { return fertility; }
            set { fertility = value; }
        }
        /// <summary>
        /// 沙漠化调整系数
        /// </summary>
        public float Desertification
        {
            get { return desertification; }
            set { desertification = value; }
        }
        /// <summary>
        ///局部生态系统是否平衡
        /// </summary>
        public bool Balanced
        {
            get { return balanced; }
            set { balanced = value; }
         }
        /// <summary>
        /// 本地生态系统的面积
        /// </summary>
        public float LocalSysArea
        {
            get;
            set;
        }
      
        FastList<PlantSpecies> plants = new FastList<PlantSpecies>();
       
        /// <summary>
        /// 动物分别有昆虫，小型动物，大型动物
        /// </summary>
        FastList<AnimalSpecies> animals = new FastList<AnimalSpecies>();

        public LocalEcoSystem GetFactors(PlantSpecies plant, AnimalSpecies animal,LocalEcoSystem local)
        {
            local.Desertification = animal.FertilisingSpeed;
            local.Humidity = plant.HumidityAdjust;
            local.Desertification = plant.DesertificationAdjust;
            return local;
        }

        public bool IsBalanced(LocalEcoSystem local)
        {
            if (local.Desertification > 0.5 && local.Humidity > 0.5 && local.Fertility >= 80)
            {
                return local.Balanced = true;
            }
            else
            {
                return local.Balanced = false;
            }

        }

     
       
       
        
        /// <summary>
        /// 动物因素对本地生态系统的影响
        /// </summary>
       
        
        public void Update(GameTime time)
        { 
            
        }



        #region ICarbon 成员

        public float CarbonChange
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
