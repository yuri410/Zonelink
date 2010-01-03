using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Config;

namespace Code2015.BalanceSystem
{
    public class CityPlugin : IConfigurable
    {
        City parent;

        public string Name
        {
            get;
            protected set;
        }
        public CityPlugin(string name)
        {
            this.Name = name;
        }
        public CityPlugin()
        { }



        #region IConfigurable 成员

        public void Parse(ConfigurationSection sect)
        {

            Cost = sect.GetSingle("Cost");
            UpgradeCost = sect.GetSingle("UpgradeCost");

            HPProductionSpeed = sect.GetSingle("HPProductionSpeed");
            LPProductionSpeed = sect.GetSingle("LPProductionSpeed");
            CarbonProduceSpeed = sect.GetSingle("CarbonProduceSpeed");
            FoodCostSpeed = sect.GetSingle("FoodCostSpeed");

        }

        #endregion

        #region  属性
        /// <summary>
        /// 开始建造一个所需费用
        /// </summary>
        public float Cost
        {
            get;
            protected set;
        }

#warning 不同的升级费用
        /// <summary>
        /// 升级所需费用
        /// </summary>
        public float UpgradeCost
        {
            get;
            protected set;
        }
        public virtual float GetUpgradeCost()
        {
            UpgradeCost = Cost * 0.5f;
            float upgradecost = UpgradeCost;         
            return UpgradeCost = 0;
        }

        public float Radius
        {
            get;
            protected set;
        }

            

        /// <summary>
        /// 高能产生的速度,速度为正表示产生能量，为负值表示消耗能量
        /// </summary>
        public float HPProductionSpeed
        {
            get;
            protected set;
        }
        /// <summary>
        /// 低能产生的速度
        /// </summary>
        public float LPProductionSpeed
        {
            get;
            protected set;
        }

        public float CarbonProduceSpeed
        {
            get;
            protected set;
        }

        public float FoodCostSpeed
        {
            get;
            protected set;
        }

        #endregion

        public void NotifyAdded(City city)
        {
            parent = city;


        }

        public void NotifyRemoved(City city)
        {
            parent = null;
        }


    }
}
