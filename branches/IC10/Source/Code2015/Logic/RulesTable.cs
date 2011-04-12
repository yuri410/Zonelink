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

        //public const float RBallProduceBall = 100;


        public static float OilBallCost { get; private set; }
        public static float GreenBallCost { get; private set; }
        public static float EducationBallInterval { get; private set; }
        public static float HealthBallInterval { get; private set; }
        public static float DiseaseBallInterval { get; private set; }
        public static float VolienceBallInterval { get; private set; }

        //每种城市的发展速度
        public static float GreenDevelopStep { get; private set; }
        public static float OilDevelopStep { get; private set; }
        public static float EducationDevelopStep { get; private set; }
        public static float HealthDevelopStep { get; private set; }
        public static float DiseaseDevelopStep { get; private set; }
        public static float VolienceDevelopStep { get; private set; }

        //每种资源球对城市的贡献量
        public static float OilBallContribution { get; private set; }
        public static float GreenBallContribution { get; private set; }
        public static float EducationBallContribution { get; private set; }
        public static float HealthBallContribution { get; private set; }
        public static float DiseaseBallContribution { get; private set; }
        public static float VolienceBallContribution { get; private set; }


        public static void LoadRules()
        {
            FileLocation fl = FileSystem.Instance.Locate("rules.xml", GameFileLocs.Config);
            GameConfiguration con = new GameConfiguration(fl);// Utils.LoadConfig("rules.xml"); 
            ConfigurationSection sect = con["CityCommon"];

            CityMaxDevelopment = sect.GetSingle("MaxDevelopment", 10000);
            CityDevHealthRate = sect.GetSingle("DevHealthRate", 0.1f);
            CityInitialDevelopment = sect.GetSingle("InitialDevelopment", 1000);
            CityRadius = sect.GetSingle("Radius", 300);

            sect = con["NRCommon"];
            FRecoverBias = sect.GetSingle("ORecoverBias", 1);
            ORecoverBias = sect.GetSingle("FRecoverBias", 1);
            FRecoverRate = sect.GetSingle("FRecoverRate", 0.0015f);

            sect = con["HarvCommon"];

            HarvLoadingSpeed = sect.GetSingle("LoadingSpeed", 2.5f);
            HarvLoadingTime = sect.GetSingle("HarvLoadingTime", 3);

            #region 各种资源球配置读取
            sect = con["OilBall"];
            OilBallCost = sect.GetSingle("Cost", 15);
            OilBallContribution = sect.GetSingle("Contribution", 10);

            sect = con["GreebBall"];
            GreenBallCost = sect.GetSingle("Cost", 15);
            GreenBallContribution = sect.GetSingle("Contribution", 10);

            sect = con["EducationBall"];
            EducationBallInterval = sect.GetSingle("GenInterval", 15);
            EducationBallContribution = sect.GetSingle("Contribution", 10); 

            sect = con["HealthBall"];
            HealthDevelopStep = sect.GetSingle("GenInterval", 15);
            HealthBallContribution = sect.GetSingle("Contribution", 10); ;

            sect = con["DiseaseBall"];
            DiseaseDevelopStep = sect.GetSingle("GenInterval", 15);
            DiseaseBallContribution = sect.GetSingle("Contribution", 10);

            sect = con["VolienceBall"];
            VolienceDevelopStep = sect.GetSingle("GenInterval", 15);
            VolienceBallContribution = sect.GetSingle("Contribution", 10); 

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
