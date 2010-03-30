using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.Vfs;
using Apoc3D.Graphics.Animation;
using Apoc3D.MathLib;

namespace Code2015.EngineEx
{
    class Cloud : IRenderable
    {
        RenderSystem renderSys;

        VertexBuffer vtxBuffer;
        IndexBuffer idxBuffer;

        Model model;

        float scale;

        public Cloud(RenderSystem rs)
        {
            this.renderSys = rs;
            this.scale = 1;

            FileLocation fl = FileSystem.Instance.Locate("cloud_lgt.mesh", GameFileLocs.Model);

            model = new Model(ModelManager.Instance.CreateInstance(rs, fl));
            model.CurrentAnimation = new NoAnimation(Matrix.Scaling(scale, scale, scale));
        }

        public float Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                if (model != null)
                {
                    model.CurrentAnimation = new NoAnimation(Matrix.Scaling(scale, scale, scale));
                }
            }
        }

        #region IRenderable 成员

        public RenderOperation[] GetRenderOperation()
        {
            return model.GetRenderOperation();
        }

        public RenderOperation[] GetRenderOperation(int level)
        {
            return model.GetRenderOperation();
        }

        #endregion
    }
}
