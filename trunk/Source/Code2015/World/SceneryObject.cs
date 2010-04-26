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
            ModelL0.CurrentAnimation = new NoAnimation(Matrix.Scaling(scale, scale, scale) * 
                Matrix.RotationY(MathEx.Degree2Radian(rot)));

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
