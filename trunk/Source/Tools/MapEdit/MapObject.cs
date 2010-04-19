using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Config;
using Apoc3D.MathLib;
using Code2015.BalanceSystem;

namespace MapEdit
{
    [Flags]
    enum ObjectType
    {
        City = 1,
        ResWood = 1 << 1,
        ResOil = 1 << 2,
        Sound = 1 << 3,
        Scene = 1 << 4
    }

    class MapCity
    {
        public MapCity() { }
        public MapCity(SimulationWorld world, ConfigurationSection sect)
        {
            City city = new City(world);
            city.Parse(sect);

            LinkableCity = city.LinkableCityName;
            FarmCount = city.FarmLandCount;
            ProblemEnvironment = city.ProblemEnvironment;
            ProblemEducation = city.ProblemEducation;
            ProblemDisease = city.ProblemDisease;
            ProblemChild = city.ProblemChild;
            ProblemGender = city.ProblemGender;
            ProblemHunger = city.ProblemHunger;
            ProblemMaternal = city.ProblemMaternal;

            Name = city.Name;
            Size = city.Size;

            Longitude = city.Longitude;
            Latitude = city.Latitude;

            StartUp = city.StartUp;
        }

        public int StartUp
        {
            get;
            set;
        }
        public UrbanSize Size { get; set; }
        public string Name
        {
            get;
            set;
        }
        public int FarmCount
        {
            get;
            set;
        }

        public float Longitude
        {
            get;
            set;
        }
        public float Latitude { get; set; }

        public float ProblemEnvironment
        {
            get;
            set;
        }
        public float ProblemDisease
        {
            get;
            set;
        }
        public float ProblemGender
        {
            get;
            set;
        }
        public float ProblemHunger
        {
            get;
            set;
        }
        public float ProblemMaternal
        {
            get;
            set;
        }
        public float ProblemChild
        {
            get;
            set;
        }
        public float ProblemEducation
        {
            get;
            set;
        }
        public string[] LinkableCity
        {
            get;
            set;
        }
    }

    class MapResource
    {
        public MapResource() { }
        public MapResource(SimulationWorld world, ConfigurationSection sect)
        {
            string type = sect["Type"].ToLowerInvariant();

            switch (type)
            {
                case "petro":
                    OilField oil = new OilField(world);
                    oil.Parse(sect);

                    Longitude = oil.Longitude;
                    Latitude = oil.Latitude;

                    Type = NaturalResourceType.Petro;
                    Amount = oil.CurrentAmount;

                    break;
                case "wood":
                    Forest fores = new Forest(world);
                    fores.Parse(sect);

                    Longitude = fores.Longitude;
                    Latitude = fores.Latitude;

                    Type = NaturalResourceType.Wood;
                    Amount = fores.CurrentAmount;
                    Radius = fores.Radius;
                    break;
            }

        }

        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public NaturalResourceType Type
        {
            get;
            set;
        }

        public float Amount
        {
            get;
            set;
        }

        public float Radius
        {
            get;
            set;
        }

    }
    class MapSoundObject
    {
        public MapSoundObject() { }
        public MapSoundObject(ConfigurationSection sect)
        {
            Radius = sect.GetSingle("Radius", 0);
            SFXName = sect.GetString("SFX", string.Empty);
        }

        public float Radius
        {
            get;
            set;
        }

        public string SFXName
        {
            get;
            set;
        }

    }

    class MapSceneObject
    {
        public MapSceneObject() { }
        public MapSceneObject(ConfigurationSection sect)
        {
            Radius = sect.GetSingle("Radius", 0);
            IsForest = sect.GetBool("IsForest", false);
            Amount = sect.GetSingle("Amount", 0);
            Model = sect.GetString("Model", string.Empty);
        }

        public float Radius
        {
            get;
            set;
        }


        public bool IsForest
        {
            get;
            set;
        }

        public float Amount
        {
            get;
            set;
        }

        public string Model
        {
            get;
            set;
        }

    }

    class MapObject
    {
        public const int IconHeight = 10;
        public const int IconWidth = 10;

        public static int MapHeight = 1188;
        public static int MapWidth = 462;

        public static void GetMapCoord(float lng, float lat, out int x, out int y)
        {
            float yspan = (14.0f / 18.0f) * MathEx.PIf;

            y = (int)(((yspan * 0.5f - lat) / yspan) * MapHeight);
            x = (int)(((lng + MathEx.PIf) / (2 * MathEx.PIf)) * MapWidth);

            if (y < 0) y += MapHeight;
            if (y >= MapHeight) y -= MapHeight;

            if (x < 0) x += MapWidth;
            if (x >= MapWidth) x -= MapWidth;
        }

        public static void GetCoord(int x, int y, out float lng, out float lat)
        {
            float yspan = (14.0f / 18.0f) * MathEx.PIf;

            lat = yspan * 0.5f - y * yspan / (float)MapHeight;
            lng = x * MathEx.PIf * 2 / (float)MapWidth - MathEx.PIf;
        }

        public bool Intersects(int x, int y)
        {
            return Math.Abs(x - X) < IconWidth / 2 && Math.Abs(y - Y) < IconHeight / 2;
        }

        public float Longitude
        {
            get;
            set;
        }

        public float Latitude
        {
            get;
            set;
        }

        public object Tag
        {
            get;
            set;
        }

        public int X
        {
            get
            {
                int x, y;
                GetMapCoord(MathEx.Degree2Radian(Longitude), MathEx.Degree2Radian(Latitude), out x, out y);
                return x;
            }
            set
            {
                int x, y;
                GetMapCoord(MathEx.Degree2Radian(Longitude), MathEx.Degree2Radian(Latitude), out x, out y);

                float lng, lat;
                GetCoord(value, y, out lng, out lat);

                Longitude = MathEx.Radian2Degree(lng);
                Latitude = MathEx.Radian2Degree(lat);
            }
        }
        
        public int Y
        {
            get
            {
                int x, y;
                GetMapCoord(MathEx.Degree2Radian(Longitude), MathEx.Degree2Radian(Latitude), out x, out y);
                return y;
            }
            set
            {
                int x, y;
                GetMapCoord(MathEx.Degree2Radian(Longitude), MathEx.Degree2Radian(Latitude), out x, out y);

                float lng, lat;
                GetCoord(x, value, out lng, out lat);

                Longitude = MathEx.Radian2Degree(lng);
                Latitude = MathEx.Radian2Degree(lat);
            }
        }


        public string SectionName
        {
            get;
            set;
        }
        public ObjectType Type
        {
            get;
            set;
        }
        public string StringDisplay
        {
            get;
            set;
        }
        public int Radius
        {
            get;
            set;
        }

        public bool IsSelected
        {
            get;
            set;
        }

    }
}
