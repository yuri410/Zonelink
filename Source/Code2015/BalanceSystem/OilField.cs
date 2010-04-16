using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Config;

namespace Code2015.BalanceSystem
{

    public class OilField : NaturalResource
    {
        //const float EMITCarbonSpeed = 10;//油田释放的C值

        [SLGValue]
        const float TimesMaxAmount = 2;
        [SLGValue]
        const float RecoverBias = 10;
        //public const float MaxAmount = 10000;

        public OilField(SimulationWorld region)
            : base(region, NaturalResourceType.Petro)
        {
           
        }

        public float MaxAmount
        {
            get;
            private set;
        }

        //public float EmitCarbonSpeed
        //{
        //    get;
        //    set;
        //}

        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);

            MaxAmount = TimesMaxAmount * CurrentAmount;
        }

        public override void Update(GameTime time)
        {
            float hours = (float)time.ElapsedGameTime.TotalHours;

            if (CurrentAmount < MaxAmount)
            {
                CurrentAmount += RecoverBias * hours;
            }
        }
        
    }
}
