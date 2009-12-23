using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;

namespace Code2015.BalanceSystem
{
    class LocalEcoSystem : IUpdatable
    {
        private float age, humidity, fertility, desertification;
        
        private bool balanced;
        /// <summary>
        /// 局部生态系统的存在时间
        /// </summary>
        public float Age
        {
            get { return age; }
            set { age = value; }
        }
        /// <summary>
        /// 局部生态系统湿度调整系数
        /// </summary>
        public float Humidity
        {
            get { return humidity; }
            set { humidity = value; }
        }
        /// <summary>
        /// 动物使土壤肥沃程度
        /// </summary>
        public float Fertility
        {
            get { return fertility; }
            set { fertility = value; }
        }
        /// <summary>
        /// 沙漠化调整系数
        /// </summary>
        public float Desertification
        {
            get { return desertification; }
            set { desertification = value; }
        }
        /// <summary>
        ///局部生态系统是否平衡
        /// </summary>
        public bool Balanced
        {
            get { return balanced; }
            set { balanced = value; }
         }

        /// <summary>
        ///分别有树木，灌木，草
        /// </summary>
        FastList<PlantSpecies> plants = new FastList<PlantSpecies>();

        //得到所有植物的数目
        public float GetPlantsAmount(FastList<PlantSpecies> plants)
        {
            float plantsAmount = 0.0f;
            for (int i = 0; i < plants.Count; i++)
            {
                plantsAmount += ((PlantSpecies)plants[i]).Strength;
            }
            return plantsAmount;
        }
      
        public void SetFactorPlant()
        {
            plants.Add(new PlantSpecies("Trees"));
            plants.Add(new PlantSpecies("Bushes"));
            plants.Add(new PlantSpecies("Grass"));
            plants[0].SetPlantFactor(300, 1.0f, 1.0f);
            plants[1].SetPlantFactor(100, 0.8f, 0.8f);
            plants[2].SetPlantFactor(70, 0.6f, 0.7f);
            float plantsAmount = 0.0f;
            plantsAmount = GetPlantsAmount(plants);
            float humi=0, desert=0;
            for (int i = 0; i < plants.Count; i++)
            {
                humi += plants[i].HumidityAdjust * plants[i].Strength;
                desert += plants[i].DesertificationAdjust * plants[i].Strength;
            }
            this.Humidity = humi;
            this.Desertification = desert;
        }


        /// <summary>
        /// 动物分别有昆虫，小型动物，大型动物
        /// </summary>
        FastList<AnimalSpecies> animals = new FastList<AnimalSpecies>();

        //得到动物的总数目
        public float GetAnimalAmount(FastList<AnimalSpecies> animals)
        {
            float animalAmount = 0.0f;
            for (int i = 0; i < animals.Count; i++)
            {
                animalAmount += ((AnimalSpecies)animals[i]).Strength;
            }
            return animalAmount;
        }
        public void SetFactorAnimal()
        {
            animals.Add(new AnimalSpecies("LargeAnimal"));
            animals.Add(new AnimalSpecies("LittleAnimal"));
            animals.Add(new AnimalSpecies("Insect"));
            float fertiliseSpeed = 0;
            animals[0].SetFertilisingSpeed(100);
            animals[1].SetFertilisingSpeed(40);
            animals[2].SetFertilisingSpeed(1);
            for (int i = 0; i < animals.Count; i++)
            {
                fertiliseSpeed += animals[i].Strength * animals[i].FertilisingSpeed;
            }

            this.Fertility = fertiliseSpeed;    
        }

        public float GetCarbonWeight()
        {
            float plantIn = 0, animalOut = 0;
            for (int i = 0; i < plants.Count; i++)
            {
                plantIn += plants[i].Strength * plants[i].CarbonTransformSpeed;
            }
        
        }
        public void Update(GameTime time)
        { 
            
        }

        public float CarbonChange
        {
            get;
            set;
        }

      
    }
}
