using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.MathLib;

namespace Code2015.EngineEx
{
    public class TerrainBlock
    {
        #region 属性

        public int X
        {
            get;
            private set;
        }

        public int Y
        {
            get;
            private set;
        }

        public int Width
        {
            get { return TerrainMesh.TerrainBlockSize; }
        }

        public int Height
        {
            get { return TerrainMesh.TerrainBlockSize; }
        }

        public float Radius
        {
            get;
            set;
        }

        public Vector3 Center
        {
            get;
            set;
        }

        /// <summary>
        ///  包含一系列不同细节的IB。
        /// </summary>
        public IndexBuffer[] IndexBuffers
        {
            get;
            set;
        }

        public GeomentryData GeoData
        {
            get;
            set;
        }

        #endregion

        public TerrainBlock(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    public class TerrainTreeNode : IDisposable
    {
        #region 字段

        public BoundingSphere BoundingVolume;

        TerrainTreeNode[] children;

        #endregion

        #region 属性

        public TerrainTreeNode[] Children
        {
            get { return children; }
        }

        public TerrainBlock Block
        {
            get;
            private set;
        }
        #endregion

        public TerrainTreeNode(FastList<TerrainBlock> blocks, int ofsX, int ofsY, int depth, int terrainSize)
        {
            if (blocks != null)
            {
                if (blocks.Count > 1)
                {
                    //children = new TerrainTreeNode[4];

                    int averageBlockCount = blocks.Count / 4;
                    FastList<TerrainBlock> tlBlocks = new FastList<TerrainBlock>(averageBlockCount);
                    FastList<TerrainBlock> trBlocks = new FastList<TerrainBlock>(averageBlockCount);
                    FastList<TerrainBlock> blBlocks = new FastList<TerrainBlock>(averageBlockCount);
                    FastList<TerrainBlock> brBlocks = new FastList<TerrainBlock>(averageBlockCount);



                    for (int i = 0; i < blocks.Count; i++)
                    {
                        BoundingVolume.Center += blocks[i].Center;

                        int cx = blocks[i].X;// + Terrain.BlockEdgeLen / 2;
                        int cy = blocks[i].Y;// +Terrain.BlockEdgeLen / 2;

                        if (cx < ofsX)
                        {
                            if (cy < ofsY)
                            {
                                tlBlocks.Add(blocks[i]);
                            }
                            else
                            {
                                blBlocks.Add(blocks[i]);
                            }
                        }
                        else
                        {
                            if (cy < ofsY)
                            {
                                trBlocks.Add(blocks[i]);
                            }
                            else
                            {
                                brBlocks.Add(blocks[i]);
                            }
                        }
                    }

                    BoundingVolume.Center /= (float)blocks.Count;

                    for (int i = 0; i < blocks.Count; i++)
                    {
                        float dist = Vector3.Distance(blocks[i].Center, BoundingVolume.Center) + blocks[i].Radius;
                        if (dist > BoundingVolume.Radius)
                        {
                            BoundingVolume.Radius = dist;
                        }
                    }

                    BoundingVolume.Radius += TerrainMesh.TerrainBlockSize * MathEx.Root2 * 0.5f;


                    int childrenCount = 0;
                    TerrainTreeNode[] ch1 = new TerrainTreeNode[4];

                    depth++;
                    int div = (int)Math.Pow(2, depth);

                    if (tlBlocks.Count > 0)
                    {
                        ch1[childrenCount++] = new TerrainTreeNode(tlBlocks,
                            ofsX - terrainSize / div,
                            ofsY - terrainSize / div, depth, terrainSize);
                    }

                    if (trBlocks.Count > 0)
                    {
                        ch1[childrenCount++] = new TerrainTreeNode(trBlocks,
                            ofsX + terrainSize / div,
                            ofsY - terrainSize / div, depth, terrainSize);
                    }

                    if (blBlocks.Count > 0)
                    {
                        ch1[childrenCount++] = new TerrainTreeNode(blBlocks,
                            ofsX - terrainSize / div,
                            ofsY + terrainSize / div, depth, terrainSize);
                    }

                    if (brBlocks.Count > 0)
                    {
                        ch1[childrenCount++] = new TerrainTreeNode(brBlocks,
                            ofsX + terrainSize / div,
                            ofsY + terrainSize / div, depth, terrainSize);
                    }

                    children = new TerrainTreeNode[childrenCount];
                    for (int i = 0; i < childrenCount; i++)
                    {
                        children[i] = ch1[i];
                    }
                }
                else
                {
                    this.Block = blocks[0];
                    BoundingVolume.Center = blocks[0].Center;
                    BoundingVolume.Radius = blocks[0].Radius;
                }
            }
        }

        #region 方法

        public void Update(TerrainBlock[][] blocks)
        {
            if (children != null)
            {
                BoundingVolume.Center = Vector3.Zero;

                for (int i = 0; i < children.Length; i++)
                {
                    children[i].Update(blocks);

                    BoundingVolume.Center += children[i].BoundingVolume.Center;
                }

                BoundingVolume.Center /= (float)children.Length;


                BoundingVolume.Radius = 0;
                for (int i = 0; i < children.Length; i++)
                {
                    float dist = Vector3.Distance(children[i].BoundingVolume.Center, BoundingVolume.Center) + children[i].BoundingVolume.Radius;
                    if (dist > BoundingVolume.Radius)
                    {
                        BoundingVolume.Radius = dist;
                    }
                }


            }
            else
            {
                BoundingVolume.Center = Block.Center;
                BoundingVolume.Radius = Block.Radius;
            }

        }

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            children = null;
        }

        #endregion
    }
}
