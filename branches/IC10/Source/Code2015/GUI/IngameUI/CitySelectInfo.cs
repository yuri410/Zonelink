using System;
using System.Collections.Generic;
using System.Text;
using Code2015.World;
using Apoc3D;
using Apoc3D.Graphics;
using Code2015.Logic;
using Code2015.EngineEx;
using Apoc3D.MathLib;

namespace Code2015.GUI.IngameUI
{
    class CitySelectInfo : UIComponent
    {
        const int X = 996;
        const int Y = 800;


        GameScene scene;
        GameState gameLogic;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;
        Player player;
        Picker picker;
        GameFontRuan f14;
        
        City selectCity;

        public City SelectedCity
        {
            get { return selectCity; }
            set
            {
                if (selectCity != value)
                {
                    selectCity = value;

                    //if (selectCity != null)
                    //{
                    //    if (selectCity.Owner != player)
                    //    {
                    //        selectCity = null;
                    //    }
                    //}
                }  
            }
        }

        public CitySelectInfo(Code2015 game, Game parent, GameScene scene, GameState gamelogic, Picker picker)
        {
            this.scene = scene;
            this.renderSys = game.RenderSystem;
            this.player = parent.HumanPlayer;
            this.picker = picker;
            this.gameLogic = gamelogic;
            this.parent = parent;
            this.game = game;


            f14 = GameFontManager.Instance.FRuan;
        }

        public override int Order
        {
            get
            {
                return 5;
            }
        }


        public override bool HitTest(int x, int y)
        {
            return false;
        }


        public override void Render(Sprite sprite)
        {
    
            if (selectCity != null)
            {
                int hp = (int)selectCity.HealthValue;
                int hpFull = (int)(selectCity.HealthValue / selectCity.HPRate);

                string hpInfo = hp.ToString() + "   " + hpFull.ToString();
                string developmentInfo = selectCity.Name.ToString().ToUpperInvariant();
                f14.DrawString(sprite, hpInfo, 100, 100, ColorValue.White);

                //string test = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                f14.DrawString(sprite, developmentInfo, 100, 250, ColorValue.White);

                f14.DrawString(sprite, "/!\"$',-.:;", 100, 400, ColorValue.White);      
            }        

        }


        public override void Update(GameTime time)
        {
            base.Update(time);
           
        }
    }
}
