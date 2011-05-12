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
using Apoc3D.Graphics.Effects;
using Apoc3D.Graphics.Geometry;
using Apoc3D.GUI.Controls;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.Effects;
using Code2015.EngineEx;
using Code2015.Logic;
using Code2015.World;

namespace Code2015.GUI
{

    /// <summary>
    ///  表示游戏菜单
    /// </summary>
    class Menu : UIComponent, IGameComponent
    {
        class DummyScreen : UIComponent
        {

        }
        Tutorial tutorial;
        Intro intro;
        CreditScreen credits;
        LoadingOverlay loadingOverlay;
        DummyScreen dummyScreen;

        Code2015 game;
        MainMenu mainMenu;
        //SelectScreen sideSelect;
        //RenderTarget renderTarget;

        Texture overlay34;
        float overlayAlpha;

        UIComponent currentScreen;
        UIComponent nextScreen;


        UIComponent CurrentScreen
        {
            get { return currentScreen; }
            set
            {
                currentScreen = value;
                if (currentScreen == tutorial)
                    tutorial.NotifyShown();
            }
        }
        public UIComponent NextScreen
        {
            get { return nextScreen; }
            set
            {
                nextScreen = value;
            }
        }
        public void Back() 
        {
            NextScreen = dummyScreen;
        }
        public CreditScreen GetCredits()
        {
            return credits;
        }
        public LoadingOverlay GetOverlay() { return loadingOverlay; }


        public Tutorial GetTutorial()
        {
            return tutorial;
        }

        //public Texture Earth
        //{
        //    get { return renderTarget.GetColorBufferTexture(); }
        //}

        public Menu(Code2015 game, RenderSystem rs)
        {
            this.game = game;
            this.mainMenu = new MainMenu(game, this);
            //this.sideSelect = new SelectScreen(game, this);
            //this.renderSys = rs;

            //CreateScene(rs);
            //this.loadScreen = new LoadingScreen(this, rs);
            this.intro = new Intro(rs);
            this.credits = new CreditScreen(rs, this);
            this.tutorial = new Tutorial(this);
            this.dummyScreen = new DummyScreen();

            FileLocation fl = FileSystem.Instance.Locate("bg_black.tex", GameFileLocs.GUI);
            overlay34 = UITextureManager.Instance.CreateInstance(fl);
            this.loadingOverlay = new LoadingOverlay(game, overlay34);


            //fl = FileSystem.Instance.Locate("mm_logo.tex", GameFileLocs.GUI);


        }
       

        public void Render()
        {

            if (!game.IsIngame)
            {
                //EffectParams.LightDir = -renderer.CurrentCamera.Front;
                //renderer.RenderScene();
            }
        }



        public override void Render(Sprite sprite)
        {
            if (!game.IsIngame)
            {
                mainMenu.Render(sprite);

                if (currentScreen != null)
                {
                    //if (overlayAlpha < 1)
                    //    overlayAlpha += 0.1f;
                    //else
                    //    overlayAlpha = 1;
                    ColorValue color = ColorValue.White;
                    color.A = (byte)(byte.MaxValue * MathEx.Saturate(overlayAlpha) * 0.5f);

                    currentScreen.Render(sprite);
                    sprite.Draw(overlay34, 0, 0, color);

                }
                else
                {
                    //if (overlayAlpha > 0)
                    //{
                    //overlayAlpha -= 0.1f;
                    ColorValue color = ColorValue.White;
                    color.A = (byte)(byte.MaxValue * MathEx.Saturate(overlayAlpha) * 0.5f);

                    sprite.Draw(overlay34, 0, 0, color);
                    //}
                    //else
                    //overlayAlpha = 0;
                }
            }


            loadingOverlay.Render(sprite);


            if (intro != null)
            {
                intro.Render(sprite);
            }
        }

        public override void Update(GameTime time)
        {
            if (intro != null)
            {
                intro.Update(time);
                if (intro.IsOver)
                {
                    intro.Dispose();
                    intro = null;
                }
            }
            else
            {

                if (nextScreen != null)
                {
                    overlayAlpha += time.ElapsedGameTimeSeconds * 9.0f;
                    if (overlayAlpha > 1)
                    {
                        CurrentScreen = nextScreen;
                        nextScreen = null;
                        overlayAlpha = 1;
                    }
                }
                else 
                {
                    overlayAlpha -= time.ElapsedGameTimeSeconds * 9.0f;
                    if (overlayAlpha < 0)
                        overlayAlpha = 0;

                }
                
                loadingOverlay.Update(time);
                if (!game.IsIngame)
                {
                    //UpdateScene(time);
                    if (currentScreen != null && dummyScreen != currentScreen)
                    {
                        currentScreen.Update(time);
                    }
                    else
                    {
                        mainMenu.Update(time);
                    }
                }
                else
                {
                    if (game.CurrentGame.IsOver)
                    {
                        //ShowScore(game.CurrentGame.ResultScore);
                        game.Back();
                    }
                }
            }
        }


    }

}
