using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.Logic;
using Code2015.World;

namespace Code2015.GUI
{
    class ScoreScreen : UIComponent
    {
        Code2015 parent;
        RenderSystem renderSys;

        FastList<ScoreEntry> scores = new FastList<ScoreEntry>();

        Texture background;

        GameFont font;

        public ScoreScreen(Code2015 parent)
        {
            this.parent = parent;
            this.renderSys = parent.RenderSystem;

            FileLocation fl = FileSystem.Instance.Locate("ss_bg.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl);

            font = GameFontManager.Instance.F18;
        }

        public void Add(ScoreEntry entry)
        {
            scores.Add(entry);
        }
        public void Clear()
        {
            scores.Clear();
        }

        public override void Render(Sprite sprite)
        {
            sprite.Draw(background, 0, 0, ColorValue.White);

            int bgc = (int)ColorValue.Black.PackedValue;
            int ftc = (int)ColorValue.White.PackedValue;

            font.DrawString(sprite, "NAME", 0, 0, ColorValue.White);
            font.DrawString(sprite, "DEVELOPMENT", 200, 0, ColorValue.White);
            font.DrawString(sprite, "CO2", 400, 0, ColorValue.White);
            font.DrawString(sprite, "TOTAL", 600, 0, ColorValue.White);


            int y = 100;
            for (int i = 0; i < scores.Count; i++, y += 30)
            {
                ftc = (int)scores.Elements[i].Player.SideColor.PackedValue;

                font.DrawString(sprite, scores.Elements[i].Player.Name, 0, y , ColorValue.White);

                string msg = ((int)Math.Round(scores.Elements[i].Development)).ToString("G");

                font.DrawString(sprite, msg, 200, y, ColorValue.White);

                msg = ((int)Math.Round(scores.Elements[i].CO2)).ToString("G");

                font.DrawString(sprite, msg, 400, y, ColorValue.White);

                msg = ((int)Math.Round(scores.Elements[i].Total)).ToString("G");

                font.DrawString(sprite, msg, 600, y, ColorValue.White);
            }
        }

        public override void Update(GameTime time)
        {
            if (MouseInput.IsMouseUpLeft)
            {
                parent.Back();
            }
        }
    }
}
