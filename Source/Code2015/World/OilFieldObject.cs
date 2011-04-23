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
using Apoc3D.Graphics;
using Apoc3D.Graphics.Animation;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.Logic;
using Apoc3D.Collections;

namespace Code2015.World
{
    class OilFieldObject : NaturalResource, ISelectableObject, IResourceObject
    {
        const int FrameCount = 29;

        int counter;
        int frameIdx;
        int sign = 1;

        public bool IsInOcean { get; private set; }

        Model[] model;
        Model board;

        FastList<RenderOperation> opBuffer = new FastList<RenderOperation>();

        SoundObject sound;


        public OilFieldObject()
        {
            frameIdx = Randomizer.GetRandomInt(OilFrameCount - 1);

        }

        public override void InitalizeGraphics(RenderSystem rs) 
        {

            float radLng = MathEx.Degree2Radian(Longitude);
            float radLat = MathEx.Degree2Radian(Latitude);

            bool isOcean = false;

            float alt = TerrainData.Instance.QueryHeight(radLng, radLat);

            if (alt < 0)
            {
                alt = 0;
                isOcean = true;
            }

            frameIdx = Randomizer.GetRandomInt(FrameCount - 1);

            float scale = Game.ObjectScale * 2.9f;// 2.2f;
            if (isOcean)
            {
                model = new Model[FrameCount];
                for (int i = 0; i < FrameCount; i++)
                {
                    FileLocation fl = FileSystem.Instance.Locate("oilderricksea" + i.ToString("D2") + ".mesh", GameFileLocs.Model);

                    model[i] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                    model[i].CurrentAnimation.Clear();
                    model[i].CurrentAnimation.Add(new NoAnimaionPlayer(
                        Matrix.Scaling(scale, scale, scale) * Matrix.Translation(0, 18, 0) * Matrix.RotationY(-MathEx.PiOver4)));
                }
            }
            else
            {
                model = new Model[FrameCount];
                for (int i = 0; i < FrameCount; i++)
                {
                    FileLocation fl = FileSystem.Instance.Locate("oilderrick" + i.ToString("D2") + ".mesh", GameFileLocs.Model);

                    model[i] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
                    model[i].CurrentAnimation.Clear();
                    model[i].CurrentAnimation.Add(new NoAnimaionPlayer(
                        Matrix.Scaling(scale, scale, scale) * Matrix.RotationY(-MathEx.PiOver4)));
                }

            }



            Position = PlanetEarth.GetPosition(radLng, radLat, PlanetEarth.PlanetRadius + alt * TerrainMeshManager.PostHeightScale);
            BoundingSphere.Center = position;

            BoundingSphere.Radius = 200;
            Orientation = PlanetEarth.GetOrientation(radLng, radLat);

            sound = SoundManager.Instance.MakeSoundObjcet("oil", null, BoundingSphere.Radius * 8.0f / 3.0f);
            sound.Position = position;
          
            
            FileLocation fl2 = FileSystem.Instance.Locate("wooden_board.mesh", GameFileLocs.Model);

            board = new Model(ModelManager.Instance.CreateInstance(rs, fl2));
            board.CurrentAnimation.Clear();
            board.CurrentAnimation.Add(
                new NoAnimaionPlayer(
                    Matrix.Translation(-50, 25, 23) *
                    Matrix.Scaling(2.1f, 2.1f, 2.1f) *
                    Matrix.RotationX(-MathEx.PiOver2) *
                    Matrix.RotationY((-MathEx.PIf * 7.0f) / 8.0f)
                    ));
        }


        public override bool IsSerializable
        {
            get { return false ; }
        }

        public override void Parse(GameConfigurationSection sect)
        {
            base.Parse(sect);
        }

        protected override void UpdateLocation()
        {
            float radLong = MathEx.Degree2Radian(this.Longitude);
            float radLat = MathEx.Degree2Radian(this.Latitude);

            float altitude = TerrainData.Instance.QueryHeight(radLong, radLat);

            IsInOcean = false;
            if (altitude < 0)
            {
                altitude = 0;
                IsInOcean = true;
            }

            this.Position = PlanetEarth.GetPosition(radLong, radLat, PlanetEarth.PlanetRadius + TerrainMeshManager.PostHeightScale * altitude);

            this.Transformation = PlanetEarth.GetOrientation(radLong, radLat);
            //this.InvTransformation = Matrix.Invert(Transformation);

            this.Transformation.TranslationValue = this.Position; // TranslationValue = pos;

            BoundingSphere.Radius = RulesTable.CityRadius;
            BoundingSphere.Center = this.Position;

        }

        public override RenderOperation[] GetRenderOperation()
        {
            if (ResVisible != null)
            {
                ResVisible(this);
            }

            opBuffer.FastClear();

            RenderOperation[] ops = board.GetRenderOperation();
            if (ops != null)
            {
                for (int i = 0; i < ops.Length; i++)
                {
                    //ops[i].Transformation *= stdTransform;
                }
                opBuffer.Add(ops);
            }
            if (ModelL0 != null)
            {
                ops = ModelL0.GetRenderOperation();
                if (ops != null)
                {
                    opBuffer.Add(ops);
                }
            }
            opBuffer.Trim();
            return opBuffer.Elements;
        }
        public override RenderOperation[] GetRenderOperation(int level)
        {
            if (ResVisible != null)
            {
                ResVisible(this);
            }
            return GetRenderOperation();
        }
        public override void Update(GameTime dt)
        {
            base.Update(dt);

            float ddt = dt.ElapsedGameTimeSeconds;

            sound.Update(dt);

            counter++;
            if (counter == 2)
            {
                frameIdx += sign;
                counter = 0;
            }
            if (frameIdx >= FrameCount || frameIdx < 0)
            {
                sign = -sign;
                frameIdx += sign;
            }
            
            if (model != null)
            {
                ModelL0 = model[frameIdx];
            }

            if (CurrentAmount < MaxAmount)
            {
                CurrentAmount += RulesTable.ORecoverBias * ddt;
            }
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
            get { return MaxAmount / (7500f * 2 / 20.0f); }
        }
        float IResourceObject.AmountPer
        {
            get { return CurrentAmount / (7500f * 2 / 20.0f); }
        }

        NaturalResourceType IResourceObject.Type
        {
            get { return Type; }
        }
        public event ResourceVisibleHander ResVisible;

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
            get { return BoundingSphere.Radius; }
        }

        #endregion
    }
}
