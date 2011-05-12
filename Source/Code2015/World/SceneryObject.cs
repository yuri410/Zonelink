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
using Apoc3D.Config;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Animation;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.EngineEx;

namespace Code2015.World
{
    class SceneryObject : StaticModelObject
    {
        public SceneryObject(RenderSystem rs, ConfigurationSection sect)
        {
            FileLocation fl = FileSystem.Instance.Locate(sect["Model"], GameFileLocs.Model);
            float scale = sect.GetSingle("Radius", 1);

            float rot = sect.GetSingle("Amount", 0);

            ModelL0 = new Model(ModelManager.Instance.CreateInstance(rs, fl));
            ModelL0.CurrentAnimation.Clear();
            ModelL0.CurrentAnimation.Add(new NoAnimaionPlayer(Matrix.Scaling(scale, scale, scale) * 
                Matrix.RotationY(MathEx.Degree2Radian(rot))));

            float lng = sect.GetSingle("Longitude");
            float lat = sect.GetSingle("Latitude");
            lng = MathEx.Degree2Radian(lng);
            lat = MathEx.Degree2Radian(lat);

            float alt = TerrainData.Instance.QueryHeight(lng, lat);

            Position = PlanetEarth.GetPosition(lng, lat, PlanetEarth.PlanetRadius + alt * TerrainMeshManager.PostHeightScale);
            Orientation =  PlanetEarth.GetOrientation(lng, lat);
        }

        public override bool IsSerializable
        {
            get { return false; }
        }

    }
}
