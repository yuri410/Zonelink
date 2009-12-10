using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Code2015.BalanceSystem
{
    //植物物种，有CO2的转换速度，湿度适应力，干燥适应力
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

    }
}
