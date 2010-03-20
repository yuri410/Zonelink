using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Scene;
using Apoc3D.Graphics;
using Apoc3D.Core;

namespace Code2015.World
{
    class RoadObject : StaticModelObject
    {
        ModelData datal0;

        public override bool IsSerializable
        {
            get { return false; }
        }


        public RoadObject()
            : base(false)
        {

        }

        public void SetData(ModelData data)
        {
            if (object.ReferenceEquals(datal0, data))
            {
                return;
            }
            else if (!object.ReferenceEquals(data, null))
            {
                ModelL0 = null;
                datal0.Dispose();
            }

            if (!object.ReferenceEquals(data, null))
            {
                datal0 = data;

                ModelL0 = new Model(new ResourceHandle<ModelData>(data));
            }
        }
    }
}
