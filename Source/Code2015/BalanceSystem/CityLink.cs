using System;
using System.Collections.Generic;
using System.Text;

namespace Code2015.BalanceSystem
{
    class CityLink
    {
        public City A
        {
            get;
            private  set;
        }
        public City B
        {
            get;
            private set;
        }

        public CityLink(City a, City b)
        {
            A = a;
            B = b;
        }


    }
}
