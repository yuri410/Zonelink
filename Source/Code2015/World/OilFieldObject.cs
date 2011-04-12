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

            float scale = Game.ObjectScale * 2.2f;
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

            BoundingSphere.Radius = 50;
            Orientation = PlanetEarth.GetOrientation(radLng, radLat);

            sound = SoundManager.Instance.MakeSoundObjcet("oil", null, BoundingSphere.Radius * 8);
            sound.Position = position;
        }


        public override bool IsSerializable
        {
            get { return false ; }
        }

        public override void Parse(GameConfigurationSection sect)
        {
            base.Parse(sect);

            float radLong = MathEx.Degree2Radian(this.Longitude);
            float radLat = MathEx.Degree2Radian(this.Latitude);

            float altitude = TerrainData.Instance.QueryHeight(radLong, radLat);

            IsInOcean = false;
            if (altitude < 0)
            {
                altitude = 0;
                IsInOcean = true;
            }
        }

        public override RenderOperation[] GetRenderOperation()
        {
            if (ResVisible != null)
            {
                ResVisible(this);
            }
            if (ModelL0 != null)
            {
                return ModelL0.GetRenderOperation();
            }
            return null;
        }
        public override RenderOperation[] GetRenderOperation(int level)
        {
            if (ResVisible != null)
            {
                ResVisible(this);
            }
            return base.GetRenderOperation(level);
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
            get { return MaxAmount / (7500f * 2); }
        }
        float IResourceObject.AmountPer
        {
            get { return CurrentAmount / (7500f * 2); }
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
