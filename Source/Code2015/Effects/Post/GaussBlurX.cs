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
            string filePath = "blurX.cps";
            FileLocation fl = FileSystem.Instance.Locate(filePath, FileLocateRule.Effects);

            LoadPixelShader(rs, fl);
        }
    }
}
