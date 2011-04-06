using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Code2015.EngineEx;

namespace Zonelink.World
{
    public enum NatureResourceType
    {
        Wood,
        Oil
    }

    class NatureResource
    {
        const float OTimesMaxAmount = 2;
        const float ORecoverBias = 1;


        const float FRecoverRate = 0.0015f;
        const float FRecoverBias = 1;
        const float FTimesMaxAmount = 2;

        public float Longitude { get; private set; }
        public float Latitude { get; private set; }
        public Vector3 Position 
        {
            get; 
            private set;
        }

        /// <summary>
        ///  获取自然资源的类型
        /// </summary>
        public NatureResourceType Type
        {
            get;
            private set;
        }


        public float MaxAmount
        {
            get;
            private set;
        }

        //当前资源数量
        public float CurrentAmount
        {
            get;
            protected set;
        }

        public object OutputTarget
        {
            get;
            set;
        }

        public bool IsLow
        {
            get { return CurrentAmount < 1000; }
        }


        public NatureResource()
            
        {
      
        }

        public void Reset(float current) 
        {
            CurrentAmount = current;

            switch (Type)
            {
                case NatureResourceType.Oil:
                    MaxAmount = CurrentAmount * OTimesMaxAmount;
                    break;
                case NatureResourceType.Wood:
                    MaxAmount = CurrentAmount * FTimesMaxAmount;
                    break;
            }
            
        }

        /// <summary>
        ///  开采一定数量的自然资源
        /// </summary>
        /// <param name="amount">申请值</param>
        /// <returns>实际得到的</returns>
        public float Exploit(float amount)
        {
            if (amount < CurrentAmount)
            {
                CurrentAmount -= amount;
                return amount;
            }

            float r = CurrentAmount;
            CurrentAmount = 0;
            return r;

        } 

        public void Update(GameTime time)
        {
            float dt = (float)time.ElapsedGameTime.TotalSeconds;

            switch (Type)
            {
                case NatureResourceType.Oil:

                    if (CurrentAmount < MaxAmount)
                    {
                        CurrentAmount +=  ORecoverBias * dt;
                    }
                    break;
                case NatureResourceType.Wood:
                    if (CurrentAmount < MaxAmount)
                    {
                        CurrentAmount += (CurrentAmount * FRecoverRate + FRecoverBias) * dt;
                    }
                    break;
            }
        }


        public void Parse(GameConfigurationSection sect)
        {
            string type = sect.GetString("Type", string.Empty).ToLowerInvariant();
             if (type == "wood")
             {
                 this.Type = NatureResourceType.Wood;
             }
             else if (type == "petro")
             {
                 this.Type = NatureResourceType.Oil;               
             }

             Longitude = sect.GetSingle("Longitude");
             Latitude = sect.GetSingle("Latitude");

             CurrentAmount = sect.GetSingle("Amount", 0);
             MaxAmount = CurrentAmount * FTimesMaxAmount;
        }
    }
}
