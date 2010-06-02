/*
-----------------------------------------------------------------------------
This source file is part of Zonelink

Copyright (c) 2009+ Tao Games

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  if not, write to the Free Software Foundation, 
Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA, or go to
http://www.gnu.org/copyleft/lesser.txt.

-----------------------------------------------------------------------------
*/
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

        public CityPluginType OilRefinaryType
        {
            get { return oilRefinaryType; }
        }
        public CityPluginType WoodFactoryType 
        {
            get { return woodFactoryType; }
        }
        public CityPluginType BiofuelFactoryType
        {
            get { return bioFuelFactoryType; }
        }
        public CityPluginType HospitalType
        {
            get { return hospitalType; }
        }
        public CityPluginType EducationOrgType 
        {
            get { return educationOrganType; }
        }

        public CityPlugin MakeOilRefinary()
        {
            return new CityPlugin(this, oilRefinaryType, CityPluginTypeId.OilRefinary);
        }
        public CityPlugin MakeWoodFactory()
        {
            return new CityPlugin(this, woodFactoryType, CityPluginTypeId.WoodFactory);
        }

        public CityPlugin MakeBioEnergeFactory()
        {
            return new CityPlugin(this, bioFuelFactoryType, CityPluginTypeId.BiofuelFactory);
        }

        public CityPlugin MakeHospital()
        {
            return new CityPlugin(this, hospitalType, CityPluginTypeId.Hospital);
        }

        public CityPlugin MakeEducationAgent()
        {
            return new CityPlugin(this, educationOrganType, CityPluginTypeId.EducationOrg);
        }
    }
}
