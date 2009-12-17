using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Collections;

namespace Code2015.BalanceSystem
{
   
    class PlantSpecies : CreationSpecies
    {
        private float carbonTransformSpeed;
        private float humidityAdjust;
        private float desertificationAdjust;
       
        /// <summary>
        ///  光合作用固化碳的速度
        /// </summary>
        public float CarbonTransformSpeed
        {
            get { return carbonTransformSpeed; }
            set { carbonTransformSpeed = value; }
        }
        /// <summary>
        ///  局部生态环境湿度调整系数
        /// </summary>
        public float HumidityAdjust
        {
            get { return humidityAdjust; }
            set { humidityAdjust = value; }
        }
        /// <summary>
        ///  局部生态环境沙漠化调整系数
        /// </summary>
        public float DesertificationAdjust
        {
            get { return desertificationAdjust; }
            set { desertificationAdjust = value; }
        }  
        public string Name
        {
            get;
            set;
        }
        public PlantSpecies(string name)
        {
            Name = name;
        }
        public void GetPlantsFactors(PlantSpecies plant,LocalEcoSystem local)
        {
            if (plant.CreationsArea > local.LocalSysArea / 3)
            {
                plant.CarbonTransformSpeed = 400;
                plant.HumidityAdjust = 1.0f;
                plant.DesertificationAdjust = 1.0f;
            }
            else if ((plant.CreationsArea > local.LocalSysArea / 5) && (plant.CreationsArea <= local.LocalSysArea / 3))
            {
                plant.CarbonTransformSpeed = 300;
                plant.HumidityAdjust = 0.7f;
                plant.DesertificationAdjust = 0.7f;
            }
            else
            {
                plant.CarbonTransformSpeed = 150;
                plant.HumidityAdjust = 0.4f;
                plant.DesertificationAdjust = 0.4f;
            }
        }
    }
}
