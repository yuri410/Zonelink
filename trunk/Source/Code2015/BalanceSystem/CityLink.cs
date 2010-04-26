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

        public float HR;
        public float LR;
        public float Food;
    }
}
