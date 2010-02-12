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
        FastList<ScreenRigidBody> sleepBodies;

        Rectangle bounds;

        public ScreenPhysicsWorld()
        {
            bodies = new FastList<ScreenRigidBody>();
            statics = new FastList<ScreenStaticBody>();
            sleepBodies = new FastList<ScreenRigidBody>();

            bounds = new Rectangle(0, 0, Properties.Settings.Default.ScreenWidth, Properties.Settings.Default.ScreenHeight);
        }

        public void Add(ScreenRigidBody body)
        {
            bodies.Add(body);
        }
        public void Remove(ScreenRigidBody body)
        {
            bodies.Remove(body);
        }

        public void Add(ScreenStaticBody body)
        {
            statics.Add(body);
        }
        public void Remove(ScreenStaticBody body)
        {
            statics.Remove(body);
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
                    Vector2 pa = bodyA.Position;
                    Vector2 pb = bodyB.Position;
                    float dist = Vector2.Distance(pa, pb);

                    if (dist < bodyA.Radius + bodyB.Radius)
                    {
                        Vector2 n = bodyA.Position - bodyB.Position;
                        n.Normalize();

                        Vector2 collPos = pa - bodyA.Radius * n;

                        Vector2 ra = pa - collPos;
                        Vector2 rb = pb - collPos;

                        Vector2 wa = new Vector2(-bodyA.AngularVelocity * ra.Y, bodyA.AngularVelocity * ra.X);
                        Vector2 wb = new Vector2(-bodyB.AngularVelocity * rb.Y, bodyB.AngularVelocity * rb.X);

                        Vector2 va = bodyA.Velocity + wa;
                        Vector2 vb = bodyB.Velocity + wb;

                        float vrn = Vector2.Dot(va - vb, n);

                        float ranCrs = MathEx.Vec2Cross(ra, n);
                        float rbnCrs = MathEx.Vec2Cross(rb, n);

                        float elasity = bodyA.Elasity * bodyB.Elasity;

                        float impluse = -2 * elasity * vrn /
                            (1 / bodyA.Mass + 1 / bodyB.Mass +
                             Vector2.Dot(new Vector2(-ranCrs * ra.Y, ranCrs * ra.X) / bodyA.Inertia, n) +
                             Vector2.Dot(new Vector2(-rbnCrs * rb.Y, rbnCrs * rb.X) / bodyB.Inertia, n));

                        Vector2 impulseVec = n * impluse;
                        bodyA.ApplyImpulse(impulseVec, collPos);
                        bodyB.ApplyImpulse(-impulseVec, collPos);





                        Vector2 tang = new Vector2(-n.X, n.Y);
                        float ratCrs = MathEx.Vec2Cross(ra, tang);
                        float rbtCrs = MathEx.Vec2Cross(rb, tang);

                        float vrt = Vector2.Dot(va - vb, tang);

                        float frictionMax = -vrt /
                            (1 / bodyA.Mass + 1 / bodyB.Mass +
                             Vector2.Dot(new Vector2(-ratCrs * ra.Y, ratCrs * ra.X) / bodyA.Inertia, tang) +
                             Vector2.Dot(new Vector2(-rbtCrs * rb.Y, rbtCrs * rb.X) / bodyB.Inertia, tang));

                        float friction = impluse * bodyA.Friction * bodyB.Friction;

                        if (friction < frictionMax)
                        {
                            impulseVec = tang * friction;
                        }
                        else
                        {
                            impulseVec = tang * frictionMax;
                        }

                        bodyA.ApplyImpulse(impulseVec, collPos);
                        bodyB.ApplyImpulse(impulseVec, collPos);

                    }
                }
            }

            for (int i = 0; i < bodies.Count; i++)
            {
                ScreenRigidBody bodyA = bodies[i];
                Vector2 pa = bodyA.Position;

                #region edge coll

                bool passed = false;
                Vector2 n = new Vector2();
                Vector2 collPos = new Vector2();
                float dist = pa.X - bounds.Left;
                if (dist < bodyA.Radius)
                {
                    n = new Vector2(1, 0);
                    collPos = pa - dist * n;
                    passed = true;
                }
                if (!passed)
                {
                    dist = pa.Y - bounds.Top;
                    if (dist < bodyA.Radius)
                    {
                        n = new Vector2(0, 1);
                        collPos = pa - dist * n;
                        passed = true;
                    }
                }
                if (!passed)
                {
                    dist = bounds.Right - pa.X;
                    if (dist < bodyA.Radius)
                    {
                        n = new Vector2(-1, 0);
                        collPos = pa - dist * n;
                        passed = true;
                    }
                }
                if (!passed)
                {
                    dist = bounds.Bottom - pa.Y;
                    if (dist < bodyA.Radius)
                    {
                        n = new Vector2(0, -1);
                        collPos = pa - dist * n;
                        passed = true;
                    }
                }

                if (passed)
                {
                    Vector2 ra = pa - collPos;
                    Vector2 wa = new Vector2(-bodyA.AngularVelocity * ra.Y, bodyA.AngularVelocity * ra.X);
                    Vector2 va = bodyA.Velocity + wa;

                    float vrn = Vector2.Dot(va, n);

                    float ranCrs = MathEx.Vec2Cross(ra, n);

                    float elasity = bodyA.Elasity;

                    float impluse = -2 * elasity * vrn /
                        (1 / bodyA.Mass +
                        Vector2.Dot(new Vector2(-ranCrs * ra.Y, ranCrs * ra.X) / bodyA.Inertia, n));
                    Vector2 impulseVec = n * impluse;
                    bodyA.ApplyImpulse(impulseVec, collPos);




                    //Vector2 tang = new Vector2(-n.X, n.Y);
                    //float ratCrs = MathEx.Vec2Cross(ra, tang);
                    //float vrt = Vector2.Dot(va, tang);

                    //float frictionMax = -vrt /
                    //    (1 / bodyA.Mass +
                    //     Vector2.Dot(new Vector2(-ratCrs * ra.Y, ratCrs * ra.X) / bodyA.Inertia, tang));

                    //float friction = impluse * bodyA.Friction;

                    //if (friction < frictionMax)
                    //{
                    //    impulseVec = tang * friction;
                    //}
                    //else
                    //{
                    //    impulseVec = tang * frictionMax;
                    //}

                    //bodyA.ApplyImpulse(impulseVec, collPos);
                }
                #endregion


                for (int j = 0; j < statics.Count; j++)
                {
                    
                    ScreenStaticBody bodyB = statics[j];

                    if (bodyB.AABBTest(pa, bodyA.Radius))
                    {
                       
                        if (bodyB.IntersectTest(pa, bodyA.Radius, out n, out collPos))
                        {
                            Vector2 ra = pa - collPos;
                            Vector2 wa = new Vector2(-bodyA.AngularVelocity * ra.Y, bodyA.AngularVelocity * ra.X);
                            Vector2 va = bodyA.Velocity + wa;

                            float vrn = Vector2.Dot(va, n);

                            float ranCrs = MathEx.Vec2Cross(ra, n);

                            float elasity = bodyA.Elasity * bodyB.Elasity;

                            float impluse = -2 * elasity * vrn /
                                (1 / bodyA.Mass +
                                Vector2.Dot(new Vector2(-ranCrs * ra.Y, ranCrs * ra.X) / bodyA.Inertia, n));
                            Vector2 impulseVec = n * impluse;
                            bodyA.ApplyImpulse(impulseVec, collPos);




                            Vector2 tang = new Vector2(-n.X, n.Y);
                            float ratCrs = MathEx.Vec2Cross(ra, tang);
                            float vrt = Vector2.Dot(va, tang);

                            float frictionMax = -vrt /
                                (1 / bodyA.Mass +
                                 Vector2.Dot(new Vector2(-ratCrs * ra.Y, ratCrs * ra.X) / bodyA.Inertia, tang));

                            float friction = impluse * bodyA.Friction * bodyB.Friction;

                            if (friction < frictionMax)
                            {
                                impulseVec = tang * friction;
                            }
                            else
                            {
                                impulseVec = tang * frictionMax;
                            }

                            bodyA.ApplyImpulse(impulseVec, collPos);
                        }
                    }
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
            Collision();
        }

    }
}
