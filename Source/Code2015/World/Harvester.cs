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
        Stopped,
        Moving,
        Rotating
    }

    public class Harvester : DynamicObject
    {
        float longtitude;
        float latitude;

        PathFinder finder;
        UnitState state;

        int destX; 
        int destY;

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
            set;
        }

        public Harvester(RenderSystem rs, Map map,  Model mdl)
        {
            finder = map.PathFinder.CreatePathFinder();

            ModelL0 = mdl;
            BoundingSphere.Radius = 50;
            
        }

        public void Move(float lng, float lat)
        {
            int sx, sy;
            Map.GetMapCoord(longtitude, latitude, out sx, out sy);

            int tx ,ty;
            Map.GetMapCoord(lng, lat, out tx, out ty);

            destX = tx;
            destY = ty;

            finder.Reset();
            cuurentPath = finder.FindPath(sx, sy, tx, ty);
            currentNode = 0;
            currentPrg = 0;
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


        public override void Update(GameTime dt)
        {
            //float xx, yy;
            //Map.GetMapCoord(longtitude, latitude, out xx, out yy);

            if (cuurentPath != null)
            {
                int nextNode = currentNode + 1;

                if (nextNode >= cuurentPath.NodeCount)
                {
                    if (cuurentPath.RequiresPathFinding)
                    {
                        Move(destX, destY);
                    }
                    else
                    {
                        cuurentPath = null;
                    }
                    nextNode = 0;
                    currentPrg = 0;
                }
                else
                {
                    Point np = cuurentPath[nextNode];
                    Point cp = cuurentPath[currentNode];

                    float x = MathEx.LinearInterpose(cp.X, np.X, currentPrg);
                    float y = MathEx.LinearInterpose(cp.Y, np.Y, currentPrg);

                    Map.GetCoord(x, y, out longtitude, out latitude);
                  
                    //Map.GetMapCoord(longtitude, latitude, out xx, out yy);

                    //Console.Write(xx);
                    //Console.Write(yy);

                    currentPrg += 0.1f;

                    if (currentPrg > 1) 
                    {
                        currentPrg = 0;
                        currentNode++;
                    }
                }
            }

            switch (state)
            {
                case UnitState.Moving:

                    break;
                case UnitState.Rotating:
                    break;
            }

            Orientation = PlanetEarth.GetOrientation(longtitude, latitude);
            Position = PlanetEarth.GetPosition(longtitude, latitude, PlanetEarth.PlanetRadius + 50);

            base.Update(dt);
        }


        public override bool IsSerializable
        {
            get { return false; ; }
        }
    }
}
