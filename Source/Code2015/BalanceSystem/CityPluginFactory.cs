using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Config;
using Apoc3D.Vfs;
using Code2015.EngineEx;

namespace Code2015.BalanceSystem
{

    public class CityPluginFactory
    {
        CityPluginType oilRefinaryType;
        CityPluginType woodFactoryType;
        CityPluginType bioFuelFactoryType;
        CityPluginType hospitalType;
        CityPluginType educationOrganType;


        /// <summary>
        /// 医院，教育机构，木材工厂，炼油厂产生的C为正值，生态工厂产生C为负值
        /// 医院，教育机构产能速度为负值，即是消耗能源，其他产能速度为正值。
        /// 木材工厂产生低能，无高能，炼油厂和生态工厂产能为高能
        /// 
        /// </summary>
        public CityPluginFactory()
        {
            FileLocation fl = FileSystem.Instance.Locate("cityplugins.xml", GameFileLocs.Config);

            Configuration pluginConf = ConfigurationManager.Instance.CreateInstance(fl);

            woodFactoryType = new CityPluginType();
            oilRefinaryType = new CityPluginType();
            bioFuelFactoryType = new CityPluginType();
            hospitalType = new CityPluginType();
            educationOrganType = new CityPluginType();

            woodFactoryType.Parse(pluginConf["WoodFactory"]);
            oilRefinaryType.Parse(pluginConf["OilRefinary"]);
            bioFuelFactoryType.Parse(pluginConf["BioFuelFactory"]);
            hospitalType.Parse(pluginConf["Hospital"]);
            educationOrganType.Parse(pluginConf["EducationOrganization"]);

        }

        public CityPlugin MakeOilRefinary()
        {
            //CityPlugin oilRefinary = new CityPlugin("OilRefinary");
            CityPlugin oilRefinary = new CityPlugin(oilRefinaryType);
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
            //CityPlugin woodfactory = new CityPlugin("WoodFactory");
            CityPlugin woodfactory = new CityPlugin(woodFactoryType);
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
            //CityPlugin biofactory = new CityPlugin("BioEnergyFactory");
            CityPlugin biofactory = new CityPlugin(bioFuelFactoryType);
            ConfigurationSection sect = new IniSection("BioEnergyFactory");
            sect.Add("Cost", "2000");
            sect.Add("UpgradeCost", "2500");
            sect.Add("HPProductionSpeed", "75");
            sect.Add("LPProductionSpeed", "75");
            sect.Add("CarbonProduceSpeed", "0");
            sect.Add("FoodCostSpeed", "-200");

            biofactory.Parse(sect);
            return biofactory;
        }

        public CityPlugin MakeHospital()
        {
            //CityPlugin hospital = new CityPlugin("Hospital");
            CityPlugin hospital = new CityPlugin(hospitalType);

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
            //CityPlugin EducationAgent = new CityPlugin("EducationAgent");
            CityPlugin EducationAgent = new CityPlugin(educationOrganType);

            ConfigurationSection sect = new IniSection("EducationAgent");
            sect.Add("Cost", "1000");
            sect.Add("UpgradeCost", "500");
            sect.Add("HPProductionSpeed", "-15");
            sect.Add("LPProductionSpeed", "-15");
            sect.Add("CarbonProduceSpeed", "50");
            sect.Add("FoodCostSpeed", "-50");

            EducationAgent.Parse(sect);
            return EducationAgent;

        }
    }
}
