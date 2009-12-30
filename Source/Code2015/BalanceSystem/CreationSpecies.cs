using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;

namespace Code2015.BalanceSystem
{
    public abstract class CreationSpecies : IUpdatable
    {

        /// <summary>
        /// 代表生物的数目
        /// </summary>
        public float Amount
        {
            get;
            set;
        }
        public float SetAmount(float amount)
        {
            return this.Amount = amount;
        }
        public float CarbonWeight
        {
            get;
            set;
        }

        public abstract float GetCarbonWeght();

        public void Update(GameTime time)
        { }
    }
}
