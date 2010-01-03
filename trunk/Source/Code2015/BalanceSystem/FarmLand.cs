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
        const float INITFoodAmount = 100000;
        const float ABSORBCarbonSpeed = 1000;
       
        FastList<PlantSpecies> FoodPlants=new FastList<PlantSpecies>();
        public FarmLand()
        {
            FoodPlants = new FastList<PlantSpecies>();
            this.InitSourceAmount = INITFoodAmount;
            this.AbsorbCarbonSpeed = ABSORBCarbonSpeed;
        }

        public float AbsorbCarbonSpeed
        {
            get;
            set;
        }
        //public float GetCarbonChange()
        //{
        //    float change = this.CarbonChange;
        //    CarbonChange = 0;
        //    return change;
        //}

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
            this.SourceConsumeSpeed = totalspeed;
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

        public PlantSpecies this[int i]
        {
            get { return FoodPlants[i]; }
        }
        public override void Update(GameTime time)
        {
            float hours = time.ElapsedGameTime.Hours;
            GetGradeofSoil();
            this.InitSourceAmount = INITFoodAmount;
            this.RemainingAmount += (SourceConsumeSpeed - ProduceFoodSpeed) * time.ElapsedGameTime.Days;
            this.CarbonChange += -(this.InitSourceAmount * this.AbsorbCarbonSpeed);
        }


    }
}
