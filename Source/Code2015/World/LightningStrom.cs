using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Scene;
using Apoc3D.Graphics;
using Apoc3D;
using Code2015.Logic;

namespace Code2015.World
{
    class LightningStrom : SceneObject
    {
        Disaster disaster;

        public LightningStrom(Disaster disaster)
            : base(false)
        {
            this.disaster = disaster;
        }

        public override RenderOperation[] GetRenderOperation()
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime dt)
        {
            
        }

        public override bool IsSerializable
        {
            get { return false; }
        }
    }
}
