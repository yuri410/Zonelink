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
        /// <summary>
        /// 森林的初始数目，固碳速度
        /// </summary>
        [SLGValueAttribute()]
        const float ForestAmount = 10000;
        const float AbsorbCarbonSpeed = 1000;
        public Forest()
        {
            this.InitAmount = ForestAmount;
        }

        public Forest(string name)
        {
            this.Name = name;
            this.InitAmount = ForestAmount;
        }

        public float AbsorbCspeed
        {
            get
            {
                return AbsorbCarbonSpeed;
            }
            set
            {
                value = AbsorbCarbonSpeed;
            }
       
        }
        /// <summary>
        /// 森林被消耗的速度
        /// </summary>
        public void GetConsumeSpeed(FastList<City> cities)
        {
            float woodconsumespeed = 0;
            for (int i = 0; i < cities.Count; i++)
            {
                woodconsumespeed += cities[i].ProduceLPSpeed * 1.5f;
            }
            this.ConsumeSpeed = woodconsumespeed;
            
        }
     
            
        /// <summary>
        /// 可以重写生产的速度
        /// </summary>
        /// <param name="speed"></param>
        public override void SetProduceSpeed(float speed)
        {
            this.ProduceSpeed = 100;
        }

        public override void Update(GameTime time)
        {
            this.InitAmount -= time.ElapsedGameTime.Days * (this.ConsumeSpeed - this.ProduceSpeed);
            this.RemainedAmount = this.InitAmount;
            this.CarbonWeight += -(this.AbsorbCspeed * this.RemainedAmount);
        }


    }
}
