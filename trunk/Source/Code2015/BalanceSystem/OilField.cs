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


        public OilField(SimulateRegion region)
            : base(region, NaturalResourceType.Oil)
        {
            this.InitSourceAmount = OILWeight;
            this.EmitCarbonSpeed = EMITCarbonSpeed;
        }

        public float EmitCarbonSpeed
        {
            get;
            set;
        }
     
        public float TransToHPAmount
        {
            get;
            set;
        }

        public void GetOilConsumeSpeed()
        {
            float consumespeed = 0;
            for (int i = 0; i < this.CityCount;i++)
            {
                consumespeed += this[i].GetPluginHPProductionSpeed();
            }
            this.ConsumeHPSpeed = consumespeed * 1.5f;//资源消耗的速度是能源生产速度的1.5倍
        }
        
        public override void Update(GameTime time)
        {
         
            float hours=(float)time.ElapsedGameTime.Hours;
            this.RemainingSourceAmount = this.InitSourceAmount;
            this.RemainingSourceAmount+=(this.SourceProduceSpeed-this.SourceConsumeSpeed)*hours;
            this.CarbonChange += this.EmitCarbonSpeed * this.RemainingSourceAmount*hours;//油田本身也要释放C
          
        }

    }
}
