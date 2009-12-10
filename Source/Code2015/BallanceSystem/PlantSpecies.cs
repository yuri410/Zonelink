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

        public float CarbonTransformSpeed
        {
            get { return carbonTransformSpeed; }
            set { carbonTransformSpeed = value; }
        }
        public float HumidityAdjust
        {
            get { return humidityAdjust; }
            set { humidityAdjust = value; }
        }
        public float DesertificationAdjust
        {
            get { return desertificationAdjust; }
            set { desertificationAdjust = value; }
        }

    }
}
