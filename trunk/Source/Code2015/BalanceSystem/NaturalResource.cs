using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;

namespace Code2015.BalanceSystem
{
    ///铁和铜
    enum SourceType { Iron,Copper}
    class NaturalResource : Simulateobject
    {
        /// <summary>
        /// 自然界总的资源,包括铁，铝等其他矿产等
        /// </summary>
        public float TotalAmount = 10000;
       
        public float ConsumeSpeed
        {
            get
            {
                switch (NatSourceType)
                { 
                    case SourceType.Iron:
                        return 10;
                    case SourceType.Copper:
                        return 4;
                       
                }
                return 0;
               
            }
        }
        /// <summary>
        /// 资源类型
        /// </summary>
        public SourceType NatSourceType
        {
            get;
           private  set;
        }
       


     
    }
}
