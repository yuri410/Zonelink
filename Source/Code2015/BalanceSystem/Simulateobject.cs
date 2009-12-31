using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;

namespace Code2015.BalanceSystem
{
    public class Simulateobject:IUpdatable
    {     
        /// <summary>
        /// CO2的改变量
        /// </summary>
        public float CarbonWeight=0.0f;
       
        public virtual float GetCarbonWeight()
        {
            return this.CarbonWeight;
        }

        public virtual void Update(GameTime time)
        { }
    }
}
