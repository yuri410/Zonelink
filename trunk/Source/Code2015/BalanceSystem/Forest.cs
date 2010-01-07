using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Config;

namespace Code2015.BalanceSystem
{

    public class Forest : NaturalResource
    {
        [SLGValueAttribute()]
        const float INITForestAmount = 100000;
        [SLGValueAttribute()]
        const float ABSORBCarbonSpeed = 1000;
       
        public float AbsorbCarbonSpeed
        {
            get;
            set;
        }

        public Forest(SimulateRegion region)
            : base(region, NaturalResourceType.Wood)
        {
           
        }

      

        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);
            
        }


        public override void Update(GameTime time)
        {
            float hours = (float)time.ElapsedGameTime.Hours;
            this.RemainingSourceAmount = this.InitSourceAmount;//开始时初始值等于剩余值。
            this.RemainingSourceAmount += (this.SourceProduceSpeed - this.SourceConsumeSpeed) * hours;
            this.CarbonProduceSpeed += -(this.AbsorbCarbonSpeed) * this.RemainingSourceAmount * hours;//负值表示吸收，正值表示产生
        }

    }
}
