using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.MathLib;

namespace Code2015.World.Screen
{
    class ScreenStaticBody : ScreenPhysicsObject
    {
        struct BoundingSphere2D
        {
            public Vector2 pos;
            public float radius2;
        }

        Vector2[] path;
        //Vector2[] pathNormal;
        Vector2[] segNormals;
        BoundingSphere2D[] segBs;

        RectangleF aabb;

        public bool IntersectTest(Vector2 pos, float radius, out Vector2 normal, out Vector2 collPos)
        {
            for (int i = 0; i < path.Length - 1; i++)
            {
                float dist = Vector2.Dot(pos - path[i], segNormals[i]);

                if (dist < radius)
                {
                    dist = Vector2.DistanceSquared(pos, segBs[i].pos);

                    if (dist < segBs[i].radius2)
                    {
                        normal = segNormals[i];
                        collPos = pos + normal * dist;
                        return true;
                    }
                }
            }

            for (int i = 0; i < path.Length; i++)
            {
                float distsqr = Vector2.DistanceSquared(path[i], pos);

                if (distsqr < radius * radius)
                {
                    normal = path[i] - pos;
                    normal.Normalize();
                    collPos = path[i];
                    return true;
                }
            }
            normal = Vector2.Zero;
            collPos = Vector2.Zero;
            return false;
        }

        public bool AABBTest(Vector2 pos, float radius)
        {
            return pos.X < aabb.X + aabb.Width + radius && 
                pos.Y < aabb.Y + aabb.Height + radius &&
                pos.X > aabb.X - radius && pos.Y > aabb.Y - radius;
        }

        public void SetShape(Vector2[] path)
        {
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MaxValue;
            float minY = float.MinValue;

            int len = path.Length;

            this.path = path;
            //this.pathNormal = new Vector2[len];

            for (int i = 0; i < len; i++)
            {
                if (minX > path[i].X)
                {
                    minX = path[i].X;
                }
                if (minY > path[i].Y) 
                {
                    minY = path[i].Y;
                }

                if (maxX > path[i].X) 
                {
                    maxX = path[i].X;
                }
                if (maxY > path[i].Y)
                {
                    maxY = path[i].Y;
                }
            }
            aabb = new RectangleF(minX, minY, maxX - minX, maxY - minY);

            segNormals = new Vector2[len - 1];
            segBs = new BoundingSphere2D[len - 1];

            for (int i = 0; i < len - 1; i++)
            {
                Vector2 dir = path[i + 1] - path[i];

                segBs[i].pos = 0.5f * (path[i + 1] - path[i]);
                segBs[i].radius2 = dir.LengthSquared();

                dir.Normalize();

                float tmp = dir.X;
                dir.X = -dir.Y;
                dir.Y = tmp;

                segNormals[i] = dir;
            }
        }
    }
}
