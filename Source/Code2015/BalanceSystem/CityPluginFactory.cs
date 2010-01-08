using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Config;

namespace Code2015.BalanceSystem
{
   
    public  class CityPluginFactory
    {
        /// <summary>
        /// 医院，教育机构，木材工厂，炼油厂产生的C为正值，生态工厂产生C为负值
        /// 医院，教育机构产能速度为负值，即是消耗能源，其他产能速度为正值。
        /// 木材工厂产生低能，无高能，炼油厂和生态工厂产能为高能
        /// 
        /// </summary>
        public CityPluginFactory()
        { }
      
        public CityPlugin MakeOilRefinary()
        {
            CityPlugin oilRefinary = new CityPlugin("OilRefinary");

            ConfigurationSection sect = new IniSection("OilRefinary");
            sect.Add("Cost", "1000");
            sect.Add("UpgradeCost", "100");
            sect.Add("HPProductionSpeed", "100");
            sect.Add("LPProductionSpeed", "0");
            sect.Add("CarbonProduceSpeed", "500");
            sect.Add("FoodCostSpeed", "0");

            oilRefinary.Parse(sect);

            return oilRefinary;
        }
        public CityPlugin MakeWoodFactory()
        {
            CityPlugin woodfactory = new CityPlugin("WoodFactory");
           
            ConfigurationSection sect = new IniSection("WoodFactory");
            sect.Add("Cost", "500");
            sect.Add("UpgradeCost", "700");
            sect.Add("HPProductionSpeed", "0");
            sect.Add("LPProductionSpeed", "50");
            sect.Add("CarbonProduceSpeed", "100");
            sect.Add("FoodCostSpeed", "0");

            woodfactory.Parse(sect);

            return woodfactory;
        }
      
        public CityPlugin MakeBioEnergeFactory()
        {
            CityPlugin biofactory = new CityPlugin("BioEnergyFactory");

            ConfigurationSection sect = new IniSection("BioEnergyFactory");
            sect.Add("Cost", "2000");
            sect.Add("UpgradeCost", "2500");
            sect.Add("HPProductionSpeed", "75");
            sect.Add("LPProductionSpeed", "75");
            sect.Add("CarbonProduceSpeed", "0");
            sect.Add("FoodCostSpeed", "-200");

            return biofactory;
        }

        public CityPlugin MakeHospital()
        {
            CityPlugin hospital = new CityPlugin("Hospital");

            ConfigurationSection sect = new IniSection("Hospital");
            sect.Add("Cost", "1000");
            sect.Add("UpgradeCost", "1000");
            sect.Add("HPProductionSpeed", "-30");
            sect.Add("LPProductionSpeed", "-30");
            sect.Add("CarbonProduceSpeed", "50");
            sect.Add("FoodCostSpeed", "-50");

            hospital.Parse(sect);
           
            return hospital;
        }

        public CityPlugin MakeEducationAgent()
        {
            CityPlugin EducationAgent = new CityPlugin("EducationAgent");

            ConfigurationSection sect = new IniSection("EducationAgent");
            sect.Add("Cost", "1000");
            sect.Add("UpgradeCost", "500");
            sect.Add("HPProductionSpeed", "-15");
            sect.Add("LPProductionSpeed", "-15");
            sect.Add("CarbonProduceSpeed", "50");
            sect.Add("FoodCostSpeed", "-50");


            return EducationAgent;

        }
    }
}
