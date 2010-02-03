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
        FastList <ScreenRigidBody> sleepBodies;

        public ScreenPhysicsWorld()
        {
            bodies = new FastList<ScreenRigidBody>();
            statics = new FastList<ScreenStaticBody>();
             sleepBodies = new FastList<ScreenRigidBody> ();
        }

        void Collision()
        {
            for (int i = 0; i < bodies.Count; i++)
            {
                for (int j = 0; j < bodies.Count; j++)
                {

                }
            }
        }

        public void EnableBody(ScreenRigidBody body, bool enable)
        {
            if (!enable)
            {
                if (bodies.Remove(body))
                {
                    sleepBodies.Add(body);
                }
            }
            else
            {
                if (sleepBodies.Remove(body))
                {
                    bodies.Add(body);
                }
            }
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
