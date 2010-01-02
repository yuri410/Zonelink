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
        const float OILWeight = 100000;
        const float EMITCarbonSpeed = 10;


        public OilField()
        {
            this.InitAmount = OILWeight;
            this.EmitCspeed = EMITCarbonSpeed;
        }

        public OilField(FastList<City> cities)
        {
            GetConsumeSpeed(cities);
            this.InitAmount = OILWeight;
            this.EmitCspeed = EMITCarbonSpeed;      
        }
      
        public float EmitCspeed
        {
            get;
            set;
        }
        public OilField(string name)
        {
            this.Name = name;
            this.InitAmount = OILWeight;
            this.EmitCspeed = EMITCarbonSpeed;
        }
        /// <summary>
        /// 得到石油的消耗速度
        /// </summary>
        /// <param name="cities"></param>
        public void GetConsumeSpeed(FastList<City> cities)
        {
            float oilconsumespeed = 0;
            if (cities.Count != 0)
            {
                this.ConsumeSpeed = 10;
                FastList<CityPlugin> plugins = new FastList<CityPlugin>();
                for (int i = 0; i < cities.Count; i++)
                {
                    plugins = cities[i].GetPlugins();
                    if (plugins.Count != 0)
                    {
                        for (int j = 0; j < plugins.Count; j++)
                        {
                            if (plugins[j].Name == "OilRefinary")
                            {
                                oilconsumespeed += plugins[j].HPProductionSpeed * 1.5f;
                            }
                        }

                    }
                }
                this.ConsumeSpeed += oilconsumespeed;
            }

            else
            {
                this.ConsumeSpeed = 10;
            }

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
            this.CarbonChange += this.EmitCspeed * this.RemainedAmount;//油田本身也要释放C
        }

    }
}
