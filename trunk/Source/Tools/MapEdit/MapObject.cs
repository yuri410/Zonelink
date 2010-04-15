using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.MathLib;

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

    class MapSoundObject
    {
        public float Radius;
        public string SFXName;
    }

    class MapSceneObject
    {
        public float Radius;

        public bool IsForest;
        public float Amount;
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
                GetMapCoord(Longitude, Latitude, out x, out y);
                return x;
            }
        }
        public int Y
        {
            get
            {
                int x, y;
                GetMapCoord(Longitude, Latitude, out x, out y);
                return y;
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


    }
}
