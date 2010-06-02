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
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.BalanceSystem;
using Code2015.EngineEx;
using Code2015.GUI.Controls;
using Code2015.Logic;
using Code2015.World;
using Code2015.Effects.Post;

namespace Code2015.GUI
{
    class CityInfoDisplay : UIComponent
    {
        Dictionary<CityObject, CityInfo> cityTable;
        GameScene scene;
        RtsCamera camera;
        Player player;
        RenderSystem renderSys;

        Texture cityGrowPop;
        Texture cityLinkPop;
        Texture cityLvPop;
        Texture citySellPop;

        FastList<Popup> popUps = new FastList<Popup>();

        PieProgressEffect pieEffect;

        public Matrix Projection;
        public Matrix View;

        public Vector3 CameraPosition;

        public CityInfoDisplay(GameScene scene, RenderSystem rs, Player player)
        {
            this.scene = scene;
            this.cityTable = new Dictionary<CityObject, CityInfo>();
            this.renderSys = rs;
            this.player = player;
            this.camera = scene.Camera;


            FileLocation fl = FileSystem.Instance.Locate("ig_pop_sell.tex", GameFileLocs.GUI);
            citySellPop = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_pop_upgradecity.tex", GameFileLocs.GUI);
            cityGrowPop = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ig_pop_upgradeplugin.tex", GameFileLocs.GUI);
            cityLvPop = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ig_pop_linkover.tex", GameFileLocs.GUI);
            cityLinkPop = UITextureManager.Instance.CreateInstance(fl);


            pieEffect = new PieProgressEffect(rs);
        }

        public CityInfo GetCityInfo(CityObject cc) 
        {
            CityInfo info;

            if (!cityTable.TryGetValue(cc, out info))
            {
                info = new CityInfo(this, renderSys, cc, player, pieEffect);
                cityTable.Add(cc, info);
            }
            return info;
        }

        public override void Render(Sprite sprite)
        {
            Projection = camera.ProjectionMatrix;
            View = camera.ViewMatrix;
            CameraPosition = camera.Position;

            for (int i = 0; i < scene.VisibleCityCount; i++)
            {
                CityObject cc = scene.GetVisibleCity(i);

                CityInfo info;

                cityTable.TryGetValue(cc, out info);
                if (info != null)
                {
                    //info = new CityInfo(this, renderSys, cc, player, pieEffect);
                    //cityTable.Add(cc, info);


                    info.Render(sprite);
                }
            }

            for (int i = 0; i < popUps.Count; i++)
            {
                popUps[i].Render(sprite);
            }
        }

        public override void Update(GameTime time)
        {
            for (int i = popUps.Count - 1; i >= 0; i--)
            {
                if (popUps[i].IsFinished)
                {
                    popUps.RemoveAt(i);
                }
                else
                {
                    popUps[i].Update(time);
                }
            } 
            for (int i = 0; i < scene.VisibleCityCount; i++)
            {
                CityObject cc = scene.GetVisibleCity(i);

                CityInfo info;

                if (!cityTable.TryGetValue(cc, out info))
                {
                    info = new CityInfo(this, renderSys, cc, player, pieEffect);
                    cityTable.Add(cc, info);
                }

                info.Update(time);
            }
        }

        public void AddLinkPopup(int x, int y)
        {
            popUps.Add(new Popup(renderSys, cityLinkPop, x - cityLinkPop.Width / 2, y - cityLinkPop.Height / 2, 2));
        }
        public void AddSellPopup(int x, int y)
        {
            popUps.Add(new Popup(renderSys, citySellPop, x - citySellPop.Width / 2, y - citySellPop.Height / 2, 2));
        }
        public void AddGrowPopup(int x, int y)
        {
            popUps.Add(new Popup(renderSys, cityGrowPop, x - cityGrowPop.Width / 2, y - cityGrowPop.Height / 2, 2));
        }
        public void AddLVPopup(int x, int y)
        {
            popUps.Add(new Popup(renderSys, cityLvPop, x - cityLvPop.Width / 2, y - cityLvPop.Height / 2, 2));
        }

    }

    class ResInfoDisplay : UIComponent
    {
        Dictionary<IResourceObject, ResourceInfo> resTable;
        GameScene scene;
        RenderSystem renderSys;
        RtsCamera camera;



        public Matrix Projection;
        public Matrix View;

        public Vector3 CameraPosition;

        public ResInfoDisplay(GameScene scene, RenderSystem rs)
        {
            this.scene = scene;
            this.camera = scene.Camera;
            this.renderSys = rs;
            this.resTable = new Dictionary<IResourceObject, ResourceInfo>();
        }

        public override void Render(Sprite sprite)
        {
            Projection = camera.ProjectionMatrix;
            View = camera.ViewMatrix;
            CameraPosition = camera.Position;

            for (int i = 0; i < scene.VisibleResourceCount; i++)
            {
                IResourceObject res = scene.GetResourceObject(i);

                ResourceInfo info;

                if (!resTable.TryGetValue(res, out info))
                {
                    info = new ResourceInfo(this, renderSys, res);
                    resTable.Add(res, info);
                }

                info.Render(sprite);
            }
        }

        public override void Update(GameTime time)
        {
           
        }
    }
}
