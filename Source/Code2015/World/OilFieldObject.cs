using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Animation;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.BalanceSystem;
using Code2015.EngineEx;

namespace Code2015.World
{
    class OilFieldObject : StaticModelObject, ISelectableObject
    {
        OilField oilField;

        public OilFieldObject(RenderSystem rs, OilField field)
        {
            oilField = field;

            FileLocation fl = FileSystem.Instance.Locate("oilderrick.mesh", GameFileLocs.Model);
            ModelL0 = new Model(ModelManager.Instance.CreateInstance(rs, fl));
            ModelL0.CurrentAnimation = new NoAnimation(
                Matrix.Scaling(Game.ObjectScale * 1.75f, Game.ObjectScale * 1.75f, Game.ObjectScale * 1.75f));

            float radLng = MathEx.Degree2Radian(field.Longitude);
            float radLat = MathEx.Degree2Radian(field.Latitude);

            Position = PlanetEarth.GetPosition(radLng,radLat);
            BoundingSphere.Center = position;

            BoundingSphere.Radius = 50;
            Orientation = PlanetEarth.GetOrientation(radLng, radLat);
        }

        public override bool IsSerializable
        {
            get { return false ; }
        }




        #region ISelectableObject 成员

        bool ISelectableObject.IsSelected
        {
            get;
            set;
        }

        #endregion
    }
}
