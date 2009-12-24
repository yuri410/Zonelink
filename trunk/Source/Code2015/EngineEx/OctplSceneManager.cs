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

            Vector3 dir;
            Plane nearPlane;
            frus.GetPlane(FrustumPlane.Near, out nearPlane);

            dir = nearPlane.Normal;

            Vector3 camPos = camera.Position;
            // do a BFS pass here

            queue.Enqueue(octRootNode);

            while (queue.Count > 0)
            {
                OctreeSceneNode node = queue.Dequeue();

                Vector3 center = node.BoundingSphere.Center;

                // if the node does't intersect the frustum we don't give a damn
                if (//Vector3.Dot(ref dir, ref center) <= 0 &&
                    frus.IntersectsSphere(ref center, node.BoundingSphere.Radius))
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
                        int level = GetLevel(ref objs.Elements[i].BoundingSphere, ref camPos);

                        if (objs.Elements[i].HasSubObjects)
                        {
                            objs.Elements[i].PrepareVisibleObjects(camera, level);
                        }
                        AddVisibleObject(objs.Elements[i], level, batchHelper);
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
