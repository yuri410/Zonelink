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

        const int StartBtnLTX = 428;
        const int StartBtnLTY = 304;

        const int StartBtnCenterX = StartBtnLTX + StartBtnRadius;
        const int StartBtnCenterY = StartBtnLTY + StartBtnRadius;
        const int StartBtnRadius = 64;

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

                Vector2 pa = new Vector2(StartBtnCenterX, StartBtnCenterY);
                Vector2 pb = new Vector2(mousePosition.X, mousePosition.Y);

                float s = Vector2.Distance(pa, pb);

                if (s < StartBtnRadius)
                    sprite.Draw(start, 428, 304, ColorValue.Red);
                else 
                    sprite.Draw(start, 428, 304, ColorValue.White);
                
                sprite.Draw(credits, 231, 464, ColorValue.White);
                sprite.Draw(exit, 79, 620, ColorValue.White);
               
                sprite.Draw(help, 672, 166, ColorValue.White);

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
                Vector2 pa = new Vector2(StartBtnCenterX, StartBtnCenterY);
                Vector2 pb = new Vector2(mousePosition.X, mousePosition.Y);

                float s = Vector2.Distance(pa, pb);


                if ((s < StartBtnRadius) && MouseInput.IsMouseUpLeft)
                {
                    GameCreationParameters gcp = new GameCreationParameters();

                    gcp.Player1 = new Player("test");
                    gcp.Player1.SideColor = ColorValue.Red;
                    game.StartNewGame(gcp);

                    return;
                }

            }


            fps = time.FramesPerSecond;

        }
    }

}
