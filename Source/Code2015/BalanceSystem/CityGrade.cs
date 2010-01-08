using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Config;

namespace Code2015.BalanceSystem
{
    /// <summary>
    ///  表示一个城市的等级
    /// </summary>
    public class CityGrade : IConfigurable
    {
        [SLGValue]
        public const int SmallPluginCount = 1;
        [SLGValue]
        public const int NormalPluginCount = 3;
        [SLGValue]
        public const int LargePluginCount = 4;

        #region 能源流通速度
        [SLGValueAttribute()]
        public const float SmallCityHPTranportSpeed = 30;
        [SLGValueAttribute()]
        public const float MediumCityHPTranportSpeed = 50;
        [SLGValueAttribute()]
        public const float LargeCityHPTranportSpeed = 100;

        [SLGValueAttribute()]
        public const float SmallCityLPTranportSpeed = 15;
        [SLGValueAttribute()]
        public const float MediumCityLPTranportSpeed = 25;
        [SLGValueAttribute()]
        public const float LargeCityLPTranportSpeed = 50;

        [SLGValue]
        public const float SmallFoodTranportSpeed = 20;
        [SLGValue]
        public const float MediumFoodTranportSpeed = 30;
        [SLGValue]
        public const float LargeFoodTranportSpeed = 50;

        #endregion

        #region 能源使用速度
        [SLGValueAttribute()]
        public const float SmallCityLPSpeed = -30;
        [SLGValueAttribute()]
        public const float MediumCityLPSpeed = -50;
        [SLGValueAttribute()]
        public const float LargeCityLPSpeed = -100;

        [SLGValueAttribute()]
        public const float SmallCityHPSpeed = -30;
        [SLGValueAttribute()]
        public const float MediumCityHPSpeed = -50;
        [SLGValueAttribute()]
        public const float LargeCityHPSpeed = -100;

        #endregion

        #region 存储 最大量
        [SLGValue]
        public const int SmallMaxLPStorage = 100;
        [SLGValue]
        public const int MediumMaxLPStorage = 1000;
        [SLGValue]
        public const int LargeMaxLPStorage = 3000;

        [SLGValue]
        public const int SmallMaxHPStorage = 100;
        [SLGValue]
        public const int MediumMaxHPStorage = 1000;
        [SLGValue]
        public const int LargeMaxHPStorage = 3000;

        [SLGValue]
        public const int SmallMaxFoodStorage = 100;
        [SLGValue]
        public const int MediumMaxFoodStorage = 1000;
        [SLGValue]
        public const int LargeMaxFoodStorage = 3000;
        #endregion

        #region 发展比
        [SLGValue]
        public const float SmallDevMult = 1;
        [SLGValue]
        public const float MediumDevMult = 0.25f;
        [SLGValue]
        public const float LargeDevMult = 0.05f;
        #endregion

        #region 参考人口
        [SLGValue]
        public const float SmallRefPop = 20;
        [SLGValue]
        public const float MediumRefPop = 400;
        [SLGValue]
        public const float LargeRefPop = 1000;
        #endregion


        [SLGValue]
        public const int SmallFoodCollectSpeed = 10;
        [SLGValue]
        public const int MediumFoodCollectSpeed = 20;
        [SLGValue]
        public const int LargeFoodCollectSpeed = 50;


        static readonly float[] DevMult = { SmallDevMult, MediumDevMult, LargeDevMult };
        static readonly float[] LPSpeed = { SmallCityLPSpeed, MediumCityLPSpeed, LargeCityLPSpeed };
        static readonly float[] HPSpeed = { SmallCityHPSpeed, MediumCityHPSpeed, LargeCityHPSpeed };

        static readonly float[] LPTSpeed = { SmallCityLPTranportSpeed, MediumCityLPTranportSpeed, LargeCityLPTranportSpeed };
        static readonly float[] HPTSpeed = { SmallCityHPTranportSpeed, MediumCityHPTranportSpeed, LargeCityHPTranportSpeed };
        static readonly float[] FoodTSpeed = { SmallFoodTranportSpeed, MediumFoodTranportSpeed, LargeFoodTranportSpeed };

        static readonly float[] FoodCollSpeed = { SmallFoodCollectSpeed, MediumFoodCollectSpeed, LargeFoodCollectSpeed };




        public UrbanSize Grade
        {
            get;
            set;
        }

        /// <summary>
        ///  获取在当前城市规模下的参考人口（标准人口）
        /// </summary>
        public float RefPopulation
        {
            get
            {
                switch (Grade)
                {
                    case UrbanSize.Small:
                        return SmallRefPop;
                    case UrbanSize.Medium:
                        return MediumRefPop;
                    case UrbanSize.Large:
                        return LargeRefPop;
                }
                return 0;
            }
        }


        public float DevelopmentMult
        {
            get { return DevMult[(int)Grade]; }
        }
        public float HPTransportSpeed
        {
            get { return HPTSpeed[(int)Grade]; }
        }
        public float LPTransportSpeed
        {
            get { return LPTSpeed[(int)Grade]; }
        }
        public float FoodTransportSpeed
        {
            get { return FoodTSpeed[(int)Grade]; }
        }
        public float SelfFoodGatheringSpeed
        {
            get { return FoodCollSpeed[(int)Grade]; }
        }


        public float SelfHRCSpeed
        {
            get
            {
                return HPSpeed[(int)Grade];
            }
        }
        public float SelfLRCSpeed
        {
            get
            {
                return LPSpeed[(int)Grade];
            }
        }


        /// <summary>
        ///  获取目前城市最多可以添加的附加设施数量
        /// </summary>
        public int MaxPlugins
        {
            get
            {
                switch (Grade)
                {
                    case UrbanSize.Small:
                        return SmallPluginCount;
                    case UrbanSize.Medium:
                        return NormalPluginCount;
                    case UrbanSize.Large:
                        return LargePluginCount;
                }
                return 0;
            }
        }


        #region IConfigurable 成员

        public void Parse(ConfigurationSection sect)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
