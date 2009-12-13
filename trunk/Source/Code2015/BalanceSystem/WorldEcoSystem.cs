using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;

namespace Code2015.BalanceSystem
{
    class WorldEcoSystem : IUpdatable
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


        Region[] regions;

        public float AccumulatedCarbon
        {
            get;
            private set;
        }
        //FastList<CarbonGroup> carbonGroups;

        ////计算碳含量
        //public float AccumulatedCarbon()
        //{
        //    float result = 0;
        //    for (int i = 0; i < carbonGroups.Count; i++) 
        //    {
        //        result += carbonGroups[i].Weight;
        //    }
        //    return result;
        //}

        public void Update(GameTime time)
        {
            for (int i = 0; i < regions.Length; i++)
            {
                LocalEcoSystem[] ecos = regions[i].EcoSystems;
                for (int j = 0; j < ecos.Length; j++)
                {
                    AccumulatedCarbon += ecos[j].CarbonChange;
                }
            }
        }
    }
}
