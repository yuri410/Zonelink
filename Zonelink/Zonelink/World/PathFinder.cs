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
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.MathLib;

namespace Code2015.Logic
{
    public class PathFinderResult
    {
        FastList<Point> path;
        bool requiresContinuePathFinding;

        public bool RequiresPathFinding
        {
            get { return requiresContinuePathFinding; }
        }

        public PathFinderResult(FastList<Point> path, bool rcpf)
        {
            this.path = path;
            requiresContinuePathFinding = rcpf;
        }

        public Point this[int idx]
        {
            get { return path[idx]; }
        }
        public int NodeCount
        {
            get { return path.Count; }
        }
    }

    public class AStarNode
    {
        public int X;
        public int Y;


        public float f;
        public float g;
        public float h;

        public int depth;

        public AStarNode parent;


        public AStarNode(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public override int GetHashCode()
        {
            return (X << 16) | Y;
        }
    }
    public class PathFinderManager
    {
        BitTable terrain;

        AStarNode[][] units;

        public const int DW = 36 * 32;
        public const int DH = 14 * 32;

        public AStarNode[][] NodeBuffer
        {
            get { return units; }
        }

        public PathFinderManager(BitTable terr)
        {
            terrain = terr;
            units = new AStarNode[DW][];
            for (int i = 0; i < DW; i++)
            {
                units[i] = new AStarNode[DH];
                for (int j = 0; j < DH; j++)
                {
                    units[i][j] = new AStarNode(i, j);
                }
            }
        }

        public PathFinder CreatePathFinder()
        {
            return new PathFinder(terrain, units);
        }

        public BitTable TerrainTable
        {
            get { return terrain; }
        }

        public int Width
        {
            get { return DW; }
        }
        public int Height
        {
            get { return DH; }
        }

    }

    /// <summary>
    /// Each techno should have a path finder in order to do pathfinding at runtime
    /// 每个部队应该有一个PathFinder对象，以便实时寻路
    /// </summary>
    public class PathFinder
    {
        const int MaxStep = 50;

        AStarNode[][] units;

        BitTable terrain;

        /// <summary>
        /// BFS队列
        /// </summary>
        Queue<AStarNode> queue = new Queue<AStarNode>();

        /// <summary>
        /// 队列中的点 判重哈希表
        /// </summary>
        Dictionary<int, AStarNode> inQueueTable = new Dictionary<int, AStarNode>();

        /// <summary>
        /// 遍历过的点 判重哈希表
        /// </summary>
        Dictionary<int, AStarNode> passedTable = new Dictionary<int, AStarNode>();

        int width;
        int height;

        FastList<Point> result = new FastList<Point>();

        readonly static int[][] stateEnum = new int[8][]
        {
            new int[2] { 0, -1 }, new int[2] { 0, 1 },
            new int[2] { -1, 0 }, new int[2] { 1, 0 },
            new int[2] { -1, -1 }, new int[2] { 1, 1 },
            new int[2] { -1, 1 }, new int[2] { 1, -1 },
        };
        readonly static float[] stateEnumCost = new float[8]
        {
            1, 1,
            1, 1,
            MathEx.Root2, MathEx.Root2,
            MathEx.Root2, MathEx.Root2,
        };

        public PathFinder(PathFinderManager mgr)
        {
            this.terrain = mgr.TerrainTable;
            this.width = mgr.Width;
            this.height = mgr.Height;
            this.units = mgr.NodeBuffer;
        }
        public PathFinder(BitTable terr, AStarNode[][] units)
        {
            this.terrain = terr;
            this.units = units;

            this.width = PathFinderManager.DW;
            this.height = PathFinderManager.DH;
        }

        public BitTable Terrain
        {
            get { return terrain; }
        }


        //int Comparision(AStarNode a, AStarNode b)
        //{
        //    return a.f.CompareTo(b.f);
        //}

        void QuickSort(FastList<AStarNode> list, int l, int r)
        {
            int i;
            int j;
            do
            {
                i = l;
                j = r;
                AStarNode p = list[(l + r) >> 1];

                do
                {
                    while (list[i].f < p.f)
                        i++;
                    while (list[j].f > p.f)
                        j--;
                    if (i <= j)
                    {
                        AStarNode t = list.Elements[i];
                        list.Elements[i] = list.Elements[j];
                        list.Elements[j] = t;
                        i++;
                        j--;
                    }
                }
                while (i <= j);
                if (l < j)
                    QuickSort(list, l, j);
                l = i;
            }
            while (i < r);
        }

        /// <summary>
        /// 准备新的寻路
        /// </summary>
        public void Reset()
        {
            queue.Clear();
            inQueueTable.Clear();
            passedTable.Clear();
            result.Clear();
        }

        /// <summary>
        /// 继续未完成的寻路
        /// </summary>
        public void Continue()
        {
            result.Clear();
        }

        public PathFinderResult FindPath(int sx, int sy, int tx, int ty)
        {
            if (sx == tx && sy == ty)
            {
                return new PathFinderResult(new FastList<Point>(), false);
            }

            int ofsX = Math.Min(sx, tx);
            int ofsY = Math.Min(sy, ty);

            FastList<AStarNode> enQueueBuffer = new FastList<AStarNode>(10);

            AStarNode startNode = units[sx][sy];
            startNode.parent = null;
            startNode.h = 0;
            startNode.g = 0;
            startNode.f = 0;
            startNode.depth = 0;

            queue.Enqueue(startNode);
            inQueueTable.Add(startNode.GetHashCode(), startNode);

            bool found = false;
            bool rcpf = false;

            AStarNode finalNode = null;

            while (queue.Count > 0 && !(found || rcpf))
            {
                AStarNode curPos = queue.Dequeue(); //将open列表中最前面的元素设为当前格
                int curHash = curPos.GetHashCode();

                if (curPos.depth > MaxStep)
                {
                    rcpf = true;
                    finalNode = curPos;
                    break;
                }


                inQueueTable.Remove(curHash);
                passedTable.Add(curHash, curPos);

                int cx = curPos.X;
                int cy = curPos.Y;

                // BFS展开新节点
                for (int i = 0; i < 8; i++)
                {
                    int nx = cx + stateEnum[i][0];
                    int ny = cy + stateEnum[i][1];

                    // 检查范围
                    if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                    {
                        AStarNode np = units[nx][ny];
                        int npHash = np.GetHashCode();


                        if (nx == tx && ny == ty) //如此方格为终点
                        {
                            found = true; //找到路径了
                            finalNode = np;

                            np.depth = curPos.depth + 1;
                            np.parent = curPos;  //当前格坐标为终点的父方格坐标
                            break;
                        }
                        else
                        {
                            if (!terrain.GetBit(ny * width + nx))  //地块能通过
                            {
                                bool isNPInQueue = false;
                                AStarNode temp;
                                if (inQueueTable.TryGetValue(npHash, out temp) && temp == np)
                                {
                                    if (np.g > curPos.g + stateEnumCost[i])
                                    {
                                        np.g = curPos.g + stateEnumCost[i];
                                        np.f = np.g + np.h;
                                    }
                                    isNPInQueue = true;
                                }

                                if (!isNPInQueue &&
                                    (!passedTable.TryGetValue(npHash, out temp) && temp != np))
                                //如果此方格不在即将展开的节点表 和 已遍历过的节点表
                                {
                                    np.parent = curPos; //当前格为此格的父方格

                                    np.g = curPos.g + stateEnumCost[i];
                                    np.h = Math.Abs(tx - nx) + Math.Abs(ty - ny);
                                    np.f = np.g + np.h;
                                    np.depth = curPos.depth + 1;

                                    enQueueBuffer.Add(np);
                                    inQueueTable.Add(npHash, np);
                                }
                            }
                        }
                    }

                }

                // A*
                //enQueueBuffer.Sort(Comparision);
                if (enQueueBuffer.Count > 0)
                {
                    QuickSort(enQueueBuffer, 0, enQueueBuffer.Count - 1);
                    for (int i = 0; i < enQueueBuffer.Count; i++)
                    {
                        queue.Enqueue(enQueueBuffer[i]);
                    }
                    enQueueBuffer.Clear();
                }
            }

            if (rcpf)
            {
                AStarNode curNode = finalNode;
                int baseOffset = result.Count;
                for (int i = 0; i < curNode.depth; i++)
                {
                    result.Add(Point.Zero);
                }
                do
                {
                    //result.Add(curNode);
                    result[baseOffset + curNode.depth - 1] = new Point(curNode.X, curNode.Y);
                    curNode = curNode.parent;
                }
                while (curNode.parent != null);

                return new PathFinderResult(result, true);
            }
            if (found)
            {
                AStarNode curNode = finalNode;
                for (int i = 0; i < curNode.depth; i++)
                {
                    result.Add(Point.Zero);
                }
                do
                {
                    //result.Add(curNode);
                    result[curNode.depth - 1] = new Point(curNode.X, curNode.Y);
                    curNode = curNode.parent;
                }
                while (curNode.parent != null);

                return new PathFinderResult(result, false);
            }
            return null;
        }

    }
}
