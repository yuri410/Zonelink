using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Scene;
using Apoc3D.Graphics;
using Apoc3D;
using Code2015.BalanceSystem;

namespace Code2015.World
{
    class CityObject : SceneObject
    {
        City city;

        public CityObject(City city)
            : base(false)
        {
            this.city = city;
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
