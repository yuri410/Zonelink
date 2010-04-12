using System;
using System.Collections.Generic;
using System.Text;

namespace Code2015.BalanceSystem
{
    public class CityLink
    {
        public City Target
        {
            get;
            private set;
        }
        
        public CityLink(City a)
        {
            Target = a;
        }

        public bool Disabled
        {
            get;
            private set;
        }

        public void Disable()
        {
            Disabled = true;
        }

        public float HR;
        public float LR;
        public float Food;
    }
}
