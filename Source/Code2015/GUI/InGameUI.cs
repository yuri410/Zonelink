using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.Vfs;
using Code2015.EngineEx;

namespace Code2015.GUI
{
    class InGameUI : UIComponent
    {
        GameScene scene;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;
        Font font;

        public InGameUI(Code2015 game,Game parent, GameScene scene)
        {
            this.parent = parent;

            this.game = game;    
            this.renderSys = game.RenderSystem;
            this.scene = scene;

            FileLocation fl = FileSystem.Instance.Locate("def.fnt", GameFileLocs.UI);
            font = FontManager.Instance.CreateInstance(renderSys, fl, "default");
        }

        public override void Render(Sprite sprite)
        {
            if (!parent.IsLoaded)
            {
                font.DrawString(sprite, "Loading", 0, 0, 34, DrawTextFormat.Center, -1);
            }
        }

        public override void Update(GameTime time)
        {

        }
    }
}
