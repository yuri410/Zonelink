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
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Animation;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.BalanceSystem;
using Code2015.EngineEx;

namespace Code2015.World
{
    class FarmObject : SceneObject, ISelectableObject, IResourceObject
    {
        const float TileLength =50;
        FarmLand farm;
        RenderSystem renderSys;


        Model[] tiles;
        Model house;

        FastList<RenderOperation> opBuffer = new FastList<RenderOperation>();

        public FarmObject(RenderSystem rs, FarmLand farm)
            : base(false)
        {
            this.farm = farm;
            this.renderSys = rs;

            float radLng = MathEx.Degree2Radian(farm.Longitude);
            float radLat = MathEx.Degree2Radian(farm.Latitude);
            float radLen = MathEx.Degree2Radian(2.5f);

            FileLocation fl = FileSystem.Instance.Locate("farmtile.mesh", GameFileLocs.Model);

            tiles = new Model[farm.BaseHeight * farm.BaseWidth];
            int idx = 0;
            for (int i = 0; i < farm.BaseHeight; i++)
            {
                for (int j = 0; j < farm.BaseWidth; j++)
                {
                    tiles[idx] = new Model(ModelManager.Instance.CreateInstance(rs, fl));

                    float nLng = radLng + i * radLen;
                    float nLat = radLat + j * radLen;
                    float alt = TerrainData.Instance.QueryHeight(nLng, nLat);
                    Matrix trans = PlanetEarth.GetOrientation(radLng, radLat);
                    trans.TranslationValue = PlanetEarth.GetPosition(nLng, nLat, alt * TerrainMeshManager.PostHeightScale + PlanetEarth.PlanetRadius);

                    tiles[idx].CurrentAnimation = new NoAnimation(trans);
                    idx++;
                }
            }

            Transformation = Matrix.Identity;


            BoundingSphere.Radius = (float)Math.Sqrt(MathEx.Sqr(TileLength * farm.BaseWidth) + MathEx.Sqr(TileLength * farm.BaseHeight));
            BoundingSphere.Center = PlanetEarth.GetPosition(radLng, radLat);

        }

        public override RenderOperation[] GetRenderOperation()
        {
            if (ResVisible != null)
            {
                ResVisible(this);
            }
            opBuffer.FastClear();
            for (int i = 0; i < tiles.Length; i++)
            {
                RenderOperation[] ops = tiles[i].GetRenderOperation();
                if (ops != null)
                {
                    opBuffer.Add(ops);
                }
            }
            opBuffer.Trim();
            return opBuffer.Elements;
        }

        public override void Update(GameTime dt)
        {
            
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

        public float AmountPer
        {
            get { return farm.CurrentAmount / BalanceSystem.Forest.MaxAmount; }
        }

        public NaturalResourceType Type
        {
            get { return NaturalResourceType.Food; }
        }

        public Vector3 Position
        {
            get { return BoundingSphere.Center; }
        }

        public float Longitude
        {
            get { return farm.Longitude; }
        }

        public float Latitude
        {
            get { return farm.Latitude; }
        }

        public float Radius
        {
            get { return BoundingSphere.Radius; }
        }

        public event ResourceVisibleHander ResVisible;

        #endregion
    }
}
