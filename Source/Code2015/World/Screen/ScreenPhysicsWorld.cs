using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Collections;
using Apoc3D;

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

        void Collision()
        {
            
        }

        public void Update(GameTime time)
        {
            float dt = time.ElapsedRealTime;
            if (dt > float.Epsilon)
            {
                for (int i = 0; i < bodies.Count; i++)
                {
                    bodies[i].Integrate(dt);
                }
            }
        }

    }
}
