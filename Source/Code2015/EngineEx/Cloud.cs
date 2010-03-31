using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.Vfs;
using Apoc3D.Graphics.Animation;
using Apoc3D.MathLib;
using Apoc3D;

namespace Code2015.EngineEx
{
    class Cloud : IRenderable, IUpdatable
    {
        RenderSystem renderSys;

        //VertexBuffer vtxBuffer;
        //IndexBuffer idxBuffer;

        Model model;

        float scale;
        Matrix trans;

        float startTime;

        public float StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        public Cloud(RenderSystem rs, float beginTime)
        {
            this.renderSys = rs;
            this.scale = 1;
            this.startTime = beginTime;

            FileLocation fl = FileSystem.Instance.Locate("cloud_lgt.mesh", GameFileLocs.Model);

            model = new Model(ModelManager.Instance.CreateInstance(rs, fl));
            model.CurrentAnimation = new NoAnimation(Matrix.Scaling(scale, scale, scale));
        }

        void UpdateModelAnim()
        {
            if (model != null)
            {
                model.CurrentAnimation = new NoAnimation(Matrix.Scaling(scale, scale, scale) * trans);
            }
        }

        public float Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                UpdateModelAnim();
            }
        }

        public Matrix Transform 
        {
            get { return trans; }
            set
            {
                trans = value;
                UpdateModelAnim();
            }
        }


        #region IRenderable 成员

        public RenderOperation[] GetRenderOperation()
        {
            if (startTime < 0)
                return model.GetRenderOperation();
            return null;
        }

        public RenderOperation[] GetRenderOperation(int level)
        {
            if (startTime < 0)
                return model.GetRenderOperation();
            return null;
        }

        #endregion

        #region IUpdatable 成员

        public void Update(GameTime dt)
        {
            startTime -= dt.ElapsedGameTimeSeconds;
        }

        #endregion
    }
}
