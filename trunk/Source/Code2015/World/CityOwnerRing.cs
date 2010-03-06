using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.MathLib;

namespace Code2015.World
{
    class CityOwnerRing : IRenderable
    {
        CityObject parent;
        CityStyleTable styleTable;

        public CityOwnerRing(CityObject obj, CityStyleTable styleTable)
        {
            this.parent = obj;
            this.styleTable = styleTable;
        }

        public Vector4 GetWeights()
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
