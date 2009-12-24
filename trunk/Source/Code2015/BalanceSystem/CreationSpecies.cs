using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;

namespace Code2015.BalanceSystem
{
    class CreationSpecies : IUpdatable
    {
      
        /// <summary>
        /// 代表生物的数目
        /// </summary>
        public float Strength
        {
            get;
            set;
        }
        public float CarbonGasWeight
        {
            get;
            set;
        }
        
        public void Update(GameTime time)
        { }
    }

}
