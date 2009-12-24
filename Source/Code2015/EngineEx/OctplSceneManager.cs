using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Scene;

namespace Code2015.EngineEx
{
    class OctplSceneManager : OctreeSceneManager
    {
        const int WorldLodLevelCount = 3;
        static readonly float[] LodThresold = new float[WorldLodLevelCount] { 0, 1.5f, 4 };

        public OctplSceneManager(float planetRadius)
            : base(new OctreeBox(planetRadius * 4f), planetRadius / 75f)
        {
        }

        protected int GetLevel(ref BoundingSphere sphere, ref Vector3 pos)
        {
            float dist = Vector3.Distance(ref sphere.Center, ref pos);


            for (int i = WorldLodLevelCount - 1; i >= 0; i--)
            {
                if (dist >= sphere.Radius * LodThresold[i])
                {
                    return i;
                }
            }
            return WorldLodLevelCount;
            //if (dist > sphere.Radius * 5)
            //    return 3;

            //if (dist > sphere.Radius * 3.5f)
            //    return 2;

            //if (dist > sphere.Radius * 2.5f)
            //    return 1;
            //return 0;
        }

        public override void PrepareVisibleObjects(ICamera camera, PassData batchHelper)
        {
            batchHelper.visibleObjects.FastClear();

            Frustum frus = camera.Frustum;

            //Plane nearPlane;
            //frus.GetPlane(FrustumPlane.Near, out nearPlane);
            //Plane leftPlane;
            //frus.GetPlane(FrustumPlane.Left, out leftPlane);
            //Plane rightPlane;
            //frus.GetPlane(FrustumPlane.Right, out rightPlane);

            //Vector3 viewUp;
            //Vector3.Cross(ref leftPlane.Normal, ref nearPlane.Normal, out viewUp);

            //Vector3 leftDir;
            //Vector3.Cross(ref viewUp, ref leftPlane.Normal, out leftDir);

            //Vector3 rightDir;
            //Vector3.Cross(ref rightPlane.Normal, ref viewUp, out rightDir);

            Vector3 camPos = camera.Position;
            // do a BFS pass here

            queue.Enqueue(octRootNode);

            while (queue.Count > 0)
            {
                OctreeSceneNode node = queue.Dequeue();

                Vector3 dir;
                Vector3 center2;
                
                if (frus.IntersectsSphere(ref node.BoundingSphere.Center, node.BoundingSphere.Radius))
                {
                    for (int i = 0; i < 2; i++)
                        for (int j = 0; j < 2; j++)
                            for (int k = 0; k < 2; k++)
                            {
                                if (node[i, j, k] != null)
                                {
                                    queue.Enqueue(node[i, j, k]);
                                }
                            }
                    FastList<SceneObject> objs = node.AttchedObjects;
                    for (int i = 0; i < objs.Count; i++)
                    {
                        SceneObject obj = objs.Elements[i];
                        dir = obj.BoundingSphere.Center;
                        dir.Normalize();
                        Vector3.Multiply(ref dir, node.BoundingSphere.Radius, out dir);
                        Vector3.Add(ref obj.BoundingSphere.Center, ref dir, out center2);

                        Vector3.Subtract(ref obj.BoundingSphere.Center, ref camPos, out dir);


                        if (Vector3.Dot(ref dir, ref center2) <= 0)
                        {
                            int level = GetLevel(ref obj.BoundingSphere, ref camPos);

                            if (obj.HasSubObjects)
                            {
                                obj.PrepareVisibleObjects(camera, level);
                            }
                            AddVisibleObject(obj, level, batchHelper);
                        }
                    }
                }

            }

            for (int i = 0; i < farObjects.Count; i++)
            {
                int level = GetLevel(ref farObjects[i].BoundingSphere, ref camPos);

                AddVisibleObject(farObjects[i], level, batchHelper);
            }
        }
    }
}
