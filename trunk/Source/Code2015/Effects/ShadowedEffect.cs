using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics.Effects;
using Apoc3D.Graphics;
using Apoc3D.Vfs;
using Code2015.EngineEx;

namespace Code2015.Effects
{
    public abstract class ShadowedEffect : Effect
    {
        protected PixelShader shdPixShader;
        protected VertexShader shdVtxShader;

        public unsafe ShadowedEffect(RenderSystem rs, string name, bool inst)
            : base(inst, name)
        {

            FileLocation fl = FileSystem.Instance.Locate("shadowMap.cvs", GameFileLocs.Effect);
            shdVtxShader = LoadVertexShader(rs, fl);

            fl = FileSystem.Instance.Locate("shadowMap.cps", GameFileLocs.Effect);
            shdPixShader = LoadPixelShader(rs, fl);
        }

        public override bool SupportsMode(RenderMode mode)
        {
            return true;
        }
    }
}
