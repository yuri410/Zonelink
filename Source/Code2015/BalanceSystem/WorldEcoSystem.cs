using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;

namespace Code2015.BalanceSystem
{
    public class WorldEcoSystem : IUpdatable
    {
        /// <summary>
        ///  厄尔尼诺程度
        /// </summary>
        public float ENWeight
        {
            get;
            set;
        }
        /// <summary>
        ///  温度提升程度（温室效应）
        /// </summary>
        public float TemperatureShift
        {
            get;
            set;
        }
        /// <summary>
        ///  海平面
        /// </summary>
        public float SeaLevel
        {
            get;
            set;
        }


        SimulationRegion[] regions;

        public float AccumulatedCarbon
        {
            get;
            private set;
        }
       

        public void Update(GameTime time)
        {
            //    for (int i = 0; i < regions.Length; i++)
            //    {
            //        LocalEcoSystem[] ecos = regions[i].EcoSystems;
            //        for (int j = 0; j < ecos.Length; j++)
            //        {
            //            AccumulatedCarbon += ecos[j].CarbonChange;
            //        }
            //    }
        }


    }
}
