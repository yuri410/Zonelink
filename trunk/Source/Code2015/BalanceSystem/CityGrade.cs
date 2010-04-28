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
        public const float SmallCityLPTranportSpeed = 25;
        [SLGValueAttribute()]
        public const float MediumCityLPTranportSpeed = 40;
        [SLGValueAttribute()]
        public const float LargeCityLPTranportSpeed = 75;

        [SLGValue]
        public const float SmallFoodTranportSpeed = 20;
        [SLGValue]
        public const float MediumFoodTranportSpeed = 30;
        [SLGValue]
        public const float LargeFoodTranportSpeed = 50;

        #endregion

        #region 能源使用速度
        [SLGValueAttribute()]
        public const float SmallCityLPSpeed = -3;
        [SLGValueAttribute()]
        public const float MediumCityLPSpeed = -6;
        [SLGValueAttribute()]
        public const float LargeCityLPSpeed = -9;

        [SLGValueAttribute()]
        public const float SmallCityHPSpeed = -4.5f;
        [SLGValueAttribute()]
        public const float MediumCityHPSpeed = -12;
        [SLGValueAttribute()]
        public const float LargeCityHPSpeed = -18f;

        #endregion

        #region 存储 最大量
        [SLGValue]
        public const int SmallMaxLPStorage = 500;
        [SLGValue]
        public const int MediumMaxLPStorage = 3000;
        [SLGValue]
        public const int LargeMaxLPStorage = 10000;

        [SLGValue]
        public const int SmallMaxHPStorage = 500;
        [SLGValue]
        public const int MediumMaxHPStorage = 3000;
        [SLGValue]
        public const int LargeMaxHPStorage = 10000;

        [SLGValue]
        public const int SmallMaxFoodStorage = 500;
        [SLGValue]
        public const int MediumMaxFoodStorage = 3000;
        [SLGValue]
        public const int LargeMaxFoodStorage = 10000;
        #endregion

        #region 发展比
        [SLGValue]
        public const float SmallDevMult = 1;
        [SLGValue]
        public const float MediumDevMult = 2;
        [SLGValue]
        public const float LargeDevMult = 3;
        #endregion

        #region 参考发展度
        [SLGValue]
        public const float SmallCityPointThreshold = 300;
        [SLGValue]
        public const float MediumCityPointThreshold = 3000;
        [SLGValue]
        public const float LargeCityPointThreshold = 30000;

        #endregion

        //#region 参考满意度
        //[SLGValue]
        //public const float SmallMinSat = 0.01f * SmallRefPop * SmallCityPointThreshold;
        //[SLGValue]
        //public const float MediumMinSat = 0.01f * MediumRefPop * MediumCityPointThreshold;
        //[SLGValue]
        //public const float LargeMinSat = 0.01f * LargeRefPop * LargeCityPointThreshold;

        //#endregion

        #region 参考人口
        [SLGValue]
        public const float SmallRefPop = 20;
        [SLGValue]
        public const float MediumRefPop = 400;
        [SLGValue]
        public const float LargeRefPop = 1000;
        #endregion

        [SLGValue]
        public const float SmallGatherRadius = 13;
        [SLGValue]
        public const float MediumGatherRadius = 14;
        [SLGValue]
        public const float LargeGatherRadius = 15;
       

        #region 城市占领分数
        [SLGValue()]
        public const float LargeCityCapturePoint = 10000 * 0.2f;

        [SLGValue()]
        public const float MediumCityCapturePoint = 2000 * 0.2f;

        [SLGValue()]
        public const float SmallCityCapturePoint = 500 * 0.2f;
        #endregion

        [SLGValue()]
        public const float LargeRecoverCD = 100;

        [SLGValue()]
        public const float MediumRecoverCD = 50;

        [SLGValue()]
        public const float SmallRecoverCD = 30;

        static readonly float[] RecoverCoolDown = { SmallRecoverCD, MediumRecoverCD, LargeRecoverCD };
        static readonly float[] UpgradePoint = { SmallCityPointThreshold, MediumCityPointThreshold, LargeCityPointThreshold };

        static readonly float[] CapturePoint = { SmallCityCapturePoint, MediumCityCapturePoint, LargeCityCapturePoint };
        static readonly float[] GatherRadius = { SmallGatherRadius, MediumGatherRadius, LargeGatherRadius };

        static readonly float[] DevMult = { SmallDevMult, MediumDevMult, LargeDevMult };
        static readonly float[] LPSpeed = { SmallCityLPSpeed, MediumCityLPSpeed, LargeCityLPSpeed };
        static readonly float[] HPSpeed = { SmallCityHPSpeed, MediumCityHPSpeed, LargeCityHPSpeed };

        static readonly float[] LPTSpeed = { SmallCityLPTranportSpeed, MediumCityLPTranportSpeed, LargeCityLPTranportSpeed };
        static readonly float[] HPTSpeed = { SmallCityHPTranportSpeed, MediumCityHPTranportSpeed, LargeCityHPTranportSpeed };
        static readonly float[] FoodTSpeed = { SmallFoodTranportSpeed, MediumFoodTranportSpeed, LargeFoodTranportSpeed };

        //static readonly float[] MinSat = { SmallMinSat, MediumMinSat, LargeMinSat };
        [SLGValue]
        public const float FoodCostPerPeople = 0.05f;

        public static float GetMinSatRatio(UrbanSize size)
        {
            return 0.01f;//MinSat[(int)size];
        }
        public static float GetCapturePoint(UrbanSize size) 
        {
            return CapturePoint[(int)size];
        }

        public static float GetRecoverCoolDown(UrbanSize size) 
        {
            return RecoverCoolDown[(int)size];
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
