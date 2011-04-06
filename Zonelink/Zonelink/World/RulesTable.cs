﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Code2015.EngineEx;
using Apoc3D.Config;
using System.IO;

namespace Zonelink
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

        public const float RBallProduceBall = 100;

        //每种城市的发展速度
        public static float GreenDevelopStep { get; private set; }
        public static float OilDevelopStep { get; private set; }
        public static float EducationDevelopStep { get; private set; }
        public static float HealthDevelopStep { get; private set; }
        public static float DiseaseDevelopStep { get; private set; }
        public static float VolienceDevelopStep { get; private set; }

        //每种城市产生资源球的时间间隔
        public static float EducationBallInterval { get; private set; }
        public static float HealthBallInterval { get; private set; }
        public static float DiseaseBallInterval { get; private set; }
        public static float VolienceBallInterval { get; private set; }

        public static void LoadRules()
        {
   
            GameConfiguration con = Utils.LoadConfig("rules.xml"); 
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
            DiseaseBallInterval = sect.GetSingle("ProduceBallInterval", 20);

            sect = con["VolienceCity"];
            VolienceDevelopStep = sect.GetSingle("VolienceCity", 10);
            VolienceBallInterval = sect.GetSingle("ProduceBallInterval", 20); 

            sect = con["HealthCity"];
            HealthDevelopStep = sect.GetSingle("HealthCity", 10);
            HealthBallInterval = sect.GetSingle("ProduceBallInterval", 20); 

            sect = con["EducationCity"];
            EducationDevelopStep = sect.GetSingle("EducationCity", 10);
            EducationBallInterval = sect.GetSingle("ProduceBallInterval", 20);

        }


    }
}
