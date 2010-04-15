using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Config;

namespace Code2015.BalanceSystem
{
    public enum PlantCategory
    {
        Grass,
        Bush,
        Forest,
    }
    public enum PlantType
    {
        TemperateZone,
        Subtropics,
        Tropics
    }

    public class Forest : NaturalResource
    {
        /// <summary>
        ///  
        /// </summary>
        [SLGValueAttribute()]
        const float AbsorbCarbonRate = 1000;

        [SLGValue]
        public const float MaxAmount = 10000;

        [SLGValue]
        const float RecoverRate = 0.015f;
        [SLGValue]
        const float RecoverBias = 2f;

        public float AbsorbCarbonSpeed
        {
            get;
            private set;
        }

        public PlantCategory Category
        {
            get;
            private set;
        }
        public PlantType PlantSize
        {
            get;
            private set;
        }
        public float Radius
        {
            get;
            private set;
        }


        public Forest(SimulationWorld region)
            : base(region, NaturalResourceType.Wood)
        {

        }



        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);

            //Category = (PlantCategory)Enum.Parse(typeof(PlantType), sect.GetString("Category", ""));
            //PlantSize = (PlantType)Enum.Parse(typeof(PlantCategory), sect.GetString("Kind", ""));
            Radius = sect.GetSingle("Radius");
        }


        public override void Update(GameTime time)
        {
            float hours = (float)time.ElapsedGameTime.TotalHours;

            if (CurrentAmount < MaxAmount)
            {
                CurrentAmount += (CurrentAmount * RecoverRate + RecoverBias) * hours;
            }
        }

    }
}
