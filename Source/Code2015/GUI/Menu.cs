using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.Vfs;
using Code2015.EngineEx;
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

        public Menu(Code2015 game, RenderSystem rs)
        {
            FileLocation fl = FileSystem.Instance.Locate("def.fnt", GameFileLocs.GUI);
            font = FontManager.Instance.CreateInstance(rs, fl, "default");
            this.game = game;
        }

        public void Render()
        {

        }

        public override void Render(Sprite sprite)
        {
            if (!game.IsIngame)
            {
                font.DrawString(sprite, "Not Implemented\nPress Enter to start a new game", 0, 0, 34, DrawTextFormat.Center, -1);
            }
            font.DrawString(sprite, "\n\nfps: " + fps.ToString(), 0, 0, 15, DrawTextFormat.Center, -1);
        }

        public override void Update(GameTime time)
        {
            fps = time.FramesPerSecond;

            if (!game.IsIngame)
            {
                XI.KeyboardState state = XI.Keyboard.GetState();

                if (state.IsKeyDown(XI.Keys.Enter))
                {
                    GameCreationParameters gcp = new GameCreationParameters();

                    game.StartNewGame(gcp);

                    return;
                }
            }
        }
    }
}
