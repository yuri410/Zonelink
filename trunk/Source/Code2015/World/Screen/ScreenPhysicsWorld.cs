using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Collections;
using Apoc3D;
using Apoc3D.MathLib;

namespace Code2015.World.Screen
{
    /// <summary>
    ///  表示屏幕空间2D物理世界环境
    /// </summary>
    class ScreenPhysicsWorld
    {
        FastList<ScreenRigidBody> bodies;
        FastList<ScreenStaticBody> statics;
        FastList <ScreenRigidBody> sleepBodies;

        Rectangle bounds;

        public ScreenPhysicsWorld()
        {
            bodies = new FastList<ScreenRigidBody>();
            statics = new FastList<ScreenStaticBody>();
             sleepBodies = new FastList<ScreenRigidBody> ();
        }

        /// <summary>
        ///  获取或设置模拟区域
        /// </summary>
        public Rectangle WorldBounds
        {
            get { return bounds; }
            set { bounds = value; }
        }

        void Collision()
        {
            for (int i = 0; i < bodies.Count; i++)
            {
                for (int j = i + 1; j < bodies.Count; j++)
                {
                    ScreenRigidBody bodyA = bodies[i];
                    ScreenRigidBody bodyB = bodies[j];

                    float dist = Vector2.Distance(bodyA.Position, bodyB.Position);
                    if (dist < bodyA.Radius + bodyB.Radius)
                    {
                        Vector2 n = bodyA.Position - bodyB.Position;
                        n.Normalize();

                    }
                }
            }

            for (int i = 0; i < bodies.Count; i++)
            {
                for (int j = 0; j < statics.Count; j++)
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
