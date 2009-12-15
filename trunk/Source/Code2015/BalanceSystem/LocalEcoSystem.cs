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
        public float Age
        {
            get { return age; }
            set { age = value; }
        }
        public float Humidity
        {
            get { return humidity; }
            set { humidity = value; }
        }
        public float Fertility
        {
            get { return fertility; }
            set { fertility = value; }
        }
        public float Desertification
        {
            get { return desertification; }
            set { desertification = value; }
        }
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
        /// <summary>
        /// 植物有草，灌木，树
        /// </summary>
        FastList<PlantSpecies> plants = new FastList<PlantSpecies>();
       
        /// <summary>
        /// 动物分别有昆虫，小型动物，大型动物
        /// </summary>
        FastList<AnimalSpecies> animals = new FastList<AnimalSpecies>();


        /// <summary>
        /// 植物的面积和本地生态系统的面积之间的比例决定植物的各个因素对系统的影响
        /// </summary>
        
        public void SetPlantFactor(PlantSpecies plant, LocalEcoSystem local)
        {
             FastList<PlantSpecies> plants = new FastList<PlantSpecies>();
             
            if (plant.CreationsArea > local.LocalSysArea / 4)
            {
                plant.CarbonTransformSpeed = 300;
                plant.HumidityAdjust = 0.8f;
                plant.DesertificationAdjust = 0.8f;
            }
            else if (local.LocalSysArea / 6 < plant.CreationsArea && (plant.CreationsArea < local.LocalSysArea / 4))
            {
                plant.CarbonTransformSpeed = 200;
                plant.HumidityAdjust = 0.6f;
                plant.DesertificationAdjust = 0.6f;
            }
            else
            {
                plant.CarbonTransformSpeed = 100;
                plant.HumidityAdjust = 0.3f;
                plant.DesertificationAdjust = 0.3f;
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
