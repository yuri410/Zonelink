/*
-----------------------------------------------------------------------------
This source file is part of Zonelink

Copyright (c) 2009+ Tao Games

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  if not, write to the Free Software Foundation, 
Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA, or go to
http://www.gnu.org/copyleft/lesser.txt.

-----------------------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Effects;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.ParticleSystem;

namespace Code2015.Effects
{
    public class SmokeRDEffectFactory : EffectFactory
    {
        static readonly string typeName = "Smoke";


        public static string Name
        {
            get { return typeName; }
        }
        RenderSystem device;

        public SmokeRDEffectFactory(RenderSystem dev)
        {
            device = dev;
        }

        public override Effect CreateInstance()
        {
            return new SmokeRDEffect(device);
        }

        public override void DestroyInstance(Effect fx)
        {
            fx.Dispose();
        }
    }

    class SmokeRDEffect : Effect
    {
        RenderSystem renderSys;

        PixelShader pixShader;
        VertexShader vtxShader;
        Texture noTexture;

        public unsafe SmokeRDEffect(RenderSystem rs)
            : base(false, SmokeRDEffectFactory.Name)
        {
            this.renderSys = rs;

            FileLocation fl = FileSystem.Instance.Locate("smoke.cvs", GameFileLocs.Effect);
            vtxShader = LoadVertexShader(renderSys, fl);

            fl = FileSystem.Instance.Locate("smoke.cps", GameFileLocs.Effect);
            pixShader = LoadPixelShader(renderSys, fl);

            fl = FileSystem.Instance.Locate("tillingmark.tex", GameFileLocs.Texture);
            noTexture = TextureManager.Instance.CreateInstance(fl);
        }

        public override bool SupportsMode(RenderMode mode)
        {
            return mode != RenderMode.Depth;
        }

        protected override int begin()
        {
            renderSys.BindShader(vtxShader);
            renderSys.BindShader(pixShader);

            return 1;
            //return effect.Begin(FX.DoNotSaveState | FX.DoNotSaveShaderState | FX.DoNotSaveSamplerState);
        }
        protected override void end()
        {
            //effect.End();
        }
        public override void BeginPass(int passId)
        {
            //effect.BeginPass(passId);
        }
        public override void EndPass()
        {
            //effect.EndPass();
        }

        public override void Setup(Material mat, ref RenderOperation op)
        {
            //EffectParams.InvView.Right
            Matrix view = EffectParams.CurrentCamera.ViewMatrix;
            //Vector3 tl = view.TranslationValue;
            //view = Matrix.Identity;
            //view.TranslationValue = tl;
            vtxShader.SetValue("right", EffectParams.InvView.Right);
            vtxShader.SetValue("up", EffectParams.InvView.Up);

            vtxShader.SetValue("mvp",
                view * EffectParams.CurrentCamera.ProjectionMatrix);

            ParticleEffect partEff = op.Sender as ParticleEffect;

            vtxShader.SetValue("size", partEff == null ? 1f : partEff.ParticleSize);

            ResourceHandle<Texture> clrTex = mat.GetTexture(0);
            if (clrTex == null)
            {
                pixShader.SetTexture("texDif", noTexture);
            }
            else
            {
                pixShader.SetTexture("texDif", clrTex);
            }
        }

        protected override void Dispose(bool disposing)
        {
        }
    }
}
