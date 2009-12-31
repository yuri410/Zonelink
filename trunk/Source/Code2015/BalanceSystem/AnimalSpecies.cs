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
    public class AnimalSpecies : CreationSpecies
    {

        public string Name
        {
            get;
            set;
        }

        public AnimalSpecies(string name)
        {
            Name = name;
        }
       new  public float CarbonWeight
        {
            get { return MakeCarbonSpeed * Amount; }
            set { CarbonWeight = value; }
        }
        /// <summary>
        /// 制造C的速度，不同动物不一样
        /// </summary>
        public float MakeCarbonSpeed
        {
            get;
            set;
        }

        public float SetMakeCarbonSpeed(float speed)
        {
            return this.MakeCarbonSpeed = speed;
        }
        /// <summary>
        ///  动物尸体、粪便使土壤肥沃的速度
        /// </summary>     
        public float FertilisingSpeed
        {
            get;
            set;
        }
        public float SetFertilisingSpeed(float fertilisingspeed)
        {
            return this.FertilisingSpeed = fertilisingspeed;
        }

        public override float GetCarbonWeght()
        {
            throw new NotImplementedException();
        }


    }
}
