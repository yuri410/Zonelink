using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.Collections;
using Apoc3D.MathLib;
using Code2015.World;

namespace Code2015.EngineEx
{
    enum PlantCategory
    {
        Forest,

    }
    enum PlantType
    {
        Cold,
        Temperate,
        Hot
    }

    struct ForestInfo 
    {
        public float Longitude;
        public float Latitude;
        public float Radius;

        public PlantType Type;
        public PlantCategory Category;

        public Model[] BigPlants;
        public Model[] SmallPlants;
    }

    class TreeBatchModel : Resource, IRenderable
    {
        FastList<RenderOperation> opBuffer = new FastList<RenderOperation>();

        VertexBuffer[] vtxBuffer;
        VertexDeclaration[] vtxDecl;
        IndexBuffer[] idxBuffer;

        public BoundingSphere BoundingVolume;

        public TreeBatchModel(ForestInfo info)
        {
            float radlng = MathEx.Degree2Radian(info.Longitude);
            float radlat = MathEx.Degree2Radian(info.Latitude);
            BoundingVolume.Center = PlanetEarth.GetPosition(radlng, radlat);

            BoundingVolume.Radius = PlanetEarth.GetTileArcLength(MathEx.Degree2Radian(info.Radius));
        }

        public override int GetSize()
        {
            throw new NotImplementedException();
        }

        protected override void load()
        {
            throw new NotImplementedException();
        }

        protected override void unload()
        {
            if (vtxBuffer != null) 
            {
                for (int i = 0; i < vtxBuffer.Length; i++)
                {
                    if (vtxBuffer[i] != null && !vtxBuffer[i].Disposed)
                        vtxBuffer[i].Dispose();
                }
                vtxBuffer = null;
            }
            if (idxBuffer != null)
            {
                for (int i = 0; i < idxBuffer.Length; i++)
                {
                    if (idxBuffer[i] != null && !idxBuffer[i].Disposed)
                        idxBuffer[i].Dispose();
                }
                idxBuffer = null;
            }
            if (vtxDecl != null) 
            {
                for (int i = 0; i < vtxDecl.Length; i++) 
                {
                    if (vtxDecl[i] != null && !vtxDecl[i].Disposed)
                        vtxDecl[i].Dispose();
                }
                vtxDecl = null;
            }

        }

        #region IRenderable 成员

        public RenderOperation[] GetRenderOperation()
        {
            throw new NotImplementedException();
        }

        public RenderOperation[] GetRenderOperation(int level)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
