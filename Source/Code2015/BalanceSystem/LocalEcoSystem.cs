using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;

namespace Code2015.BalanceSystem
{
    class LocalEcoSystem : IUpdatable, ICarbon
    {
        private float age, humidity, fertility, desertification;
        private bool balanced;
        public float Age
        {
            get { return age; }
            set { age = value; }
        }
        public float Humidity
        {
            get { return humidity; }
            set { humidity = value; }
        }
        public float Fertility
        {
            get { return fertility; }
            set { fertility = value; }
        }
        public float Desertification
        {
            get { return desertification; }
            set { desertification = value; }
        }
        public bool Balanced
        {
            get { return balanced; }
            set { balanced = true; }
         }
        AnimalSpecies[] animals = new AnimalSpecies[3];//分别有昆虫，小型动物和大型动物
        PlantSpecies[] plants = new PlantSpecies[3];//分别有草，灌木，树

        public void Update(GameTime time)
        { 
            
        }

        #region ICarbon 成员

        public CarbonGroup[] GetCarbonGroup()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
