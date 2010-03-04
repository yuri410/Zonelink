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

            //if (!game.IsIngame)
            //{
            //    font.DrawString(sprite, "Not Implemented\nPress Enter to start a new game", 0, 0, 34, DrawTextFormat.Center, -1);
            //}
            font.DrawString(sprite, "\n\nfps: " + fps.ToString(), 0, 0, 15, DrawTextFormat.Center, -1);
            sprite.Draw(credits, 231, 464, ColorValue.White);
            sprite.Draw(exit, 79, 620, ColorValue.White);
            sprite.Draw(start, 428, 304, ColorValue.White);
            sprite.Draw(help, 672, 166, ColorValue.White); 

            sprite.SetTransform(Matrix.Identity);
            sprite.Draw(cursor, mousePosition.X, mousePosition.Y, ColorValue.White);
        }

        public override void Update(GameTime time)
        { 
            XI.MouseState mstate = XI.Mouse.GetState();
            mousePosition.X = mstate.X;
            mousePosition.Y = mstate.Y;
            if (!game.IsIngame)
            {
                float s = (float)Math.Sqrt((mousePosition.X - 460) * (mousePosition.X - 460) + (mousePosition.Y - 336) * (mousePosition.Y - 336));
                if ((s < 32)&&(mstate.LeftButton==XI.ButtonState.Pressed))
                {
                   
                    GameCreationParameters gcp = new GameCreationParameters();


                    gcp.Player1 = new Player("test");
                    gcp.Player1.SideColor = ColorValue.Red;
                    game.StartNewGame(gcp);

                    return;
                }
                
            }

            
            fps = time.FramesPerSecond;

            if (!game.IsIngame)
            {
                XI.KeyboardState state = XI.Keyboard.GetState();
                
                if (state.IsKeyDown(XI.Keys.Enter))
                {
                    GameCreationParameters gcp = new GameCreationParameters();

                    
                    gcp.Player1 = new Player("test");
                    gcp.Player1.SideColor = ColorValue.Red;
                    game.StartNewGame(gcp);

                    return;


                   
                }
            }

            
        }
    }
    
}
