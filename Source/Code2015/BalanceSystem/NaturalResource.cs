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

        FastList<NaturalResource> naturalresource = new FastList<NaturalResource>();
        public NaturalResource()
        {
            naturalresource.Add(new FarmLand());
            naturalresource.Add(new OilField());
            naturalresource.Add(new Forest());
        }
        public string Name
        {
            get;
            set;

        }
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

       
        /// <summary>
        /// 留以作为玩家花费金钱或时间来使生产速度加速用
        /// </summary>
        /// <param name="speed"></param>
        public virtual void SetProduceSpeed(float speed)
        {
            this.ProduceSpeed = speed;
        }

        public override void Update(GameTime time)
        {
            ((FarmLand)naturalresource[0]).Update(time);
            ((OilField)naturalresource[1]).Update(time);
            ((Forest)naturalresource[2]).Update(time);
            float totalcarbonweight = 0;
            for (int i = 0; i < naturalresource.Count; i++)
            {
                totalcarbonweight += naturalresource[i].CarbonWeight;   
            }
            this.CarbonWeight = totalcarbonweight;
        }
       

    }
}
