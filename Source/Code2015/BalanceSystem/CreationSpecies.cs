using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;

namespace Code2015.BalanceSystem
{
    class CreationSpecies : IUpdatable
    {
      
        /// <summary>
        /// 生物所拥有的生存领地面积
        /// </summary>
        public float CreationsArea
        {
            get;
            set;
        }
       
        public void Update(GameTime time)
        { }
    }

}
