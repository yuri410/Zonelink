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
        Auto
    }

    class Harvester : DynamicObject
    {
        float longtitude;
        float latitude;

        PathFinder finder;
        UnitState state;

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


        

        public Harvester(RenderSystem rs, Map map,  Model mdl)
        {
            finder = map.PathFinder.CreatePathFinder();

            ModelL0 = mdl;
            BoundingSphere.Radius = 50;
        }

        public override void Update(GameTime dt)
        {
            switch (state)
            {
                case UnitState.Auto:
                    break;
            }

            Orientation = PlanetEarth.GetOrientation(longtitude, latitude);
            Position = PlanetEarth.GetPosition(longtitude, latitude);

            base.Update(dt);
        }


        public override bool IsSerializable
        {
            get { return false; ; }
        }
    }
}
