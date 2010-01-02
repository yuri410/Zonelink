using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Collections;

namespace Code2015.BalanceSystem
{

    public class PlantSpecies : CreationSpecies
    {
        public string Name
        {
            get;
            set;
        }
        public PlantSpecies()
        { }
        public PlantSpecies(string name)
        {
            Name = name;
        }

        /// <summary>
        ///  植物固碳的速度
        /// </summary>
        public float CarbonTransformSpeed
        {
            get;
            set;
        }
        public float SetCTransSpeed(float speed)
        {
            return this.CarbonTransformSpeed = speed;
        }
        /// <summary>
        /// 植物固碳量
        /// </summary>
        new public float CarbonWeight
        {
            get { return CarbonTransformSpeed * Amount; }
            set { CarbonWeight = value; }
        }
        /// <summary>
        ///  局部生态环境湿度调整系数
        /// </summary>
        public float HumidityAdjust
        {
            get;
            set;
        }
        /// <summary>
        ///  局部生态环境沙漠化调整系数
        /// </summary>
        public float DesertificationAdjust
        {
            get;
            set;
        }


        //生态环境湿度调整系数，沙漠化调整系数
        public void SetPlantAdjust(PlantSpecies plant, float humidityAdjust, float desertificationAdjust)
        {
            plant.HumidityAdjust = humidityAdjust;
            plant.DesertificationAdjust = desertificationAdjust;
        }

        public override float GetCarbonWeght()
        {
            throw new NotImplementedException();
        }



    }
}
