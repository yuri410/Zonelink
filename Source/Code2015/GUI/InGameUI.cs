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
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.Logic;
using Code2015.World;
using XI = Microsoft.Xna.Framework.Input;
using Code2015.GUI.IngameUI;
using Apoc3D.GUI.Controls;

namespace Code2015.GUI
{
    class SelFilter : IObjectFilter
    {
        static SelFilter singleton;

        public static SelFilter Instance
        {
            get
            {
                if (singleton == null)
                    singleton = new SelFilter();
                return singleton;
            }
        }

        #region IObjectFilter 成员

        public bool Check(SceneObject obj)
        {
            return obj is ISelectableObject;
        }

        public bool Check(OctreeSceneNode node)
        {
            return true;
        }

        #endregion
    }

    /// <summary>
    ///  表示游戏过程中的界面
    /// </summary>
    class InGameUI : GUIScreen
    {
        GameScene scene;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;
        GameState logic;

        
        InfoUI infoUI;
        MiniMap miniMap;

        Picker picker;
        Cursor layeredCursor;

        SelectionMarker selectionMarker;


        CitySelectInfo cityStatusInfo;
        NResSelectInfo nResSelectInfo;
        RBallTypeSelect sendBallSelect;

        RankInfo rankInfo;

        
        NIGMenu nigMenu;
        NIGWin nigWin;
        NIGFail nigFail;
        NIGObjective nigObjective;
        NIGColor nigColor;

        //ExitConfirm exitConfirm;
        bool isEscPressed;

        ExitGame exit;
        Quest quest;

        Player player;


        public InGameUI(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.parent = parent;
            this.logic = gamelogic;

            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;
        

            this.player = parent.HumanPlayer;


            picker = new Picker(game, parent, scene, gamelogic);
            AddElement(picker);


            this.infoUI = new InfoUI(game, parent, scene, gamelogic);
            AddElement(infoUI);

          
            this.miniMap = new MiniMap(game, parent, scene, gamelogic);
            AddElement(miniMap);
         

            //-----Ruan-----------


            cityStatusInfo = new CitySelectInfo(game, parent, scene, gamelogic);
            AddElement(cityStatusInfo);

            nResSelectInfo = new NResSelectInfo(game, parent, scene, gamelogic);
            AddElement(nResSelectInfo);
          
            sendBallSelect = new RBallTypeSelect(game, parent, scene, gamelogic);
            AddElement(sendBallSelect);



            nigMenu = new NIGMenu(game, parent, scene, gamelogic);
            AddElement(nigMenu);
            nigColor = new NIGColor(game, parent, scene, gamelogic);
            AddElement(nigColor);
            nigWin = new NIGWin(game, parent, scene, gamelogic);
            AddElement(nigWin);
            nigFail = new NIGFail(game, parent, scene, gamelogic);
            AddElement(nigFail);
            nigObjective = new NIGObjective(game, parent, scene, gamelogic);
            AddElement(nigObjective);
            nigColor = new NIGColor(game, parent, scene, gamelogic);
            AddElement(nigColor);

            selectionMarker = new SelectionMarker(renderSys, gamelogic.Field, player);
            scene.Scene.AddObjectToScene(selectionMarker);


            layeredCursor = new Cursor(game, parent, scene, gamelogic, picker, cityStatusInfo, nResSelectInfo,
                                           sendBallSelect,  miniMap, selectionMarker);
            AddElement(layeredCursor);


            exit = new ExitGame(game, parent, scene, gamelogic, nigMenu);
            AddElement(exit);

            quest = new Quest(game, parent, scene, gamelogic);
            AddElement(quest);

            rankInfo = new RankInfo(game, parent, scene, gamelogic);
            AddElement(rankInfo);

            
        }

        public override void Render(Sprite sprite)
        {
            if (!parent.IsOver)
            {
                if (parent.IsLoaded)
                {                  
                    base.Render(sprite);
                }
              
            }
        }


        public override void Update(GameTime time)
        {
            if (!parent.IsOver)
            {
                if (parent.IsLoaded)
                {
                    XI.KeyboardState keyState = XI.Keyboard.GetState();

                    if (!isEscPressed)
                    {
                        isEscPressed = keyState.IsKeyDown(XI.Keys.Escape);
                        if (isEscPressed)
                        {
                            //if (!exitConfirm.IsShown)
                            //{
                            //    exitConfirm.Show();
                            //}
                            //else
                            //{
                            //    parent.Over();
                            //}
                        }
                    }
                    else
                    {
                        isEscPressed = keyState.IsKeyDown(XI.Keys.Escape);
                    }


                    if (exit.IsExitClicked)
                    {
                        //if (!exitConfirm.IsShown)
                        //{
                        //    exitConfirm.Show();
                        //}
                        //else
                        //{
                        //    parent.Over();
                        //}
                    }

                    //parent.IsPaused = exitConfirm.IsShown;

                    base.Update(time);
                }
            }
        }
    }
}
