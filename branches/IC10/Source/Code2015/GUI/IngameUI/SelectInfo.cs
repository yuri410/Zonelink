using System;
using System.Collections.Generic;
using System.Text;
using Code2015.World;
using Apoc3D;
using Apoc3D.Graphics;
using Code2015.Logic;
using Code2015.EngineEx;
using Apoc3D.MathLib;
using Apoc3D.Vfs;

namespace Code2015.GUI.IngameUI
{
    class SelectInfo : UIComponent
    {
        private GameScene scene;
        private GameState gameLogic;
        private RenderSystem renderSys;
        private Code2015 game;
        private Game parent;
        private Player player;

        GameFontRuan f6;


        private const int X = 900;
        private const int Y = 550;



        #region 选择城市
        /// <summary>
        /// Currently selected city, Null if not selected
        /// </summary>
        private City selectCity;

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

        /// <summary>
        /// 泡泡漂浮在城市上空的高度
        /// </summary>
        private const float bubbleHeight = 500;

        private Texture cityBubbleTex;
       
        #endregion


        //#region 选择资源球

        ///// <summary>
        ///// Currently selected ball, Null if not selected
        ///// </summary>
        //private RBall selectBall;

        //public RBall SelectedRBall
        //{
        //    get { return selectBall; }
        //    set
        //    {
        //        if (selectBall != value)
        //        {
        //            selectBall = value;
        //            if (selectBall != null)
        //            {
        //                if (selectBall.Owner != player)
        //                {
        //                    selectBall = null;
        //                }
        //            }
        //        }
        //    }
        //}

        //#endregion


        #region 选择矿车
        private Harvester selectHarvester;

        public Harvester SelectedHarvester
        {
            get { return selectHarvester; }
            set
            {
                if (selectHarvester != value)
                {
                    selectHarvester = value;
                    //if (selectHarvester != null)
                    //{
                    //    if (selectHarvester.Owner != player)
                    //    {
                    //        selectHarvester = null;
                    //    }
                    //}
                }
            }
        }

        private Texture harvesterBubbleTex;
        private const float harvesterBubbleHeight = 125;
        #endregion


        public SelectInfo(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.scene = scene;
            this.renderSys = game.RenderSystem;
            this.player = parent.HumanPlayer;
            this.gameLogic = gamelogic;
            this.parent = parent;
            this.game = game;
            this.f6 = GameFontManager.Instance.FRuanEdged6;

            FileLocation fl = FileSystem.Instance.Locate("bubble_256x64.tex", GameFileLocs.GUI);
            cityBubbleTex = UITextureManager.Instance.CreateInstance(fl);


            fl = FileSystem.Instance.Locate("bubble_64.tex", GameFileLocs.GUI);
            harvesterBubbleTex = UITextureManager.Instance.CreateInstance(fl);


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
             RenderSelectCity(sprite);
             RenderSelectHarvester(sprite);
         }

         public override void Update(GameTime time)
         {
             base.Update(time);

             
         }


         private void RenderSelectCity(Sprite sprite)
         {
             if (selectCity != null)
             {
                 float radLng = MathEx.Degree2Radian(selectCity.Longitude);
                 float radLat = MathEx.Degree2Radian(selectCity.Latitude);

                 Vector3 tangy = PlanetEarth.GetTangentY(radLng, radLat);
                 Vector3 tangx = PlanetEarth.GetTangentX(radLng, radLat);

                 Vector3 cityNormal = PlanetEarth.GetNormal(radLng, radLat);
                 cityNormal.Normalize();

                 Vector3 BubblePos = selectCity.Position + cityNormal * bubbleHeight;

                 Viewport vp = renderSys.Viewport;
                 Vector3 screenPos = vp.Project(BubblePos, scene.Camera.ProjectionMatrix,
                                                scene.Camera.ViewMatrix, Matrix.Identity);

                 Vector3 lp = vp.Project(BubblePos + tangx, scene.Camera.ProjectionMatrix,
                                                        scene.Camera.ViewMatrix, Matrix.Identity);

                 Vector3 rp = vp.Project(BubblePos - tangx, scene.Camera.ProjectionMatrix,
                                                        scene.Camera.ViewMatrix, Matrix.Identity);

    
                 float scale = 1.5f * Vector3.Distance(lp, rp) / 2;

                 Matrix trans = Matrix.Translation(-cityBubbleTex.Width / 2, -cityBubbleTex.Height / 2, 0) *
                            Matrix.Scaling(scale, scale, 1) * Matrix.Translation(screenPos.X, screenPos.Y, 0);

               
                 sprite.SetTransform(trans);
                 sprite.Draw(cityBubbleTex, 0, 0, ColorValue.White);
                 sprite.SetTransform(Matrix.Identity);


                 //属性,右下角显示
                 int hp = (int)selectCity.HealthValue;
                 int hpFull = (int)(selectCity.HealthValue / selectCity.HPRate);

                 string hpInfo = "HP: " + hp.ToString() + "/" + hpFull.ToString() + "\n";
                 string developmentInfo = "Development: " + selectCity.Name.ToString();
                 string info = hpInfo + developmentInfo;
                 info = info.ToUpperInvariant();

                 Size size = f6.MeasureString(info);

                 f6.DrawString(sprite, info, 1150 - size.Width, 700 - size.Height, ColorValue.White);               
                 


             }
         }

         private void RenderSelectHarvester(Sprite sprite)
         {
             if (SelectedHarvester != null)
             {
                 float radLng = MathEx.Degree2Radian(SelectedHarvester.Longtitude);
                 float radLat = MathEx.Degree2Radian(SelectedHarvester.Latitude);

                 Vector3 tangy = PlanetEarth.GetTangentY(radLng, radLat);
                 Vector3 tangx = PlanetEarth.GetTangentX(radLng, radLat);

                 Vector3 normal = PlanetEarth.GetNormal(radLng, radLat);
                 normal.Normalize();

                 Vector3 BubblePos = SelectedHarvester.Position + normal * harvesterBubbleHeight;

                 Viewport vp = renderSys.Viewport;
                 Vector3 screenPos = vp.Project(BubblePos, scene.Camera.ProjectionMatrix,
                                                scene.Camera.ViewMatrix, Matrix.Identity);

                 Vector3 lp = vp.Project(BubblePos + tangx, scene.Camera.ProjectionMatrix,
                                                        scene.Camera.ViewMatrix, Matrix.Identity);

                 Vector3 rp = vp.Project(BubblePos - tangx, scene.Camera.ProjectionMatrix,
                                                        scene.Camera.ViewMatrix, Matrix.Identity);


                 float scale = 2.5f * Vector3.Distance(lp, rp) / 2;

                 Matrix trans = Matrix.Translation(-harvesterBubbleTex.Width / 2, -harvesterBubbleTex.Height / 2, 0) *
                            Matrix.Scaling(scale, scale, 1) * Matrix.Translation(screenPos.X, screenPos.Y, 0);


                 sprite.SetTransform(trans);
                 sprite.Draw(harvesterBubbleTex, 0, 0, ColorValue.White);
                 sprite.SetTransform(Matrix.Identity);


                 //属性,右下角显示
                 string storage = SelectedHarvester.GetProps().Storage.ToString();
                 string hp = SelectedHarvester.GetProps().HP.ToString();
                 string speed = SelectedHarvester.GetProps().Speed.ToString();

                 string info = "Storage: " + storage + "\n" + "HP: " + hp + "\n" +
                                "Speed: " + speed;
                 info = info.ToUpperInvariant();

                 Size size = f6.MeasureString(info);

                 f6.DrawString(sprite, info, 1150 - size.Width, 700 - size.Height, ColorValue.White);               



             }
         }



    }
}
