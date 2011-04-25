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
        static readonly ColorValue[] Colors = new ColorValue[] { ColorValue.Red, ColorValue.Yellow, ColorValue.Green, ColorValue.Blue };

        Code2015 game;
        Menu parent;

        Texture credits;
        Texture exit;
        Texture help;
        Texture start;

        Texture credits_hover;
        Texture exit_hover;
        Texture help_hover;
        Texture start_hover;

        Texture background;
        Texture linkbg;

        Texture mapTex;
        Texture waterTex;
        Texture rolesTex;
        Texture boarderTex;

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

            fl = FileSystem.Instance.Locate("mm_start_link.tex", GameFileLocs.GUI);
            linkbg = UITextureManager.Instance.CreateInstance(fl);

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
                parent.CurrentScreen = parent.GetTutorial();
              
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
                parent.CurrentScreen = parent.GetCredits();

            }
        }



        public override void Render(Sprite sprite)
        {
            sprite.SetTransform(Matrix.Identity);

            sprite.Draw(background, 0, 0, ColorValue.White);
            sprite.Draw(waterTex, 0, 0, ColorValue.White);
            sprite.Draw(mapTex, 0, 0, ColorValue.White);
            sprite.Draw(rolesTex, 258, 75, ColorValue.White);      
            sprite.Draw(boarderTex, 0, 0, ColorValue.White);

            parent.RenderLogo(sprite);

            if (startButton.IsMouseOver)
            {
                sprite.Draw(start, startButton.X, startButton.Y, ColorValue.Yellow);
            }
            else
            {
                sprite.Draw(start, startButton.X, startButton.Y, ColorValue.White);
            }


            if (helpButton.IsMouseOver)
            {
                sprite.Draw(help, helpButton.X, helpButton.Y, ColorValue.Yellow);
            }
            else
            {
                sprite.Draw(help, helpButton.X, helpButton.Y, ColorValue.White);
            }

            if (exitButton.IsMouseOver) 
            {
                sprite.Draw(exit, exitButton.X, exitButton.Y, ColorValue.Yellow);
            }
            else
            {
                sprite.Draw(exit, exitButton.X, exitButton.Y, ColorValue.White);
            }
 
            if (creditButton.IsMouseOver) 
            {
                sprite.Draw(credits, creditButton.X, creditButton.Y, ColorValue.Yellow);
            }
            else
            {
                sprite.Draw(credits, creditButton.X, creditButton.Y, ColorValue.White);
            }

            //int x = 818 - 322 / 2;
            //int y = 158 - 281 / 2;


            //if (startButton.IsPressed)
            //{
            //    sprite.Draw(start, x, y, ColorValue.White);
            //}
            //else if (startButton.IsMouseOver)
            //{
            //    sprite.Draw(start, x, y, ColorValue.Yellow);
            //}
            //else
            //{
            //    sprite.Draw(start, x, y, ColorValue.White);
            //}

            //x = 1107 - 192 / 2;
            //y = 241 - 195 / 2;


            //if (helpButton.IsPressed)
            //{
            //    sprite.Draw(help_down, x, y, ColorValue.White);
            //}
            //else if (helpButton.IsMouseOver)
            //{
            //    sprite.Draw(help_hover, x, y, ColorValue.White);
            //}
            //else
            //{
            //    sprite.Draw(help, x, y, ColorValue.White);
            //}

            //x = 835;// -225 / 2;
            //y = 336;// -180 / 2;

            //if (creditButton.IsPressed)
            //{
            //    sprite.Draw(credits_down, x, y, ColorValue.White);
            //}
            //else if (creditButton.IsMouseOver)
            //{
            //    sprite.Draw(credits_hover, x, y, ColorValue.White);
            //}
            //else
            //{
            //    sprite.Draw(credits, x, y, ColorValue.White);
            //}

            //x = 1129 - 160 / 2;
            //y = 594 - 160 / 2;


            //if (exitButton.IsPressed)
            //{
            //    sprite.Draw(exit_down, x, y, ColorValue.White);
            //}
            //else if (exitButton.IsMouseOver)
            //{
            //    sprite.Draw(exit_hover, x, y, ColorValue.White);
            //}
            //else
            //{
            //    sprite.Draw(exit, x, y, ColorValue.White);
            //}

            //sprite.Draw(parent.Earth, 0, 0, ColorValue.White);
            if (parent.CurrentScreen == null)
            {
                sprite.Draw(cursor, mousePosition.X, mousePosition.Y, ColorValue.White);
            }
        }

        struct ColorSortEntry
        {
            public float Weight;
            public ColorValue Color;
        }
        static int Comparison(ColorSortEntry a, ColorSortEntry b)
        {
            return a.Weight.CompareTo(b.Weight);
        }

        ColorValue[] GetSideColors()
        {
            ColorSortEntry[] entries = new ColorSortEntry[8];
          
            entries[0].Color = ColorValue.Red;
            entries[1].Color = ColorValue.Yellow;
            entries[2].Color = ColorValue.Green;
            entries[3].Color = ColorValue.Blue;
            entries[4].Color = ColorValue.Purple;
            entries[5].Color = ColorValue.Orange;
            entries[6].Color = ColorValue.LightBlue;
            entries[7].Color = ColorValue.Pink;
            for (int i = 0; i < entries.Length; i++)
            {
                entries[i].Weight = Randomizer.GetRandomSingle();
            }
            Array.Sort<ColorSortEntry>(entries, Comparison);



            ColorValue[] reuslt = new ColorValue[entries.Length];
            for (int i = 0; i < entries.Length; i++) 
            {
                reuslt[i] = entries[i].Color;
            }
            return reuslt;
        }

        public override void Update(GameTime time)
        {
            mousePosition.X = MouseInput.X;
            mousePosition.Y = MouseInput.Y; 
            

            startButton.Update(time);
            creditButton.Update(time);
            exitButton.Update(time);
            helpButton.Update(time);

            if (isStarting)
            {
                countDown -= time.ElapsedGameTimeSeconds;
                if (countDown < 0) 
                {
                    isStarting = false;
                    GameCreationParameters gcp = new GameCreationParameters();

                    gcp.Player1 = new Player("Player",  0);
                    gcp.Player2 = new AIPlayer( 1);
                    gcp.Player3 = new AIPlayer( 2);
                    gcp.Player4 = new AIPlayer( 3);
                    gcp.Player5 = new AIPlayer(4);
                    gcp.Player6 = new AIPlayer(5);
                    gcp.Player7 = new AIPlayer(6);
                    gcp.Player8 = new AIPlayer(7);

                    ColorValue[] colors = GetSideColors();
                    gcp.Player1.SideColor = colors[0];
                    gcp.Player2.SideColor = colors[1];
                    gcp.Player3.SideColor = colors[2];
                    gcp.Player4.SideColor = colors[3];
                    gcp.Player5.SideColor = colors[4];
                    gcp.Player6.SideColor = colors[5];
                    gcp.Player7.SideColor = colors[6];
                    gcp.Player8.SideColor = colors[7];

                    game.StartNewGame(gcp);
                    
                    //isPostStarting = true;
                }
            }

            //else if (isPostStarting)
            //{
               
            //}
        }
    }
}
