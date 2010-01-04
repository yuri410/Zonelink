using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Config;

namespace Code2015.BalanceSystem
{

    public class EnergyStatus : IConfigurable, IUpdatable
    {
        [SLGValueAttribute()]
        const float InitHPEnergy = 10000;
        [SLGValueAttribute()]
        const float InitLPEnergy = 10000;

        FastList<City> cities;

        /// <summary>
        ///  当前已储备的能量
        /// </summary>
        public float CurrentHPEnergy
        {
            get;
            private set;
        }
        public float CurrentLPEnergy
        {
            get;
            private set;
        }

        /// <summary>
        ///  自然环境中，剩余未开发不可再生资源可转化为的能量
        /// </summary>
        public float RemainingHPEnergy
        {
            get;
            private set;
        }
        public float RemainingLPEnergy
        {
            get;
            private set;
        }


        /// <summary>
        ///  申请获得低能资源
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>实际申请到的能源量</returns>
        public float ApplyLPEnergy(float amount)
        {
            if (CurrentLPEnergy >= amount)
            {
                CurrentLPEnergy -= amount;
                return amount;
            }
            float r = CurrentLPEnergy;
            CurrentLPEnergy = 0;
            return r;
        }
        /// <summary>
        ///  申请获得高能资源
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>实际申请到的能源量</returns>
        public float ApplyHPEnergy(float amount)
        {
            if (CurrentHPEnergy >= amount)
            {
                CurrentHPEnergy -= amount;
                return amount;
            }
            float r = CurrentHPEnergy;
            CurrentHPEnergy = 0;
            return r;
        }

        public EnergyStatus()
        {
            cities = new FastList<City>();
            this.CurrentHPEnergy = InitHPEnergy;
            this.CurrentLPEnergy = InitLPEnergy;
            //this.RemainingHPEnergy = CurrentHPEnergy;
            //this.RemainingLPEnergy = CurrentLPEnergy;
        }

       
        public void Update(GameTime time)
        {
            for (int i = 0; i < cities.Count; i++)
            { 
                
            }
        }


        #region IConfigurable 成员

        public void Parse(ConfigurationSection sect)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
