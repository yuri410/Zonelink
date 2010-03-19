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
    enum UnitState
    {
        TargetAuto,
        HomeAuto
    }

    struct MoveNode 
    {
        public float Longitude;
        public float Latitude;

        public float Alt;

        public Quaternion Ori;
    }


    public class Harvester : DynamicObject
    {
        float longtitude;
        float latitude;
        MoveNode src;
        MoveNode target;

        Map map;
        PathFinder finder;
        UnitState state;

        float autoSLng;
        float autoSLat;
        float autoTLng;
        float autoTLat;

        int destX; 
        int destY;


        bool rotUpdated;
        bool stateUpdated;

        float currentPrg;
        int currentNode;
        PathFinderResult cuurentPath;

        const float Speed = 1;

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


        public bool IsAuto
        {
            get;
            private set;
        }

        public Harvester(RenderSystem rs, Map map, Model mdl)
        {
            finder = map.PathFinder.CreatePathFinder();
            this.map = map;

            ModelL0 = mdl;
            BoundingSphere.Radius = 50;

        }

        public void SetAuto(float tlng, float tlat, float slng, float slat)
        {
            IsAuto = true;
           
            autoSLat = slat;
            autoSLng = slng;
            autoTLat = tlat;
            autoTLng = tlng;

            move(autoTLng, autoTLat);
            state = UnitState.TargetAuto;
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
            IsAuto = false;
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
            return Quaternion.RotationMatrix(result);
        }

        public override void Update(GameTime dt)
        {
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
                        if (IsAuto)
                        {
                            if (state == UnitState.HomeAuto)
                            {
                                move(autoTLng, autoTLat);
                                state = UnitState.TargetAuto;
                            }
                            else if (state == UnitState.TargetAuto)
                            {
                                move(autoSLng, autoSLat);
                                state = UnitState.HomeAuto;
                            }

                        }
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

                    Orientation = Matrix.RotationQuaternion(
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
           
            Position = PlanetEarth.GetPosition(longtitude, latitude, PlanetEarth.PlanetRadius + altitude);

            base.Update(dt);
        }


        public override bool IsSerializable
        {
            get { return false; ; }
        }
    }
}
