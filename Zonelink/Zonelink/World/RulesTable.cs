using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Code2015.EngineEx;
using Apoc3D.Config;

namespace Zonelink
{
    static class RulesTable
    {
        public static float CityMaxDevelopment { get; private set; }

        public static float CityDevHealthRate { get; private set; }

        public static float CityInitialDevelopment { get; private set; }
        public static float CityRadius { get; private set; }

        public static float HarvLoadingSpeed { get; private set; }
        public static float HarvLoadingTime { get; private set; }

        public static float OilGatherDistance { get; private set; }
        public static float OilHarvHP { get; private set; }
        public static float OilHarvStorage { get; private set; }
        public static float OilHarvSpeed { get; private set; }

        public static float GreenGatherDistance { get; private set; }
        public static float GreenHarvHP { get; private set; }
        public static float GreenHarvStorage { get; private set; }
        public static float GreenHarvSpeed { get; private set; }

        public static void LoadRules()
        {
            GameConfiguration con = Utils.LoadConfig("rules.xml");
            ConfigurationSection sect = con["CityCommon"];

            CityMaxDevelopment = sect.GetSingle("MaxDevelopment", 10000);
            CityDevHealthRate = sect.GetSingle("DevHealthRate", 0.1f);
            CityInitialDevelopment = sect.GetSingle("InitialDevelopment", 1000);

            CityRadius = sect.GetSingle("Radius", 300);

            sect = con["HarvCommon"];

            HarvLoadingSpeed = sect.GetSingle("LoadingSpeed", 2.5f);
            HarvLoadingTime = sect.GetSingle("HarvLoadingTime", 3);

            sect = con["OilCity"];
            OilGatherDistance = sect.GetSingle("GatherDistance", 150);
            OilHarvHP = sect.GetSingle("HarvHealth", 300);
            OilHarvStorage = sect.GetSingle("HarvStorage", 300);
            OilHarvSpeed = sect.GetSingle("HarvSpeed", 1);

            sect = con["GreenCity"];
            GreenGatherDistance = sect.GetSingle("GatherDistance", 150);
            GreenHarvHP = sect.GetSingle("HarvHealth", 300);
            GreenHarvStorage = sect.GetSingle("HarvStorage", 300);
            GreenHarvSpeed = sect.GetSingle("HarvSpeed", 1);

        }


    }
}
