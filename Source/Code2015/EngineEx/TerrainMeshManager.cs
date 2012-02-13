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
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;

namespace Code2015.EngineEx
{
    /// <summary>
    ///  Stores the index data for each size of the terrain tile at a specific LOD, 
    ///  since the mesh in one LOD share the same index data as the topology is unchanged.
    /// </summary>
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
        public SharedIndexData(RenderSystem rs, int terrSize)
        {
            TerrainSize = terrSize;

            ObjectFactory factory = rs.ObjectFactory;

            int primCount = MathEx.Sqr(terrSize) * 2;
            int indexCount = primCount * 3;
            indexBuffer = factory.CreateIndexBuffer(IndexBufferType.Bit32, indexCount, BufferUsage.WriteOnly);
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

    class TerrainMeshManager : ResourceManager
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
                            singleton = new TerrainMeshManager(1048576 * 100);
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

        bool loaded;
        RenderSystem renderSystem;
        //Dictionary<int, SharedBlockIndexData> sharedIBCache = new Dictionary<int, SharedBlockIndexData>();
        SharedIndexData index33;
        SharedIndexData index17;

        private TerrainMeshManager() { }
        private TerrainMeshManager(int cacheSize)
            : base(cacheSize)
        {
        }
        public SharedIndexData GetIndexData(int size)
        {
            if (size == 33)
                return index33;
            return index17;
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
        /// <summary>
        ///  Create a terrain mesh at the given tile-based coordinate and with a given size.
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="size">The size can be either 33 or 17</param>
        /// <returns></returns>
        public ResourceHandle<TerrainMesh> CreateInstance(RenderSystem rs, int x, int y, int size)
        {
            if (!loaded)
            {
                lock (syncHelper)
                {
                    if (!loaded)
                    {
                        loaded = true;
                        renderSystem = rs;

                        index33 = new SharedIndexData(rs, 33);
                        index17 = new SharedIndexData(rs, 17);
                        //SharedBlockIndexData sharedIdxBuffer1025 = new SharedBlockIndexData(rs, 513);
                        //SharedBlockIndexData sharedIdxBuffer257 = new SharedBlockIndexData(rs, 129);
                        //SharedBlockIndexData sharedIdxBuffer65 = new SharedBlockIndexData(rs, 33);
                        //sharedIBCache.Add(513, sharedIdxBuffer1025);
                        //sharedIBCache.Add(129, sharedIdxBuffer257);
                        //sharedIBCache.Add(33, sharedIdxBuffer65);
                    }
                }
            }
            Resource retrived = base.Exists(TerrainMesh.GetHashString(x, y, size));
            if (retrived == null)
            {
                TerrainMesh mdl = new TerrainMesh(rs, x, y, size);
                retrived = mdl;
                base.NotifyResourceNew(mdl);
            }
            //else
            //{
            //    retrived.Use();
            //}
            return new ResourceHandle<TerrainMesh>((TerrainMesh)retrived);
        }
    }
}
