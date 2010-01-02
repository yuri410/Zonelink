using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Collections;
using Apoc3D;

namespace Code2015.BalanceSystem
{

    public enum Grade { Bad, Medium, Fine };
   
    public class FarmLand : NaturalResource
    {
        [SLGValueAttribute()]
        const float InitFoodAmount = 100000;
        const float AbsorbCarbonSpeed = 1000;

        FastList<PlantSpecies> FoodPlants;
        public FarmLand()
        {
            FoodPlants = new FastList<PlantSpecies>();
            this.InitAmount = InitFoodAmount;
            this.AbsorbCspeed = AbsorbCarbonSpeed;
        }

        public FarmLand(string name)
        {
            FoodPlants = new FastList<PlantSpecies>();
            this.Name = name;
            this.InitAmount = InitFoodAmount ;
            this.AbsorbCspeed = AbsorbCarbonSpeed;
        }

        
        public float AbsorbCspeed
        {
            get;
            set;
        }
        /// <summary>
        /// 土壤等级
        /// </summary>
        public Grade GradeOfSoil
        {
            get;
            private set;
        }
        /// <summary>
        /// 每天产量
        /// </summary>
        public float ProduceFoodSpeed
        {
            get
            {
                switch (GradeOfSoil)
                {
                    case Grade.Fine:
                        return 1000;
                    case Grade.Medium:
                        return 800;
                    case Grade.Bad:
                        return 400;
                }
                return 0;
            }
        }
        /// <summary>
        /// 得以得到用户选择的农作物
        /// </summary>
        /// <returns></returns>
        public PlantSpecies GetUserCplant()
        { 
            PlantSpecies plant=new PlantSpecies();
            return plant;
        }
        /// <summary>
        /// 得到土壤等级
        /// </summary>
        public void GetGradeofSoil()
        {
            PlantSpecies foodplant = GetUserCplant();
            if (Add(foodplant))
            {
                if (FoodPlants.Count == 0)
                {
                    this.GradeOfSoil = Grade.Fine;
                }
                else if (FoodPlants.Count > 0 && (GradeOfSoil > 0))
                {
                    int i = FoodPlants.Count;
                    if (FoodPlants[i - 1].Name == foodplant.Name)
                    {
                        int grade = (int)GradeOfSoil;
                        GradeOfSoil = (Grade)(grade - 1);
                    }

                }
            }
        }
        /// <summary>
        /// 得到所有城市消耗的粮食速度，包括工厂消耗和城市本身消耗
        /// </summary>
        /// <param name="cyties"></param>
        public void GetConsumeSpeed(FastList<City> cyties)
        {
            float totalspeed = 0;
            for (int i = 0; i < cyties.Count; i++)
            {
                totalspeed += cyties[i].FoodCostSpeed;
            }
            this.ConsumeSpeed = totalspeed;
        }

        public bool Add(PlantSpecies foodplant)
        {
            FoodPlants.Add(foodplant);
            GetGradeofSoil();
            return true;
        }

        public void Remove(PlantSpecies foodplant)
        {
            FoodPlants.Remove(foodplant);
        }

        public override void Update(GameTime time)
        {
            GetGradeofSoil();
            this.InitAmount = InitFoodAmount;
            this.InitAmount += (ConsumeSpeed - ProduceFoodSpeed) * time.ElapsedGameTime.Days;
            this.CarbonWeight += -(this.InitAmount * this.AbsorbCspeed);
        }


    }
}
