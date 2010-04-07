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
        RenderSystem renderSys;

        FastList<ScoreEntry> scores = new FastList<ScoreEntry>();

        Texture background;

        Font font;

        public ScoreScreen(RenderSystem rs)
        {
            this.renderSys = rs;

            FileLocation fl = FileSystem.Instance.Locate("ss_bg.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl);

            font = FontManager.Instance.GetFont("default");
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
            const float FontSize = 24;
            sprite.Draw(background, 0, 0, ColorValue.White);

            int bgc = (int)ColorValue.Black.PackedValue;
            int ftc = (int)ColorValue.White.PackedValue;

            font.DrawString(sprite, "Name", 1, 1, FontSize, DrawTextFormat.Center, bgc);
            font.DrawString(sprite, "Development", 201, 1, FontSize, DrawTextFormat.Center, bgc);
            font.DrawString(sprite, "CO2", 401, 1, FontSize, DrawTextFormat.Center, bgc);
            font.DrawString(sprite, "Total", 601, 1, FontSize, DrawTextFormat.Center, bgc);

            font.DrawString(sprite, "Name", 0, 0, FontSize, DrawTextFormat.Center, ftc);
            font.DrawString(sprite, "Development", 200, 0, FontSize, DrawTextFormat.Center, ftc);
            font.DrawString(sprite, "CO2", 400, 0, FontSize, DrawTextFormat.Center, ftc);
            font.DrawString(sprite, "Total", 600, 0, FontSize, DrawTextFormat.Center, ftc);

            int y = 100;
            for (int i = 0; i < scores.Count; i++, y += 30)
            {
                font.DrawString(sprite, scores.Elements[i].Player.Name, 1, y + 1, FontSize, DrawTextFormat.Center, bgc);
                font.DrawString(sprite, scores.Elements[i].Player.Name, 0, y, FontSize, DrawTextFormat.Center, ftc);

                string msg = scores.Elements[i].Development.ToString("G");
                font.DrawString(sprite, msg, 201, y + 1, FontSize, DrawTextFormat.Center, bgc);
                font.DrawString(sprite, msg, 200, y, FontSize, DrawTextFormat.Center, ftc);

                msg =  (-scores.Elements[i].CO2).ToString("G");
                font.DrawString(sprite, msg, 401, y + 1, FontSize, DrawTextFormat.Center, bgc);
                font.DrawString(sprite, msg, 400, y, FontSize, DrawTextFormat.Center, ftc);

                msg = scores.Elements[i].Total.ToString("G");
                font.DrawString(sprite, msg, 601, y + 1, FontSize, DrawTextFormat.Center, bgc);
                font.DrawString(sprite, msg, 600, y, FontSize, DrawTextFormat.Center, ftc);
            }
        }

        public override void Update(GameTime time)
        {
            
        }
    }
}
