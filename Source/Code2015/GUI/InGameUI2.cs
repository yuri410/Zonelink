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
    ///  表示页面的类型
    /// </summary>
    enum PanelPage
    {
        Info,
        WoodFactory,
        OilRefinary,
        Hospital,
        EduOrg,

    }


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

        //Texture cursor;
        Texture statusBar;

        PanelPage page;

        Texture yellowpanel;
        Texture bluePanel;
        Texture greenPanel;
        Texture redPanel;

        Texture ico_wood;
        Texture ico_info;
        Texture ico_edu;
        Texture ico_hosp;
        Texture ico_oil;

        //侧边栏图标
        Texture ico_book;
        Texture ico_buger;
        Texture ico_pill;
        Texture ico_leaf;
        Texture ico_sidebar;

        Texture selimglarge;



        RoundButton btnInfo;
        RoundButton btnEduorg;
        RoundButton btnHosp;
        RoundButton btnOilref;
        RoundButton btnWood;


        //Button captureBtn;
        Button buildBtn;



        Texture co2meterBg;
        Texture co2tl;
        Texture co2tr;
        Texture co2bl;
        Texture co2br;

        Texture co2tl_blue;
        Texture co2tr_blue;
        Texture co2bl_blue;
        Texture co2br_blue;

        CityInfoDisplay cityInfoDisplay;
        ResInfoDisplay resInfoDisplay;

        RtsCamera camera;


        ISelectableObject selected;
        CityObject city;
        NaturalResource resource;
        CityMeasure cityMeasure;
        CityFactoryPluginMeasure pluginMeasure;
        ResourceMeasure resourceMeasure;

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

                        cityMeasure.Current = null;
                        pluginMeasure.Current = null;
                        resource = null;
                        resourceMeasure.Current = null;

                        if (city != null)
                        {
                            Vector3 ppos = renderSys.Viewport.Project(city.Position, camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);

                            selectedProjPos.X = (int)ppos.X;
                            selectedProjPos.Y = (int)ppos.Y;

                            //isCapturable = city.CanCapture(player);
                            cityMeasure.Current = city.City;
                            pluginMeasure.Current = city.City;
                            return;
                        }

                        OilFieldObject oilObj = selected as OilFieldObject;
                        if (oilObj != null)
                        {
                            resource = oilObj.OilField;
                            resourceMeasure.Current = resource;
                            return;
                        }

                        ForestObject forestObj = selected as ForestObject;
                        if (forestObj != null)
                        {
                            resource = forestObj.Forest;
                            resourceMeasure.Current = resource;
                            return;
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

            this.cityMeasure = new CityMeasure(renderSys);
            this.pluginMeasure = new CityFactoryPluginMeasure(renderSys);
            this.resourceMeasure = new ResourceMeasure();

            FileLocation fl = FileSystem.Instance.Locate("def.fnt", GameFileLocs.GUI);
            font = FontManager.Instance.CreateInstance(renderSys, fl, "default");

            fl = FileSystem.Instance.Locate("Algerian.fnt", GameFileLocs.GUI);
            algerFont = FontManager.Instance.CreateInstance(renderSys, fl, "Algerian");


            this.cityInfoDisplay = new CityInfoDisplay(scene, renderSys, player);
            this.resInfoDisplay = new ResInfoDisplay(scene, renderSys);
            ////读取纹理
            //fl = FileSystem.Instance.Locate("cursor.tex", GameFileLocs.GUI);
            //cursor = UITextureManager.Instance.CreateInstance(fl);


            fl = FileSystem.Instance.Locate("ig_statusBar.tex", GameFileLocs.GUI);
            statusBar = UITextureManager.Instance.CreateInstance(fl);


            fl = FileSystem.Instance.Locate("ig_yellow_panel.tex", GameFileLocs.GUI);
            yellowpanel = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_blue_panel.tex", GameFileLocs.GUI);
            bluePanel = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_green_panel.tex", GameFileLocs.GUI);
            greenPanel = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_red_panel.tex", GameFileLocs.GUI);
            redPanel = UITextureManager.Instance.CreateInstance(fl);



            fl = FileSystem.Instance.Locate("ig_selimg_large_1.tex", GameFileLocs.GUI);
            selimglarge = UITextureManager.Instance.CreateInstance(fl);

            //fl = FileSystem.Instance.Locate("ig_earthGlow.tex", GameFileLocs.GUI);
            //earthGlow = UITextureManager.Instance.CreateInstance(fl);

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



            #region 信息按钮
            fl = FileSystem.Instance.Locate("ig_btn_info.tex", GameFileLocs.GUI);
            Texture btnbg = UITextureManager.Instance.CreateInstance(fl);
            btnInfo = new RoundButton();
            btnInfo.X = 734;
            btnInfo.Y = 590;
            btnInfo.Radius = 48;
            //btnInfo.Width = 96;
            //btnInfo.Height = 96;
            btnInfo.Image = btnbg;
            btnInfo.ImageMouseOver = btnbg;
            btnInfo.IsValid = true;
            btnInfo.Enabled = true;

            btnInfo.MouseClick += this.InfoBtn_Click;
            #endregion

            #region 教育机构按钮
            fl = FileSystem.Instance.Locate("ig_btn_eduorg.tex", GameFileLocs.GUI);
            btnbg = UITextureManager.Instance.CreateInstance(fl);
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

            fl = FileSystem.Instance.Locate("ico_info.tex", GameFileLocs.GUI);
            ico_info = UITextureManager.Instance.CreateInstance(fl);


            #region 建造按钮
            fl = FileSystem.Instance.Locate("btn_build.tex", GameFileLocs.GUI);
            btnbg = UITextureManager.Instance.CreateInstance(fl);

            buildBtn = new Button();
            buildBtn.X = 425;
            buildBtn.Y = 630;

            buildBtn.Width = 200;
            buildBtn.Height = 125;
            buildBtn.Image = btnbg;
            buildBtn.ImageMouseOver = btnbg;
            buildBtn.IsValid = true;
            buildBtn.Enabled = true;

            buildBtn.MouseClick += BuildBtn_Click;
            #endregion

            fl = FileSystem.Instance.Locate("ig_co2_meter.tex", GameFileLocs.GUI);
            co2meterBg = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_co2_yellow_tl.tex", GameFileLocs.GUI);
            co2tl = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ig_co2_yellow_tr.tex", GameFileLocs.GUI);
            co2tr = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ig_co2_yellow_bl.tex", GameFileLocs.GUI);
            co2bl = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ig_co2_yellow_br.tex", GameFileLocs.GUI);
            co2br = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_co2_blue_tl.tex", GameFileLocs.GUI);
            co2tl_blue = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ig_co2_blue_tr.tex", GameFileLocs.GUI);
            co2tr_blue = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ig_co2_blue_bl.tex", GameFileLocs.GUI);
            co2bl_blue = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ig_co2_blue_br.tex", GameFileLocs.GUI);
            co2br_blue = UITextureManager.Instance.CreateInstance(fl);

        }

        public CityInfoDisplay CityInfoDisplay
        {
            get { return cityInfoDisplay; }
        }

        void BuildBtn_Click(object sender, MouseButtonFlags btn)
        {
            switch (page)
            {
                case PanelPage.WoodFactory:
                    city.City.Add(gameLogic.PluginFactory.MakeWoodFactory());

                    pluginMeasure.UpdateInfo();
                    break;
                case PanelPage.OilRefinary:
                    city.City.Add(gameLogic.PluginFactory.MakeOilRefinary());

                    pluginMeasure.UpdateInfo();
                    break;
            }
        }
        void InfoBtn_Click(object sender, MouseButtonFlags btn)
        {
            page = PanelPage.Info;
        }
        void EduBtn_Click(object sender, MouseButtonFlags btn)
        {
            page = PanelPage.EduOrg;
        }
        void OilBtn_Click(object sender, MouseButtonFlags btn)
        {
            page = PanelPage.OilRefinary;
        }
        void WoodBtn_Click(object sender, MouseButtonFlags btn)
        {
            page = PanelPage.WoodFactory;
        }
        void HospBtn_Click(object sender, MouseButtonFlags btn)
        {
            page = PanelPage.Hospital;
        }


        public override void Render(Sprite sprite)
        {
            
            cityInfoDisplay.Render(sprite);
            resInfoDisplay.Render(sprite);

            #region 渲染城市信息
            if (city != null)
            {
                switch (page)
                {
                    case PanelPage.Info:
                        sprite.Draw(yellowpanel, 401, 580, ColorValue.White);
                        sprite.Draw(ico_info, 394, 563, ColorValue.White);

                        algerFont.DrawString(sprite, city.Name, 457, 600, 14, DrawTextFormat.Center, (int)ColorValue.Black.PackedValue);
                        algerFont.DrawString(sprite, city.Size.ToString() + " City", 615, 600, 14, DrawTextFormat.Center, (int)ColorValue.Black.PackedValue);

                        if (city.IsCaptured)
                        {
                            if (city.Owner != player)
                            {
                                // 城市是别人的 
                            }
                            else
                            {
                                cityMeasure.Render(sprite, algerFont);//画城市具体信息

                            }
                        }
                        else
                        {
                            //if (isCapturable)
                            //{
                            //    captureBtn.Render(sprite);
                            //}
                            if (isPlayerCapturing)
                            {
                                font.DrawString(sprite, "Helping...", 470, 692, 14,
                                   DrawTextFormat.Center, (int)ColorValue.Black.PackedValue);


                            }
                            else
                            {
                                font.DrawString(sprite, "This city is too far to help it.", 470, 692, 14,
                                    DrawTextFormat.Center, (int)ColorValue.Black.PackedValue);
                            }
                        }
                        break;
                    case PanelPage.EduOrg:
                        sprite.Draw(redPanel, 401, 580, ColorValue.White);
                        sprite.Draw(ico_edu, 394, 563, ColorValue.White);

                        font.DrawString(sprite,
                            pluginMeasure.HasSchool ? "education org of " + city.Name : "the city have no edu org",
                            457, 600, 14, DrawTextFormat.Center, (int)ColorValue.Black.PackedValue);

                        if (city.City.CanAddPlugins)
                            buildBtn.Render(sprite);

                        break;
                    case PanelPage.WoodFactory:
                        sprite.Draw(bluePanel, 401, 580, ColorValue.White);
                        sprite.Draw(ico_wood, 394, 563, ColorValue.White);


                        font.DrawString(sprite,
                            pluginMeasure.HasWoodFactory ? "wood factory of " + city.Name : "the city have no wood factory",
                            457, 600, 14, DrawTextFormat.Center, (int)ColorValue.Black.PackedValue);

                        if (city.City.CanAddPlugins)
                            buildBtn.Render(sprite);

                        if (pluginMeasure.HasWoodFactory)
                            pluginMeasure.RenderWoodFactory(sprite, font);


                        break;
                    case PanelPage.OilRefinary:
                        sprite.Draw(greenPanel, 401, 580, ColorValue.White);
                        sprite.Draw(ico_oil, 394, 563, ColorValue.White);

                        font.DrawString(sprite,
                            pluginMeasure.HasSchool ? "oil producer of " + city.Name : "the city have no oil producer",
                            457, 600, 14, DrawTextFormat.Center, (int)ColorValue.Black.PackedValue);

                        if (city.City.CanAddPlugins)
                            buildBtn.Render(sprite);

                        if (pluginMeasure.HasOilRefinary)
                            pluginMeasure.RenderOil(sprite, font);

                        break;
                    case PanelPage.Hospital:
                        sprite.Draw(redPanel, 401, 580, ColorValue.White);
                        sprite.Draw(ico_hosp, 394, 563, ColorValue.White);

                        font.DrawString(sprite,
                            pluginMeasure.HasSchool ? "hospital of " + city.Name : "the city have no hospital",
                            457, 600, 14, DrawTextFormat.Center, (int)ColorValue.Black.PackedValue);

                        if (city.City.CanAddPlugins)
                            buildBtn.Render(sprite);
                        break;
                }

                sprite.Draw(selimglarge, 785, 575, ColorValue.White);

                if (city.Owner == player)
                {
                    btnInfo.Render(sprite);
                    btnEduorg.Render(sprite);
                    btnHosp.Render(sprite);
                    btnOilref.Render(sprite);
                    btnWood.Render(sprite);
                }
            }
            #endregion

            #region 资源信息
            if (resource != null)
            {
                sprite.Draw(greenPanel, 401, 580, ColorValue.White);

                resourceMeasure.Render(sprite, font);
            }

            #endregion

            sprite.Draw(statusBar, 188, -8, ColorValue.White);
            sprite.Draw(co2meterBg, 447, -1, ColorValue.White);
            sprite.Draw(co2tl, 459, 11, ColorValue.White);
            sprite.Draw(co2tr, 517, 11, ColorValue.White);
            sprite.Draw(co2bl, 459, 69, ColorValue.White);
            sprite.Draw(co2br, 517, 69, ColorValue.White);

            sprite.Draw(co2br_blue, 517, 69, ColorValue.White);


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
                        btnInfo.Update(time);
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
                        switch (page)
                        {
                            case PanelPage.Info:

                                cityMeasure.Update(time);

                                break;
                            default:

                                buildBtn.Update(time);

                                break;
                        }
                    }
                }
                else
                {
                    page = PanelPage.Info;
                    isCapturable = city.CanCapture(player);
                    isPlayerCapturing = city.IsPlayerCapturing(player);


                }
            }
            #endregion

            #region 资源
            if (resource != null)
            {
                resourceMeasure.Update(time);
            }
            #endregion
        }
    }
}
