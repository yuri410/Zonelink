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
        const float OilWeight = 100000;
        const float EmitCarbonSpeed = 50;
        public OilField()
        {
            this.InitAmount = OilWeight;
        }

        public float EmitCspeed
        {
            get { return EmitCarbonSpeed; }
            set { EmitCspeed = EmitCarbonSpeed; }
        }
        public OilField(string name)
        {
            this.Name = name;
            this.InitAmount = OilWeight;
        }
        /// <summary>
        /// 得到区域内部石油的消耗速度
        /// </summary>
        /// <param name="cities"></param>
        public void GetConsumeSpeed(FastList<City> cities)
        {
            float oilconsumespeed = 0;
            for (int i = 0; i < cities.Count; i++)
            {
                oilconsumespeed += cities[i].ProduceLPSpeed * 1.5f;
            }
            this.ConsumeSpeed = oilconsumespeed;

        }
        /// <summary>
        /// 石油的产生速度为0
        /// </summary>
        /// <param name="plugins"></param>
        public void GetConsumeSpeed(FastList<CityPlugin> plugins)
        {
            this.ProduceSpeed = 0;
        }

        public override void Update(GameTime time)
        {
            this.RemainedAmount = this.InitAmount;
            this.RemainedAmount -= time.ElapsedGameTime.Days * this.ConsumeSpeed;
            this.CarbonWeight += this.EmitCspeed * this.RemainedAmount;
        }

    }
}
