using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.Vfs;

namespace Code2015.Effects
{
    class GaussBlurX : PostEffect
    {
        public GaussBlurX(RenderSystem rs)
            : base(rs)
        {
            FileLocation fl = FileSystem.Instance.Locate("blurX.cps", FileLocateRule.Effects);
            LoadPixelShader(rs, fl);

            //fl = FileSystem.Instance.Locate("postQuad.cvs", FileLocateRule.Effects);
            //LoadVertexShader(rs, fl);
        }
    }
}
