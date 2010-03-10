using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Graphics;
using System.IO;
using Apoc3D.Vfs;

namespace LookHeight
{
    class Program
    {
        static void Main(string[] args)
        {
            QueryHeight q = new QueryHeight(132, 40);
            float h = q.Height;
            Console.WriteLine(h);
            Console.ReadLine();
        }
    }

    class QueryHeight
    {
        TDMPIO td;
        float[] data;
        string file = null;
        public float Height;

        public QueryHeight(float longtitude, float lattiude)
        {
            //int x = 0;
            //int y = 0;
            //if (longtitude / 10 != 0)
            //    longtitude = (int)(longtitude / 10) * 10;
            //if (lattiude / 10 != 0)
            //    lattiude = (int)(lattiude / 10) * 10;
            
            //x = (int)((longtitude + 185) / 5);
            //y = (int)((95 - lattiude) / 5);

            double x = 0;
            double y = 0;
            x = (longtitude + 185) / 5;
            y = (95 - lattiude) / 5;

            int x1 = (int)(Math.Truncate(x));
            if (x1 / 2 == 0)
                x1--;
            int y1 = (int)(Math.Truncate(y));
            if (y1 / 2 == 0)
                y1--;
            string dir = "C:\\Users\\penser\\Documents\\Visual Studio 2008\\Projects\\lrvbsvnicg\\Source\\Code2015\\bin\\x86\\Debug\\terrain.lpk\\";

            file = dir + "tile_" + x1.ToString("D2") + "_" + y1.ToString("D2").ToString() + "_0.tdmp";
            FileLocation fl = new FileLocation(file);

            td = new TDMPIO();
            td.Load(new FileLocation(fl));

            int width = td.Width;
            int height = td.Height;
            data = td.Data;

            float detaX = td.XSpan / width;
            float detaY = td.YSpan / height;

            //td.Xllcorner表示经度，角度制。

            if (longtitude <= 0)
                longtitude = -longtitude;
            int posX = (int)(Math.Abs(longtitude - td.Xllcorner) / detaX);

            if (lattiude <= 0)
                lattiude = -lattiude;
            int posY = (int)(Math.Abs(lattiude - td.Yllcorner) / detaY);

            this.Height = data[posY * 513 + posX];
           
        }

    }
}
