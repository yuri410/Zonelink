using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Collections;

namespace Code2015.World.Screen
{
    class ScreenPhysicsWorld
    {
        FastList<ScreenRigidBody> bodies;
        FastList<ScreenStaticBody> statics;

        public ScreenPhysicsWorld()
        {
            bodies = new FastList<ScreenRigidBody>();
            statics = new FastList<ScreenStaticBody>();
        }
    }
}
