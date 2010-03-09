using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Config;

namespace Code2015.BalanceSystem
{
    public enum FarmHouseSize 
    {
        Small,
        Medium,
        Large
    }
    public class FarmLand : NaturalResource
    {
        //[SLGValueAttribute()]
        //const float INITFoodAmount = 100000;
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
            private set;
        }

        public int BaseHeight
        {
            get;
            private set;
        }

        public int BaseWidth
        {
            get;
            private set;
        }

        public FarmHouseSize HouseSize
        {
            get;
            private set;
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
        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);

            string fund = sect["BasePos"];

            string[] v = fund.Split('X');

            if (v.Length == 2)
            {
                BaseHeight = int.Parse(v[0]);
                BaseWidth = int.Parse(v[1]);
            }
            else 
            {
                BaseHeight = 1;
                BaseWidth = 1;
            }

            HouseSize = (FarmHouseSize)Enum.Parse(typeof(FarmHouseSize), sect.GetString("Size", string.Empty));
        }
        
        public override void Update(GameTime time)
        {
            float hours = (float)time.ElapsedGameTime.Hours;       
        }


    }
}
