using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;

namespace Code2015.BalanceSystem
{

    public class OilField : NaturalResource
    {
        [SLGValueAttribute()]
        const float OILWeight = 100000;//油田含油量的初始值
        const float EMITCarbonSpeed = 10;//油田释放的C值


        public OilField(SimulationRegion region)
            : base(region, NaturalResourceType.Petro)
        {
           
        }

        public float EmitCarbonSpeed
        {
            get;
            set;
        }
     

      
        
        public override void Update(GameTime time)
        {
         
        }

    }
}
