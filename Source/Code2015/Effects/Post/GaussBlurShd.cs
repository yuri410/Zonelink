using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.Vfs;

namespace Code2015.Effects
{
    class GaussBlurShd : PostEffect
    {
        public GaussBlurShd(RenderSystem rs)
            : base(rs)
        {
            FileLocation fl = FileSystem.Instance.Locate("blurShd.cps", FileLocateRule.Effects);
            LoadPixelShader(rs, fl);

            //fl = FileSystem.Instance.Locate("postQuad.cvs", FileLocateRule.Effects);
            //LoadVertexShader(rs, fl);
        }
    }
}
