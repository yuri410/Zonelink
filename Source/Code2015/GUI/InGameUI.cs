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


        int selCurIndex;
        int selCurAnimSign = 1;

        Texture cursor;
        Texture[] cursor_sel;
        Texture cursor_up;
        Texture cursor_down;
        Texture cursor_left;
        Texture cursor_right;
        Texture cursor_ul;
        Texture cursor_ur;
        Texture cursor_dl;
        Texture cursor_dr;
        
        InfoUI infoUI;
        MiniMap miniMap;

        Picker picker;
        Cursor layeredCursor;

        SelectionMarker selectionMarker;

        SelectInfo selectInfo;

        ExitConfirm exitConfirm;
        bool isEscPressed;

        Player player;


        
        

        public InGameUI(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.parent = parent;
            this.logic = gamelogic;

            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;
           // this.physWorld = new ScreenPhysicsWorld();

            this.player = parent.HumanPlayer;



            FileLocation fl = FileSystem.Instance.Locate("cursor.tex", GameFileLocs.GUI);
            cursor = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("cursor_u.tex", GameFileLocs.GUI);
            cursor_up = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("cursor_l.tex", GameFileLocs.GUI);
            cursor_left = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("cursor_d.tex", GameFileLocs.GUI);
            cursor_down = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("cursor_r.tex", GameFileLocs.GUI);
            cursor_right = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("cursor_lu.tex", GameFileLocs.GUI);
            cursor_ul = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("cursor_ru.tex", GameFileLocs.GUI);
            cursor_ur = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("cursor_ld.tex", GameFileLocs.GUI);
            cursor_dl = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("cursor_rd.tex", GameFileLocs.GUI);
            cursor_dr = UITextureManager.Instance.CreateInstance(fl);

            cursor_sel = new Texture[11];

            for (int i = 0; i < cursor_sel.Length; i++) 
            {
                fl = FileSystem.Instance.Locate("selcursor" + (i + 13).ToString("D2") + ".tex", GameFileLocs.GUI);
                cursor_sel[i] = UITextureManager.Instance.CreateInstance(fl);
            }

            
            cursorState = MouseCursor.Normal;

            

            picker = new Picker(game, parent, scene, gamelogic);
            AddElement(picker);

            //AddElement(new ColorNotify(player));
            //this.cityEdit = new CityEditPanel(game, parent, scene, gamelogic);
            //AddElement(cityEdit);

            this.infoUI = new InfoUI(game, parent, scene, gamelogic);
            AddElement(infoUI);

            //this.brackets = new Brackets(game, parent, scene, gamelogic, picker);
           // AddElement(brackets);

            //this.icons = new GoalIcons(parent, this, infoUI.CityInfoDisplay, scene, physWorld, brackets);
           // AddElement(icons);

           // brackets.SetGoalIcons(icons);

            //this.pieceMaker = new GoalPieceMaker(player.Area, renderSys, scene.Camera, icons);

           
           // this.playerProgress = new DevelopmentMeter(game, parent, scene, gamelogic);
           // AddElement(playerProgress);
            this.miniMap = new MiniMap(game, parent, scene, gamelogic);
            AddElement(miniMap);
            //this.noticeBar = new NoticeBar(game, parent, scene, gamelogic, miniMap);
            //AddElement(noticeBar);

            //this.container = new PieceContainer(game, parent, scene, gamelogic, icons);
            //AddElement(container);
            //PieceContainerOverlay overlay = new PieceContainerOverlay(game, parent, scene, gamelogic);
            //AddElement(overlay);

            //icons.SetPieceContainer(container);
            //co2graph = new CO2Graph(game, parent, scene, gamelogic);
            //AddElement(co2graph);

            //-----Ruan-----------
            selectInfo = new SelectInfo(game, parent, scene, gamelogic);
            AddElement(selectInfo);

            exitConfirm = new ExitConfirm();
            AddElement(exitConfirm);


            layeredCursor = new Cursor(game, parent, scene, gamelogic, picker, selectInfo, selectionMarker);
            AddElement(layeredCursor);


            selectionMarker = new SelectionMarker(renderSys, player);
            scene.Scene.AddObjectToScene(selectionMarker);
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


        Point mouseRightPosition;

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
                            if (!exitConfirm.IsShown)
                            {
                                exitConfirm.Show();
                            }
                            else
                            {
                                parent.Over();
                            }
                        }
                    }
                    else
                    {
                        isEscPressed = keyState.IsKeyDown(XI.Keys.Escape);
                    }

                    parent.IsPaused = exitConfirm.IsShown;

                    base.Update(time);
                }
            }
        }
    }
}
