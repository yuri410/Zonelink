using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.Logic;
using Code2015.World;

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
        GameFontRuan fe8;

        #region 选择城市


        /// <summary>
        /// Currently selected city, Null if not selected
        /// </summary>
        private City selectedCity;
        private Harvester selectedHarv;
        private NaturalResource selectedResource;

        private ISelectableObject selectedObject;

        public ISelectableObject SelectedObject
        {
            get { return selectedObject; }
            set
            {
                selectedObject = value;

                selectedCity = selectedObject as City;
                selectedHarv = selectedObject as Harvester;
                selectedResource = selectedObject as NaturalResource;
            }
        }


        /// <summary>
        /// 泡泡漂浮在城市上空的高度
        /// </summary>
        private const float bubbleHeight = 150;

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



        private Texture harvesterBubbleTex;
        private const float harvesterBubbleHeight = 75;



        public SelectInfo(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.scene = scene;
            this.renderSys = game.RenderSystem;
            this.player = parent.HumanPlayer;
            this.gameLogic = gamelogic;
            this.parent = parent;
            this.game = game;
            this.f6 = GameFontManager.Instance.FRuanEdged6;
            fe8 = GameFontManager.Instance.FRuanEdged8;

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
             RenderSelectedCity(sprite);
             RenderSelectedHarvester(sprite);
             RenderSelectedResource(sprite);
         }

         public override void Update(GameTime time)
         {
             base.Update(time);

             
         }

         private void RenderSelectedCity(Sprite sprite)
         {
             if (selectedCity != null)
             {
                 float radLng = MathEx.Degree2Radian(selectedCity.Longitude);
                 float radLat = MathEx.Degree2Radian(selectedCity.Latitude);

                 Vector3 tangy = PlanetEarth.GetTangentY(radLng, radLat);
                 Vector3 tangx = PlanetEarth.GetTangentX(radLng, radLat);

                 Vector3 cityNormal = PlanetEarth.GetNormal(radLng, radLat);
                 cityNormal.Normalize();

                 Vector3 BubblePos = selectedCity.Position + tangy * City.CityRadius + cityNormal * bubbleHeight;

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

                 if (selectedCity.CityState == CityState.Sleeping)
                 {
                     Size s = fe8.MeasureString("ZZZ....");

                     fe8.DrawString(sprite, "ZZZ....", (int)screenPos.X - s.Width / 2, (int)screenPos.Y - s.Height / 2, ColorValue.White);
                 }

                 //属性,右下角显示
                 int hp = (int)selectedCity.HealthValue;
                 int hpFull = (int)(selectedCity.HealthValue / selectedCity.HPRate);

                 string hpInfo = "HP: " + hp.ToString() + "/" + hpFull.ToString() + "\n";
                 string developmentInfo = "Development: " + selectedCity.Name.ToString();
                 string info = hpInfo + developmentInfo;
                 info = info.ToUpperInvariant();

                 Size size = f6.MeasureString(info);

                 f6.DrawString(sprite, info, 1150 - size.Width, 700 - size.Height, ColorValue.White);



             }
         }

         private void RenderSelectedHarvester(Sprite sprite)
         {
             if (selectedHarv != null)
             {
                 float radLng = selectedHarv.Longitude;
                 float radLat = selectedHarv.Latitude;

                 Vector3 tangy = PlanetEarth.GetTangentY(radLng, radLat);
                 Vector3 tangx = PlanetEarth.GetTangentX(radLng, radLat);

                 Vector3 normal = PlanetEarth.GetNormal(radLng, radLat);
                 normal.Normalize();

                 Vector3 BubblePos = selectedHarv.Position + normal * harvesterBubbleHeight;

                 Viewport vp = renderSys.Viewport;
                 Vector3 screenPos = vp.Project(BubblePos, scene.Camera.ProjectionMatrix,
                                                scene.Camera.ViewMatrix, Matrix.Identity);

                 Vector3 lp = vp.Project(BubblePos + tangx, scene.Camera.ProjectionMatrix,
                                                        scene.Camera.ViewMatrix, Matrix.Identity);

                 Vector3 rp = vp.Project(BubblePos - tangx, scene.Camera.ProjectionMatrix,
                                                        scene.Camera.ViewMatrix, Matrix.Identity);


                 float scale = 2.5f * Vector3.Distance(lp, rp) / 2;

                 Matrix trans = Matrix.Translation(-harvesterBubbleTex.Width / 6, -harvesterBubbleTex.Height, 0) *
                            Matrix.Scaling(scale, scale, 1) * Matrix.Translation(screenPos.X, screenPos.Y, 0);


                 sprite.SetTransform(trans);
                 sprite.Draw(harvesterBubbleTex, 0, 0, ColorValue.White);
                 sprite.SetTransform(Matrix.Identity);


                 //属性,右下角显示
                 string storage = selectedHarv.GetProps().Storage.ToString();
                 string hp = selectedHarv.GetProps().HP.ToString();
                 string speed = selectedHarv.GetProps().Speed.ToString();

                 string info = "Storage: " + storage + "\n" + "HP: " + hp + "\n" +
                                "Speed: " + speed;
                 info = info.ToUpperInvariant();

                 Size size = f6.MeasureString(info);

                 f6.DrawString(sprite, info, 1150 - size.Width, 700 - size.Height, ColorValue.White);



             }
         }

         private void RenderSelectedResource(Sprite sprite)
         {
             if (selectedResource != null)
             {
                 float radLng = MathEx.Degree2Radian(selectedResource.Longitude);
                 float radLat = MathEx.Degree2Radian(selectedResource.Latitude);

                 Vector3 tangy = PlanetEarth.GetTangentY(radLng, radLat);
                 Vector3 tangx = PlanetEarth.GetTangentX(radLng, radLat);

                 Vector3 normal = PlanetEarth.GetNormal(radLng, radLat);
                 normal.Normalize();


                 //属性,右下角显示
                 string storage = ((int)selectedResource.MaxAmount).ToString("D");
                 string remaining = ((int)selectedResource.CurrentAmount).ToString("D");


                 string info = remaining + " / " + storage;

                 Size size = f6.MeasureString(info);

                 f6.DrawString(sprite, info, 1150 - size.Width, 700 - size.Height, ColorValue.White);

             }
         }

    }
}
