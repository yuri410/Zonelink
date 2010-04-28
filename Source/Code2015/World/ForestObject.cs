using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Code2015.BalanceSystem;
using Code2015.EngineEx;

namespace Code2015.World
{
    public delegate void ResourceVisibleHander(IResourceObject obj);

    public interface IResourceObject 
    {
        float AmountPer
        {
            get;
        }
        float MaxValue { get; }
        NaturalResourceType Type { get; }
        Vector3 Position { get; }
        float Longitude { get; }
        float Latitude { get; }
        float Radius { get; }
        event ResourceVisibleHander ResVisible;
    }

    class ForestObject : SceneObject, ISelectableObject, IResourceObject
    {
        Forest forest;
        ResourceHandle<TreeBatchModel> model;
        SoundObject sound;

        Vector3 stdPosition;

        public ForestObject(RenderSystem rs, Forest forest)
            : base(false)
        {
            this.forest = forest;


            ForestInfo info;
            info.Latitude = forest.Latitude;
            info.Longitude = forest.Longitude;
            info.Radius = forest.Radius;

            info.Amount = forest.CurrentAmount;

            //info.Plants = TreeModelLibrary.Instance.Get(0);

            model = TreeBatchModelManager.Instance.CreateInstance(rs, info);

            model.Touch();

            Transformation = model.GetWeakResource().Transformation;
            BoundingSphere = model.GetWeakResource().BoundingVolume;

            sound = SoundManager.Instance.MakeSoundObjcet("forest", null, BoundingSphere.Radius);
            sound.Position = BoundingSphere.Center;

            {
                float radLng = MathEx.Degree2Radian(forest.Longitude);
                float radLat = MathEx.Degree2Radian(forest.Latitude);

                float alt = TerrainData.Instance.QueryHeight(radLng, radLat);

                stdPosition = PlanetEarth.GetPosition(radLng, radLat, alt * TerrainMeshManager.PostHeightScale + PlanetEarth.PlanetRadius);
            }
        }

        public override RenderOperation[] GetRenderOperation()
        {
            if (ResVisible != null)
            {
                ResVisible(this);
            }
            TreeBatchModel mdl = model;

            if (forest.CurrentAmount < 0.2 * forest.MaxAmount)
            {
                return mdl.GetRenderOperation2();
            }
            return mdl.GetRenderOperation();
        }

        public override void Update(GameTime dt)
        {
            sound.Update(dt);
        }

        public override bool IsSerializable
        {
            get { return false; }
        }

        public Forest Forest
        {
            get { return forest; }
        }

        #region ISelectableObject 成员

        bool ISelectableObject.IsSelected
        {
            get;
            set;
        }

        #endregion

        #region IResourceObject 成员
        public float MaxValue 
        {
            get { return forest.MaxAmount / (7500f * 2); }
        }
        public float AmountPer
        {
            get { return forest.CurrentAmount / (7500f * 2); }
        }
        public NaturalResourceType Type
        {
            get { return forest.Type; }
        }
        public event ResourceVisibleHander ResVisible;


        public Vector3 Position
        {
            get { return stdPosition; }
        }

        public float Longitude
        {
            get { return forest.Longitude; }
        }

        public float Latitude
        {
            get { return forest.Latitude; }
        }

        public float Radius 
        {
            get { return forest.Radius; }
        }

        #endregion
    }
}
