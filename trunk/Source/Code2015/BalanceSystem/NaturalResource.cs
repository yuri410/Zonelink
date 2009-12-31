using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;

namespace Code2015.BalanceSystem
{
    public enum TypeofResource { LPEnergy, HPEnergy };
    public class NaturalResource : Simulateobject
    {
        /// <summary>
        /// 生产速度，黑色不可再生生产速度为0
        /// </summary>
        public float ProductSpeed
        {
            get
            { return ProductSpeed; }

            set
            {
                if (NatureSourceType == TypeofResource.HPEnergy)
                    ProductSpeed = 0;
            }
        }

        public virtual float SetProductSpeed(float speed)
        {
            return this.ProductSpeed = speed;
        }
        /// <summary>
        /// 消耗速度
        /// </summary>
        public float ConsumeSpeed
        {
            get;
            set;
        }
        public virtual float SetConsumeSpeed(float speed)
        {
            return this.SourceAmount = speed;
        }
        /// <summary>
        /// 资源的数量
        /// </summary>
        public float SourceAmount
        {
            get;
            set;
        }

        public void SetSourceAmount(float amount)
        {
            this.SourceAmount = amount;
        }
        /// <summary>
        /// 类型
        /// </summary>
        public TypeofResource NatureSourceType
        {
            get;
            private set;
        }

        public override void Update(GameTime time)
        {
            
        }
       

    }
}
