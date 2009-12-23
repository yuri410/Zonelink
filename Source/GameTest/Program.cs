using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Code2015.World;
using Apoc3D.MathLib;

namespace GameTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(PlanetEarth.GetTileWidth(MathEx.Degree2Radian(60), MathEx.Degree2Radian(10)));
            Console.WriteLine(PlanetEarth.GetTileHeight(MathEx.Degree2Radian(10)));
            Console.ReadKey();
        }
    }
}
