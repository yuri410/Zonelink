using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Collections;
using Apoc3D;


namespace Code2015.BalanceSystem
{
      
    public class Forest : NaturalResource
    {
        [SLGValueAttribute()]
        const float INITForestAmount = 100000;
        const float ABSORBCarbonSpeed = 1000;
        const float TRANStoEnergySpeed = 100;

        public float AbsorbCarbonSpeed
        {
            get;
            set;
        }

        public Forest()
        {
            this.InitSourceAmount = INITForestAmount;
            this.AbsorbCarbonSpeed = ABSORBCarbonSpeed;
            this.SourceProduceSpeed = 100;
            this.SourceConsumeSpeed = TRANStoEnergySpeed;
        }
        /// <summary>
        /// 暂时设置森林的再生速度为定值，玩家用于设置森林的再生速度
        /// </summary>
        /// <param name="speed"></param>
        public override void GetProduceSpeed(float speed)
        {
            base.GetProduceSpeed(speed);
        }

        public float GetCarbonChange()
        {
            float change = this.CarbonChange;
            this.CarbonChange = 0;
            return change;
        }

        public float TransToLPAmount
        {
            get;
            set;
        }
        public override void Update(GameTime time)
        {
            //base.Update(time);
            float hours = time.ElapsedGameTime.Hours;
            this.RemainingSourceAmount = this.InitSourceAmount;//开始时初始值等于剩余值。
            this.RemainingSourceAmount += (this.SourceProduceSpeed - this.SourceConsumeSpeed) * hours;
            this.TransToLPAmount = this.SourceConsumeSpeed * hours;
            this.CarbonChange += -(this.AbsorbCarbonSpeed) * this.RemainingSourceAmount*hours;//负值表示吸收，正值表示产生

        }
     
    }
}
