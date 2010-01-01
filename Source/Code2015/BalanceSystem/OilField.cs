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
        new public float InitAmount
        {
            get;
            set;
        }

       
        public OilField()
        {
            this.InitAmount = OilWeight;
        }

        public void GetConsumeSpeed(FastList<CityPlugin> plugins)
        {
            CityPluginFactory factory = new CityPluginFactory();
            CityPlugin oilrefinary = factory.MakeOilRefinary();
            int num=0;
            for (int i = 0; i < plugins.Count; i++)
            {
                if (plugins[i].Name == "OilRefinary")
                {
                    num++;
                }
            }
            this.ConsumeSpeed = num * oilrefinary.ProduceHLSpeed * 1.5f;
        }

        public override void Update(GameTime time)
        {
            this.RemainedAmount = this.InitAmount;
            this.RemainedAmount -= time.ElapsedGameTime * this.ConsumeSpeed;
            base.HPAmount = this.RemainedAmount;
        }
     

      

       


    }
}
