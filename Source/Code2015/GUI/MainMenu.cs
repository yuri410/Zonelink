using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.GUI.Controls;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.Logic;

namespace Code2015.GUI
{
    class MainMenu : UIComponent
    {
        Font font;

        Code2015 game;
        Menu parent;

        float fps;

        Texture credits;
        Texture exit;
        Texture help;
        Texture start;

        Texture background;

        Texture cursor;
        Point mousePosition;

        const int StartBtnX = 428;
        const int StartBtnY = 304;

        //const int StartBtnCenterX = StartBtnLTX + StartBtnRadius;
        //const int StartBtnCenterY = StartBtnLTY + StartBtnRadius;
        const int StartBtnRadius = 64;
        const int ExitBtnRadius = 64;
        const int CreditBtnRadius = 64;
        const int HelpBtnRadius = 64;


        const int ExitBtnX = 79;
        const int ExitBtnY = 620;
        const int CreditBtnX = 231;
        const int CreditBtnY = 464;

        const int HelpBtnX = 672;
        const int HelpBtnY = 166;

        RoundButton startButton;
        RoundButton exitButton;
        RoundButton creditButton;
        RoundButton helpButton;

        public MainMenu(Code2015 game, Menu parent)
        {
            RenderSystem rs = game.RenderSystem;
            FileLocation fl = FileSystem.Instance.Locate("def.fnt", GameFileLocs.GUI);
            font = FontManager.Instance.CreateInstance(rs, fl, "default");
            this.game = game;
            this.parent = parent;

            fl = FileSystem.Instance.Locate("cursor.tex", GameFileLocs.GUI);
            cursor = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("mm_btn_credits.tex", GameFileLocs.GUI);
            credits = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("mm_btn_quit.tex", GameFileLocs.GUI);
            exit = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("mm_btn_help.tex", GameFileLocs.GUI);
            help = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("mm_btn_single.tex", GameFileLocs.GUI);
            start = UITextureManager.Instance.CreateInstance(fl);


            fl = FileSystem.Instance.Locate("mm_start_bg.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl);

            #region 配置按钮
            startButton = new RoundButton();
            startButton.X = StartBtnX;
            startButton.Y = StartBtnY;
            startButton.Radius = 64;
            startButton.Image = start;
            startButton.ImageMouseOver = start;
            startButton.Enabled = true;
            startButton.IsValid = true;

            startButton.MouseClick += StartButton_Click;

            exitButton = new RoundButton();
            exitButton.X = ExitBtnX;
            exitButton.Y = ExitBtnY;
            exitButton.Radius = 64;
            exitButton.Image = exit;
            exitButton.ImageMouseOver = exit;
            exitButton.Enabled = true;
            exitButton.IsValid = true;

            exitButton.MouseClick += ExitButton_Click;


            creditButton = new RoundButton();
            creditButton.X = CreditBtnX;
            creditButton.Y = CreditBtnY;
            creditButton.Radius = 64;
            creditButton.Image = credits;
            creditButton.ImageMouseOver = credits;
            creditButton.Enabled = true;
            creditButton.IsValid = true;


            helpButton = new RoundButton();
            helpButton.X = HelpBtnX;
            helpButton.Y = HelpBtnY;
            helpButton.Radius = 64;
            helpButton.Image = help;
            helpButton.ImageMouseOver = help;
            helpButton.Enabled = true;
            helpButton.IsValid = true;
            #endregion

        }

        void ExitButton_Click(object sender, MouseButtonFlags btn)
        {
            if (btn == MouseButtonFlags.Left)
            {

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


        public void Render()
        {

        }

        public override void Render(Sprite sprite)
        {
            //sprite.SetTransform(Matrix.Identity);


            sprite.SetTransform(Matrix.Identity);

            sprite.Draw(background, 0, 0, ColorValue.White);
            font.DrawString(sprite, "\n\nfps: " + fps.ToString(), 0, 0, 15, DrawTextFormat.Center, -1);

            startButton.Render(sprite);
            creditButton.Render(sprite);
            exitButton.Render(sprite);
            helpButton.Render(sprite);

            sprite.Draw(cursor, mousePosition.X, mousePosition.Y, ColorValue.White);


        }
        public override void Update(GameTime time)
        {
            mousePosition.X = MouseInput.X;
            mousePosition.Y = MouseInput.Y;
            fps = time.FramesPerSecond;


            startButton.Update(time);
            creditButton.Update(time);
            exitButton.Update(time);
            helpButton.Update(time);
            fps = time.FramesPerSecond;
        }
    }
}
