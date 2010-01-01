using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;

namespace Code2015.BalanceSystem
{

   public  class EnergyStatus : IUpdatable
   {
       [SLGValueAttribute()]
       const float InitHPEnergy = 10000;
       const float InitLPEnergy = 10000;

        public float CurrentHPEnergy
        {
            get;
            set;
        }
        public float CurrentLPEnergy
        {
            get;
            set;
        }
        public float RenmainingHPEnergy
        {
            get;
            set;
        }
        public float RenmainingLPEnergy
        {
            get;
            set;
        }

     
        public void Update(GameTime time)
        { }

    }
}
