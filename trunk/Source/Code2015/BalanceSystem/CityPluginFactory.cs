using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Code2015.BalanceSystem
{
   
    public  class CityPluginFactory
    {
        /// <summary>
        /// 医院，教育机构，木材工厂，炼油厂产生的C为正值，生态工厂产生C为负值
        /// 医院，教育机构产能速度为负值，即是消耗能源，其他产能速度为正值。木材工厂产生低能，无高能，炼油厂和生态工厂产能为高能
        /// 
        /// </summary>
        public CityPluginFactory()
        { }

        public CityPlugin ImproveFactory(string name)
        {
            CityPlugin factory = new CityPlugin(name);

            return factory;
        }
        public CityPlugin MakeOilRefinary()
        {
            CityPlugin oilRefinary = new CityPlugin("OilRefinary");
            oilRefinary.CostMoney = 100;
            oilRefinary.ImproveCost = 100;
            oilRefinary.ProduceHLSpeed = 100;
            oilRefinary.ProduceLPSpeed = 0;
            oilRefinary.CarbonWeight = 500;
            
            return oilRefinary;
        }
        public CityPlugin MakeWoodFactory()
        {
            CityPlugin woodfactory = new CityPlugin("WoodFactory");
            woodfactory.CostMoney = 50;
            woodfactory.ImproveCost = 50;
            woodfactory.ProduceHLSpeed = 0;
            woodfactory.ProduceLPSpeed = 50;
            woodfactory.CarbonWeight = 100;
           
            return woodfactory;
        }
      
        public CityPlugin MakeBioEnergeFactory()
        {
            CityPlugin biofactory = new CityPlugin("BioEnergyFactory");
            biofactory.CostMoney = 400;
            biofactory.ImproveCost = 400;
            biofactory.ProduceHLSpeed = 1000;
            biofactory.ProduceLPSpeed = 0;
            biofactory.CarbonWeight = 0;
          
            return biofactory;
        }

        public CityPlugin MakeHospital()
        {
            CityPlugin hospital = new CityPlugin("Hospital");
            hospital.CostMoney = 100;
            hospital.ImproveCost = 100;
            hospital.ProduceHLSpeed = -20;
            hospital.ProduceLPSpeed =-20;
            hospital.CarbonWeight = 50;
           
            return hospital;
        }

        public CityPlugin MakeEducationAgent()
        {
            CityPlugin EducationAgent = new CityPlugin("EducationAgent");
            EducationAgent.CostMoney = 100;
            EducationAgent.ImproveCost = 50;
            EducationAgent.ProduceHLSpeed = -10;
            EducationAgent.ProduceLPSpeed = -10;
            EducationAgent.CarbonWeight = 10;

            return EducationAgent;

        }
    }
}
