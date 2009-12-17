using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Code2015.BalanceSystem
{
    /// <summary>
    ///  表示动物物种
    /// </summary>
    class AnimalSpecies : CreationSpecies
    {
        /// <summary>
        ///  动物使土壤肥沃的速度
        /// </summary>
        public float FertilisingSpeed
        {
            get;
            set;
        }
        /// <summary>
        /// 动物的出生率和死亡率，环境条件好时出生率略大于死亡率，但是这个在数量上
        /// 只是改变很小，如果环境恶化时，死亡率会大于出生率
        /// </summary>
        public float BreedSpeed
        {
            get;
            set;
        }
        public float DeadSpeed
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

        public void GetAniSpeed(PlantSpecies plant, AnimalSpecies animal, LocalEcoSystem local)
        {
            if (plant.CreationsArea > local.LocalSysArea / 3)
            {
                animal.FertilisingSpeed = 100;
                animal.BreedSpeed = 0.01f;
                animal.DeadSpeed = 0;
            }
            else if (plant.CreationsArea < local.LocalSysArea / 5)
            {
                animal.FertilisingSpeed = 50;
                animal.BreedSpeed = 0;
                animal.DeadSpeed = 0.1f;
            }
            else
            {
                animal.FertilisingSpeed = 80;
                animal.BreedSpeed = animal.DeadSpeed = 0;
            }
        }
    }
}
