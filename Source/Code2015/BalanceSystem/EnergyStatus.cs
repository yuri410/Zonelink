using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Config;
using Apoc3D.Collections;
using Apoc3D;

namespace Code2015.BalanceSystem
{
    public class EnergyStatus : IConfigurable, IUpdatable
    {
        [SLGValue()]
        const float InitHPEnergy = 1000000;
        [SLGValue()]
        const float InitLPEnergy = 1000000;

        [SLGValue()]
        const float HPLowThreshold = 300;
        [SLGValue()]
        const float LPLowThreshold = 300;
        [SLGValue()]
        const float FoodLowThreshold = 300;


        FastList<City> cities;

        public SimulateRegion Region
        {
            get;
            private set;
        }
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
        public float CurrentFood
        {
            get;
            private set;
        }

        /// <summary>
        ///  获取一个布尔值，表示当前已储备的高能资源量是否过低
        /// </summary>
        public bool IsHPLow
        {
            get { return CurrentHPEnergy < HPLowThreshold; }
        }
        public bool IsLPLow
        {
            get { return CurrentLPEnergy < LPLowThreshold; }
        }
        public bool IsFoodLow
        {
            get { return CurrentFood < FoodLowThreshold; }
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

        /// <summary>
        ///  申请获得食物
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>实际申请到的食物量</returns>
        public float ApplyFood(float amount)
        {
            if (CurrentFood >= amount)
            {
                CurrentFood -= amount;
                return amount;
            }
            float r = CurrentFood;
            CurrentFood = 0;
            return r;
        }


        public void CommitLPEnergy(float amount)
        {
            CurrentLPEnergy += amount;
        }
        public void CommitHPEnergy(float amount)
        {
            CurrentHPEnergy += amount;
        }
        public void CommitFood(float amount)
        {
            CurrentFood += amount;
        }

        public EnergyStatus(SimulateRegion region)
        {
            cities = new FastList<City>();
            this.CurrentHPEnergy = InitHPEnergy;
            this.CurrentLPEnergy = InitLPEnergy;
            this.CurrentFood = 1000000;
            //this.RemainingHPEnergy = CurrentHPEnergy;
            //this.RemainingLPEnergy = CurrentLPEnergy;
            Region = region;
        }


        public void Update(GameTime time)
        {
            // TODO：统计
        }


        #region IConfigurable 成员

        public void Parse(ConfigurationSection sect)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
