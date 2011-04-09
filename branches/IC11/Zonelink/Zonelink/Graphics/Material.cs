/*
-----------------------------------------------------------------------------
This source file is part of Apoc3D Engine

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
using System.IO;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace Apoc3D.Graphics
{

    public abstract class MaterialBase
    {
        public const int MaxTexLayers = 16;
        
        protected MaterialBase()
        {
            ZWriteEnabled = true;
            ZEnabled = true;

        }
        
        #region Properties


        public CullMode CullMode
        {
            get;
            set;
        }

        public bool IsTransparent
        {
            get;
            set;
        }

        public float AlphaRef
        {
            get;
            set;
        }

        /// <summary>
        ///  获取或设置一个值表示材质的法向量是否总是朝上。
        /// </summary>
        public bool IsVegetation
        {
            get;
            set;
        }

        public bool ZEnabled
        {
            get;
            set;
        }
        public bool ZWriteEnabled
        {
            get;
            set;
        }
        #endregion
    }

    /// <summary>
    ///  定义材质的基本结构
    /// </summary>
    /// <typeparam name="TexType"></typeparam>
    public class Material : MaterialBase
    {
        
        #region Constants

        public static Material DefaultMaterial
        {
            get;
            private set;
        }

        #endregion

        #region 构造函数
        static Material()
        {
            DefaultMaterial = new Material();

            Vector4 clr = new Vector4();
            clr.W = 1;
            clr.X = 1;
            clr.Y = 1;
            clr.Z = 1;

            DefaultMaterial.Ambient = clr;
            DefaultMaterial.Diffuse = clr;
            DefaultMaterial.Power = 0;
            clr.W = 0;
            clr.X = 0;
            clr.Y = 0;
            clr.Z = 0;
            DefaultMaterial.Emissive = clr;
            DefaultMaterial.Specular = clr;

            DefaultMaterial.CullMode = CullMode.None;
        }

        public Material()
        {
        }
        #endregion

        #region Field
        protected Vector4 ambient;
        protected Vector4 diffuse;
        protected Vector4 specular;
        protected Vector4 emissive;
        protected float power;


        protected Texture[] textures = new Texture[MaxTexLayers];


        
        #endregion

        #region 属性


        public Vector4 Ambient
        {
            get { return ambient; }
            set { ambient = value; }
        }

        public Vector4 Diffuse
        {
            get { return diffuse; }
            set { diffuse = value; }
        }

        public Vector4 Specular
        {
            get { return specular; }
            set { specular = value; }
        }

        public Vector4 Emissive
        {
            get { return emissive; }
            set { emissive = value; }
        }

        public float Power
        {
            get { return power; }
            set { power = value; }
        }


        public int BatchIndex
        {
            get;
            protected set;
        }


        public Texture GetTexture(int idx)
        {
            return textures[idx];
        }
        public void SetTexture(int idx, Texture tex)
        {
            textures[idx] = tex;
        }
        #endregion

    }


}
