using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;

namespace Code2015.BalanceSystem
{
    class NaturalResource : IUpdatable
    {
       
    
        /// <summary>
        /// 自然的资源生产速度
        /// </summary>
        public float ProductSpeed
        {
            get;
            set;
        }
       
        /// <summary>
        /// 自然界总的资源
        /// </summary>
        public float TotalAmount
        {
            get;
            set;
        }
        /// <summary>
        /// 生产类型
        /// </summary>
        public string ProductType
        {
            get;
            set;
        }

        public NaturalResource()
        { }

        public void Update(GameTime time)
        { }
    }
}
