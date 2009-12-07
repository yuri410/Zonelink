using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Code2015.balanceSystem
{
    class EnergyStatus:IUpdatable
    {
        private float currentHPEnergy, currentLPEnergy, remainingLPEnergy, renmainingHPEnergy;
        public float CurrentHPEnergy
        {
            get { return currentHPEnergy; }
            set { currentHPEnergy = value; }
        }
        public float CurrentLPEnergy
        {
            get { return currentLPEnergy; }
            set { currentLPEnergy = value; }
        }
        public float RenmainingHPEnergy
        {
            get { return renmainingHPEnergy; }
            set { renmainingHPEnergy = value; }
        }
        public float RenmainingLPEnergy
        {
            get { return remainingLPEnergy; }
            set { remainingLPEnergy = value; }
        }

        EnergyProducer[] energyProduers;
        public void Update(float time)
        { }
        
    }
}
