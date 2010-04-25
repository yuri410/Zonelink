using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.GUI.Controls;
using Code2015.Logic;
using Code2015.World;

namespace Code2015.GUI
{
    class DevelopmentMeter : UIComponent
    {
        struct Entry 
        {
            public Player Player;
            public ProgressBar Bar;
        }

        GameScene scene;
        GameState gameLogic;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;
        Player player;

        GameFont f14;

        FastList<Entry> prgBars = new FastList<Entry>();
        Texture background;

        Point GetColorPosition(ColorValue color)
        {
            if (color == ColorValue.Red)
            {
                return new Point(1090, 86);
            }
            else if (color == ColorValue.Green)
            {
                return new Point(1090, 27);
            }
            else if (color == ColorValue.Yellow)
            {
                return new Point(1092, 67);
            }
            return new Point(1091, 46);
        }
        string GetColorBar(ColorValue color)
        {
            if (color == ColorValue.Red) 
            {
                return "ig_red.tex";
            }
            else if (color == ColorValue.Green) 
            {
                return "ig_green.tex";
            }
            else if (color == ColorValue.Yellow)
            {
                return "ig_yellow.tex";
            }
            return "ig_blue.tex";
        }
        public override int Order
        {
            get { return 7; }
        }

        public DevelopmentMeter(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.parent = parent;
            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;
            this.gameLogic = gamelogic;
            this.player = parent.HumanPlayer;
            this.f14 = GameFontManager.Instance.F14;

            FileLocation fl = FileSystem.Instance.Locate("ig_development.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl);

            for (int i = 0; i < gamelogic.LocalPlayerCount; i++) 
            {
                Player pl = gamelogic.GetLocalPlayer(i);

                ProgressBar playerProgress = new ProgressBar();

                playerProgress.Width = 182;
                playerProgress.Height = 16;

                fl = FileSystem.Instance.Locate(GetColorBar(pl.SideColor), GameFileLocs.GUI);
                playerProgress.ProgressImage = UITextureManager.Instance.CreateInstance(fl);

                Point p = GetColorPosition(pl.SideColor);
                playerProgress.X = p.X;
                playerProgress.Y = p.Y;

                Entry ent;
                ent.Player = pl;
                ent.Bar = playerProgress;
                prgBars.Add(ent);
            }


        }
        
        public override void Render(Sprite sprite)
        {
            sprite.Draw(background, 985, -7, ColorValue.White);
            for (int i = 0; i < prgBars.Count; i++)
            {
                prgBars.Elements[i].Bar.Render(sprite);
            }

            f14.DrawString(sprite, "CO2", 998, 7, ColorValue.White);
            f14.DrawString(sprite, "DEVELOPMENT", 1091, 7, ColorValue.White);

        }

        public override void Update(GameTime time)
        {
            for (int i = 0; i < prgBars.Count; i++)
            {
                prgBars.Elements[i].Bar.Value = prgBars.Elements[i].Player.Goal.DevelopmentPercentage;
            }
        }
    }
}
