using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Effects;
using Apoc3D.Vfs;

namespace Code2015.Effects
{
    class Composite : PostEffect
    {
        public Composite(RenderSystem rs)
            : base(rs)
        {
            FileLocation fl = FileSystem.Instance.Locate("composite.cps", FileLocateRule.Effects);
            LoadPixelShader(rs, fl);

            fl = FileSystem.Instance.Locate("postQuad.cvs", FileLocateRule.Effects);
            LoadVertexShader(rs, fl);
        }
    }
}
