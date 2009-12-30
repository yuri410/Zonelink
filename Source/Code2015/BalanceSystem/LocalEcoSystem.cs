using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;

namespace Code2015.BalanceSystem
{
    public class LocalEcoSystem : IUpdatable
    {

        /// <summary>
        /// 生态系统存在时间
        /// </summary>
        public float Age
        {
            get;
            set;
        }
        /// <summary>
        /// 局部生态系统湿度调整系数
        /// </summary>
        public float Humidity
        {
            get;
            set;
        }
        /// <summary>
        /// 动物使土壤肥沃程度
        /// </summary>
        public float Fertility
        {
            get;
            set;
        }
        /// <summary>
        /// 沙漠化调整系数
        /// </summary>
        public float Desertification
        {
            get;
            set;
        }
        /// <summary>
        ///局部生态系统是否平衡
        /// </summary>
        public bool Balanced
        {
            get;
            set;
        }

        public float CarbonWeght
        {
            get;
            set;
        }

        /// <summary>
        ///分别有树木，灌木，草
        /// </summary>
        FastList<PlantSpecies> plants = new FastList<PlantSpecies>();
        FastList<AnimalSpecies> animals = new FastList<AnimalSpecies>();
        /// <summary>
        /// 初始化植物
        /// </summary>

        public LocalEcoSystem()
        {
            InitAnimals();
            InitPlants();
        }
        public FastList<PlantSpecies> InitPlants()
        {
            plants.Add(new PlantSpecies("Trees"));
            plants.Add(new PlantSpecies("Bush"));
            plants.Add(new PlantSpecies("Grass"));

            plants[0].Amount = 10000;
            plants[1].Amount = 30000;
            plants[2].Amount = 50000;

            plants[0].SetCTransSpeed(1000);
            plants[1].SetCTransSpeed(400);
            plants[2].SetCTransSpeed(100);

            plants[0].SetPlantAdjust(plants[0], 1, 1);
            plants[1].SetPlantAdjust(plants[1], 0.5f, 0.5f);
            plants[2].SetPlantAdjust(plants[2], 0.3f, 0.3f);

            return plants;
        }
        public FastList<AnimalSpecies> InitAnimals()
        {
            animals.Add(new AnimalSpecies("LargeAnimal"));
            animals.Add(new AnimalSpecies("LittleAnimal"));
            animals.Add(new AnimalSpecies("Insect"));

            animals[0].Amount = 5000;
            animals[1].Amount = 10000;
            animals[2].Amount = 100000;

            animals[0].SetFertilisingSpeed(100);
            animals[1].SetFertilisingSpeed(80);
            animals[2].SetFertilisingSpeed(10);

            animals[0].SetMakeCarbonSpeed(500);
            animals[1].SetMakeCarbonSpeed(200);
            animals[2].SetMakeCarbonSpeed(50);
            return animals;
        }
        /// <summary>
        /// 得到该区域的湿度调节和沙漠化调节系数
        /// </summary>
        public void GetAdjustPlant()
        {
           
            float totalamount = 0;
            float totalhumidity = 0;           
            float totaldesertify = 0;
            for (int i = 0; i < plants.Count; i++)
            {
                totalamount += plants[i].Amount;
                totalhumidity += plants[i].Amount * plants[i].HumidityAdjust;
                totaldesertify += plants[i].Amount * plants[i].DesertificationAdjust;                
            }
            this.Humidity = totalhumidity / totalamount;
            this.Desertification = totaldesertify / totalamount;
        }

        /// <summary>
        /// 得到该区域的使土壤肥沃的速度
        /// </summary>
        /// <returns></returns>
        public float GetFerlisingSpeed()
        {
            float speed = 0;
            float total = 0;
            float totalamount = 0;
            for (int i = 0; i < animals.Count; i++)
            {
                total += animals[i].Amount * animals[i].FertilisingSpeed;
                totalamount += animals[i].Amount;
                speed = total / totalamount;
            }

            return this.Fertility = speed;
        }
        /// <summary>
        /// 得到本地生态系统的C增量
        /// </summary>
        /// <returns></returns>
        public float GetLSCarbon()
        {
            float carbonweight = 0;
            float PlantIN=0;
            float AnimalOUT=0;

            for (int i = 0; i < plants.Count; i++)
            {
                PlantIN += plants[i].CarbonTransformSpeed * plants[i].Amount;
            }

            for (int i = 0; i < animals.Count; i++)
            {
                AnimalOUT += animals[i].MakeCarbonSpeed * animals[i].Amount;
            }
            carbonweight = PlantIN - AnimalOUT;
            return CarbonWeght = carbonweight;

        }
        public void Update(GameTime time)
        {

        }




    }
}
