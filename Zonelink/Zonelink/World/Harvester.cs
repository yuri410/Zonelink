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
using Apoc3D.Scene;
using Code2015.EngineEx;
using Code2015.Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Zonelink.World;
using Zonelink.MathLib;
using Zonelink;

namespace Code2015.World
{
    //enum UnitState
    //{
    //    TargetAuto,
    //    HomeAuto
    //}
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


    class Harvester : SceneObject
    {
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
        //Model model;
        //int mdlIndex;
        GatherCity parent;
        Matrix orientation = Matrix.Identity;
        Vector3 position;

        float longtitude;
        float latitude;
        MoveNode src;
        MoveNode target;

        Map map;
        PathFinder finder;
        MovePurpose movePurpose;

        float harvStorage;


        int destX;
        int destY;


        bool rotUpdated;
        bool stateUpdated;

        float currentPrg;
        int currentNode;
        PathFinderResult cuurentPath;

        

        NatureResource exRes;
        bool isLoading;
        bool isUnloading;
        float loadingTime;


        float currentHp;





        public NatureResource ExRes
        {
            get { return exRes; }
            set { exRes = value; }
        }
        public float Longtitude
        {
            get { return longtitude; }
            set { longtitude = value; }
        }
        public float Latitude
        {
            get { return latitude; }
            set { latitude = value; }
        }

        public Props GetProps() { return props; }
        public void SetProps(Props p)
        {
            props = p;
            currentHp = p.HP;
        }

        public GatherCity Parent { get { return parent; } }
        ///// <summary>
        /////  留着做动画
        ///// </summary>
        //public Model Model         
        //{
        //    get { return model; }
        //}

 


        public bool IsIdle 
        {
            get { return cuurentPath == null; }
        }

        public Harvester(GatherCity parent, Map map)
        {
            this.parent = parent;

            

            finder = map.PathFinder.CreatePathFinder();
            this.map = map;


            BoundingSphere.Radius = 50;

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
        

        void move(float lng, float lat)
        {
            int sx, sy;
            Map.GetMapCoord(longtitude, latitude, out sx, out sy);

            int tx, ty;
            Map.GetMapCoord(lng, lat, out tx, out ty);

            destX = tx;
            destY = ty;

            finder.Reset();
            cuurentPath = finder.FindPath(sx, sy, tx, ty);

            currentNode = 0;
            currentPrg = 0;
        }
        public void Move(float lng, float lat)
        {
            //IsAuto = false;
            movePurpose = MovePurpose.None;
            move(lng, lat);
        }
        void Move(int x, int y)
        {

            int sx, sy;
            Map.GetMapCoord(longtitude, latitude, out sx, out sy);

            destX = x;
            destY = y;

            //finder.Continue();
            //cuurentPath = finder.FindPath(sx, sy, x, y);
            finder.Reset();
            cuurentPath = finder.FindPath(sx, sy, x, y);
            currentNode = 0;
            currentPrg = 0;
        }

        Quaternion GetOrientation(Point pa, Point pb)
        {
            float alng;
            float alat;
            float blng;
            float blat;

            Map.GetCoord(pa.X, pa.Y, out alng, out alat);
            Map.GetCoord(pb.X, pb.Y, out blng, out blat);

            Vector3 n = PlanetEarth.GetNormal(alng, alat);

            float altA = map.GetHeight(alng, alat);
            Vector3 posA = PlanetEarth.GetPosition(alng, alat, altA + PlanetEarth.PlanetRadius);
            float altB = map.GetHeight(blng, blat);
            Vector3 posB = PlanetEarth.GetPosition(blng, blat, altB + PlanetEarth.PlanetRadius);

            Vector3 dir = posB - posA;
            dir.Normalize();
            Vector3 bi = Vector3.Cross(n, dir);
            bi.Normalize();

            n = Vector3.Cross(dir, bi);

            Matrix result = Matrix.Identity;
            result.Right = bi;
            result.Up = n;
            result.Forward = -dir;
            return Quaternion.CreateFromRotationMatrix(result);
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        public override void Update(GameTime dt)
        {
            float ddt = (float)dt.ElapsedGameTime.TotalSeconds;
            
            if (isLoading && exRes != null)
            {
                // 计算开矿倍数
                float scale = props.Storage / (RulesTable.HarvLoadingSpeed * RulesTable.HarvLoadingTime);

                harvStorage += exRes.Exploit(RulesTable.HarvLoadingSpeed * ddt * scale);
                loadingTime -= ddt;

                if (loadingTime < 0)
                {
                    isLoading = false;
                    if (GotThere != null)
                        GotThere(this, EventArgs.Empty);
                }
            }
            if (isUnloading)
            {
                // 计算开矿倍数
                float scale = props.Storage / (RulesTable.HarvLoadingSpeed * RulesTable.HarvLoadingTime);

                float change = RulesTable.HarvLoadingSpeed * ddt * scale;

                if (harvStorage - change > 0)
                {
                    harvStorage -= change;
                    parent.NotifyGotResource(change);
                }
                else
                {
                    harvStorage = 0;
                    parent.NotifyGotResource(harvStorage);
                }

                loadingTime -= ddt;
                if (loadingTime < 0)
                {
                    isLoading = false;
                    if (GotHome != null)
                        GotHome(this, EventArgs.Empty);
                }
            }

            float altitude = map.GetHeight(longtitude, latitude);

            if (cuurentPath != null)
            {
                int nextNode = currentNode + 1;

                if (nextNode >= cuurentPath.NodeCount)
                {
                    nextNode = 0;
                    currentPrg = 0;

                    if (cuurentPath.RequiresPathFinding)
                    {
                        Move(destX, destY);
                    }
                    else
                    {
                        cuurentPath = null;

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

                        //if (IsAuto)
                        //{
                        //    if (state == UnitState.HomeAuto)
                        //    {
                        //        move(autoTLng, autoTLat);
                        //        state = UnitState.TargetAuto;
                        //    }
                        //    else if (state == UnitState.TargetAuto)
                        //    {
                        //        move(autoSLng, autoSLat);
                        //        state = UnitState.HomeAuto;
                        //    }

                        //}
                    }
                }
                else
                {
                    Point np = cuurentPath[nextNode];
                    Point cp = cuurentPath[currentNode];


                    if (!stateUpdated)
                    {
                        Map.GetCoord(cp.X, cp.Y, out src.Longitude, out src.Latitude);
                        Map.GetCoord(np.X, np.Y, out target.Longitude, out target.Latitude);

                        src.Alt = map.GetHeight(src.Longitude, src.Latitude);
                        target.Alt = map.GetHeight(target.Longitude, target.Latitude);
                        stateUpdated = true;
                    }

                    if (currentPrg > 0.5f && !rotUpdated)
                    {
                        if (nextNode < cuurentPath.NodeCount - 1)
                        {
                            src.Ori = GetOrientation(cp, np);
                            target.Ori = GetOrientation(np, cuurentPath[nextNode + 1]);
                        }
                        else
                        {
                            target.Ori = GetOrientation(cp, np);
                            src.Ori = target.Ori;
                        }
                        rotUpdated = true;
                    }

                    float x = MathEx.LinearInterpose(cp.X, np.X, currentPrg);
                    float y = MathEx.LinearInterpose(cp.Y, np.Y, currentPrg);

                    Map.GetCoord(x, y, out longtitude, out latitude);

                    altitude = MathEx.LinearInterpose(src.Alt, target.Alt, currentPrg);

                    //if (altitude < 0)
                    //    mdlIndex++;
                    //else
                    //    mdlIndex--;

                    //if (mdlIndex < 0)
                    //    mdlIndex = 0;
                    //if (mdlIndex >= model.Length)
                    //    mdlIndex = model.Length - 1;

                    //ModelL0 = model[mdlIndex];

                    if (altitude < 0)
                        altitude = 0;

                    orientation = Matrix.CreateFromQuaternion(
                        Quaternion.Slerp(src.Ori, target.Ori, currentPrg > 0.5f ? currentPrg - 0.5f : currentPrg + 0.5f));

                    currentPrg += 0.05f;

                    if (currentPrg > 1)
                    {
                        currentPrg = 0;
                        currentNode++;
                        rotUpdated = false;
                        stateUpdated = false;
                    }

                }
            }



            //Orientation *= PlanetEarth.GetOrientation(longtitude, latitude);

            position = PlanetEarth.GetPosition(longtitude, latitude, PlanetEarth.PlanetRadius + altitude);

            Matrix.CreateTranslation(ref position, out Transformation);
            Matrix.Multiply(ref orientation, ref Transformation, out Transformation);

        }


        public override void Render()
        {
        }
    }
}
