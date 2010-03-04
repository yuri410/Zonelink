using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Config;

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
        const float RecoverRate = 1;

        public float AbsorbCarbonSpeed
        {
            get;
            set;
        }

        public Forest(SimulationRegion region)
            : base(region, NaturalResourceType.Wood)
        {
           
        }

      

        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);
            
        }


        public override void Update(GameTime time)
        {
            float hours = (float)time.ElapsedGameTime.Hours;
        }

    }
}
