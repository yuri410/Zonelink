using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.GUI.Controls;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.Logic;
using Code2015.World;

namespace Code2015.GUI
{
    class MainMenu : UIComponent
    {
        Code2015 game;
        Menu parent;

        float fps;

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

        Texture logo;
        Texture background;
        Texture linkbg;


        RoundButton startButton;
        RoundButton exitButton;
        RoundButton creditButton;
        RoundButton helpButton;

        NormalSoundObject mouseHover;
        NormalSoundObject mouseDown;
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

            fl = FileSystem.Instance.Locate("mm_logo.tex", GameFileLocs.GUI);
            logo = UITextureManager.Instance.CreateInstance(fl);

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


            helpButton = new RoundButton();
            helpButton.X = 1031;
            helpButton.Y = 182;
            helpButton.Radius = 138 / 2;
            helpButton.Enabled = true;
            helpButton.IsValid = true;
            helpButton.MouseEnter += Button_MouseIn;
            helpButton.MouseDown += Button_DownSound;
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
        void StartButton_Click(object sender, MouseButtonFlags btn)
        {
            if (btn == MouseButtonFlags.Left)
            {
                parent.CurrentScreen = parent.GetSelectScreen();
                //GameCreationParameters gcp = new GameCreationParameters();
                //gcp.Player1 = new Player("test");
                //gcp.Player1.SideColor = ColorValue.Red;
                //game.StartNewGame(gcp);
            }
        }



        public override void Render(Sprite sprite)
        {
            //sprite.SetTransform(Matrix.Identity);


            sprite.SetTransform(Matrix.Identity);

            sprite.Draw(background, 0, 0, ColorValue.White);
            sprite.Draw(linkbg, 0, 0, ColorValue.White);
            sprite.Draw(logo, 0, 0, ColorValue.White);

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

            
        }
        public override void Update(GameTime time)
        {
            fps = time.FramesPerSecond;


            startButton.Update(time);
            creditButton.Update(time);
            exitButton.Update(time);
            helpButton.Update(time);
            fps = time.FramesPerSecond;
        }
    }
}
