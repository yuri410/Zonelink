using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;

namespace Code2015.BalanceSystem
{
    /// <summary>
    ///  表示自然资源的类型
    /// </summary>
    public enum NaturalResourceType
    {
        None,
        Wood,
        Food,
        Oil
    }


    public abstract class NaturalResource : SimulateObject
    {
       
        protected NaturalResource(SimulateRegion region, NaturalResourceType type)
            : base(region)
        {
          

            Type = type;
        }
        
        /// <summary>
        ///  获取自然资源的类型
        /// </summary>
        public NaturalResourceType Type
        {
            get;
            private set;
        }

        /// <summary>
        ///  获取当前自然资源
        /// </summary>
        public float CurrentAmount
        {
            get;
            protected set;
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


        #region Unk
        /// <summary>
        /// 资源消耗速度
        /// </summary>
        public float SourceConsumeSpeed
        {
            get;
            set;
        }
        /// <summary>
        /// 资源再生产速度
        /// </summary>
        public float SourceProduceSpeed
        {
            get;
            set;
        }
        /// <summary>
        /// 资源初始值
        /// </summary>
        public float InitSourceAmount
        {
            get;
            set;
        }


        /// <summary>
        /// 资源剩余值
        /// </summary>
        public float RemainingSourceAmount
        {
            get;
            set;
        }


        /// <summary>
        /// 留以作为玩家花费金钱或时间来使再生产速度加速用
        /// </summary>
        /// <param name="speed"></param>
        public virtual void GetProduceSpeed(float speed)
        {
            this.SourceProduceSpeed = speed;
        }

        #endregion

        public override void Update(GameTime time)
        {
            
        }
       

    }
}
