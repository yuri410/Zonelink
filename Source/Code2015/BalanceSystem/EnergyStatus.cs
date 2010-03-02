using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Config;

namespace Code2015.BalanceSystem
{
    public class EnergyStatus : IConfigurable, IUpdatable
    {
        [SLGValue()]
        const float InitHPEnergy = 1000000;
        [SLGValue()]
        const float InitLPEnergy = 1000000;

        [SLGValue()]
        public const float HPLowThreshold = 300;
        [SLGValue()]
        public const float LPLowThreshold = 300;
        [SLGValue()]
        public const float FoodLowThreshold = 300;


        public SimulationRegion Region
        {
            get;
            private set;
        }
        /// <summary>
        ///  当前已储备的能量
        /// </summary>
        public float CurrentHR
        {
            get;
            private set;
        }
        /// <summary>
        ///  当前已储备的能源
        /// </summary>
        public float CurrentLR
        {
            get;
            private set;
        }
        /// <summary>
        ///  当前已储备的食物
        /// </summary>
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
            get { return CurrentHR < HPLowThreshold; }
        }
        public bool IsLPLow
        {
            get { return CurrentLR < LPLowThreshold; }
        }
        public bool IsFoodLow
        {
            get { return CurrentFood < FoodLowThreshold; }
        }

        /// <summary>
        ///  自然环境中，剩余未开发不可再生资源可转化为的能量
        /// </summary>
        public float RemainingHR
        {
            get;
            private set;
        }
        public float RemainingLR
        {
            get;
            private set;
        }

        public EnergyStatus(SimulationRegion region)
        {
            this.CurrentHR = InitHPEnergy;
            this.CurrentLR = InitLPEnergy;
            this.CurrentFood = 10000;
            //this.RemainingHPEnergy = CurrentHPEnergy;
            //this.RemainingLPEnergy = CurrentLPEnergy;
            Region = region;
        }


        public void Update(GameTime time)
        {
            // 统计资源量

            float hr = 0;
            float lr = 0;
            float food = 0;
            SimulationRegion region  = Region;
            // 已经采集的资源
            for (int i = 0; i < region.CityCount; i++)
            {
                City city = region.GetCity (i);
                hr += city.LocalHR.Current;
                lr += city.LocalLR.Current;
                food += city.LocalFood.Current;
            }
            CurrentFood = food;
            CurrentHR = hr;
            CurrentLR = lr;


            hr = 0;
            lr = 0;
            food = 0;
            // 自然中未开发的
            for (int i = 0; i < region.ResourceCount; i++)
            {
                NaturalResource res = region.GetResource(i);
                if (res.Type == NaturalResourceType.Petro) 
                {
                    hr += res.CurrentAmount;
                }
                else if (res.Type == NaturalResourceType.Wood) 
                {
                    lr += res.CurrentAmount;
                }
            }
            RemainingHR = hr;
            RemainingLR = lr;
           
        }


        #region IConfigurable 成员

        public void Parse(ConfigurationSection sect)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
