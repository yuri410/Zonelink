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
        /// <summary>
        /// 植物的数目
        /// </summary>
        public float Amount
        {
            get;
            set;
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
        
        //设置不同植物的固碳速度，生态环境湿度调整系数，沙漠化调整系数
        public void SetPlantFactor(float plantCSpeed, float plantHumidity, float plantDesert)
        {
            this.CarbonTransformSpeed = plantCSpeed;
            this.HumidityAdjust = plantHumidity;
            this.DesertificationAdjust = plantDesert;
        }


    }
}
