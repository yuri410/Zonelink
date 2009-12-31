using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Code2015.BalanceSystem
{
   
    public  class CityPluginFactory
    {
        public CityPluginFactory()
        { }

        public CityPlugin MakeOilRefinary()
        {
            CityPlugin oilRefinary = new CityPlugin("OilRefinary");
            oilRefinary.CostMoney = 100;
            oilRefinary.ImproveCost = 30;
            oilRefinary.HighPowerSpeed = 100;
            oilRefinary.LowPower = 0;
            oilRefinary.CarbonWeight = 500;
            oilRefinary.ConsumePower = 0;
            return oilRefinary;
        }
        public CityPlugin MakeWoodFactory()
        {
            CityPlugin woodfactory = new CityPlugin("WoodFactory");
            woodfactory.CostMoney = 30;
            woodfactory.ImproveCost = 10;
            woodfactory.HighPowerSpeed = 0;
            woodfactory.LowPower = 30;
            woodfactory.CarbonWeight = 100;
            woodfactory.ConsumePower = 0;
            return woodfactory;
        }
        /// <summary>
        /// 医院和大学的参数一致
        /// </summary>
        /// <returns></returns>
        public CityPlugin MakeBioEnergeFactory()
        {
            CityPlugin biofactory = new CityPlugin("BioEnergyFactory");
            biofactory.CostMoney = 400;
            biofactory.ImproveCost = 0;
            biofactory.HighPowerSpeed = 200;
            biofactory.LowPower = 0;
            biofactory.CarbonWeight = 0;
            biofactory.ConsumePower = 0;
            return biofactory;
        }

        public CityPlugin MakeHospital()
        {
            CityPlugin hospital = new CityPlugin("Hospital");
            hospital.CostMoney = 100;
            hospital.ImproveCost = 50;
            hospital.HighPowerSpeed = 0;
            hospital.LowPower = 0;
            hospital.CarbonWeight = 10;
            hospital.ConsumePower = 20;
            return hospital;
        }

        public CityPlugin MakeCollege()
        {
            CityPlugin college = new CityPlugin("College");
            college.CostMoney = 100;
            college.ImproveCost = 50;
            college.HighPowerSpeed = 0;
            college.LowPower = 0;
            college.CarbonWeight = 10;
            college.ConsumePower = 20;
            return college;

        }
    }
}
