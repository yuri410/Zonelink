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

        //FastList<PlantSpecies> foodPlants;
        public FarmLand(SimulationRegion region)
            : base(region, NaturalResourceType.Food)
        {
            //foodPlants = new FastList<PlantSpecies>();

        }
      
        public float AbsorbCarbonSpeed
        {
            get;
            set;
        }     
     


        //public bool Add(PlantSpecies foodplant)
        //{
        //    foodPlants.Add(foodplant);

        //    return true;
        //}

        //public void Remove(PlantSpecies foodplant)
        //{
        //    foodPlants.Remove(foodplant);
        //}

        
        public override void Update(GameTime time)
        {
            float hours = (float)time.ElapsedGameTime.Hours;       
        }


    }
}
