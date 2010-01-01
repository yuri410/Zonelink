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
        const float ForestAmount = 10000;
        new public float InitAmount
        {
            get;
            set;

        }
        public Forest()
        {
            this.InitAmount = ForestAmount;
        }

       
        /// <summary>
        /// 得到木材消耗的速度
        /// </summary>
        /// <param name="plugins"></param>
        public void GetConsumeSpeed(FastList<CityPlugin> plugins)
        {
            
            CityPluginFactory factory=new CityPluginFactory();
            CityPlugin woodfactory = factory.MakeWoodFactory();
            int num=0;
            for (int i = 0; i < plugins.Count; i++)
            {
                if (plugins[i].Name == "WoodFactory")
                    num++;
            }
            this.ConsumeSpeed = num * woodfactory.ProduceLPSpeed * 1.5f;
        }
        /// <summary>
        /// 可以重写生产的速度
        /// </summary>
        /// <param name="speed"></param>
        public override void SetProduceSpeed(float speed)
        {
           
        }

        public override void Update(GameTime time)
        {
            this.InitAmount -= time.ElapsedGameTime * (this.ConsumeSpeed - this.ProduceSpeed);
            this.RemainedAmount = this.InitAmount;
        }


    }
}
