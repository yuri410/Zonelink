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
using Apoc3D.GUI.Controls;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.AI;
using Code2015.EngineEx;
using Code2015.Logic;
using Code2015.World;

namespace Code2015.GUI
{
    class MainMenu : UIComponent
    {        
        Code2015 game;
        Menu parent;

        Texture credits;
        Texture exit;
        Texture help;
        Texture start;

        Texture background;
        //Texture linkbg;

        Texture mapTex;
        Texture waterTex;
        Texture rolesTex;
        Texture boarderTex;

        Texture logo;

        Texture cursor;
        Point mousePosition;

        RoundButton startButton;
        RoundButton exitButton;
        RoundButton creditButton;
        RoundButton helpButton;

        NormalSoundObject mouseHover;
        NormalSoundObject mouseDown;

        bool isStarting;
        float countDown;
        //bool isPostStarting;

        public MainMenu(Code2015 game, Menu parent)
        {
            RenderSystem rs = game.RenderSystem;

            this.game = game;
            this.parent = parent;


            FileLocation fl = FileSystem.Instance.Locate("nmm_credits.tex", GameFileLocs.GUI);
            credits = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("nmm_quit.tex", GameFileLocs.GUI);
            exit = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("nmm_help.tex", GameFileLocs.GUI);
            help = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("nmm_play.tex", GameFileLocs.GUI);
            start = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nmm_logo.tex", GameFileLocs.GUI);
            logo = UITextureManager.Instance.CreateInstance(fl);
            //fl = FileSystem.Instance.Locate("mm_btn_credits_hover.tex", GameFileLocs.GUI);
            //credits_hover = UITextureManager.Instance.CreateInstance(fl);
            //fl = FileSystem.Instance.Locate("mm_btn_quit_hover.tex", GameFileLocs.GUI);
            //exit_hover = UITextureManager.Instance.CreateInstance(fl);
            //fl = FileSystem.Instance.Locate("mm_btn_help_hover.tex", GameFileLocs.GUI);
            //help_hover = UITextureManager.Instance.CreateInstance(fl);
            //fl = FileSystem.Instance.Locate("mm_btn_single_hover.tex", GameFileLocs.GUI);
            //start_hover = UITextureManager.Instance.CreateInstance(fl);



            fl = FileSystem.Instance.Locate("nmm_bg.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl);

            //fl = FileSystem.Instance.Locate("mm_start_link.tex", GameFileLocs.GUI);
            //linkbg = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nmm_roles.tex", GameFileLocs.GUI);
            rolesTex = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nmm_water.tex", GameFileLocs.GUI);
            waterTex = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nmm_border.tex", GameFileLocs.GUI);
            boarderTex = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nmm_map.tex", GameFileLocs.GUI);
            mapTex = UITextureManager.Instance.CreateInstance(fl);


            fl = FileSystem.Instance.Locate("cursor.tex", GameFileLocs.GUI);
            cursor = UITextureManager.Instance.CreateInstance(fl);
            

            #region 配置按钮
            startButton = new RoundButton();
            startButton.X = 650;
            startButton.Y = 170;
            startButton.Radius = 244 / 2;
            startButton.Enabled = true;
            startButton.IsValid = true;

            startButton.MouseClick += StartButton_Click;
            startButton.MouseEnter += Button_MouseIn;
            startButton.MouseDown += Button_DownSound;

            exitButton = new RoundButton();
            exitButton.X = 896;
            exitButton.Y = 386;
            exitButton.Radius = 106 / 2;
            exitButton.Enabled = true;
            exitButton.IsValid = true;

            exitButton.MouseClick += ExitButton_Click;
            exitButton.MouseEnter += Button_MouseIn;
            exitButton.MouseDown += Button_DownSound;


            creditButton = new RoundButton();
            creditButton.X = 955;
            creditButton.Y = 198;
            creditButton.Radius = 138 / 2;
            creditButton.Enabled = true;
            creditButton.IsValid = true;
            creditButton.MouseEnter += Button_MouseIn;
            creditButton.MouseDown += Button_DownSound;
            creditButton.MouseClick += CreditButton_Click;


            helpButton = new RoundButton();
            helpButton.X = 225;
            helpButton.Y = 425;
            helpButton.Radius = 138 / 2;
            helpButton.Enabled = true;
            helpButton.IsValid = true;
            helpButton.MouseEnter += Button_MouseIn;
            helpButton.MouseDown += Button_DownSound;
            helpButton.MouseClick += HelpButton_Click;

            #endregion

            mouseHover = (NormalSoundObject)SoundManager.Instance.MakeSoundObjcet("buttonHover", null, 0);
            mouseDown = (NormalSoundObject)SoundManager.Instance.MakeSoundObjcet("buttonDown", null, 0);

        }


        void Button_MouseIn(object sender, MouseButtonFlags btn)
        {
            mouseHover.Fire();
        }
        void Button_DownSound(object sender, MouseButtonFlags btn)
        {
            if (btn == MouseButtonFlags.Left)
            {
                mouseDown.Fire();
            }
        }

        void ExitButton_Click(object sender, MouseButtonFlags btn)
        {
            if (btn == MouseButtonFlags.Left)
            {
                game.Exit();
            }
        }
        void HelpButton_Click(object sender, MouseButtonFlags btn)
        {
            if (btn == MouseButtonFlags.Left)
            {
                parent.GetTutorial().Reset();
                parent.NextScreen = parent.GetTutorial();
              
            }
        }

        //GameGoal CreateGoal()
        //{
        //    GameGoal goal = new GameGoal(RulesTable.CityMaxDevelopment * 8);

        //    return goal;
        //}

        void StartButton_Click(object sender, MouseButtonFlags btn)
        {
            if (btn == MouseButtonFlags.Left)
            {
                parent.GetOverlay().In();
                isStarting = true;
                countDown = 1.2f;
            }
        }
        void CreditButton_Click(object sender, MouseButtonFlags btn)
        {
            if (btn == MouseButtonFlags.Left)
            {
                parent.NextScreen = parent.GetCredits();

            }
        }



        public override void Render(Sprite sprite)
        {
            sprite.SetTransform(Matrix.Identity);

            sprite.Draw(background, 0, 0, ColorValue.White);
            sprite.Draw(waterTex, 0, 0, ColorValue.White);
            sprite.Draw(mapTex, 0, 0, ColorValue.White);
            //sprite.Draw(rolesTex, 258, 75, ColorValue.White);      
            sprite.Draw(boarderTex, 0, 0, ColorValue.White);

            float rolescale = -MathEx.Sqr(MathEx.Saturate(rolesProgress * 6) * 1.5f - 1) + 1;
            rolescale = (1.0f / 0.75f) * rolescale;
            Matrix roletrans = Matrix.Scaling(rolescale, rolescale, 1) *
                    Matrix.Translation(258 + rolesTex.Width / 2, 75 + rolesTex.Height / 2, 0);
            sprite.SetTransform(roletrans);
            sprite.Draw(rolesTex, -rolesTex.Width / 2, -rolesTex.Height / 2, ColorValue.White);
            sprite.SetTransform(Matrix.Identity);


            if (logoProgress > 0)
            {
                float logoScale = -MathEx.Sqr(MathEx.Saturate(logoProgress * 6) * 1.5f - 1) + 1;
                logoScale = (1.0f / 0.75f) * logoScale;
                Matrix logoTrans = Matrix.Scaling(logoScale, logoScale, 1) *
                        Matrix.Translation(31 + logo.Width / 2, 29 + logo.Height / 2, 0);
                Rectangle rect = new Rectangle(-logo.Width / 2, -logo.Height / 2, 554 - 31, 315 - 28);
                sprite.SetTransform(logoTrans);
                sprite.Draw(logo, rect, ColorValue.White);
                sprite.SetTransform(Matrix.Identity);
            }

            if (logoProgress >= 0.67f)
            {
                if (startButton.IsMouseOver)
                {
                    // y=-(x-1)^2+1
                    float scale = -MathEx.Sqr(MathEx.Saturate(startProgress * 6) * 1.5f - 1) + 1;
                    scale = 0.5f + 1.5f * scale;

                    Matrix trans = Matrix.Scaling(scale, scale, 1) *
                        Matrix.Translation(startButton.X + start.Width / 2, startButton.Y + start.Height / 2, 0);
                    //sprite.Draw(start, startButton.X, startButton.Y, ColorValue.Yellow);
                    sprite.SetTransform(trans);
                    sprite.Draw(start, -start.Width / 2, -start.Height / 2, ColorValue.Yellow);
                    sprite.SetTransform(Matrix.Identity);
                }
                else
                {
                    sprite.Draw(start, startButton.X, startButton.Y, ColorValue.White);
                }


                if (helpButton.IsMouseOver)
                {
                    //  sprite.Draw(help, helpButton.X, helpButton.Y, ColorValue.Yellow);
                    float scale = -MathEx.Sqr(MathEx.Saturate(helpProgress * 6) * 1.5f - 1) + 1;
                    scale = 0.5f + 1.5f * scale;

                    Matrix trans = Matrix.Scaling(scale, scale, 1) *
                        Matrix.Translation(helpButton.X + help.Width / 2, helpButton.Y + help.Height / 2, 0);

                    sprite.SetTransform(trans);
                    sprite.Draw(help, -help.Width / 2, -help.Height / 2, ColorValue.Yellow);
                    sprite.SetTransform(Matrix.Identity);
                }
                else
                {
                    sprite.Draw(help, helpButton.X, helpButton.Y, ColorValue.White);
                }

                if (exitButton.IsMouseOver)
                {
                    //sprite.Draw(exit, exitButton.X, exitButton.Y, ColorValue.Yellow);
                    float scale = -MathEx.Sqr(MathEx.Saturate(exitProgress * 6) * 1.5f - 1) + 1;
                    scale = 0.5f + 1.5f * scale;

                    Matrix trans = Matrix.Scaling(scale, scale, 1) *
                        Matrix.Translation(exitButton.X + exit.Width / 2, exitButton.Y + exit.Height / 2, 0);

                    sprite.SetTransform(trans);
                    sprite.Draw(exit, -help.Width / 2, -exit.Height / 2, ColorValue.Yellow);
                    sprite.SetTransform(Matrix.Identity);
                }
                else
                {
                    sprite.Draw(exit, exitButton.X, exitButton.Y, ColorValue.White);
                }

                if (creditButton.IsMouseOver)
                {
                    //sprite.Draw(credits, creditButton.X, creditButton.Y, ColorValue.Yellow);
                    float scale = -MathEx.Sqr(MathEx.Saturate(creditProgress * 6) * 1.5f - 1) + 1;
                    scale = 0.5f + 1.5f * scale;

                    Matrix trans = Matrix.Scaling(scale, scale, 1) *
                        Matrix.Translation(creditButton.X + credits.Width / 2, creditButton.Y + credits.Height / 2, 0);

                    sprite.SetTransform(trans);
                    sprite.Draw(credits, -credits.Width / 2, -credits.Height / 2, ColorValue.Yellow);
                    sprite.SetTransform(Matrix.Identity);
                }
                else
                {
                    sprite.Draw(credits, creditButton.X, creditButton.Y, ColorValue.White);
                }


                //if (parent.NextScreen == null)
                {
                    sprite.Draw(cursor, mousePosition.X, mousePosition.Y, ColorValue.White);
                }
            }
        }


        float startProgress = 0;
        float exitProgress = 0;
        float helpProgress = 0;
        float creditProgress = 0;
        float rolesProgress = 0;
        float logoProgress = -0.4f;

        public override void Update(GameTime time)
        {
            mousePosition.X = MouseInput.X;
            mousePosition.Y = MouseInput.Y; 
            
            float dt = (float)time.ElapsedGameTime.TotalSeconds;

            startButton.Update(time);
            creditButton.Update(time);
            exitButton.Update(time);
            helpButton.Update(time);

            rolesProgress += dt * 1.5f;
            if (rolesProgress > 1)
            {
                rolesProgress = 1;
            }

            logoProgress += dt * 1.5f;
            if (logoProgress > 1) 
            {
                logoProgress = 1;
            }

            if (startButton.IsMouseOver)
            {
                startProgress += dt * 3;
                if (startProgress > 1)
                {
                    startProgress = 1;
                }
            }
            else
            {
                startProgress = 0;
            }

            if (exitButton.IsMouseOver)
            {
                exitProgress += dt * 3;
                if (exitProgress > 1)
                {
                    exitProgress = 1;
                }
            }
            else
            {
                exitProgress = 0;
            }

            if (helpButton.IsMouseOver)
            {
                helpProgress += dt * 3;
                if (helpProgress > 1)
                {
                    helpProgress = 1;
                }
            }
            else
            {
            
                helpProgress = 0;
            }

            if (creditButton.IsMouseOver)
            {
                creditProgress += dt * 3;
                if (creditProgress > 1)
                {
                    creditProgress = 1;
                }
            }
            else
            {
                creditProgress = 0;
            }



            if (isStarting)
            {
                countDown -= time.ElapsedGameTimeSeconds;
                if (countDown < 0) 
                {
                    isStarting = false;
                   
                    game.StartNewGame();
                    
                    //isPostStarting = true;
                }
            }

            //else if (isPostStarting)
            //{
               
            //}
        }
    }
}
