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
            float rad10 = MathEx.Degree2Radian(10);
            Console.WriteLine(Vector3.Distance(PlanetEarth.GetPosition(rad10, 0), PlanetEarth.GetPosition(rad10, rad10)));
            Console.WriteLine(Vector3.Distance(PlanetEarth.GetPosition(0, rad10), PlanetEarth.GetPosition(rad10, rad10)));

            Console.WriteLine(MathEx.Radian2Degree(PlanetEarth.GetTileArcAngle(90 * 6)));
            Console.WriteLine(PlanetEarth.GetTileHeight(rad10));
            Console.WriteLine(PlanetEarth.GetTileWidth(rad10, rad10));



            Console.WriteLine(PlanetEarth.GetPosition(0, rad10));

            Console.ReadKey();
        }
    }
}
