using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.Logic;
using XI = Microsoft.Xna.Framework.Input;
using Apoc3D.GUI.Controls;

namespace Code2015.GUI
{
    /// <summary>
    ///  表示游戏菜单
    /// </summary>
    class Menu : UIComponent, IGameComponent
    {
        Font font;

        Code2015 game;

        float fps;

        Texture credits;
        Texture exit;
        Texture help;
        Texture start;

        Texture cursor;
        Point mousePosition;

        const int StartBtnX = 428;
        const int StartBtnY = 304;

        //const int StartBtnCenterX = StartBtnLTX + StartBtnRadius;
        //const int StartBtnCenterY = StartBtnLTY + StartBtnRadius;
        const int StartBtnRadius = 64;


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

        public Menu(Code2015 game, RenderSystem rs)
        {
            FileLocation fl = FileSystem.Instance.Locate("def.fnt", GameFileLocs.GUI);
            font = FontManager.Instance.CreateInstance(rs, fl, "default");
            this.game = game;

            fl = FileSystem.Instance.Locate("cursor.tex", GameFileLocs.GUI);
            cursor = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("credits.tex", GameFileLocs.GUI);
            credits = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("exit.tex", GameFileLocs.GUI);
            exit = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("help.tex", GameFileLocs.GUI);
            help = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("start.tex", GameFileLocs.GUI);
            start = UITextureManager.Instance.CreateInstance(fl);

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
            exitButton.ImageMouseOver = start;
            exitButton.Enabled = true;
            exitButton.IsValid = true;

            exitButton.MouseClick += ExitButton_Click;


            creditButton = new RoundButton();
            creditButton.X = CreditBtnX;
            creditButton.Y = CreditBtnY;
            creditButton.Radius = 64;
            creditButton.Image = credits;
            creditButton.ImageMouseOver = start;
            creditButton.Enabled = true;
            creditButton.IsValid = true;


            helpButton = new RoundButton();
            helpButton.X = HelpBtnX;
            helpButton.Y = HelpBtnY;
            helpButton.Radius = 64;
            helpButton.Image = help;
            helpButton.ImageMouseOver = start;
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
                GameCreationParameters gcp = new GameCreationParameters();

                gcp.Player1 = new Player("test");
                gcp.Player1.SideColor = ColorValue.Red;
                game.StartNewGame(gcp);
            }
        }

        public void Render()
        {

        }

        public override void Render(Sprite sprite)
        {
            //sprite.SetTransform(Matrix.Identity);

            if (!game.IsIngame)
            {
                
                font.DrawString(sprite, "\n\nfps: " + fps.ToString(), 0, 0, 15, DrawTextFormat.Center, -1);

                //Vector2 pa = new Vector2(StartBtnCenterX, StartBtnCenterY);
                //Vector2 pb = new Vector2(mousePosition.X, mousePosition.Y);

                //float s = Vector2.Distance(pa, pb);

                //if (s < StartBtnRadius)
                //    sprite.Draw(start, 428, 304, ColorValue.Red);
                //else 
                //    sprite.Draw(start, 428, 304, ColorValue.White);
                startButton.Render(sprite);
                creditButton.Render(sprite);
                exitButton.Render(sprite);
                helpButton.Render(sprite);
                //sprite.Draw(credits, CreditBtnX, CreditBtnY, ColorValue.White);
                //sprite.Draw(exit, ExitBtnX, ExitBtnY, ColorValue.White);
               
                //sprite.Draw(help, 672, 166, ColorValue.White);

                sprite.SetTransform(Matrix.Identity);
                sprite.Draw(cursor, mousePosition.X, mousePosition.Y, ColorValue.White);
            }
        }

        

        public override void Update(GameTime time)
        {
            mousePosition.X = MouseInput.X;
            mousePosition.Y = MouseInput.Y;

            if (!game.IsIngame)
            {
                startButton.Update(time);
                creditButton.Update(time);
                exitButton.Update(time);
                helpButton.Update(time);

            }


            fps = time.FramesPerSecond;

        }
    }

}
