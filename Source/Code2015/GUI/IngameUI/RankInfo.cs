using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Code2015.World;
using Code2015.Logic;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;

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
        

        public override int Order
        {
            get
            {
                return 99;
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

        }


        public override bool HitTest(int x, int y)
        {
            return base.HitTest(x, y);
        }


        public override void Render(Sprite sprite)
        {

            List<Player> players = new List<Player>();
            for (int i = 0; i < gameLogic.LocalPlayerCount; i++)
                players.Add(gameLogic.GetLocalPlayer(i));
          
            StatisticRank(players);

            int startY = 41;
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].Type != PlayerType.LocalHuman)
                {
                    sprite.Draw(rankBackground, -6, startY, players[i].SideColor);
                    startY += 37;
                }
                else
                {
                    sprite.Draw(homeBackground, -6, startY - 4, players[i].SideColor);
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
        }



        private static int ComparePlayer(Player a, Player b)
        {
            return b.Area.CityCount.CompareTo(a.Area.CityCount);
        }
    

        private void StatisticRank(List<Player> players)
        {
            players.Sort(ComparePlayer);   
        }

            
    }
}
