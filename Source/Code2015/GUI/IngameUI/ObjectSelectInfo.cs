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
using Apoc3D.GUI.Controls;

namespace Code2015.GUI.IngameUI
{
    class ObjectSelectInfo : UIComponent
    {
        GameScene scene;
        GameState gameLogic;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;
        Player player;



        GameFontRuan f8;
        GameFontRuan f6;


        Texture statusHarvBackground;
        Texture statusHPBarBarEdge;
        Texture statusHPBarBarValue;
        Texture statusSTBarBarEdge;
        Texture statusSTBarBarValue;


        Texture statusOilBackground;
        Texture statusOilBarEdge;
        Texture statusOilBarValue;


        Texture statusGreenBackground;
        Texture statusGreenBarEdge;
        Texture statusGreenBarValue;


        Texture statusEmptyBackground;



        Texture harvHPBorader;
        Texture harvHPValue;
        Texture harvHPBarBackground;



        ISelectableObject selectedObject;
        NaturalResource selectedResource;
        Harvester selectedHarv;

        public ISelectableObject SelectedObject
        {
            get { return selectedObject; }
            set
            {
                if (selectedObject != value)
                {
                    selectedObject = value;

                    if (selectedObject != null)
                    {
                        selectedHarv = selectedObject as Harvester;
                        selectedResource = selectedObject as NaturalResource;
                    }
                    else
                    {
                        selectedHarv = null;
                        selectedResource = null;
                    }
                }
            }
        }

        public override int Order
        {
            get
            {
                return 49;
            }
        }


        public ObjectSelectInfo(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.scene = scene;
            this.renderSys = game.RenderSystem;
            this.player = parent.HumanPlayer;
            this.gameLogic = gamelogic;
            this.parent = parent;
            this.game = game;


            #region Harv
            FileLocation fl = FileSystem.Instance.Locate("nig_status_harv_bk.tex", GameFileLocs.GUI);
            statusHarvBackground = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_status_harv_hpbar_edge.tex", GameFileLocs.GUI);
            statusHPBarBarEdge = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_status_harv_hpbar_value.tex", GameFileLocs.GUI);
            statusHPBarBarValue = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_status_harv_stbar_edge.tex", GameFileLocs.GUI);
            statusSTBarBarEdge = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_status_harv_stbar_value.tex", GameFileLocs.GUI);
            statusSTBarBarValue = UITextureManager.Instance.CreateInstance(fl);

            #endregion


            fl = FileSystem.Instance.Locate("nig_status_empty.tex", GameFileLocs.GUI);
            statusEmptyBackground = UITextureManager.Instance.CreateInstance(fl);


            #region Green
            fl = FileSystem.Instance.Locate("nig_status_green_bk.tex", GameFileLocs.GUI);
            statusGreenBackground = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_status_green_bar_edge.tex", GameFileLocs.GUI);
            statusGreenBarEdge = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_status_green_bar_value.tex", GameFileLocs.GUI);
            statusGreenBarValue = UITextureManager.Instance.CreateInstance(fl);
            #endregion

            #region Oil

            fl = FileSystem.Instance.Locate("nig_status_oil_bk.tex", GameFileLocs.GUI);
            statusOilBackground = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_status_oil_bar_edge.tex", GameFileLocs.GUI);
            statusOilBarEdge = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_status_oil_bar_value.tex", GameFileLocs.GUI);
            statusOilBarValue = UITextureManager.Instance.CreateInstance(fl);

            #endregion

            fl = FileSystem.Instance.Locate("nig_harv_hp_edge.tex", GameFileLocs.GUI);
            harvHPBorader = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("nig_harv_hp_value.tex", GameFileLocs.GUI);
            harvHPValue = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("nig_harv_hp_bg.tex", GameFileLocs.GUI);
            harvHPBarBackground = UITextureManager.Instance.CreateInstance(fl);

            f8 = GameFontManager.Instance.FRuanEdged8;
            f6 = GameFontManager.Instance.FRuanEdged6;


        }

        public override bool HitTest(int x, int y)
        {
            Rectangle rect = new Rectangle(417, 586, 498, 134);
            return Control.IsInBounds(x, y, ref rect);
        }

        private void RenderSelectedHarvHP(Sprite sprite)
        {
            if (selectedHarv != null)
            {
                float radLng = (selectedHarv.Longitude);
                float radLat = (selectedHarv.Latitude);

                Vector3 tangy = PlanetEarth.GetTangentY(radLng, radLat);
                Vector3 tangx = PlanetEarth.GetTangentX(radLng, radLat);

                Vector3 cityNormal = PlanetEarth.GetNormal(radLng, radLat);
                cityNormal.Normalize();

                Vector3 hpPos = selectedHarv.Position + tangy * 50;

                Viewport vp = renderSys.Viewport;
                Vector3 screenPos = vp.Project(hpPos, scene.Camera.ProjectionMatrix,
                                               scene.Camera.ViewMatrix, Matrix.Identity);

                Vector3 lp = vp.Project(hpPos + tangx, scene.Camera.ProjectionMatrix,
                                                       scene.Camera.ViewMatrix, Matrix.Identity);

                Vector3 rp = vp.Project(hpPos - tangx, scene.Camera.ProjectionMatrix,
                                                       scene.Camera.ViewMatrix, Matrix.Identity);


                float scale = 1.25f * Vector3.Distance(lp, rp);

                Matrix trans = Matrix.Translation(-harvHPBarBackground.Width / 2, -harvHPBarBackground.Height / 2, 0) *
                           Matrix.Scaling(scale, scale, 1) * Matrix.Translation(screenPos.X, screenPos.Y, 0);



                int hpTexWidth = (int)(harvHPValue.Width * selectedHarv.HPRate);

                sprite.SetTransform(trans);
                sprite.Draw(harvHPBarBackground, 0, 0, ColorValue.White);
                sprite.Draw(harvHPValue, new Rectangle(0, 0, hpTexWidth, harvHPValue.Height),
                    new Rectangle(0, 0, hpTexWidth, harvHPValue.Height), ColorValue.White);
                sprite.Draw(harvHPBorader, 0, 0, ColorValue.White);
                //f6.DrawString(sprite, SelectedCity.Level.ToString(), 25, -16, ColorValue.White);

                sprite.SetTransform(Matrix.Identity);



            }
        }
        void RenderEmpty(Sprite sprite)
        {
            sprite.Draw(statusEmptyBackground, 405, 580, ColorValue.White);

        }
        void RenderHarv(Sprite sprite)
        {
            sprite.Draw(statusHarvBackground, 405, 580, ColorValue.White);

            
            int harvHPBarWidth = (int)(statusHPBarBarValue.Width * selectedHarv.HealthValue / selectedHarv.GetProps().HP);
            int harvSTBarWidth = (int)(statusHPBarBarValue.Width * selectedHarv.CurrentStorage / selectedHarv.GetProps().Storage);

            string hpInfo = ((int)selectedHarv.HealthValue).ToString() + "/" + ((int)selectedHarv.GetProps().HP).ToString();
            string stInfo = ((int)selectedHarv.CurrentStorage).ToString() + "/" + ((int)selectedHarv.GetProps().Storage).ToString();

            
            sprite.Draw(statusHarvBackground, 405, 580, ColorValue.White);

            sprite.Draw(statusHPBarBarValue, new Rectangle(588, 658, harvHPBarWidth, statusHPBarBarValue.Height),
                    new Rectangle(0, 0, harvHPBarWidth, statusHPBarBarValue.Height), ColorValue.White);
            sprite.Draw(statusHPBarBarEdge, 570, 655, ColorValue.White);


            sprite.Draw(statusSTBarBarValue, new Rectangle(699, 690, harvSTBarWidth, statusSTBarBarValue.Height),
                new Rectangle(0, 0, harvSTBarWidth, statusSTBarBarValue.Height), ColorValue.White);
            sprite.Draw(statusSTBarBarEdge, 685, 686, ColorValue.White);

            Matrix trans = Matrix.Scaling(0.8f, 0.8f, 1) * Matrix.Translation(new Vector3(676, 660, 0));
            sprite.SetTransform(trans);
            f8.DrawString(sprite, hpInfo, 0, 0, ColorValue.White);
            sprite.SetTransform(Matrix.Identity);
         
        }
        void RenderOil(Sprite sprite)
        {
            OilFieldObject oil = (OilFieldObject)selectedResource;
            sprite.Draw(statusOilBackground, 405, 580, ColorValue.White);
            {
                int hp = (int)oil.CurrentAmount;
                int hpFull = (int)(oil.MaxAmount);

                int hpTexWidth = (int)(statusOilBarValue.Width * (oil.CurrentAmount / oil.MaxAmount));
                sprite.Draw(statusOilBarValue, new Rectangle(580, 625, hpTexWidth, statusOilBarValue.Height),
                         new Rectangle(0, 0, hpTexWidth, statusOilBarValue.Height), ColorValue.White);
                sprite.Draw(statusOilBarEdge, 566, 625, ColorValue.White);

                string hpInfo = hp.ToString() + "/" + hpFull.ToString();

                Matrix trans = Matrix.Scaling(0.8f, 0.8f, 1) * Matrix.Translation(new Vector3(666, 634, 0));
                sprite.SetTransform(trans);
                f8.DrawString(sprite, hpInfo, 0, 0, ColorValue.White);
                sprite.SetTransform(Matrix.Identity);
            }
          
        }
        void RenderGreen(Sprite sprite)
        {
            sprite.Draw(statusGreenBackground, 405, 580, ColorValue.White);
            ForestObject oil = (ForestObject)selectedResource;
            {
                int hp = (int)oil.CurrentAmount;
                int hpFull = (int)(oil.MaxAmount);

                int hpTexWidth = (int)(statusGreenBarValue.Width * (oil.CurrentAmount / oil.MaxAmount));
                sprite.Draw(statusGreenBarValue, new Rectangle(598, 625, hpTexWidth, statusGreenBarValue.Height),
                         new Rectangle(0, 0, hpTexWidth, statusGreenBarValue.Height), ColorValue.White);
                sprite.Draw(statusGreenBarEdge, 580, 610, ColorValue.White);

                string hpInfo = hp.ToString() + "/" + hpFull.ToString();

                Matrix trans = Matrix.Scaling(0.8f, 0.8f, 1) * Matrix.Translation(new Vector3(676, 634, 0));
                sprite.SetTransform(trans);
                f8.DrawString(sprite, hpInfo, 0, 0, ColorValue.White);
                sprite.SetTransform(Matrix.Identity);
            }
        }

        public override void Render(Sprite sprite)
        {
            if (selectedHarv != null)
            {
                RenderHarv(sprite);
                RenderSelectedHarvHP(sprite);
            }
            else if (selectedResource != null)
            {
                if (selectedResource.Type == NaturalResourceType.Oil)
                    RenderOil(sprite);
                else
                    RenderGreen(sprite);
            }
            else
            {
                RenderEmpty(sprite);
            }
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
        }

        public override void UpdateInteract(GameTime time)
        {
            base.UpdateInteract(time);
        }
    }
}
