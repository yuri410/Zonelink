using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.GUI.Controls;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.BalanceSystem;
using Code2015.EngineEx;
using Code2015.GUI.Controls;
using Code2015.Logic;
using Code2015.World;
using Code2015.World.Screen;

namespace Code2015.GUI
{
    /// <summary>
    ///  用于显示游戏中已选择物体的信息
    /// </summary>
    class InGameUI2 : UIComponent
    {
        GameScene scene;
        GameState gameLogic;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;
        Font font;


        Font algerFont;
        Player player;

        Texture ico_wood;
        Texture ico_edu;
        Texture ico_hosp;
        Texture ico_oil;

        //侧边栏图标
        Texture ico_book;
        Texture ico_buger;
        Texture ico_pill;
        Texture ico_leaf;
        Texture ico_sidebar;

        RoundButton btnEduorg;
        RoundButton btnHosp;
        RoundButton btnOilref;
        RoundButton btnWood;


        CityInfoDisplay cityInfoDisplay;
        ResInfoDisplay resInfoDisplay;

        RtsCamera camera;

        CityLinkableMark linkArrow;

        ISelectableObject selected;
        CityObject city;
        NaturalResource resource;

        bool isCapturable;
        bool isPlayerCapturing;

        Point selectedProjPos;

        public ISelectableObject SelectedObject
        {
            get { return selected; }
            set
            {
                if (selected != value)
                {
                    if (selected != null)
                    {
                        selected.IsSelected = false;
                    }
                    selected = value;

                    if (selected != null)
                    {
                        selected.IsSelected = true;

                        city = selected as CityObject;

                        resource = null;

                        if (city != null)
                        {
                            Vector3 ppos = renderSys.Viewport.Project(city.Position, camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);

                            selectedProjPos.X = (int)ppos.X;
                            selectedProjPos.Y = (int)ppos.Y;

                            City cc = city.City;
                            //isCapturable = city.CanCapture(player);

                            CityObject[] nearby = new CityObject[cc.LinkableCityCount];

                            for (int i = 0; i < cc.LinkableCityCount; i++)
                            {
                                nearby[i] = cc.GetLinkableCity(i).Parent;
                            }

                            linkArrow.SetCity(city, nearby);

                            return;
                        }
                        else
                        {
                            linkArrow.SetCity(null, null);
                        }
                    }
                    else
                    {
                        city = null;
                    }
                }
            }
        }

        public InGameUI2(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.parent = parent;
            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;
            this.gameLogic = gamelogic;

            this.camera = scene.Camera;
            this.player = parent.HumanPlayer;

            FileLocation fl = FileSystem.Instance.Locate("def.fnt", GameFileLocs.GUI);
            font = FontManager.Instance.CreateInstance(renderSys, fl, "default");

            fl = FileSystem.Instance.Locate("Algerian.fnt", GameFileLocs.GUI);
            algerFont = FontManager.Instance.CreateInstance(renderSys, fl, "Algerian");


            this.cityInfoDisplay = new CityInfoDisplay(scene, renderSys, player);
            this.resInfoDisplay = new ResInfoDisplay(scene, renderSys);


            //侧边栏
            fl = FileSystem.Instance.Locate("ico_book.tex", GameFileLocs.GUI);
            ico_book = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ico_buger.tex", GameFileLocs.GUI);
            ico_buger = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ico_pill.tex", GameFileLocs.GUI);
            ico_pill = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ico_leaf.tex", GameFileLocs.GUI);
            ico_leaf = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_sidebar.tex", GameFileLocs.GUI);
            ico_sidebar = UITextureManager.Instance.CreateInstance(fl);


            #region 教育机构按钮
            fl = FileSystem.Instance.Locate("ig_btn_eduorg.tex", GameFileLocs.GUI);
            Texture btnbg = UITextureManager.Instance.CreateInstance(fl);
            btnEduorg = new RoundButton();
            btnEduorg.X = 885;
            btnEduorg.Y = 531;
            btnEduorg.Radius = 48;
            //btnEduorg.Width = 96;
            //btnEduorg.Height = 96;
            btnEduorg.Image = btnbg;
            btnEduorg.ImageMouseOver = btnbg;
            btnEduorg.IsValid = true;
            btnEduorg.Enabled = true;

            btnEduorg.MouseClick += this.EduBtn_Click;
            #endregion

            #region 医院按钮

            fl = FileSystem.Instance.Locate("ig_btn_hosp.tex", GameFileLocs.GUI);
            btnbg = UITextureManager.Instance.CreateInstance(fl);
            btnHosp = new RoundButton();
            btnHosp.X = 931;
            btnHosp.Y = 672;

            btnHosp.Radius = 48;
            //btnHosp.Width = 96;
            //btnHosp.Height = 96;
            btnHosp.Image = btnbg;
            btnHosp.ImageMouseOver = btnbg;
            btnHosp.IsValid = true;
            btnHosp.Enabled = true;

            btnHosp.MouseClick += this.HospBtn_Click;
            #endregion

            #region 石油加工按钮

            fl = FileSystem.Instance.Locate("ig_btn_oilref.tex", GameFileLocs.GUI);
            btnbg = UITextureManager.Instance.CreateInstance(fl);
            btnOilref = new RoundButton();
            btnOilref.X = 936;
            btnOilref.Y = 595;
            btnOilref.Radius = 48;
            //btnOilref.Width = 96;
            //btnOilref.Height = 96;
            btnOilref.Image = btnbg;
            btnOilref.ImageMouseOver = btnbg;
            btnOilref.IsValid = true;
            btnOilref.Enabled = true;

            btnOilref.MouseClick += this.OilBtn_Click;
            #endregion

            #region 木材厂按钮
            fl = FileSystem.Instance.Locate("ig_btn_woodfac.tex", GameFileLocs.GUI);
            btnbg = UITextureManager.Instance.CreateInstance(fl);

            btnWood = new RoundButton();
            btnWood.X = 795;
            btnWood.Y = 528;
            btnWood.Radius = 48;
            //btnWood.Width = 96;
            //btnWood.Height = 96;
            btnWood.Image = btnbg;
            btnWood.ImageMouseOver = btnbg;
            btnWood.IsValid = true;
            btnWood.Enabled = true;

            btnWood.MouseClick += this.WoodBtn_Click;
            #endregion

            fl = FileSystem.Instance.Locate("ico_oilref.tex", GameFileLocs.GUI);
            ico_oil = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ico_hosp.tex", GameFileLocs.GUI);
            ico_hosp = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ico_school.tex", GameFileLocs.GUI);
            ico_edu = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ico_woodfac.tex", GameFileLocs.GUI);
            ico_wood = UITextureManager.Instance.CreateInstance(fl);


            linkArrow = new CityLinkableMark(renderSys);
            scene.Scene.AddObjectToScene(linkArrow);
        }

        public CityInfoDisplay CityInfoDisplay
        {
            get { return cityInfoDisplay; }
        }

        //void BuildBtn_Click(object sender, MouseButtonFlags btn)
        //{
        //    switch (page)
        //    {
        //        case PanelPage.WoodFactory:
        //            city.City.Add(gameLogic.PluginFactory.MakeWoodFactory());

        //            pluginMeasure.UpdateInfo();
        //            break;
        //        case PanelPage.OilRefinary:
        //            city.City.Add(gameLogic.PluginFactory.MakeOilRefinary());

        //            pluginMeasure.UpdateInfo();
        //            break;
        //    }
        //}
        void InfoBtn_Click(object sender, MouseButtonFlags btn)
        {
            //page = PanelPage.Info;
        }
        void EduBtn_Click(object sender, MouseButtonFlags btn)
        {
            //page = PanelPage.EduOrg;
        }
        void OilBtn_Click(object sender, MouseButtonFlags btn)
        {
            //page = PanelPage.OilRefinary;
        }
        void WoodBtn_Click(object sender, MouseButtonFlags btn)
        {
            //page = PanelPage.WoodFactory;
        }
        void HospBtn_Click(object sender, MouseButtonFlags btn)
        {
            //page = PanelPage.Hospital;
        }


        public override void Render(Sprite sprite)
        {
            cityInfoDisplay.Render(sprite);
            resInfoDisplay.Render(sprite);

            #region 渲染城市信息
            if (city != null)
            {
                sprite.Draw(ico_edu, 394, 563, ColorValue.White);
                sprite.Draw(ico_wood, 394, 563, ColorValue.White);
                sprite.Draw(ico_oil, 394, 563, ColorValue.White);
                sprite.Draw(ico_hosp, 394, 563, ColorValue.White);


                if (city.Owner == player)
                {
                    btnEduorg.Render(sprite);
                    btnHosp.Render(sprite);
                    btnOilref.Render(sprite);
                    btnWood.Render(sprite);
                }
            }
            #endregion


            sprite.Draw(ico_sidebar, 0, 168, ColorValue.White);
            sprite.Draw(ico_buger, -3, 197, ColorValue.White);
            sprite.Draw(ico_leaf, -3, 275, ColorValue.White);
            sprite.Draw(ico_book, -3, 358, ColorValue.White);
            sprite.Draw(ico_pill, -3, 429, ColorValue.White);

            Dictionary<Player, float> list = gameLogic.SLGWorld.EnergyStatus.GetCarbonRatios();

            int yy = 60;
            foreach (KeyValuePair<Player, float> e in list)
            {
                font.DrawString(sprite, e.Key.Name + " CO2: " + e.Value.ToString("P"), 0, yy, 25, DrawTextFormat.Center, (int)ColorValue.White.PackedValue);
                yy += 30;
            }

        }

        public bool HitTest(int x, int y)
        {
            if (city != null || resource != null)
            {
                if (y > 595 && x > 420)
                {
                    return true;
                }
                //if (y > 168 && x < 155)
                //{
                //    return true;
                //}
            }
            return false;
        }

        public void Interact(GameTime time)
        {
            if (city != null)
            {
                if (city.IsCaptured)
                {
                    if (city.Owner == player)
                    {
                        btnEduorg.Update(time);
                        btnHosp.Update(time);
                        btnOilref.Update(time);
                        btnWood.Update(time);
                    }
                }
            }
        }

        public override void Update(GameTime time)
        {
            cityInfoDisplay.Update(time);
            resInfoDisplay.Update(time);

            #region 城市
            if (city != null)
            {
                if (city.IsCaptured)
                {
                    if (city.Owner == player)
                    {
                        
                    }
                }
                else
                {
                    isCapturable = city.CanCapture(player);
                    isPlayerCapturing = city.IsPlayerCapturing(player);
                }
            }
            #endregion

        }
    }
}
