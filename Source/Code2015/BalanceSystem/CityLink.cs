using System;
using System.Collections.Generic;
using System.Text;

namespace Code2015.BalanceSystem
{
    class CityLink
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

        public bool IsTransporting;
    }
}
