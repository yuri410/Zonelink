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
using Apoc3D;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Scene;
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

    class ForestObject : NaturalResource, ISelectableObject, IResourceObject
    {
        ResourceHandle<TreeBatchModel> model;
        SoundObject sound;

        Vector3 stdPosition;

        public ForestObject()
        {
           

        }

        public override void InitalizeGraphics(RenderSystem rs)
        {

            ForestInfo info;
            info.Latitude = Latitude;
            info.Longitude = Longitude;
            info.Radius = Radius;

            info.Amount = CurrentAmount;

            //info.Plants = TreeModelLibrary.Instance.Get(0);

            model = TreeBatchModelManager.Instance.CreateInstance(rs, info);

            model.Touch();

            Transformation = model.GetWeakResource().Transformation;
            BoundingSphere = model.GetWeakResource().BoundingVolume;

            sound = SoundManager.Instance.MakeSoundObjcet("forest", null, BoundingSphere.Radius);
            sound.Position = BoundingSphere.Center;

            {
                float radLng = MathEx.Degree2Radian(Longitude);
                float radLat = MathEx.Degree2Radian(Latitude);

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
            get { return MaxAmount / (7500f * 2); }
        }
        public float AmountPer
        {
            get { return CurrentAmount / (7500f * 2); }
        }
        public NaturalResourceType Type
        {
            get { return Type; }
        }
        public event ResourceVisibleHander ResVisible;


        public Vector3 Position
        {
            get { return stdPosition; }
        }

        public float Longitude
        {
            get { return Longitude; }
        }

        public float Latitude
        {
            get { return Latitude; }
        }

        public float Radius 
        {
            get { return Radius; }
        }

        #endregion
    }
}
