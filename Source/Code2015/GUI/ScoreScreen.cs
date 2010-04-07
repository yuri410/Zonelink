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

            font.DrawString(sprite, "Name", 0, 0, FontSize, DrawTextFormat.Center, (int)ColorValue.White.PackedValue);
            font.DrawString(sprite, "Development", 150, 0, FontSize, DrawTextFormat.Center, (int)ColorValue.White.PackedValue);
            font.DrawString(sprite, "CO2", 250, 0, FontSize, DrawTextFormat.Center, (int)ColorValue.White.PackedValue);
            font.DrawString(sprite, "Total", 350, 0, FontSize, DrawTextFormat.Center, (int)ColorValue.White.PackedValue);

            int y = 100;
            for (int i = 0; i < scores.Count; i++, y += 30)
            {
                font.DrawString(sprite, scores.Elements[i].Player.Name, 0, 0, FontSize, DrawTextFormat.Center, (int)ColorValue.White.PackedValue);
                font.DrawString(sprite, scores.Elements[i].Development.ToString("D"), 150, 0, FontSize, DrawTextFormat.Center, (int)ColorValue.White.PackedValue);
                font.DrawString(sprite, (-scores.Elements[i].CO2).ToString("D"), 250, 0, FontSize, DrawTextFormat.Center, (int)ColorValue.White.PackedValue);
                font.DrawString(sprite, scores.Elements[i].Total.ToString("D"), 350, 0, FontSize, DrawTextFormat.Center, (int)ColorValue.White.PackedValue);
            }
        }

        public override void Update(GameTime time)
        {
            
        }
    }
}
