using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Code2015.BalanceSystem
{
    public abstract class ObjectFactory
    {
        public Simulateobject OrderObject(string type)
        {
            Simulateobject simulateObject;
            simulateObject = CreateObject(type);


            return simulateObject;
            
        }

        protected abstract Simulateobject CreateObject(string type);
        
    }
}
