using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;

namespace Code2015.BalanceSystem
{

    public class EnergyStatus : IUpdatable
    {
        [SLGValueAttribute()]
        const float InitHPEnergy = 10000;
        const float InitLPEnergy = 10000;

        FastList<City> cities;
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
        public float RemainingHPEnergy
        {
            get;
            set;
        }
        public float RemainingLPEnergy
        {
            get;
            set;
        }

        public EnergyStatus()
        {
            cities = new FastList<City>();
            this.CurrentHPEnergy = InitHPEnergy;
            this.CurrentLPEnergy = InitLPEnergy;
            this.RemainingHPEnergy = CurrentHPEnergy;
            this.RemainingLPEnergy = CurrentLPEnergy;
        }

       
        public void Update(GameTime time)
        {
            for (int i = 0; i < cities.Count; i++)
            { 
                
            }
        }

    }
}
