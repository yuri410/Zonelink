using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Config;

namespace Code2015.BalanceSystem
{
    public class SimulateObject :IConfigurable, IUpdatable
    {
        /// <summary>
        /// 经度
        /// </summary>
        public float Longitude
        {
            get;
            private set;
        }
        /// <summary>
        /// 纬度
        /// </summary>
        public float Latitude
        {
            get;
            private set;
        }

        /// <summary>
        /// CO2的改变量
        /// </summary>
        protected float CarbonChange
        {
            get;
            set;
        }
        public float CarbonProduceSpeed
        {
            get;
            protected set;
        }
        public float GetCarbonChange()
        {
            float r = CarbonChange;
            CarbonChange = 0;
            return r;
        }

        /// <summary>
        /// 高能
        /// </summary>
        public float InitHPAmount
        {
            get;
            set;
        }
        public float RemainingHPAmount
        {
            get;
            set;
        }
        public float ConsumeHPSpeed
        {
            get;
            set;
        }


        /// <summary>
        /// 低能
        /// </summary>
        public float InitLPAmount
        {
            get;
            set;
        }
        public float RemainingAmount
        {
            get;
            set;
        }
        public float ConsumeLPSpeed
        {
            get;
            set;
        }
             
        public SimulateRegion Region
        {
            get;
            private set;
        }

        public SimulateObject(SimulateRegion region)
        {
            Region = region;
        }
       
        public virtual void Update(GameTime time)
        {
            float hours = (float)time.ElapsedGameTime.Hours;
            this.CarbonChange = this.CarbonProduceSpeed * hours;
        }
        #region IConfigurable 成员

    
        public virtual void Parse(ConfigurationSection sect)
        {
            Longitude = sect.GetSingle("Longitude");
            Latitude = sect.GetSingle("Latitude");
        }
        #endregion
    }
}
