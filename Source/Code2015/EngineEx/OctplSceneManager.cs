using System;
using System.Collections.Generic;
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
        const int WorldLodLevelCount = 4;
        static readonly float[] LodThresold = new float[WorldLodLevelCount] { 0.5f, 1.33f, 3, 4.5f };

        public OctplSceneManager(float planetRadius)
            : base(new OctreeBox(planetRadius * 4f), planetRadius / 75f)
        {
        }

        protected int GetLevel(ref BoundingSphere sphere, ref Vector3 pos)
        {
            float dist = Vector3.Distance(ref sphere.Center, ref pos);

            for (int i = 0; i < WorldLodLevelCount; i++)
            {
                if (dist <= sphere.Radius * LodThresold[i])
                {
                    return i;
                }
            }
            return WorldLodLevelCount;
        }

        public override void PrepareVisibleObjects(ICamera camera, PassData batchHelper)
        {
            batchHelper.visibleObjects.FastClear();

            Frustum frus = camera.Frustum;

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

                        Vector3.Multiply(ref dir, obj.BoundingSphere.Radius, out dir);
                        Vector3.Subtract(ref obj.BoundingSphere.Center, ref dir, out center2);

                        Vector3.Subtract(ref center2, ref camPos, out dir);

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
