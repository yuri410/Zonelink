using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;

namespace Code2015.BalanceSystem
{

    public class OilField : NaturalResource
    {
        [SLGValueAttribute()]
        const float OILWeight = 100000;//油田含油量的初始值
        const float EMITCarbonSpeed = 10;//油田释放的C值
        const float TRANStoHPSpeed = 100;//油田固定转化为高能的速度

        public OilField()
        {
            this.InitSourceAmount = OILWeight;
            this.SourceConsumeSpeed = TRANStoHPSpeed;
            this.EmitCarbonSpeed = EMITCarbonSpeed;
        }

        public float EmitCarbonSpeed
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public float TransToHPAmount
        {
            get;
            set;
        }

        public float GetCarbonChange()
        {
            float change = this.CarbonChange;
            CarbonChange = 0;
            return change;
        }
        /// <summary>
        /// 用于设置油田固定转化为高能的速度
        /// </summary>
        public void SetTransSpeed()
        { 
            
        }
        public override void Update(GameTime time)
        {
            float hours=time.ElapsedGameTime.Hours;
            this.RemainingSourceAmount = this.InitSourceAmount;
            this.RemainingSourceAmount+=(this.SourceProduceSpeed-this.SourceConsumeSpeed)*hours;
            this.CarbonChange += this.EmitCarbonSpeed * this.RemainingSourceAmount*hours;//油田本身也要释放C
            this.TransToHPAmount = this.SourceConsumeSpeed * hours;
        }

    }
}
