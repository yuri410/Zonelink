using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;

namespace Code2015.BalanceSystem
{
   
    public class NaturalResource : Simulateobject
    {
        /// <summary>
        /// 消耗速度
        /// </summary>
        public float ConsumeSpeed
        {
            get;
            set;
        }
        /// <summary>
        /// 生产速度
        /// </summary>
        public float ProduceSpeed
        {
            get;
            set;
        }
        /// <summary>
        /// 初始值
        /// </summary>
        public float InitAmount
        {
            get;
            set;
        }
        /// <summary>
        /// 剩余值
        /// </summary>
        public float RemainedAmount
        {
            get;
            set;
        }
        /// <summary>
        /// 低能数量
        /// </summary>
        public float LPAmount
        {
            get;
            set;
        }
        /// <summary>
        /// 高能数量
        /// </summary>
        public float HPAmount
        {
            get;
            set;
        }

       

        public virtual void SetProduceSpeed(float speed)
        {
            this.ProduceSpeed = speed;
        }
        public override void Update(GameTime time)
        {
            OilField oil = new OilField();
            this.HPAmount = oil.InitAmount;
        }
       

    }
}
