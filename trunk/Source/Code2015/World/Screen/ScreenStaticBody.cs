using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.MathLib;

namespace Code2015.World.Screen
{
    class ScreenStaticBody : ScreenPhysicsObject
    {
        Vector2[] path;
        Vector2[] pathNormal;
        Vector2[] segNormals;

        public void SetShape(Vector2[] path)
        {
            int len = path.Length;

            this.path = path;
            this.pathNormal = new Vector2[len];

            segNormals = new Vector2[len - 1];
 
            for (int i = 0; i < len - 1; i++)
            {
                Vector2 dir = path[i + 1] - path[i];

                dir.Normalize();

                float tmp = dir.X;
                dir.X = -dir.Y;
                dir.Y = tmp;

                segNormals[i] = dir;                
            }

            pathNormal[0] = segNormals[0];
            pathNormal[len - 1] = segNormals[len - 2];

            for (int i = 1; i < len - 1; i++)
            {
                pathNormal[i] = 0.5f * (segNormals[i - 1] + segNormals[i]);
            }
        }
    }
}
