﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;

namespace Code2015.BalanceSystem
{

    public class OilField : NaturalResource
    {
        [SLGValueAttribute()]
        const float OilWeight = 100000;

        /// <summary>
        /// 剩余的量
        /// </summary>
        public float RemainedOilWeight
        {
            get;
            set;
        }
        
       
        public float GetRemainedWeight(GameTime time1,GameTime time2)
        {
            return RemainedOilWeight = SourceAmount -(time1.ElapsedGameTime-time2.ElapsedGameTime)* ConsumeSpeed;
        }
        /// <summary>
        /// 开采石油也要产生C
        /// </summary>
        /// <returns></returns>
        public override float GetCarbonWeight()
        {
            return (SourceAmount - RemainedOilWeight) * 200;
        }

        public OilField()
        {
            SetSourceAmount(OilWeight);
            SetConsumeSpeed(10);
        }


       


    }
}
