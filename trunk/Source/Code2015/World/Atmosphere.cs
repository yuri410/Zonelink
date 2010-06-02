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
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Effects;
using Apoc3D.Graphics.Geometry;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Code2015.Effects;
using Code2015.EngineEx;

namespace Code2015.World
{
    class Atmosphere : StaticModelObject
    {
        const float AtmosphereThickness = 2500;

        Sphere atmoSphere;
        RenderSystem renderSys;

        public Atmosphere(RenderSystem rs)
            : base(false)
        {
            renderSys = rs;

            Material[][] mats = new Material[1][];
            mats[0] = new Material[1];
            mats[0][0] = new Material(renderSys);
            mats[0][0].AlphaRef = -1;
            mats[0][0].ZEnabled = true;
            mats[0][0].ZWriteEnabled = true;
            mats[0][0].IsTransparent = false;
            mats[0][0].CullMode = CullMode.None;
            
            mats[0][0].Ambient = new Color4F(1, 1, 1, 1);
            mats[0][0].Diffuse = new Color4F(1, 1, 1, 1);

            mats[0][0].PriorityHint = RenderPriority.First;
            mats[0][0].SetEffect(EffectManager.Instance.GetModelEffect(AtmosphereEffectFactory.Name));
            atmoSphere = new Sphere(rs, PlanetEarth.PlanetRadius + AtmosphereThickness,
               PlanetEarth.ColTileCount, PlanetEarth.LatTileCount, mats);

            base.ModelL0 = atmoSphere;

            Transformation = Matrix.Identity;
            BoundingSphere.Radius = PlanetEarth.PlanetRadius;// +AtmosphereThickness;
        }


        public override bool IsSerializable
        {
            get { return false; }
        }
    }
}
