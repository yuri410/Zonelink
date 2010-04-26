using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.Vfs;

namespace Code2015.Effects.Post
{
    class PieProgressEffect : PostEffect
    {
        public PieProgressEffect(RenderSystem rs)
            : base(rs)
        {
            FileLocation fl = FileSystem.Instance.Locate("pieprogress.cps", FileLocateRule.Effects);
            LoadPixelShader(rs, fl);

            fl = FileSystem.Instance.Locate("pieprogress.cvs", FileLocateRule.Effects);
            LoadVertexShader(rs, fl);            
        }

        public override void Begin()
        {
            
            renderSys.BindShader(vtxShader);
            renderSys.BindShader(pixShader);
       
        }
    }
}
