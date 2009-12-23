using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Collections;

namespace Code2015.BalanceSystem
{
    /// <summary>
    ///  表示动物物种
    /// </summary>
    class AnimalSpecies : CreationSpecies
    {
        /// <summary>
        ///  动物尸体、粪便使土壤肥沃的速度，与动物的数目成正比
        /// </summary>
        public float FertilisingSpeed
        {
            get;
            set;
        }


        public string Name
        {
            get;
            set;
        }
        public AnimalSpecies(string name)
        {
            Name = name;
        }



        //得到使土壤肥沃的速度
        public void SetFertilisingSpeed(float fertiliseSpeed)
        {
            this.FertilisingSpeed = fertiliseSpeed;
        }

        //得到各种动物产生CO2的数量
        public float ProduceGgas(AnimalSpecies animal, float speed)
        {
            return animal.Strength * speed;
        }

    }
}
