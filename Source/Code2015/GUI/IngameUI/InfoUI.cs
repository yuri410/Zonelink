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
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.GUI.Controls;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.GUI.Controls;
using Code2015.Logic;
using Code2015.World;

namespace Code2015.GUI
{
    /// <summary>
    ///  用于显示游戏中物体的信息
    /// </summary>
    class InfoUI : UIComponent
    {
        GameScene scene;
        GameState gameLogic;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;
        Player player;


        CityInfoDisplay cityInfoDisplay;
        ResInfoDisplay resInfoDisplay;

        RtsCamera camera;

        CitySelectionMarker selectionMarker;

        ISelectableObject mouseHoverObject;
        ISelectableObject selected;

        City mouseHoverCity;
        City selectedCity;
       

        //Point selectedProjPos;

        public ISelectableObject MouseHoverObject 
        {
            get { return mouseHoverObject; }
            set 
            {
                if (mouseHoverObject != value)
                {
                    mouseHoverObject = value;

                    if (mouseHoverObject != null)
                    {
                        mouseHoverCity = mouseHoverObject as City;
                        selectionMarker.MouseHoverCity = mouseHoverCity;
                    }
                    else
                    {
                        mouseHoverCity = null;
                    }


                    if (mouseHoverCity == null)
                    {
                        selectionMarker.MouseHoverCity = mouseHoverCity;
                    }
                }
               

            }
        }

        public ISelectableObject SelectedObject
        {
            get { return selected; }
            set
            {
                if (selected != value)
                {
                    selected = value;

                    if (selected != null)
                    {
                        selectedCity = selected as City;

                        if (selectedCity != null)
                        {
                            //Vector3 ppos = renderSys.Viewport.Project(selectedCity.Position, camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);

                            //selectedProjPos.X = (int)ppos.X;
                            //selectedProjPos.Y = (int)ppos.Y;

                            City cc = selectedCity;
                            City[] nearby = new City[cc.LinkableCityCount];

                            for (int i = 0; i < cc.LinkableCityCount; i++)
                            {
                                nearby[i] = cc.GetLinkableCity(i);
                            }
                            
                            selectionMarker.SetCity(selectedCity, nearby);
                        }
                    }
                    else
                    {
                        selectedCity = null;
                    }

                    if (selectedCity == null)
                    {
                        selectionMarker.SetCity(null, null);
                    }
                }
            }
        }

        public InfoUI(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.parent = parent;
            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;
            this.gameLogic = gamelogic;

            this.camera = scene.Camera;
            this.player = parent.HumanPlayer;

            this.cityInfoDisplay = new CityInfoDisplay(scene, renderSys, player);
            this.resInfoDisplay = new ResInfoDisplay(scene, renderSys);

            selectionMarker = new CitySelectionMarker(renderSys, player);
            scene.Scene.AddObjectToScene(selectionMarker);
        }

        public CityInfoDisplay CityInfoDisplay
        {
            get { return cityInfoDisplay; }
        }

        public override int Order
        {
            get { return 1; }
        }
        public override bool HitTest(int x, int y)
        {
            return false;
        }
      



        public override void Render(Sprite sprite)
        {
            cityInfoDisplay.Render(sprite);
            resInfoDisplay.Render(sprite);

        }


        public override void Update(GameTime time)
        {
            cityInfoDisplay.Update(time);
            resInfoDisplay.Update(time);

        }
    }
}
