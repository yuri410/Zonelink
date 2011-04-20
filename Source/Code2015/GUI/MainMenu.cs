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

        Texture credits_down;
        Texture exit_down;
        Texture help_down;
        Texture start_down;


        Texture background;
        Texture linkbg;


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
          
            FileLocation fl = FileSystem.Instance.Locate("mm_btn_credits.tex", GameFileLocs.GUI);
            credits = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("mm_btn_quit.tex", GameFileLocs.GUI);
            exit = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("mm_btn_help.tex", GameFileLocs.GUI);
            help = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("mm_btn_single.tex", GameFileLocs.GUI);
            start = UITextureManager.Instance.CreateInstance(fl);


            fl = FileSystem.Instance.Locate("mm_btn_credits_hover.tex", GameFileLocs.GUI);
            credits_hover = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("mm_btn_quit_hover.tex", GameFileLocs.GUI);
            exit_hover = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("mm_btn_help_hover.tex", GameFileLocs.GUI);
            help_hover = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("mm_btn_single_hover.tex", GameFileLocs.GUI);
            start_hover = UITextureManager.Instance.CreateInstance(fl);



            fl = FileSystem.Instance.Locate("mm_btn_credits_down.tex", GameFileLocs.GUI);
            credits_down = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("mm_btn_quit_down.tex", GameFileLocs.GUI);
            exit_down = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("mm_btn_help_down.tex", GameFileLocs.GUI);
            help_down = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("mm_btn_single_down.tex", GameFileLocs.GUI);
            start_down = UITextureManager.Instance.CreateInstance(fl);



            fl = FileSystem.Instance.Locate("mm_start_bg.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("mm_start_link.tex", GameFileLocs.GUI);
            linkbg = UITextureManager.Instance.CreateInstance(fl);



            fl = FileSystem.Instance.Locate("cursor.tex", GameFileLocs.GUI);
            cursor = UITextureManager.Instance.CreateInstance(fl);
            

            #region 配置按钮
            startButton = new RoundButton();
            startButton.X = 663;
            startButton.Y = 48;
            startButton.Radius = 244 / 2;
            startButton.Enabled = true;
            startButton.IsValid = true;

            startButton.MouseClick += StartButton_Click;
            startButton.MouseEnter += Button_MouseIn;
            startButton.MouseDown += Button_DownSound;

            exitButton = new RoundButton();
            exitButton.X = 1061;
            exitButton.Y = 554;
            exitButton.Radius = 106 / 2;
            exitButton.Enabled = true;
            exitButton.IsValid = true;

            exitButton.MouseClick += ExitButton_Click;
            exitButton.MouseEnter += Button_MouseIn;
            exitButton.MouseDown += Button_DownSound;


            creditButton = new RoundButton();
            creditButton.X = 901;
            creditButton.Y = 357;
            creditButton.Radius = 138 / 2;
            creditButton.Enabled = true;
            creditButton.IsValid = true;
            creditButton.MouseEnter += Button_MouseIn;
            creditButton.MouseDown += Button_DownSound;
            creditButton.MouseClick += CreditButton_Click;


            helpButton = new RoundButton();
            helpButton.X = 1031;
            helpButton.Y = 182;
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

        GameGoal CreateGoal()
        {
            GameGoal goal = new GameGoal(RulesTable.CityMaxDevelopment * 8);

            return goal;
        }
        ColorValue GetColor(int selIndex)
        {
            ColorValue selectedColor = ColorValue.Red;
            switch (selIndex)
            {
                case 1:
                    selectedColor = ColorValue.Red;
                    break;
                case 3:
                    selectedColor = ColorValue.Yellow;
                    break;
                case 2:
                    selectedColor = ColorValue.Green;

                    break;
                case 0:
                    selectedColor = ColorValue.Blue;

                    break;

            } return selectedColor;
        }
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
            sprite.Draw(linkbg, 0, 0, ColorValue.White);

            parent.RenderLogo(sprite);


            int x = 818 - 322 / 2;
            int y = 158 - 281 / 2;


            if (startButton.IsPressed)
            {
                sprite.Draw(start_down, x, y, ColorValue.White);
            }
            else if (startButton.IsMouseOver)
            {
                sprite.Draw(start_hover, x, y, ColorValue.White);
            }
            else
            {
                sprite.Draw(start, x, y, ColorValue.White);
            }

            x = 1107 - 192 / 2;
            y = 241 - 195 / 2;


            if (helpButton.IsPressed)
            {
                sprite.Draw(help_down, x, y, ColorValue.White);
            }
            else if (helpButton.IsMouseOver)
            {
                sprite.Draw(help_hover, x, y, ColorValue.White);
            }
            else
            {
                sprite.Draw(help, x, y, ColorValue.White);
            }

            x = 835;// -225 / 2;
            y = 336;// -180 / 2;

            if (creditButton.IsPressed)
            {
                sprite.Draw(credits_down, x, y, ColorValue.White);
            }
            else if (creditButton.IsMouseOver)
            {
                sprite.Draw(credits_hover, x, y, ColorValue.White);
            }
            else
            {
                sprite.Draw(credits, x, y, ColorValue.White);
            }

            x = 1129 - 160 / 2;
            y = 594 - 160 / 2;


            if (exitButton.IsPressed)
            {
                sprite.Draw(exit_down, x, y, ColorValue.White);
            }
            else if (exitButton.IsMouseOver)
            {
                sprite.Draw(exit_hover, x, y, ColorValue.White);
            }
            else
            {
                sprite.Draw(exit, x, y, ColorValue.White);
            }

            sprite.Draw(parent.Earth, 0, 0, ColorValue.White);
            if (parent.CurrentScreen == null)
            {
                sprite.Draw(cursor, mousePosition.X, mousePosition.Y, ColorValue.White);
            }
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

                    int selIndex = Randomizer.GetRandomInt(4);

                    gcp.Player1 = new Player("Player", CreateGoal(), 0);
                    gcp.Player1.SideColor = GetColor(selIndex);
                    gcp.Player2 = new AIPlayer(CreateGoal(), 1);
                    gcp.Player3 = new AIPlayer(CreateGoal(), 2);
                    gcp.Player4 = new AIPlayer(CreateGoal(), 3);


                    int i = Randomizer.GetRandomInt(3);


                    while (i == selIndex)
                    {
                        i++;
                        i %= Colors.Length;
                    }
                    gcp.Player2.SideColor = GetColor(i);
                    int sel1 = i;

                    i = Randomizer.GetRandomInt(2);
                    while (i == selIndex || i == sel1)
                    {
                        i++;
                        i %= Colors.Length;
                    }
                    gcp.Player3.SideColor = GetColor(i);
                    int sel2 = i;

                    while (i == selIndex || i == sel1 || i == sel2)
                    {
                        i++;
                        i %= Colors.Length;
                    }
                    gcp.Player4.SideColor = GetColor(i);
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
