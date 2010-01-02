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

        public float AbsorbCarbonSpeed
        {
            get;
            set;
        }
        public Forest(FastList<City> cities)
        {
            GetConsumSpeed(cities);
            this.InitAmount = INITForestAmount;
            this.AbsorbCarbonSpeed = ABSORBCarbonSpeed;
            this.ProduceSpeed = 100;
        }
        public Forest()
        {
            this.InitAmount = INITForestAmount;
            this.AbsorbCarbonSpeed = ABSORBCarbonSpeed;
            this.ProduceSpeed = 100;
        }
        /// <summary>
        /// 玩家用于设置森林的再生速度
        /// </summary>
        /// <param name="speed"></param>
        public override void GetProduceSpeed(float speed)
        {
            base.GetProduceSpeed(speed);
        }

        /// <summary>
        /// 得到森林消耗的速度
        /// </summary>
        /// <param name="cities"></param>
        public void GetConsumSpeed(FastList<City> cities)
        {
            float totalspeed = 0;
            if (cities.Count != 0)
            {
                for (int i = 0; i < cities.Count; i++)
                {
                    FastList<CityPlugin> plugins = cities[i].GetPlugins();
                    if (cities[i].GetPlugins().Count != 0)
                    {  
                        for (int j = 0; j < plugins.Count; j++)
                        {
                            totalspeed += plugins[i].LPProductionSpeed * 1.5f;//工厂中只有木材工厂才消耗森林
                        }
                    }
                }
                this.ConsumeSpeed += totalspeed;
            }
            else
            {
                this.ConsumeSpeed = 0;
            }
        }
        public override void Update(GameTime time)
        {
            base.Update(time);
            this.RemainedAmount = this.InitAmount;
            this.RemainedAmount += (this.ProduceSpeed - this.ConsumeSpeed) * time.ElapsedGameTime.Days;
            this.CarbonChange = -(this.AbsorbCarbonSpeed * this.RemainedAmount);//负值表示吸收，正值表示产生
        }
     
    }
}
