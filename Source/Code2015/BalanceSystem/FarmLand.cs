using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;

namespace Code2015.BalanceSystem
{
 
    public class FarmLand : NaturalResource
    {
        [SLGValueAttribute()]
        const float INITFoodAmount = 100000;
        [SLGValue]
        const float ABSORBCarbonSpeed = 1000;
        [SLGValue]
        const float SOURCEProduceSpeed = 500;

        FastList<PlantSpecies> FoodPlants;
        public FarmLand(SimulateRegion region)
            : base(region, NaturalResourceType.Food)
        {
            FoodPlants = new FastList<PlantSpecies>();
           
        }
      
        public float AbsorbCarbonSpeed
        {
            get;
            set;
        }     
     


        public bool Add(PlantSpecies foodplant)
        {
            FoodPlants.Add(foodplant);

            return true;
        }

        public void Remove(PlantSpecies foodplant)
        {
            FoodPlants.Remove(foodplant);
        }

        
        public override void Update(GameTime time)
        {
           
        }


    }
}
