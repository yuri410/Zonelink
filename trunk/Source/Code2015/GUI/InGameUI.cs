using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.World;
using Code2015.World.Screen;
using XI = Microsoft.Xna.Framework.Input;

namespace Code2015.GUI
{
    /// <summary>
    ///  表示游戏过程中的界面
    /// </summary>
    class InGameUI : UIComponent
    {
        GameScene scene;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;
        Font font;

        GoalIcons icons;

        ScreenPhysicsWorld physWorld;

        Texture cursor;
        Point mousePosition;


        public ScreenPhysicsWorld PhysicsWorld
        {
            get { return physWorld; }
        }


        public InGameUI(Code2015 game, Game parent, GameScene scene)
        {
            this.parent = parent;

            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;
            this.physWorld = new ScreenPhysicsWorld();

            

            this.icons = new GoalIcons(physWorld);

            FileLocation fl = FileSystem.Instance.Locate("def.fnt", GameFileLocs.GUI);
            font = FontManager.Instance.CreateInstance(renderSys, fl, "default");

            fl = FileSystem.Instance.Locate("cursor.tex", GameFileLocs.GUI);
            cursor = UITextureManager.Instance.CreateInstance(fl);
        }

        public override void Render(Sprite sprite)
        {
            if (!parent.IsLoaded)
            {
                font.DrawString(sprite, "Loading", 0, 0, 34, DrawTextFormat.Center, -1);
            }
            else
            {
                icons.Render(sprite);

                sprite.SetTransform(Matrix.Identity);
                sprite.Draw(cursor, mousePosition.X, mousePosition.Y, ColorValue.White);
            }
        }

        public override void Update(GameTime time)
        {
            physWorld.Update(time);

            XI.MouseState mstate = XI.Mouse.GetState();
            mousePosition.X = mstate.X;
            mousePosition.Y = mstate.Y;

            icons.Update(time);

        }
    }
}
