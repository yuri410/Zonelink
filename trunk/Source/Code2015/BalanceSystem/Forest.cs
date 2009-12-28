using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Code2015.BalanceSystem
{
    class Forest:Simulateobject
    {
        
        PlantSpecies Tree = new PlantSpecies("Tree");
        PlantSpecies Grass = new PlantSpecies("Grass");
        AnimalSpecies LargeAnimal = new AnimalSpecies("LargeAnimal");
        AnimalSpecies LittleAnimal = new AnimalSpecies("LittleAnimal");
        public override float GetCarbonWeight()
        {
            float OutAnimal = LargeAnimal.Amount * 300+ LittleAnimal.Amount * 100;
            float InPlant = Tree.Amount * 500 + Grass.Amount * 50;
            return CarbonWeight = InPlant - OutAnimal;
        }


    }
}
