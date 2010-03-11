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
            return new CityPlugin(oilRefinaryType, CityPluginTypeId.OilRefinary);
        }
        public CityPlugin MakeWoodFactory()
        {
            return new CityPlugin(woodFactoryType, CityPluginTypeId.WoodFactory);
        }

        public CityPlugin MakeBioEnergeFactory()
        {
            return new CityPlugin(bioFuelFactoryType, CityPluginTypeId.BiofuelFactory);
        }

        public CityPlugin MakeHospital()
        {
            return new CityPlugin(hospitalType, CityPluginTypeId.Hospital);
        }

        public CityPlugin MakeEducationAgent()
        {
            return new CityPlugin(educationOrganType, CityPluginTypeId.EducationOrg);
        }
    }
}
