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
using Apoc3D.Graphics;
using Apoc3D.Graphics.Animation;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.Logic;

namespace Code2015.World
{
    enum MovePurpose
    {
        None,
        Gather,
        Home,
    }

    struct MoveNode
    {
        public float Longitude;
        public float Latitude;

        public float Alt;

        public Quaternion Ori;
    }


    class Harvester : WorldDynamicObject, ISelectableObject
    {
        public const int NumModels = 30;
        /// <summary>
        ///  矿车属性
        /// </summary>
        public struct Props
        {
            public float Speed;
            public float HP;
            public float Storage;
        }

        Props props;
        Model[] model;
        Model[] model_bad;
        int mdlIndex;
        GatherCity parent;


        float longtitude;
        float latitude;
        MoveNode src;
        MoveNode target;



        Map map;
        PathFinder finder;
        MovePurpose movePurpose;



        int destX;
        int destY;

        float acceleration;

        /// <summary>
        ///  表示是否在原地旋转对准出发方向
        /// </summary>
        bool isAdjustingDirection;
        /// <summary>
        ///  表示是否获得下一节点的朝向信息
        /// </summary>
        bool rotUpdated;
        /// <summary>
        ///  表示是否已经获得下一节点的位置信息
        /// </summary>
        bool stateUpdated;

        float nodeMotionProgress;
        int currentNode;
        PathFinderResult currentPath;

        #region 仓库
        float harvStorage;

        bool isFullLoaded;
        NaturalResource exRes;
        bool isLoading;
        bool isUnloading;
        float loadingTime;
        #endregion
        /// <summary>
        /// 矿车当前的生命值
        /// </summary>
        float currentHp;



        public bool IsFullLoaded { get { return isFullLoaded; } set { isFullLoaded = value; } }

        public NaturalResource ExRes
        {
            get { return exRes; }
            set { exRes = value; }
        }

        public float Longitude
        {
            get { return longtitude; }
            set { longtitude = value; }
        }
        public float Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }
        public int ModelIndex { get { return mdlIndex; } }

        public Props GetProps() { return props; }
        public void SetProps(Props p)
        {
            props = p;
            currentHp = p.HP;
        }

        public GatherCity Parent { get { return parent; } }
        




        public bool IsIdle
        {
            get { return currentPath == null; }
        }

        public Harvester(GatherCity parent, Map map)
        {
            this.parent = parent;

            
            finder = map.PathFinder.CreatePathFinder();
            this.map = map;


            BoundingSphere.Radius = 50;
            
        }

        public void InitializeGraphics(RenderSystem rs)
        {
            model = new Model[NumModels];
            model_bad = new Model[NumModels];

            for (int i = 0; i < NumModels; i++)
            {
                FileLocation fl = FileSystem.Instance.Locate("cow" + i.ToString("D2") + ".mesh", GameFileLocs.Model);

                model[i] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                model[i].CurrentAnimation.Clear();
                model[i].CurrentAnimation.Add(new NoAnimaionPlayer(
                    Matrix.RotationY(MathEx.PIf) *
                    Matrix.Scaling(Game.ObjectScale * 0.67f, Game.ObjectScale * 0.67f, Game.ObjectScale * 0.67f)));
            
            fl = FileSystem.Instance.Locate("cow_bad" + i.ToString("D2") + ".mesh", GameFileLocs.Model);
                model_bad[i] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                model_bad[i].CurrentAnimation.Clear();
                model_bad[i].CurrentAnimation.Add(new NoAnimaionPlayer(
                    Matrix.RotationY(MathEx.PIf) *
                    Matrix.Scaling(Game.ObjectScale * 0.67f, Game.ObjectScale * 0.67f, Game.ObjectScale * 0.67f)));
            }
            //style.Cow = new ResourceHandle<ModelData>[CowFrameCount];
            //for (int i = 0; i < CowFrameCount; i++)
            //{
            //    fl = FileSystem.Instance.Locate(Cow_Inv + i.ToString("D2") + ".mesh", GameFileLocs.Model);
            //    style.Cow[i] = ModelManager.Instance.CreateInstance(rs, fl);
            //}
            //this.model = mdl;

            ModelL0 = model[0];
        }
        public event EventHandler GotThere;
        public event EventHandler GotHome;

        /// <summary>
        ///  调用Move之后设置目的，否则为无目的
        /// </summary>
        /// <param name="purpose"></param>
        public void SetMovePurpose(MovePurpose purpose)
        {
            this.movePurpose = purpose;
        }

        /// <summary>
        ///  强制设置采集车位置
        /// </summary>
        /// <param name="longtitude"></param>
        /// <param name="latitude"></param>
        public void SetPosition(float longtitude, float latitude)
        {
            Point pt;
            Map.GetMapCoord(longtitude, latitude, out pt.X, out pt.Y);
            Point pt2 = pt;
            pt2.X++;
            // 计算pt点左边的一点


            // 然后计算采集车在pt点朝向pt2的方向
            Quaternion q = GetOrientation(pt, pt2);
            Orientation = Matrix.RotationQuaternion(q);

            // 设置位置
            this.longtitude = longtitude;
            this.latitude = latitude;
        }


        /// <summary>
        ///  让采集车移动到有经纬度确定的地点
        /// </summary>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        public void Move(float lng, float lat)
        {
            //IsAuto = false;
            movePurpose = MovePurpose.None;

            int sx, sy;
            Map.GetMapCoord(longtitude, latitude, out sx, out sy);

            int tx, ty;
            Map.GetMapCoord(lng, lat, out tx, out ty);

            destX = tx;
            destY = ty;

            finder.Reset();
            currentPath = finder.FindPath(sx, sy, tx, ty);
            
            // 新的动作要先调整朝向
            isAdjustingDirection = true;

            currentNode = 0;
            nodeMotionProgress = 0;
        }

        /// <summary>
        ///  当寻路需要RCPF的时候，调用这个继续寻路
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void Move(int x, int y)
        {
            int sx, sy;
            Map.GetMapCoord(longtitude, latitude, out sx, out sy);

            destX = x;
            destY = y;

            finder.Reset();
            currentPath = finder.FindPath(sx, sy, x, y);
            currentNode = 0;
            nodeMotionProgress = 0;
        }

        /// <summary>
        ///  有两个Map的坐标计算采集车朝向
        /// </summary>
        /// <param name="pa"></param>
        /// <param name="pb"></param>
        /// <returns></returns>
        Quaternion GetOrientation(Point pa, Point pb)
        {
            float alng;
            float alat;
            float blng;
            float blat;

            // 先得到两个点的经纬度
            Map.GetCoord(pa.X, pa.Y, out alng, out alat);
            Map.GetCoord(pb.X, pb.Y, out blng, out blat);

            // 法向
            Vector3 n = PlanetEarth.GetNormal(alng, alat);

            // 高度
            float altA = map.GetHeight(alng, alat);
            Vector3 posA = PlanetEarth.GetPosition(alng, alat, altA + PlanetEarth.PlanetRadius);
            float altB = map.GetHeight(blng, blat);
            Vector3 posB = PlanetEarth.GetPosition(blng, blat, altB + PlanetEarth.PlanetRadius);

            Vector3 dir = posB - posA;
            dir.Normalize();
            Vector3 bi = Vector3.Cross(n, dir);
            bi.Normalize();

            n = Vector3.Cross(dir, bi);

            // 采集车旋转World矩阵由向量基构建
            Matrix result = Matrix.Identity;
            result.Right = bi;
            result.Up = n;
            result.Forward = -dir;
            return Quaternion.RotationMatrix(result);
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        public override void Update(GameTime dt)
        {
            float ddt = (float)dt.ElapsedGameTimeSeconds;

            #region 装货卸货
            if (isLoading && exRes != null)
            {
                // 计算开矿倍数，保证能够完成卸货
                float scale = props.Storage / (RulesTable.HarvLoadingSpeed * RulesTable.HarvLoadingTime);

                harvStorage += exRes.Exploit(RulesTable.HarvLoadingSpeed * ddt * scale);
                loadingTime -= ddt;

                // 开矿loadingTime时间之后，停止并引发事件
                if (loadingTime < 0)
                {
                    isFullLoaded = harvStorage >= props.Storage;
                    isLoading = false;
                    if (GotThere != null)
                        GotThere(this, EventArgs.Empty);
                }
            }
            if (isUnloading)
            {
                // 计算开矿倍数，保证能够完成卸货
                float scale = props.Storage / (RulesTable.HarvLoadingSpeed * RulesTable.HarvLoadingTime);

                float change = RulesTable.HarvLoadingSpeed * ddt * scale;

                // 检查车上的存量是否足够
                if (harvStorage - change > 0)
                {
                    // 足够时定量卸下
                    harvStorage -= change;
                    // 并且通知城市得到资源
                    parent.NotifyGotResource(change);
                }
                else
                {
                    // 不够时把剩下的都卸了
                    harvStorage = 0;
                    parent.NotifyGotResource(harvStorage);
                }

                loadingTime -= ddt;

                // 一定时间后停止
                if (loadingTime < 0)
                {
                    isUnloading = false;
                    if (GotHome != null)
                        GotHome(this, EventArgs.Empty);
                }
            }
            #endregion

            float altitude = map.GetHeight(longtitude, latitude);

            if (currentPath != null)
            {
                int nextNode = currentNode + 1;

                if (nextNode >= currentPath.NodeCount)
                {
                    #region 寻路完毕，状态转换
                    nextNode = 0;
                    nodeMotionProgress = 0;

                    if (currentPath.RequiresPathFinding)
                    {
                        Move(destX, destY);
                    }
                    else
                    {
                        currentPath = null;

                        if (movePurpose == MovePurpose.Gather)
                        {
                            isLoading = true;
                            loadingTime = RulesTable.HarvLoadingTime;
                        }
                        else if (movePurpose == MovePurpose.Home)
                        {
                            isUnloading = true;
                            loadingTime = RulesTable.HarvLoadingTime;
                        }
                    }
                    #endregion
                }
                else
                {
                    #region 路径节点插值

                    // 采集车在每两个节点之间移动都是一定过程
                    // 其位置/朝向是插值结果。差值参数为nodeMotionProgress

                    Point cp = currentPath[currentNode];
                    Point np = currentPath[nextNode];

                    // 在一开始就尝试获取下一节点位置
                    if (!stateUpdated)
                    {
                        if (isAdjustingDirection)
                        {
                            // 在调整方向时，车是位置不动的
                            Map.GetCoord(np.X, np.Y, out src.Longitude, out src.Latitude);

                            src.Alt = map.GetHeight(src.Longitude, src.Latitude);

                            target.Longitude = src.Longitude;
                            target.Latitude = src.Latitude;
                            target.Alt = src.Alt;
                        }
                        else
                        {
                            Map.GetCoord(cp.X, cp.Y, out src.Longitude, out src.Latitude);
                            Map.GetCoord(np.X, np.Y, out target.Longitude, out target.Latitude);

                            src.Alt = map.GetHeight(src.Longitude, src.Latitude);
                            target.Alt = map.GetHeight(target.Longitude, target.Latitude);
                        }
                        stateUpdated = true;
                    }
                    
                    // 在进行了一半之后开始获取下一节点朝向
                    if (nodeMotionProgress > 0.5f && !rotUpdated)
                    {
                        if (isAdjustingDirection)
                        {
                            target.Ori = GetOrientation(cp, np);
                        }
                        else 
                        {
                            if (nextNode < currentPath.NodeCount - 1)
                            {
                                src.Ori = GetOrientation(cp, np);
                                target.Ori = GetOrientation(np, currentPath[nextNode + 1]);
                            }
                            else
                            {
                                target.Ori = GetOrientation(cp, np);
                                src.Ori = target.Ori;
                            }
                        }
                        rotUpdated = true;
                    }

                    if (!isAdjustingDirection)
                    {
                        float x = MathEx.LinearInterpose(cp.X, np.X, nodeMotionProgress);
                        float y = MathEx.LinearInterpose(cp.Y, np.Y, nodeMotionProgress);

                        Map.GetCoord(x, y, out longtitude, out latitude);
                    }
                    altitude = MathEx.LinearInterpose(src.Alt, target.Alt, nodeMotionProgress);

                    #region 动画控制
                    if (altitude < 0)
                        mdlIndex++;
                    else
                        mdlIndex--;

                    if (mdlIndex < 0)
                        mdlIndex = 0;
                    if (mdlIndex >= NumModels)
                        mdlIndex = NumModels - 1;

                    if (model != null)
                    {
                        if (parent.Owner != null)
                        {
                            if (parent.Type == CityType.Oil)
                            {
                                ModelL0 = model_bad[mdlIndex];
                            }
                            else
                            {
                                ModelL0 = model[mdlIndex];
                            }
                        }
                        else
                        {
                            ModelL0 = null;
                        }                        
                    }
                    #endregion

                    // 采集车不会潜水
                    if (altitude < 0)
                        altitude = 0;

                    // 球面插值，计算出朝向
                    Orientation = Matrix.RotationQuaternion(
                        Quaternion.Slerp(src.Ori, target.Ori, nodeMotionProgress > 0.5f ? nodeMotionProgress - 0.5f : nodeMotionProgress + 0.5f));

                    if (isAdjustingDirection)
                    {
                        nodeMotionProgress += 0.5f * ddt;
                    }
                    else 
                    {
                        // 挺进节点插值进度
                        nodeMotionProgress += 0.05f * acceleration;
                    }

                    // 检查节点之间插值是否完成
                    if (nodeMotionProgress > 1)
                    {
                        nodeMotionProgress = 0;

                        if (isAdjustingDirection)
                            // 我们只允许调整方向一次，调整这次不会让车在节点上移动，currentNode不变
                            isAdjustingDirection = false; 
                        else
                            currentNode++;
                        
                        rotUpdated = false;
                        stateUpdated = false;
                    }
                    #endregion

                }

                #region 加速度计算
                if (currentPath != null)
                {
                    // 检查是否是最后一个节点了
                    if (nextNode == currentPath.NodeCount - 1)
                    {
                        // 开始减速
                        acceleration -= ddt * 1.5f;
                        // 防止减速的过慢
                        if (acceleration < 0.33f)
                            acceleration = 0.33f;
                    }
                    else if (!isAdjustingDirection)
                    {
                        // 平时都是加速到最大为止
                        acceleration += ddt * 1.5f;
                        if (acceleration > 1)
                            acceleration = 1;
                    }
                }
                #endregion
            }
            else
            {
                acceleration = 0;
            }


            //Orientation *= PlanetEarth.GetOrientation(longtitude, latitude);

            // 通过插值计算的经纬度得到坐标
            Position = PlanetEarth.GetPosition(longtitude, latitude, PlanetEarth.PlanetRadius + altitude);
            base.Update(dt);
        }



        #region ISelectableObject 成员

        public bool IsSelected
        {
            get;
            set;
        }

        #endregion
    }

}
