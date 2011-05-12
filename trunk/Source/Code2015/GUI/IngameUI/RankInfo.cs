using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Code2015.World;
using Code2015.Logic;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Apoc3D.GUI.Controls;

namespace Code2015.GUI.IngameUI
{
    class RankInfo : UIComponent
    {
        GameScene scene;
        GameState gameLogic;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;
     

        Texture rankBackground;
        Texture homeBackground;
        Texture rankColor;

        GameFontRuan f8;
        GameFontRuan f6;

        List<Player> players = new List<Player>();

        public override int Order
        {
            get
            {
                return 57;
            }
        }


        public RankInfo(Code2015 game, Game parent, GameScene scene, GameState logic)
        {
            this.game = game;
            this.parent = parent;
            this.scene = scene;
            this.gameLogic = logic;
            this.renderSys = game.RenderSystem;

          

            FileLocation fl = FileSystem.Instance.Locate("nig_rank_bk.tex", GameFileLocs.GUI);
            rankBackground = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_home.tex", GameFileLocs.GUI);
            homeBackground = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_rank_color.tex", GameFileLocs.GUI);
            rankColor = UITextureManager.Instance.CreateInstance(fl);

            f6 = GameFontManager.Instance.FRuanEdged4;
            f8 = GameFontManager.Instance.FRuanEdged6;

           
            //for (int i = 0; i < gameLogic.LocalPlayerCount; i++)
                //players.Add(gameLogic.GetLocalPlayer(i));
        }


        public override bool HitTest(int x, int y)
        {
            if (x < 128)
            {
                int startY = 41;
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].Type != PlayerType.LocalHuman)
                    {
                        startY += 37;
                    }
                    else
                    {
                        Rectangle rect = new Rectangle(-6, startY - 4, homeBackground.Width, homeBackground.Height);
                        if (Control.IsInBounds(x, y, ref rect))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }


        public override void Render(Sprite sprite)
        {
            //List<Player> players = new List<Player>();
            players.Clear();
            for (int i = 0; i < gameLogic.LocalPlayerCount; i++)
                players.Add(gameLogic.GetLocalPlayer(i));
            StatisticRank(players);

            int startY = 41;
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].Type != PlayerType.LocalHuman)
                {
                    string s = (i+1).ToString();
                    sprite.Draw(rankBackground, -6, startY, ColorValue.White);
                    f8.DrawString(sprite, s, 0, startY - 20, ColorValue.White);


                    Matrix trans = Matrix.Scaling(0.33f, 0.33f, 1) * Matrix.Translation(new Vector3(19, startY + 14, 0));
                    sprite.SetTransform(trans);

                    f8.DrawString(sprite, "TH", 0, 0, ColorValue.White);
                    sprite.SetTransform(Matrix.Identity);

                    sprite.Draw(rankColor, 35, startY + 9, players[i].SideColor);
                    f8.DrawString(sprite, players[i].Area.CityCount.ToString(), 80, startY - 20, ColorValue.White);
                    startY += 37;
                }
                else
                {
                    string s = (i + 1).ToString();
                    sprite.Draw(homeBackground, -6, startY - 4, players[i].SideColor);
                    f8.DrawString(sprite, s, 2, startY, ColorValue.White);

                    Matrix trans = Matrix.Scaling(0.35f, 0.35f, 1) * Matrix.Translation(new Vector3(22, startY + 35, 0));
                    sprite.SetTransform(trans);
                    f8.DrawString(sprite, "TH", 0, 0, ColorValue.White);
                    sprite.SetTransform(Matrix.Identity);

                    f6.DrawString(sprite, players[i].Area.CityCount.ToString(), 73, startY - 45, ColorValue.White);
                    startY += 70;
                }           
            }
           
        }


        public override void Update(Apoc3D.GameTime time)
        {
            base.Update(time);
         
        }

        public override void UpdateInteract(Apoc3D.GameTime time)
        {
            base.UpdateInteract(time);

            if (MouseInput.IsMouseUpLeft) 
            {
                Player player = gameLogic.LocalHumanPlayer;

                PlayerArea area = player.Area;
                for (int i = 0; i < area.CityCount; i++)
                {
                    if (area.GetCity(i).IsHomeCity) 
                    {
                        scene.Camera.Longitude = MathEx.Degree2Radian(area.GetCity(i).Longitude);
                        scene.Camera.Latitude = MathEx.Degree2Radian(area.GetCity(i).Latitude);

                        return;
                    }
                }
            }
        }



        private static int ComparePlayer(Player a, Player b)
        {
            return b.Area.CityCount.CompareTo(a.Area.CityCount);
        }
    

        public static void StatisticRank(List<Player> players)
        {
            players.Sort(ComparePlayer);   
        }

            
    }
}
