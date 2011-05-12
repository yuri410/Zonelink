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
using Apoc3D.Vfs;
using Apoc3D.MathLib;

namespace Code2015.Effects
{
    public abstract class PostEffect
    {
        protected PixelShader pixShader;
        protected VertexShader vtxShader;

        protected RenderSystem renderSys;

        protected PostEffect(RenderSystem rs)
        {
            renderSys = rs;
        }
        public void SetSamplerStateDirect(int si, ref ShaderSamplerState state)
        {
            pixShader.SetSamplerStateDirect(si, ref state);
        }
        public void SetSamplerState(string name, ref ShaderSamplerState state)
        {
            pixShader.SetSamplerState(name, ref state);
        }
        public void SetTexture(string name, Texture tex)
        {
            pixShader.SetTexture(name, tex);
        }
        public void SetTextureDirect(int si, Texture tex)
        {
            pixShader.SetTextureDirect(si, tex);
        }
        public void SetValue(string name, ref Vector2 value)
        {
            pixShader.SetValue(name, ref value);
        }
        public void SetValue(string name, Vector2[] value)
        {
            pixShader.SetValue(name, value);
        }
        public void SetValue(string name, float[] value) 
        {
            pixShader.SetValue(name, value);
        }

        public void SetValue(string name, float  value)
        {
            pixShader.SetValue(name, value);
        }
        public void SetValue(int register, float value)
        {
            pixShader.SetValue(register, value);
        }
        public void SetValue(int register, ref Vector2 value)
        {
            pixShader.SetValue(register, ref value);
        }
        public void SetValueDirect(int register, float value)
        {
            pixShader.SetValueDirect(register, value);
        }
        public void SetValueDirect(int register, ref Vector2 value)
        {
            pixShader.SetValueDirect(register, ref value);
        }
        public void SetVSValue(int register, ref Vector2 value)
        {
            vtxShader.SetValueDirect(register, ref value);
        }



        public virtual void Begin()
        {
            renderSys.BindShader(vtxShader);
            //renderSys.BindShader((VertexShader)null);
            renderSys.BindShader(pixShader);
        }

        public void End()
        {
            renderSys.BindShader((VertexShader)null);
            renderSys.BindShader((PixelShader)null);
        }

        #region Loading Shaders
        protected void LoadVertexShader(RenderSystem rs, ResourceLocation vs)
        {
            ObjectFactory fac = rs.ObjectFactory;

            vtxShader = fac.CreateVertexShader(vs);

        }
        protected void LoadPixelShader(RenderSystem rs, ResourceLocation vs)
        {
            ObjectFactory fac = rs.ObjectFactory;

            pixShader = fac.CreatePixelShader(vs);
        }
        #endregion
    }
}
