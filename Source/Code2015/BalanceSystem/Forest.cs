using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Config;
using Code2015.Network;

namespace Code2015.BalanceSystem
{
    public class Forest : NaturalResource
    {
        /// <summary>
        ///  
        /// </summary>
        [SLGValueAttribute()]
        const float AbsorbCarbonRate = 1000;


        [SLGValue]
        const float RecoverRate = 0.015f;
        [SLGValue]
        const float RecoverBias = 20f;

        [SLGValue]
        const float TimesMaxAmount = 2;


        public float MaxAmount
        {
            get;
            private set;
        }


        public float Radius
        {
            get;
            private set;
        }


        public Forest(SimulationWorld region)
            : base(region, NaturalResourceType.Wood)
        {

        }



        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);

            Radius = sect.GetSingle("Radius");

            MaxAmount = CurrentAmount * TimesMaxAmount;
        }


        public override void Update(GameTime time)
        {
            float hours = (float)time.ElapsedGameTime.TotalHours;

            if (CurrentAmount < MaxAmount)
            {
                CurrentAmount += (CurrentAmount * RecoverRate + RecoverBias) * hours;
            }
        }


    }
}
