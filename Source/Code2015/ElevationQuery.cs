using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;

namespace Code2015
{
    class ElevotionQuery
    {
        static ElevotionQuery singleton;

        public static ElevotionQuery Instance
        {
            get 
            {
                return singleton;
            }
        }

        public static void Initialize()
        {
            singleton = new ElevotionQuery();
        }

        byte[,] data;
        int width;
        int height;
        public ElevotionQuery()
        {
            FileLocation file = FileSystem.Instance.Locate("ElevotionQuery", GameFileLocs.Nature); 
            ContentBinaryReader br = new ContentBinaryReader(file);
            width = br.ReadInt32();
            height = br.ReadInt32();
            data = new byte[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    data[i, j] = br.ReadByte();

                }
            }
        }
        public byte GetData(float longitude, float latitude)
        {

            int y = (int)(0.5f * latitude / Math.PI * height);
            int x = (int)((-longitude + Math.PI / 2) / Math.PI * width);
            if (x < 0)
                x += width;
            if (y < 0)
                y += height;
            if (x >= width)
                x -= width;
            if (y >= height)
                y -= height;

            return data[y, x];
        }
    }
}
