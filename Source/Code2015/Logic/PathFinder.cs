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
        FastList<AStarNode> path;
        bool requiresContinuePathFinding;

        public bool RequiresPathFinding
        {
            get { return requiresContinuePathFinding; }
        }

        public PathFinderResult(FastList<AStarNode> path, bool rcpf)
        {
            this.path = path;
            requiresContinuePathFinding = rcpf;
        }

        public AStarNode this[int idx]
        {
            get { return path[idx]; }
        }
        public int NodeCount
        {
            get { return path.Count; }
        }
    }
    //public struct PathNode
    //{
    //    public int X;
    //    public int Y;
    //    public int Z;

    //    public PathNode(int x, int y, int z) 
    //    {
    //        X = x;
    //        Y = y;
    //        Z = z;
    //    }
    //}
    public class AStarNode
    {
        public int X;
        public int Y;

        public float f;
        public float g;
        public float h;

        public int depth;
        //public int pX;
        //public int pY;
        //public int pZ;
        public AStarNode parent;

        //public AStarNode() { }
        public AStarNode(int x, int y)
        {
            X = x;
            Y = y;
        }
        public override int GetHashCode()
        {
            return (X << 16) | (Y);
        }
    }
    public enum CellTraffic : byte
    {
        Clear = 0,
        Jam,
        Blocked
    }
    public class PathFinderManager
    {
        CellTraffic[][] traffic;
        BitTable terrain;

        AStarNode[][] units;

        ///// <summary>
        ///// Weight value for differents <seealso cref="R3D.IsoMap.TerrainType">TerrainType</seealso>s
        ///// </summary>
        //[Obsolete()]
        //float[] terrainWeights;


        public PathFinderManager(CellTraffic[][] traffic, BitTable terr)
        {
            this.traffic = traffic;
            terrain = terr;
            units = new AStarNode[traffic.Length][];
            for (int i = 0; i < units.Length; i++)
            {
                units[i] = new AStarNode[traffic[i].Length];
                for (int j = 0; j < units[i].Length; j++)
                {
                    units[i][j] = new AStarNode(i, j);
                }
            }
        }

        public PathFinder CreatePathFinder()
        {
            return new PathFinder(traffic, terrain, units);
        }

        public CellTraffic[][] TrafficTable
        {
            get { return traffic; }
        }
        public BitTable TerrainTable
        {
            get { return terrain; }
        }

        public int Width
        {
            get { return traffic.Length; }
        }
        public int Height
        {
            get { return traffic[0].Length; }
        }

        public AStarNode[][] AStarNodes
        {
            get { return units; }
        }
    }

    /// <summary>
    /// Each techno should have a path finder in order to do pathfinding at runtime
    /// 每个部队应该有一个PathFinder对象，以便实时寻路
    /// </summary>
    public class PathFinder
    {
        [Flags()]
        enum PassState
        {
            None = 0,
            Open = 1 << 0,
            Close = 1 << 1
        }

        const int MaxStep = 50;

        AStarNode[][] units;

        CellTraffic[][] traffic;
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

        FastList<AStarNode> result = new FastList<AStarNode>();

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
            this.traffic = mgr.TrafficTable;
            this.terrain = mgr.TerrainTable;
            this.width = mgr.Width;
            this.height = mgr.Height;
            this.units = mgr.AStarNodes;
        }
        public PathFinder(CellTraffic[][] traffic, BitTable terr, AStarNode[][] units)
        {
            this.traffic = traffic;
            this.terrain = terr;
            this.units = units;


            this.width = traffic.Length;
            this.height = traffic[0].Length;
        }

        public CellTraffic[][] TrafficTable
        {
            get { return traffic; }
            set { traffic = value; }
        }
        public BitTable Terrain
        {
            get { return terrain; }
            set { terrain = value; }
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
                return new PathFinderResult(new FastList<AStarNode>(), false);
            }

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
                for (int i = 0; i < 26; i++)
                {
                    int nx = cx + stateEnum[i][0];
                    int ny = cy + stateEnum[i][1];

                    bool zShifted = stateEnum[i][2] != 0;



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
                            //if (zShifted || ((terrain[nx][ny] & TerrainType.BirdgeSE) == 0))
                            //{

                            //}


                            if (traffic[nx][ny] != CellTraffic.Blocked && // 畅通
                                terrain.GetBit(ny * width + nx))  //地块能通过
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
                                    //if (np.parent == null)
                                    //    throw new Exception();

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
                    result.Add((AStarNode)null);
                }
                do
                {
                    //result.Add(curNode);
                    result[baseOffset + curNode.depth - 1] = curNode;
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
                    result.Add((AStarNode)null);
                }
                do
                {
                    //result.Add(curNode);
                    result[curNode.depth - 1] = curNode;
                    curNode = curNode.parent;
                }
                while (curNode.parent != null);

                return new PathFinderResult(result, false);
            }
            return null;
        }

    }
}
