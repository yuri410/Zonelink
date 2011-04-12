using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Collections;
using Apoc3D.MathLib;

namespace Code2015.World
{
    class BallPathFinderResult
    {
        FastList<City> path;

        public BallPathFinderResult(FastList<City> path)
        {
            this.path = path;
        }

        public City this[int idx]
        {
            get { return path[idx]; }
        }
        public int NodeCount
        {
            get { return path.Count; }
        }
    }

    class AStarNodeBall
    {
        public City City;


        public float f;
        public float g;
        public float h;

        public int depth;

        public AStarNodeBall parent;


        public AStarNodeBall(City c)
        {
            this.City = c;
        }
        public override int GetHashCode()
        {
            return City.GetHashCode();
        }
    }
    class BallPathFinderManager
    {
        City[] cityList;
        AStarNodeBall[] units;
        Dictionary<City, AStarNodeBall> nodeTable = new Dictionary<City, AStarNodeBall>();

        public BallPathFinderManager(City[] terr) 
        {
            this.cityList = terr;

            units = new AStarNodeBall[terr.Length];
            for (int i = 0; i < terr.Length; i++) 
            {
                units[i] = new AStarNodeBall(terr[i]);
                nodeTable.Add(terr[i], units[i]);
            }


        }


        public BallPathFinder CreatePathFinder()
        {
            return new BallPathFinder(cityList, units, nodeTable);
        }

    }
    class BallPathFinder
    {
        AStarNodeBall[] units;
        /// <summary>
        /// 城市到节点的表
        /// </summary>
        Dictionary<City, AStarNodeBall> nodeTable;

        City[] terrain;

        /// <summary>
        /// BFS队列
        /// </summary>
        Queue<AStarNodeBall> queue = new Queue<AStarNodeBall>();

        /// <summary>
        /// 队列中的点 判重哈希表
        /// </summary>
        Dictionary<int, AStarNodeBall> inQueueTable = new Dictionary<int, AStarNodeBall>();

        /// <summary>
        /// 遍历过的点 判重哈希表
        /// </summary>
        Dictionary<int, AStarNodeBall> passedTable = new Dictionary<int, AStarNodeBall>();



        FastList<City> result = new FastList<City>();

        //readonly static int[][] stateEnum = new int[8][]
        //{
        //    new int[2] { 0, -1 }, new int[2] { 0, 1 },
        //    new int[2] { -1, 0 }, new int[2] { 1, 0 },
        //    new int[2] { -1, -1 }, new int[2] { 1, 1 },
        //    new int[2] { -1, 1 }, new int[2] { 1, -1 },
        //};
        //readonly static float[] stateEnumCost = new float[8]
        //{
        //    1, 1,
        //    1, 1,
        //    MathEx.Root2, MathEx.Root2,
        //    MathEx.Root2, MathEx.Root2,
        //};


        public BallPathFinder(City[] terr, AStarNodeBall[] units, Dictionary<City, AStarNodeBall> nodeTable)
        {
            this.terrain = terr;
            this.units = units;
            this.nodeTable = nodeTable;
        }

        public City[] Terrain
        {
            get { return terrain; }
        }


        //int Comparision(AStarNode a, AStarNode b)
        //{
        //    return a.f.CompareTo(b.f);
        //}

        void QuickSort(FastList<AStarNodeBall> list, int l, int r)
        {
            int i;
            int j;
            do
            {
                i = l;
                j = r;
                AStarNodeBall p = list[(l + r) >> 1];

                do
                {
                    while (list[i].f < p.f)
                        i++;
                    while (list[j].f > p.f)
                        j--;
                    if (i <= j)
                    {
                        AStarNodeBall t = list.Elements[i];
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

        public BallPathFinderResult FindPath(City start, City target)
        {
            if (start == target)
            {
                return new BallPathFinderResult(new FastList<City>());
            }

            //int ofsX = Math.Min(sx, tx);
            //int ofsY = Math.Min(sy, ty);

            FastList<AStarNodeBall> enQueueBuffer = new FastList<AStarNodeBall>(10);

            AStarNodeBall startNode = nodeTable[start];
            startNode.parent = null;
            startNode.h = 0;
            startNode.g = 0;
            startNode.f = 0;
            startNode.depth = 0;

            queue.Enqueue(startNode);
            inQueueTable.Add(startNode.GetHashCode(), startNode);

            bool found = false;

            AStarNodeBall finalNode = null;

            while (queue.Count > 0 && !(found))
            {
                AStarNodeBall curPos = queue.Dequeue(); //将open列表中最前面的元素设为当前格
                int curHash = curPos.GetHashCode();

                //if (curPos.depth > MaxStep)
                //{
                //    rcpf = true;
                //    finalNode = curPos;
                //    break;
                //}


                inQueueTable.Remove(curHash);
                passedTable.Add(curHash, curPos);

                City cc = curPos.City;
                //int cx = curPos.X;
                //int cy = curPos.Y;
                // BFS展开新节点
                for (int i = 0; i < cc.LinkableCityCount; i++) 
                {
                    City nc = cc.GetLinkableCity(i);


                    AStarNodeBall np = nodeTable[nc];

                    if (np.City == target)
                    {
                        found = true; //找到路径了
                        finalNode = np;

                        np.depth = curPos.depth + 1;
                        np.parent = curPos;  //当前格坐标为终点的父方格坐标
                        break;
                    }
                    else
                    {
                        int npHash = np.GetHashCode();
                        float cost = Vector3.Distance(cc.Position, nc.Position) / 1000.0f;

                        bool isNPInQueue = false;
                        AStarNodeBall temp;
                        if (inQueueTable.TryGetValue(npHash, out temp) && temp == np)
                        {
                            if (np.g > curPos.g + cost)
                            {
                                np.g = curPos.g + cost;
                                np.f = np.g + np.h;
                            }
                            isNPInQueue = true;
                        }

                        if (!isNPInQueue &&
                            (!passedTable.TryGetValue(npHash, out temp) && temp != np))
                        //如果此方格不在即将展开的节点表 和 已遍历过的节点表
                        {
                            np.parent = curPos; //当前格为此格的父方格

                            np.g = curPos.g + cost;
                            np.h = Vector3.Distance(target.Position, nc.Position);
                            np.f = np.g + np.h;
                            np.depth = curPos.depth + 1;

                            enQueueBuffer.Add(np);
                            inQueueTable.Add(npHash, np);
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


            if (found)
            {
                AStarNodeBall curNode = finalNode;
                for (int i = 0; i < curNode.depth; i++)
                {
                    result.Add((City)null);
                }
                do
                {
                    //result.Add(curNode);
                    result[curNode.depth - 1] = curNode.City;
                    curNode = curNode.parent;
                }
                while (curNode.parent != null);

                return new BallPathFinderResult(result);
            }
            return null;
        }
    }
}
