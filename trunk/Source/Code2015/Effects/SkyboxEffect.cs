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
http://www.gnu.org/copyleft/gpl.txt.

-----------------------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Effects;
using Apoc3D.MathLib;

namespace Code2015.Effects
{
    public class SkyboxEffectFactory : EffectFactory
    {
        static readonly string typeName = "Skybox";

        public static string Name
        {
            get { return typeName; }
        }


        RenderSystem renderSystem;

        public SkyboxEffectFactory(RenderSystem rs)
        {
            renderSystem = rs;
        }

        public override Effect CreateInstance()
        {
            return new WaterEffect(renderSystem);
        }

        public override void DestroyInstance(Effect fx)
        {
            fx.Dispose();
        }
    }

    class SkyboxEffect : Effect
    {
        RenderSystem renderSystem;

        PixelShader pixShader;
        VertexShader vtxShader;

        public SkyboxEffect(RenderSystem renderSystem)
            : base(false, SkyboxEffectFactory.Name)
        {
            this.renderSystem = renderSystem;
        }

        protected override int begin()
        {
            renderSystem.BindShader(vtxShader);
            renderSystem.BindShader(pixShader);

            return 1;
        }

        protected override void end()
        {
            renderSystem.BindShader((VertexShader)null);
            renderSystem.BindShader((PixelShader)null);
        }

        public override void BeginPass(int passId)
        {
        }

        public override void EndPass()
        {

        }

        //public override void BeginShadowPass()
        //{
        //}

        //public override void EndShadowPass()
        //{
        //}

        public override void Setup(Material mat, ref RenderOperation op)
        {
            pixShader.SetTexture("texColor", mat.GetTexture(0));
            Matrix mvp = op.Transformation * EffectParams.CurrentCamera.ViewMatrix * EffectParams.CurrentCamera.ProjectionMatrix;

            vtxShader.SetValue("mvp", ref mvp);
        }

        //public override void SetupShadowPass(Material mat, ref RenderOperation op)
        //{
        //}

        protected override void Dispose(bool disposing)
        {
            vtxShader.Dispose();
            pixShader.Dispose();
        }
    }
}
