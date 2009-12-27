using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Apoc3D.MathLib;
using Code2015.World;
using DevIl;

namespace GameTest
{
    class Program
    {
        static void TestIl()
        {
            Il.ilInit();

            int image = Il.ilGenImage();

            Il.ilBindImage(image);
            Il.ilLoadImage(@"E:\Desktop\jlxphys1.dds");

            int mipCount = Il.ilGetInteger(Il.IL_NUM_MIPMAPS) + 1;

           
            Console.WriteLine(" Format: {0}", Il.ilGetInteger(Il.IL_IMAGE_FORMAT).ToString());

            Console.WriteLine(" Type: {0}", Il.ilGetInteger(Il.IL_IMAGE_TYPE).ToString());
            Console.WriteLine(" Mipmap Count: {0}", mipCount);
            //Console.WriteLine(" DT: {0}", Il.ilGetInteger (Il.IL_
            for (int i = 0; i < mipCount; i++)
            {
                Il.ilBindImage(image);

                Il.ilActiveMipmap(i);
                Console.WriteLine("Layer: {0}", i);
                Console.WriteLine(" Width: {0}", Il.ilGetInteger(Il.IL_IMAGE_WIDTH).ToString());
                Console.WriteLine(" Height: {0}", Il.ilGetInteger(Il.IL_IMAGE_HEIGHT).ToString());
                Console.WriteLine(" Depth: {0}", Il.ilGetInteger(Il.IL_IMAGE_DEPTH).ToString());

                Console.WriteLine(" Byte per pix: {0}", Il.ilGetInteger(Il.IL_IMAGE_BYTES_PER_PIXEL).ToString());
                Console.WriteLine(" Data Size: {0}", Il.ilGetInteger(Il.IL_IMAGE_SIZE_OF_DATA).ToString());
                Console.WriteLine(" Data: {0}", Il.ilGetData());
            }

            Il.ilDeleteImage(image);
        }

        static void Main(string[] args)
        {
            //float rad10 = MathEx.Degree2Radian(10);
            //Console.WriteLine(Vector3.Distance(PlanetEarth.GetPosition(rad10, 0), PlanetEarth.GetPosition(rad10, rad10)));
            //Console.WriteLine(Vector3.Distance(PlanetEarth.GetPosition(0, rad10), PlanetEarth.GetPosition(rad10, rad10)));

            //Console.WriteLine(MathEx.Radian2Degree(PlanetEarth.GetTileArcAngle(90 * 6)));
            //Console.WriteLine(PlanetEarth.GetTileHeight(rad10));
            //Console.WriteLine(PlanetEarth.GetTileWidth(rad10, rad10));

            //Console.WriteLine(PlanetEarth.GetPosition(0, rad10));
            TestIl();
            Console.ReadKey();
        }
    }
}
