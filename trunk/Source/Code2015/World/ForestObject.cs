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
using Code2015.Logic;
using Apoc3D.Vfs;
using Apoc3D.Graphics.Animation;
using Apoc3D.Collections;

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
        Model board;

        Vector3 stdPosition;
        Matrix stdTransform;
        BoundingSphere selectionSphere;

        FastList<RenderOperation> opBuffer = new FastList<RenderOperation>();

        public Vector3 ForestCenter 
        {
            get { return stdPosition; }
        }
        public Matrix ForestTransform 
        {
            get { return stdTransform; }
        }
        public float Radius
        {
            get;
            private set;
        }

        public ForestObject()
        {
           

        }

        public override bool IntersectsSelectionRay(ref Ray ray)
        {
            bool d = Vector3.DistanceSquared(ref ray.Position, ref stdPosition) < 6000 * 6000;
            if (d)
            {
                return MathEx.BoundingSphereIntersects(ref selectionSphere, ref ray);
            }
            return false;
        }
        public override void Parse(GameConfigurationSection sect)
        {
            base.Parse(sect);

            Radius = sect.GetSingle("Radius");

            float radLng = MathEx.Degree2Radian(Longitude);
            float radLat = MathEx.Degree2Radian(Latitude);

            stdPosition = PlanetEarth.GetPosition(radLng, radLat, PlanetEarth.PlanetRadius);
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

                stdTransform = PlanetEarth.GetOrientation(radLng, radLat);
                stdTransform.TranslationValue = stdPosition;


                selectionSphere.Center = stdPosition;
                selectionSphere.Radius = 200;
            }

            FileLocation fl = FileSystem.Instance.Locate("wooden_board_green.mesh", GameFileLocs.Model);

            board = new Model(ModelManager.Instance.CreateInstance(rs, fl));
            board.CurrentAnimation.Clear();
            board.CurrentAnimation.Add(
                new NoAnimaionPlayer(
                    Matrix.Translation(0, 0, 25) *
                    Matrix.Scaling(2.7f, 2.7f, 2.7f) *
                    Matrix.RotationX(-MathEx.PiOver2) *
                    Matrix.RotationY((-MathEx.PIf * 7.0f) / 8.0f)
                    ));

        }

        public override RenderOperation[] GetRenderOperation()
        {
            if (ResVisible != null)
            {
                ResVisible(this);
            }

            opBuffer.FastClear();

            TreeBatchModel mdl = model;

            RenderOperation[] ops = mdl.GetRenderOperation();
            if (ops != null)
            {
                opBuffer.Add(ops);
            }

            ops = board.GetRenderOperation();
            if (ops != null)
            {
                for (int i = 0; i < ops.Length; i++)
                {
                    ops[i].Sender = this;
                    ops[i].Transformation *= stdTransform;
                }
                opBuffer.Add(ops);
            }

            opBuffer.Trim();
            return opBuffer.Elements;
        }

        public override void Update(GameTime dt)
        {
            base.Update(dt);

            float ddt = dt.ElapsedGameTimeSeconds;
            sound.Update(dt);
         
            if (CurrentAmount < MaxAmount)
            {
                CurrentAmount += (CurrentAmount * RulesTable.FRecoverRate + RulesTable.FRecoverBias) * ddt;
            }
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
        float IResourceObject.MaxValue 
        {
            get { return MaxAmount; }
        }
        float IResourceObject.AmountPer
        {
            get { return CurrentAmount; }
        }
        NaturalResourceType IResourceObject.Type
        {
            get { return Type; }
        }
        public event ResourceVisibleHander ResVisible;


        Vector3 IResourceObject.Position
        {
            get { return stdPosition; }
        }

        float IResourceObject.Longitude
        {
            get { return Longitude; }
        }

        float IResourceObject.Latitude
        {
            get { return Latitude; }
        }

        float IResourceObject.Radius 
        {
            get { return Radius; }
        }

        #endregion
    }
}
