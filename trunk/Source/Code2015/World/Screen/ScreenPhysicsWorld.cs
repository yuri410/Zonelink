using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.MathLib;

namespace Code2015.World.Screen
{
    /// <summary>
    ///  
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns>返回true放弃碰撞处理</returns>
    delegate bool CollisionHandler(ScreenRigidBody a, ScreenRigidBody b);

    /// <summary>
    ///  表示屏幕空间2D物理世界环境
    /// </summary>
    class ScreenPhysicsWorld
    {
        FastList<ScreenRigidBody> bodies;
        FastList<ScreenStaticBody> statics;
        FastList<ScreenRigidBody> sleepBodies;

        Rectangle bounds;
        float boundsRadius;
        public ScreenPhysicsWorld()
        {
            bodies = new FastList<ScreenRigidBody>();
            statics = new FastList<ScreenStaticBody>();
            sleepBodies = new FastList<ScreenRigidBody>();

            WorldBounds = new Rectangle(0, 0, Properties.Settings.Default.ScreenWidth, Properties.Settings.Default.ScreenHeight);
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

        public event CollisionHandler BodyCollision;

        public float BoundsRadius 
        {
            get { return boundsRadius; }
        }
        /// <summary>
        ///  获取或设置模拟区域
        /// </summary>
        public Rectangle WorldBounds
        {
            get { return bounds; }
            set
            {
                bounds = value;
                boundsRadius = (float)Math.Sqrt(bounds.Width * bounds.Width + bounds.Height * bounds.Height);
            }
        }

        void EdgeHandle(ScreenRigidBody bodyA, Vector2 n, Vector2 collPos, float depth, float invdt)
        {
            if (depth < 0) depth = 0;

            Vector2 pa = bodyA.Position;
            Vector2 ra = pa - collPos;
            Vector2 wa = new Vector2(-bodyA.AngularVelocity * ra.Y, bodyA.AngularVelocity * ra.X);
            Vector2 va = bodyA.Velocity + wa;

            float vrn = Vector2.Dot(va, n);

            //if (vrn >= 0)
                vrn -= depth;

            if (vrn < 0)
            {
                float ranCrs = Vector2.Cross(ra, n);

                float elasity = bodyA.Elasity;

                float impluse = -(1 + elasity) * vrn /
                    (1 / bodyA.Mass +
                    Vector2.Dot(new Vector2(-ranCrs * ra.Y, ranCrs * ra.X) / bodyA.Inertia, n));
                Vector2 impulseVec = n * impluse;
                bodyA.ApplyImpulse(impulseVec, collPos);


                Vector2 tang = new Vector2(-n.Y, n.X);
                float ratCrs = Vector2.Cross(ra, tang);
                float vrt = Vector2.Dot(va, tang);

                float frictionMax = -vrt /
                    (1 / bodyA.Mass +
                     Vector2.Dot(new Vector2(-ratCrs * ra.Y, ratCrs * ra.X) / bodyA.Inertia, tang));

                float friction = impluse * bodyA.Friction;
                if (friction < frictionMax)
                {
                    impulseVec = tang * friction;
                }
                else
                {
                    impulseVec = tang * frictionMax;
                }

                bodyA.ApplyImpulse(impulseVec, collPos);
                bodyA.IsColliding = false;
            }
        }

        void Collision(float dt)
        {
            float invdt = 1.0f / dt;
            for (int i = 0; i < bodies.Count; i++)
            {
                ScreenRigidBody bodyA = bodies[i];
                if (bodyA.CollisionEnabled)
                {
                    for (int j = i + 1; j < bodies.Count; j++)
                    {
                        ScreenRigidBody bodyB = bodies[j];
                        if (bodyB.CollisionEnabled)
                        {
                            #region CD
                            Vector2 pa = bodyA.Position;
                            Vector2 pb = bodyB.Position;
                            float dist = Vector2.Distance(pa, pb);

                            if (dist < bodyA.Radius + bodyB.Radius)
                            {
                                Vector2 n = bodyA.Position - bodyB.Position;
                                n.Normalize();

                                Vector2 collPos = pa - bodyA.Radius * n;
                                float depth = bodyA.Radius + bodyB.Radius - dist;

                                Vector2 ra = pa - collPos;
                                Vector2 rb = pb - collPos;

                                Vector2 wa = new Vector2(-bodyA.AngularVelocity * ra.Y, bodyA.AngularVelocity * ra.X);
                                Vector2 wb = new Vector2(-bodyB.AngularVelocity * rb.Y, bodyB.AngularVelocity * rb.X);

                                Vector2 va = bodyA.Velocity + wa;
                                Vector2 vb = bodyB.Velocity + wb;

                                float vrn = Vector2.Dot(va - vb, n);
                                //if (vrn >= 0)
                                vrn -= depth * invdt;

                                if (vrn < 0)
                                {
                                    bool passed = true;
                                    if (BodyCollision != null)
                                    {
                                        if (BodyCollision(bodyA, bodyB))
                                        {
                                            passed = false;
                                        }
                                    }

                                    if (passed)
                                    {
                                        float ranCrs = Vector2.Cross(ra, n);
                                        float rbnCrs = Vector2.Cross(rb, n);

                                        float elasity = bodyA.Elasity * bodyB.Elasity;

                                        float impluse = -(1 + elasity) * vrn /
                                            (1 / bodyA.Mass + 1 / bodyB.Mass +
                                            Vector2.Dot(new Vector2(-ranCrs * ra.Y, ranCrs * ra.X) / bodyA.Inertia, n) +
                                            Vector2.Dot(new Vector2(-rbnCrs * rb.Y, rbnCrs * rb.X) / bodyB.Inertia, n));

                                        Vector2 impulseVec = n * impluse;
                                        bodyA.ApplyImpulse(impulseVec, collPos);
                                        bodyB.ApplyImpulse(-impulseVec, collPos);


                                        Vector2 tang = new Vector2(-n.Y, n.X);
                                        float ratCrs = Vector2.Cross(ra, tang);
                                        float rbtCrs = Vector2.Cross(rb, tang);

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
                                        bodyB.ApplyImpulse(-impulseVec, collPos);
                                        bodyA.IsColliding = false;
                                        bodyB.IsColliding = false;
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
            }

            for (int i = 0; i < bodies.Count; i++)
            {
                ScreenRigidBody bodyA = bodies[i];
                Vector2 pa = bodyA.Position;

                #region edge coll

                Vector2 n = new Vector2();
                Vector2 collPos = new Vector2();
                float depth = 0;

                float dist = pa.X - bounds.Left;
                if (dist < bodyA.Radius)
                {
                    n = new Vector2(1, 0);
                    collPos = pa - dist * n;
                    depth = bodyA.Radius - dist;
                    EdgeHandle(bodyA, n, collPos, depth, invdt);
                }

                dist = pa.Y - bounds.Top;
                if (dist < bodyA.Radius)
                {
                    n = new Vector2(0, 1);
                    collPos = pa - dist * n;
                    depth = bodyA.Radius - dist;
                    EdgeHandle(bodyA, n, collPos, depth, invdt);
                }
                dist = bounds.Right - pa.X;
                if (dist < bodyA.Radius)
                {
                    n = new Vector2(-1, 0);
                    collPos = pa - dist * n;
                    depth = bodyA.Radius - dist;
                    EdgeHandle(bodyA, n, collPos, depth, invdt);
                }
                dist = bounds.Bottom - pa.Y;
                if (dist < bodyA.Radius)
                {
                    n = new Vector2(0, -1);
                    collPos = pa - dist * n;
                    depth = bodyA.Radius - dist;
                    EdgeHandle(bodyA, n, collPos, depth, invdt);
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

                            if (vrn < 0)
                            {
                                float ranCrs = Vector2.Cross(ra, n);

                                float elasity = bodyA.Elasity * bodyB.Elasity;

                                float impluse = -(1 + elasity) * vrn /
                                    (1 / bodyA.Mass +
                                    Vector2.Dot(new Vector2(-ranCrs * ra.Y, ranCrs * ra.X) / bodyA.Inertia, n));
                                Vector2 impulseVec = n * impluse;
                                bodyA.ApplyImpulse(impulseVec, collPos);




                                Vector2 tang = new Vector2(-n.Y, n.X);
                                float ratCrs = Vector2.Cross(ra, tang);
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
            float h_wid = 0.5f * bounds.Width;
            float h_hgt = 0.5f * bounds.Height;

            float dt = time.ElapsedGameTimeSeconds;
            if (dt > float.Epsilon)
            {
                if (dt > 0.05f)
                    dt = 0.05f;
                for (int i = 0; i < bodies.Count; i++)
                {
                    //if (!bodies[i].IsColliding)
                    //{
                    //    float x = bodies[i].Position.X;

                    //    if (x < h_wid)
                    //    {
                    //        float r = (bodies[i].Position.X) / (float)bounds.Width;
                    //        float r2 = 1 - r * r;
                    //        if (r2 > float.Epsilon)
                    //        {
                    //            Vector2 f1 = Vector2.UnitX * (1 / r2);
                    //            bodies[i].Force -= f1;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        float r = ((float)bounds.Width - bodies[i].Position.X) / (float)bounds.Width;
                    //        float r2 = 1 - r * r;
                    //        if (r2 > float.Epsilon)
                    //        {
                    //            Vector2 f1 = Vector2.UnitX * (1 / r2);
                    //            bodies[i].Force += f1;
                    //        }
                    //    }
                    //}

                    bodies[i].Integrate(dt);
                }
                Collision(dt);
            }
            //else Console.Write('!');
        }
    }
}
