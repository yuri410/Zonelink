using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Code2015.BalanceSystem;

namespace Code2015.World
{
    class ForestObject : SceneObject
    {
        struct Tree 
        {
            public Matrix Transformation;
            public Model Model;
        }

        Forest forest;

        RenderOperation[] opBuffer;

        public ForestObject(Forest forest)
            : base(false)
        {
            this.forest = forest;
        }

        public override RenderOperation[] GetRenderOperation()
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime dt)
        {
            throw new NotImplementedException();
        }

        public override bool IsSerializable
        {
            get { return false; }
        }
    }
}
