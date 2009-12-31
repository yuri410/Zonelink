using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Collections;
using Apoc3D;


namespace Code2015.BalanceSystem
{
    public class Forest : NaturalResource
    {

        FastList<PlantSpecies> plants = new FastList<PlantSpecies>();
        FastList<AnimalSpecies> animals = new FastList<AnimalSpecies>();

        public Forest()
        {

        }

        public FastList<PlantSpecies> InitPlants()
        {
            plants.Add(new PlantSpecies("Trees"));
            plants.Add(new PlantSpecies("Grass"));
            plants[0].SetAmount(10000);
            plants[1].SetAmount(100000);
            return plants;
        }

        public FastList<AnimalSpecies> InitAnimals()
        {
            animals.Add(new AnimalSpecies("LargeAnimal"));
            animals.Add(new AnimalSpecies("LittleAnimal"));

            animals[0].SetAmount(1000);
            animals[1].SetAmount(5000);
            return animals;
        }


        public TypeofResource GetTypeofSource()
        {
            return TypeofResource.LPEnergy;
        }

        public override void Update(GameTime time)
        {
            
        }


    }
}
