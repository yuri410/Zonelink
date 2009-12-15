using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Code2015.BalanceSystem
{
    class Forest : NaturalResource,ICarbon
    {

        #region ICarbon 成员

        public float CarbonChange
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

       
    }
}
