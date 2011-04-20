using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Apoc3D;
using Apoc3D.Config;
using Apoc3D.Vfs;
using Code2015.EngineEx;

namespace Code2015.Logic
{
    static class RulesTable
    {

        public static float ORecoverBias { get; private set; }
        public static float FRecoverBias { get; private set; }
        public static float FRecoverRate { get; private set; }

        public static float CityMaxDevelopment { get; private set; }
        /// <summary>
        ///  发展度的多少是最大生命
        /// </summary>
        public static float CityDevHealthRate { get; private set; }        
        public static float CityInitialDevelopment { get; private set; }
        public static float CityRadius { get; private set; }
        public static float CityDevRBallHealthRate { get; private set; }
        public static float CityArmor { get; private set; }

        public static float HarvLoadingSpeed { get; private set; }
        public static float HarvLoadingTime { get; private set; }

        public static float OilGatherDistance { get; private set; }
        public static float OilHarvHP { get; private set; }
        /// <summary>
        ///  这个存量决定了采集速度加成
        /// </summary>
        public static float OilHarvStorage { get; private set; }
        public static float OilHarvSpeed { get; private set; }

        public static float GreenGatherDistance { get; private set; }
        public static float GreenHarvHP { get; private set; }
        public static float GreenHarvStorage { get; private set; }
        public static float GreenHarvSpeed { get; private set; }


        public static float OilBallBaseHealth { get; private set; }
        public static float OilBallCost { get; private set; }
        public static float OilBallContribution { get; private set; }
        public static float OilBallBaseDamage { get; private set; }
        public static float OilBallBaseHeal { get; private set; }

        public static float GreenBallBaseHealth { get; private set; }
        public static float GreenBallCost { get; private set; }
        public static float GreenBallContribution { get; private set; }
        public static float GreenBallBaseDamage { get; private set; }
        public static float GreenBallBaseHeal { get; private set; }

        public static float EducationBallBaseHealth { get; private set; }
        public static float EducationBallGenInterval { get; private set; }
        public static float EducationBallContribution { get; private set; }
        public static float EducationBallBaseDamage { get; private set; }
        public static float EducationBallBaseHeal { get; private set; }

        public static float VolienceBallBaseHealth { get; private set; }
        public static float VolienceBallGenInterval { get; private set; }
        public static float VolienceBallContribution { get; private set; }
        public static float VolienceBallBaseDamage { get; private set; }
        public static float VolienceBallBaseHeal { get; private set; }

        public static float HealthBallBaseHealth { get; private set; }
        public static float HealthBallGenInterval { get; private set; }
        public static float HealthBallContribution { get; private set; }
        public static float HealthBallBaseDamage { get; private set; }
        public static float HealthBallBaseHeal { get; private set; }

        public static float DiseaseBallBaseHealth { get; private set; }
        public static float DiseaseBallGenInterval { get; private set; }
        public static float DiseaseBallContribution { get; private set; }
        public static float DiseaseBallBaseDamage { get; private set; }
        public static float DiseaseBallBaseHeal { get; private set; }


        //每种城市的发展速度
        public static float GreenDevelopStep { get; private set; }
        public static float OilDevelopStep { get; private set; }
        public static float EducationDevelopStep { get; private set; }
        public static float HealthDevelopStep { get; private set; }
        public static float DiseaseDevelopStep { get; private set; }
        public static float VolienceDevelopStep { get; private set; }




        public static void LoadRules()
        {
            FileLocation fl = FileSystem.Instance.Locate("rules.xml", GameFileLocs.Config);
            GameConfiguration con = new GameConfiguration(fl);// Utils.LoadConfig("rules.xml"); 
            ConfigurationSection sect = con["CityCommon"];

            CityMaxDevelopment = sect.GetSingle("MaxDevelopment", 10000);
            CityDevHealthRate = sect.GetSingle("DevHealthRate", 0.1f);
            CityInitialDevelopment = sect.GetSingle("InitialDevelopment", 1000);
            CityRadius = sect.GetSingle("Radius", 300);
            CityDevRBallHealthRate = sect.GetSingle("DevRBallHealthRate");
            CityArmor = sect.GetSingle("Armor");

            sect = con["NRCommon"];
            FRecoverBias = sect.GetSingle("ORecoverBias", 1);
            ORecoverBias = sect.GetSingle("FRecoverBias", 1);
            FRecoverRate = sect.GetSingle("FRecoverRate", 0.0015f);

            sect = con["HarvCommon"];

            HarvLoadingSpeed = sect.GetSingle("LoadingSpeed", 2.5f);
            HarvLoadingTime = sect.GetSingle("HarvLoadingTime", 3);

            #region 各种资源球配置读取
            sect = con["OilBall"];
            OilBallBaseHeal = sect.GetSingle("BaseHealth");
            OilBallCost = sect.GetSingle("Cost");
            OilBallContribution = sect.GetSingle("Contribution");
            OilBallBaseDamage = sect.GetSingle("BaseDamage");
            OilBallBaseHeal = sect.GetSingle("BaseHeal");

            sect = con["GreenBall"];
            GreenBallBaseHeal = sect.GetSingle("BaseHealth");
            GreenBallCost = sect.GetSingle("Cost");
            GreenBallContribution = sect.GetSingle("Contribution");
            GreenBallBaseDamage = sect.GetSingle("BaseDamage");
            GreenBallBaseHeal = sect.GetSingle("BaseHeal");

            sect = con["EducationBall"];
            EducationBallBaseHeal = sect.GetSingle("BaseHealth");
            EducationBallGenInterval = sect.GetSingle("GenInterval");
            EducationBallContribution = sect.GetSingle("Contribution");
            EducationBallBaseDamage = sect.GetSingle("BaseDamage");
            EducationBallBaseHeal = sect.GetSingle("BaseHeal");

            sect = con["HealthBall"];
            EducationBallBaseHeal = sect.GetSingle("BaseHealth");
            HealthDevelopStep = sect.GetSingle("GenInterval");
            HealthBallContribution = sect.GetSingle("Contribution"); ;
            HealthBallBaseDamage = sect.GetSingle("BaseDamage");
            HealthBallBaseHeal = sect.GetSingle("BaseHeal");

            sect = con["DiseaseBall"];
            DiseaseBallBaseHeal = sect.GetSingle("BaseHealth");
            DiseaseDevelopStep = sect.GetSingle("GenInterval");
            DiseaseBallContribution = sect.GetSingle("Contribution");
            DiseaseBallBaseDamage = sect.GetSingle("BaseDamage");
            DiseaseBallBaseHeal = sect.GetSingle("BaseHeal");

            sect = con["VolienceBall"];
            VolienceBallBaseHeal = sect.GetSingle("BaseHealth");
            VolienceDevelopStep = sect.GetSingle("GenInterval");
            VolienceBallContribution = sect.GetSingle("Contribution");
            VolienceBallBaseDamage = sect.GetSingle("BaseDamage");
            VolienceBallBaseHeal = sect.GetSingle("BaseHeal");

            #endregion

            #region 各种城市的配置读取
            sect = con["OilCity"];
            OilGatherDistance = sect.GetSingle("GatherDistance", 150);
            OilHarvHP = sect.GetSingle("HarvHealth", 300);
            OilHarvStorage = sect.GetSingle("HarvStorage", 300);
            OilHarvSpeed = sect.GetSingle("HarvSpeed", 1);
            OilDevelopStep = sect.GetSingle("DevelopStep", 10);

            sect = con["GreenCity"];
            GreenGatherDistance = sect.GetSingle("GatherDistance", 150);
            GreenHarvHP = sect.GetSingle("HarvHealth", 300);
            GreenHarvStorage = sect.GetSingle("HarvStorage", 300);
            GreenHarvSpeed = sect.GetSingle("HarvSpeed", 1);
            GreenDevelopStep = sect.GetSingle("DevelopStep", 10);

            sect = con["DiseaseCity"];
            DiseaseDevelopStep = sect.GetSingle("DevelopStep", 10);

            sect = con["VolienceCity"];
            VolienceDevelopStep = sect.GetSingle("VolienceCity", 10);

            sect = con["HealthCity"];
            HealthDevelopStep = sect.GetSingle("HealthCity", 10);

            sect = con["EducationCity"];
            EducationDevelopStep = sect.GetSingle("EducationCity", 10);
            #endregion
        }


    }
}
