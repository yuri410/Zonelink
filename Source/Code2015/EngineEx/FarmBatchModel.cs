using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Code2015.BalanceSystem;
using Code2015.World;

namespace Code2015.EngineEx
{
    class FarmBatchModel : Resource , IRenderable
    {
        RenderSystem renderSys;

        //VertexBuffer vtxBuffer;
        //VertexDeclaration vtxDecl;
        //IndexBuffer[] idxBuffer;
        //Material[] materials;
        //RenderOperation[] opBuf;



        int resourceSize;

        public BoundingSphere BoundingVolume;
        public Matrix Transformation;

        public FarmBatchModel(RenderSystem rs, FarmLand farm)
            : base(TreeBatchModelManager.Instance,
                   farm.Longitude.ToString() + "_" + farm.Latitude.ToString())
        {
            //this.info = info;
            this.renderSys = rs;

            float radlng = MathEx.Degree2Radian(farm.Longitude);
            float radlat = MathEx.Degree2Radian(farm.Latitude);

            Transformation = PlanetEarth.GetOrientation(radlng, radlat);
            Transformation.TranslationValue = PlanetEarth.GetPosition(radlng, radlat);

            BoundingVolume.Center = PlanetEarth.GetPosition(radlng, radlat);
            //BoundingVolume.Radius = PlanetEarth.GetTileArcLength(MathEx.Degree2Radian(farm.Radius));
        }

        public override int GetSize()
        {
            return resourceSize;
        }

        protected override void load()
        {
            resourceSize = 0;
            throw new NotImplementedException();
        }

        protected override void unload()
        {
            if (vtxBuffer != null)
            {
                vtxBuffer.Dispose();

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
                vtxDecl.Dispose();
                vtxDecl = null;
            }
        }

        #region IRenderable 成员

        public RenderOperation[] GetRenderOperation()
        {
            if (State == ResourceState.Loaded)
            {
                for (int i = 0; i < opBuf.Length; i++)
                {
                    opBuf[i].Transformation = Matrix.Identity;
                }
                return opBuf;
            }
            return null;
        }

        public RenderOperation[] GetRenderOperation(int level)
        {
            return GetRenderOperation();
        }

        #endregion
    }
}
