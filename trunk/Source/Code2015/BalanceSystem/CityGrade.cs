using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Config;
using Apoc3D.MathLib;

namespace Code2015.BalanceSystem
{
    public static class CityGrade
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

        #region 参考发展度
        [SLGValue]
        public const float SmallCityPointThreshold = 500;
        [SLGValue]
        public const float MediumCityPointThreshold = 5000;
        [SLGValue]
        public const float LargeCityPointThreshold = 50000;

        #endregion

        #region 参考人口
        [SLGValue]
        public const float SmallRefPop = 20;
        [SLGValue]
        public const float MediumRefPop = 400;
        [SLGValue]
        public const float LargeRefPop = 1000;
        #endregion

        #region 食物自采集速度
        [SLGValue]
        public const int SmallFoodCollectSpeed = 10;
        [SLGValue]
        public const int MediumFoodCollectSpeed = 20;
        [SLGValue]
        public const int LargeFoodCollectSpeed = 50;

        [SLGValue]
        public const float SmallGatherRadius = 7;
        [SLGValue]
        public const float MediumGatherRadius = 9 ;
        [SLGValue]
        public const float LargeGatherRadius = 10;
        #endregion

        #region 城市占领分数
        [SLGValue()]
        public const float LargeCityCapturePoint = 10000;

        [SLGValue()]
        public const float MediumCityCapturePoint = 2000;

        [SLGValue()]
        public const float SmallCityCapturePoint = 500;
        #endregion

        static readonly float[] UpgradePoint = { SmallCityPointThreshold, MediumCityPointThreshold, LargeCityPointThreshold };

        static readonly float[] CapturePoint = { SmallCityCapturePoint, MediumCityCapturePoint, LargeCityCapturePoint };
        static readonly float[] GatherRadius = { SmallGatherRadius, MediumGatherRadius, LargeGatherRadius };

        static readonly float[] DevMult = { SmallDevMult, MediumDevMult, LargeDevMult };
        static readonly float[] LPSpeed = { SmallCityLPSpeed, MediumCityLPSpeed, LargeCityLPSpeed };
        static readonly float[] HPSpeed = { SmallCityHPSpeed, MediumCityHPSpeed, LargeCityHPSpeed };

        static readonly float[] LPTSpeed = { SmallCityLPTranportSpeed, MediumCityLPTranportSpeed, LargeCityLPTranportSpeed };
        static readonly float[] HPTSpeed = { SmallCityHPTranportSpeed, MediumCityHPTranportSpeed, LargeCityHPTranportSpeed };
        static readonly float[] FoodTSpeed = { SmallFoodTranportSpeed, MediumFoodTranportSpeed, LargeFoodTranportSpeed };

        static readonly float[] FoodCollSpeed = { SmallFoodCollectSpeed, MediumFoodCollectSpeed, LargeFoodCollectSpeed };

        [SLGValue]
        public const float FoodCostPerPeople = 0.05f;

        [SLGValue]
        public const float CityDeathThreshold = 0.1f;


        public static float GetCapturePoint(UrbanSize size) 
        {
            return CapturePoint[(int)size];
        }


        public static float GetGatherRadius(UrbanSize citySize)
        {
            return GatherRadius[(int)citySize];
        }

        public static float GetUpgradePoint(UrbanSize size)
        {
            return UpgradePoint[(int)size];
        }

        /// <summary>
        ///  获取在当前城市规模下的参考人口（标准人口）
        /// </summary>
        public static float GetRefPopulation(UrbanSize grade)
        {
            switch (grade)
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
        public static float GetDevelopmentMult(UrbanSize grade)
        {
            return DevMult[(int)grade];
        }


        public static float GetHPTransportSpeed(UrbanSize grade)
        {
            return HPTSpeed[(int)grade];
        }
        public static float GetLPTransportSpeed(UrbanSize grade)
        {
            return LPTSpeed[(int)grade];
        }
        public static float GetFoodTransportSpeed(UrbanSize grade)
        {
            return FoodTSpeed[(int)grade];
        }
        public static float GetSelfFoodGatheringSpeed(UrbanSize grade)
        {
            return FoodCollSpeed[(int)grade];
        }

        /// <summary>
        ///  资源充足条件下，消耗高能资源的速度
        /// </summary>
        /// <param name="grade"></param>
        /// <returns></returns>
        public static float GetSelfHRCSpeed(UrbanSize grade)
        {
            return HPSpeed[(int)grade];
        }

        /// <summary>
        ///  资源充足条件下，消耗低能资源的速度
        /// </summary>
        /// <param name="grade"></param>
        /// <returns></returns>
        public static float GetSelfLRCSpeed(UrbanSize grade)
        {
            return LPSpeed[(int)grade];
        }


        /// <summary>
        ///  获取目前城市最多可以添加的附加设施数量
        /// </summary>
        public static int GetMaxPlugins(UrbanSize grade)
        {
            switch (grade)
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

}
