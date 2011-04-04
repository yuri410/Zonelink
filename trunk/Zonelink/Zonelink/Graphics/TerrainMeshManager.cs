/*
-----------------------------------------------------------------------------
This source file is part of Zonelink

Copyright (c) 2009+ Tao Games

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  if not, write to the Free Software Foundation, 
Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA, or go to
http://www.gnu.org/copyleft/gpl.txt.

-----------------------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Zonelink;
using Zonelink.MathLib;

namespace Code2015.EngineEx
{
    class SharedIndexData
    {
        IndexBuffer indexBuffer;

        public IndexBuffer Index
        {
            get { return indexBuffer; }
        }
        public int TerrainSize
        {
            get;
            private set;
        }
        public SharedIndexData(Game1 rs, int terrSize)
        {
            TerrainSize = terrSize;



            int primCount = MathEx.Sqr(terrSize) * 2;
            int indexCount = primCount * 3;
            indexBuffer = new IndexBuffer(rs.GraphicsDevice, IndexElementSize.ThirtyTwoBits, indexCount, BufferUsage.WriteOnly);
            int[] indexArray = new int[indexCount];

            int idx = 0;
            for (int i = 0; i < terrSize - 1; i++)
            {
                int remi = i % 2;

                for (int j = 0; j < terrSize - 1; j++)
                {
                    int remj = j % 2;
                    if (remi == remj)
                    {
                        indexArray[idx++] = i * terrSize + j;
                        indexArray[idx++] = i * terrSize + (j + 1);
                        indexArray[idx++] = (i + 1) * terrSize + j;


                        indexArray[idx++] = i * terrSize + (j + 1);
                        indexArray[idx++] = (i + 1) * terrSize + (j + 1);
                        indexArray[idx++] = (i + 1) * terrSize + j;
                    }
                    else
                    {
                        indexArray[idx++] = i * terrSize + j;
                        indexArray[idx++] = (i + 1) * terrSize + (j + 1);
                        indexArray[idx++] = i * terrSize + (j + 1);

                        indexArray[idx++] = i * terrSize + j;
                        indexArray[idx++] = (i + 1) * terrSize + j;
                        indexArray[idx++] = (i + 1) * terrSize + (j + 1);
                    }
                }
            }
            indexBuffer.SetData<int>(indexArray);
        }
    }

    class TerrainMeshManager
    {
        static volatile TerrainMeshManager singleton;
        static volatile object syncHelper = new object();

        public static TerrainMeshManager Instance
        {
            get
            {
                if (singleton == null)
                {
                    lock (syncHelper)
                    {
                        if (singleton == null)
                        {
                            singleton = new TerrainMeshManager();
                        }
                    }
                }
                return singleton;
            }
        }

        //public const float TerrainScale = 1;

        //public const float HeightScale = 5500;
        public const float PostZeroLevel = 1100;

        public const float PostHeightScale = 0.04f;//534f;



        Dictionary<long, TerrainMesh> loadedTerrain = new Dictionary<long, TerrainMesh>(100);
        //Dictionary<int, SharedBlockIndexData> sharedIBCache = new Dictionary<int, SharedBlockIndexData>();
        SharedIndexData index33;


        private TerrainMeshManager() { }

        public SharedIndexData GetIndexData() 
        {
            return index33;
        }
        //public SharedBlockIndexData GetSharedIndexData(int terrEdgeSize) 
        //{
        //    SharedBlockIndexData result;
        //    if (!sharedIBCache.TryGetValue(terrEdgeSize, out result ))
        //    {
        //        result = new SharedBlockIndexData(renderSystem, terrEdgeSize);
        //        sharedIBCache.Add(terrEdgeSize, result);
        //    }
        //    return result;
        //}
        public TerrainMesh CreateInstance(Game1 rs, int x, int y)
        {
            if (index33 == null)
            {
                index33 = new SharedIndexData(rs, 33);
            }
            long hash = ((int)x << 32) | ((int)y);
            TerrainMesh result;
            if (loadedTerrain.TryGetValue(hash, out result)) 
            {
                return result;
            }
            TerrainMesh mdl = new TerrainMesh(rs, x, y);
            loadedTerrain.Add(hash, mdl);
            return mdl;

        }
    }
}
