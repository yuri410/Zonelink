using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Collections;
using Apoc3D;

namespace Code2015.BalanceSystem
{
    public class SocietyStatus : IUpdatable
    {
        public SocietyStatus(SimulateRegion region)
        {

        }

        public float Development
        {
            get;
        }


        public float Population
        {
            get;
        }

        public void Update(GameTime time)
        {
        
        }
    }
}
