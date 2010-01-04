using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;

namespace Code2015.BalanceSystem
{
    public class SimulateObject : IUpdatable
    {
        /// <summary>
        /// 经度
        /// </summary>
        public float Long
        {
            get;
            private set;
        }
        /// <summary>
        /// 纬度
        /// </summary>
        public float Lat
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


        public SimulateObject()
        {

        }


        public virtual void Update(GameTime time)
        {
            float hours = time.ElapsedGameTime.Hours;
            this.CarbonChange = this.CarbonProduceSpeed * hours;
        }
    }
}
