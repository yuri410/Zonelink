using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Core;
using Apoc3D.Graphics;

namespace Code2015.EngineEx
{
    class TreeBatchModel : Resource, IRenderable
    {
        public TreeBatchModel(float lng, float lat, float r, Model[] treeModels, Model[] smVeg)
        {

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
            throw new NotImplementedException();
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
