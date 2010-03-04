using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Config;

namespace Code2015.BalanceSystem
{
    /// <summary>
    ///  硬编码内定附加物的行为
    /// </summary>
    public enum CityPluginBehaviourType
    {
        None,
        CollectorFactory,
        Hospital,
        Education
    }

    /// <summary>
    ///  表示城市附属物的一种类型
    /// </summary>
    public class CityPluginType : IConfigurable
    {
        public CityPluginType() { }
        public CityPluginType(string typeName) { TypeName = typeName; }

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

        public CityPluginBehaviourType Behaviour
        {
            get;
            private set;
        }
        ///// <summary>
        /////  采集资源的速度
        ///// </summary>
        //public float HRPSpeed
        //{
        //    get;
        //    private set;
        //}

        //public float LRPSpeed
        //{
        //    get;
        //    private set;
        //}

        /// <summary>
        ///  获取在资源充足的条件下，高能资源消耗的速度。
        /// </summary>
        public float HRCSpeed
        {
            get;
            private set;
        }
        /// <summary>
        ///  获取在资源充足的条件下，低能资源消耗的速度
        /// </summary>
        public float LRCSpeed
        {
            get;
            private set;
        }

        public float FoodCostSpeed
        {
            get;
            protected set;
        }


        public float GetUpgradeCost(int level)
        {
            UpgradeCostBase = level * Cost;
            float upgradecost = UpgradeCostBase;
            UpgradeCostBase = 0;
            return upgradecost;
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

        public string TypeName
        {
            get;
            private set;
        }

        #region IConfigurable 成员

        public void Parse(ConfigurationSection sect)
        {
            Cost = sect.GetSingle("Cost");

            //HRPSpeed = sect.GetSingle("HRPSpeed", 0);
            //LRPSpeed = sect.GetSingle("LRPSpeed", 0);

            HRCSpeed = sect.GetSingle("HRCSpeed", 0);
            LRCSpeed = sect.GetSingle("LRCSpeed", 0);

            FoodCostSpeed = sect.GetSingle("FoodCostSpeed", 0);

            FoodConvRate = sect.GetSingle("FoodConvRate", 0.75f);
            HRPConvRate = sect.GetSingle("HRPConvRate", 0.75f);
            LRPConvRate = sect.GetSingle("LRPConvRate", 0.75f);

            Behaviour = (CityPluginBehaviourType)
                Enum.Parse(typeof(CityPluginBehaviourType), 
                sect.GetString("Behaviour", CityPluginBehaviourType.None.ToString()), true);
        }

        #endregion

    }
}
