using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Apoc3D.MathLib;
using Code2015.World;
using DevIl;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using Apoc3D.Config;
using Code2015.EngineEx;

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
        static void Vec2Ang()
        {
            for (int i = 0; i < 36; i++)
            {
                Console.Write(i * 10);
                Console.Write(' ');
                if ((i + 1) % 12 == 0)
                    Console.WriteLine();
            }
            Console.WriteLine();

            for (int i = 0; i < 36; i++)
            {
                float rad = MathEx.Degree2Radian(i * 10);

                Vector2 a = new Vector2((float)Math.Cos(rad), (float)Math.Sin(rad));

                float ang = MathEx.Vector2DirAngle(a);
                ang = MathEx.Radian2Degree(ang);
                //ang = -ang;

                Console.Write(ang);
                Console.Write(' ');
                if ((i + 1) % 12 == 0)
                    Console.WriteLine();
            }
        }
        static void PlanetPosition()
        {
            for (int i = 0; i < 90; i++)
            {
                float deg =  MathEx.Degree2Radian(i);

                Console.WriteLine("degree {0}: {1}", i, PlanetEarth.GetPosition(1, deg));
            }
        }

        unsafe static void Main2(string[] args)
        {
            Bitmap bmp = new Bitmap(11601, 10801);
            BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, 11601, 10801), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            BinaryReader br = new BinaryReader(File.Open(@"E:\Desktop\gebco_bathy.21601x10801.bin", FileMode.Open));

            byte* dst = (byte*)data.Scan0;

            for (int i = 0; i < 11601 * 10801; i++)
            {
                br.ReadByte();
                byte v = br.ReadByte();
                *dst++ = 0xff;
                *dst++ = v;
                *dst++ = v;
                *dst++ = v;

                //Half h = new Half(br.ReadUInt16());

                //float v = h.ToSingle();
                //if (v < 0) v = -v;


                //byte b = (byte)(byte.MaxValue * (v / short.MaxValue));

                //*dst++ = b;
                //*dst++ = b;
                //*dst++ = b;
                //Console.WriteLine(h);
                //if (i % 10000 == 0)
                //{
                //    Thread.Sleep(1000);
                //}
            }

            bmp.UnlockBits(data);

            bmp.Save(@"E:\Desktop\sample.png", ImageFormat.Png);

            br.Close();
            bmp.Dispose();

            //float rad10 = MathEx.Degree2Radian(10);
            //Console.WriteLine(Vector3.Distance(PlanetEarth.GetPosition(rad10, 0), PlanetEarth.GetPosition(rad10, rad10)));
            //Console.WriteLine(Vector3.Distance(PlanetEarth.GetPosition(0, rad10), PlanetEarth.GetPosition(rad10, rad10)));

            //Console.WriteLine(MathEx.Radian2Degree(PlanetEarth.GetTileArcAngle(90 * 6)));
            //Console.WriteLine(PlanetEarth.GetTileHeight(rad10));
            //Console.WriteLine(PlanetEarth.GetTileWidth(rad10, rad10));

            //Console.WriteLine(PlanetEarth.GetPosition(0, rad10));
            //TestIl();
            //PlanetPosition();
            Console.ReadKey();
        }

        unsafe static void Main(string[] args)
        {
            ConfigurationManager.Initialize();
            ConfigurationManager.Instance.Register(new GameConfigurationFormat());

            ConfigurationManager.Instance.CreateInstance(@"");

        }
    }
}
