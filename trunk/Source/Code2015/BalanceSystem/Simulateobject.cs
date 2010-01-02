using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;

namespace Code2015.BalanceSystem
{
    public class SimulateObject : IUpdatable
    {
        /// <summary>
        /// 经度
        /// </summary>
        public float Col
        {
            get;
            private set;
        }
        /// <summary>
        /// 纬度
        /// </summary>
        public float Lat
        {
            get;
            private set;
        }



        /// <summary>
        /// CO2的改变量
        /// </summary>
        protected float CarbonChange
        {
            get;
            set;
        }
        public float CarbonProduceSpeed
        {
            get;
            protected set;
        }
        public float GetCarbonChange()
        {
            float r = CarbonChange;
            CarbonChange = 0;
            return r;
        }



        //NaturalResource naturalresource;
        //City city;

        public SimulateObject()
        {
            //naturalresource= new NaturalResource();
            //city=new City();
        }


        public virtual void Update(GameTime time)
        {
            //naturalresource.Update(time);
            //city.Update(time);
            //this.CarbonWeight += (naturalresource.CarbonWeight + city.CarbonWeight);
        }
    }
}
