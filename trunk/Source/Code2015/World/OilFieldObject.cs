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
    class OilFieldObject : StaticModelObject, ISelectableObject, IResourceObject
    {
        OilField oilField;

        SoundObject sound;
        public OilFieldObject(RenderSystem rs, OilField field)
        {
            oilField = field;

           
            float radLng = MathEx.Degree2Radian(field.Longitude);
            float radLat = MathEx.Degree2Radian(field.Latitude);

            bool isOcean = false;

            float alt = TerrainData.Instance.QueryHeight(radLng, radLat);

            if (alt < 0)
            {
                alt = 0;
                isOcean = true;
            }
            if (isOcean)
            {
                FileLocation fl = FileSystem.Instance.Locate("oilderrick_oc.mesh", GameFileLocs.Model);

                ModelL0 = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                ModelL0.CurrentAnimation = new NoAnimation(
                    Matrix.Scaling(Game.ObjectScale * 1.75f, Game.ObjectScale * 1.75f, Game.ObjectScale * 1.75f) * Matrix.Translation(0, 18, 0) * Matrix.RotationY(-MathEx.PiOver4));
            }
            else
            {
                FileLocation fl = FileSystem.Instance.Locate("oilderrick.mesh", GameFileLocs.Model);

                ModelL0 = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                ModelL0.CurrentAnimation = new NoAnimation(
                    Matrix.Scaling(Game.ObjectScale * 1.75f, Game.ObjectScale * 1.75f, Game.ObjectScale * 1.75f) * Matrix.RotationY(-MathEx.PiOver4));
            }


          
            Position = PlanetEarth.GetPosition(radLng, radLat, PlanetEarth.PlanetRadius + alt * TerrainMeshManager.PostHeightScale);
            BoundingSphere.Center = position;

            BoundingSphere.Radius = 50;
            Orientation = PlanetEarth.GetOrientation(radLng, radLat);

            sound = SoundManager.Instance.MakeSoundObjcet("oil", null, BoundingSphere.Radius * 8);
            sound.Position = position;
        }

        public override bool IsSerializable
        {
            get { return false ; }
        }


        public OilField OilField 
        {
            get { return oilField; }
        }

        public override RenderOperation[] GetRenderOperation()
        {
            if (ResVisible != null)
            {
                ResVisible(this);
            }
            return base.GetRenderOperation();
        }
        public override RenderOperation[] GetRenderOperation(int level)
        {
            if (ResVisible != null)
            {
                ResVisible(this);
            }
            return base.GetRenderOperation(level);
        }
        public override void Update(Apoc3D.GameTime dt)
        {
            base.Update(dt);
            sound.Update(dt);
        }
        #region ISelectableObject 成员

        bool ISelectableObject.IsSelected
        {
            get;
            set;
        }

        #endregion

        #region IResourceObject 成员

        public float AmountPer
        {
            get { return oilField.CurrentAmount / BalanceSystem.Forest.MaxAmount; }
        }

        public NaturalResourceType Type
        {
            get { return oilField.Type; }
        }
        public event ResourceVisibleHander ResVisible;

        public float Longitude
        {
            get { return oilField.Longitude; }
        }

        public float Latitude
        {
            get { return oilField.Latitude; }
        }

        public float Radius
        {
            get { return BoundingSphere.Radius; }
        }

        #endregion
    }
}
