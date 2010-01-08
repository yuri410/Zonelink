using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Config;

namespace Code2015.BalanceSystem
{
    /// <summary>
    ///  表示城市附属物的一种类型
    /// </summary>
    public class CityPluginType : IConfigurable
    {

        public float HRPConvRate
        {
            get;
            private set;
        }

        public float LRPConvRate
        {
            get;
            private set;
        }

        public float FoodConvRate
        {
            get;
            private set;
        }


        /// <summary>
        ///  获取在资源充足的条件下，高能资源消耗的速度
        /// </summary>
        public float HRCSpeedFull
        {
            get;
            private set;
        }
        /// <summary>
        ///  获取在资源充足的条件下，低能资源消耗的速度
        /// </summary>
        public float LRCSpeedFull
        {
            get;
            private set;
        }

        public float FoodCostSpeed
        {
            get;
            protected set;
        }


        public float GatherRadius
        {
            get;
            protected set;
        }
        //public virtual float GetUpgradeCost()
        //{
        //    UpgradeCostBase = Cost * 0.5f;
        //    float upgradecost = UpgradeCostBase;
        //    return UpgradeCostBase = 0;
        //}
        /// <summary>
        /// 升级所需费用
        /// </summary>
        public float UpgradeCostBase
        {
            get;
            protected set;
        }
        /// <summary>
        /// 建造一个所需费用
        /// </summary>
        public float Cost
        {
            get;
            protected set;
        }



        #region IConfigurable 成员

        public void Parse(ConfigurationSection sect)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
