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
    class CityMeasure
    {
        Texture rightArrowR;
        Texture leftArrowR;
        Texture rightArrowG;
        Texture leftArrowG;


        ValueSmoother populationDir;
        ValueSmoother diseaseDir;
        ValueSmoother devDir;

        float lastPopulation;
        float lastDisease;
        float lastDev;



        ProgressBar devBar;
        ProgressBar popBar;
        ProgressBar disBar;
        ProgressBar supBar;


        City current;

        public City Current
        {
            get { return current; }
            set
            {
                if (!object.ReferenceEquals(current, value))
                {
                    current = value;
                    if (value != null)
                    {
                        populationDir.Clear();
                        diseaseDir.Clear();
                        devDir.Clear();
                    }
                }
            }
        }

        public CityMeasure(RenderSystem rs)
        {
            populationDir = new ValueSmoother(10);
            diseaseDir = new ValueSmoother(10);
            devDir = new ValueSmoother(10);

            FileLocation fl = FileSystem.Instance.Locate("ig_leftArrow_red.tex", GameFileLocs.GUI);
            leftArrowR = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_right_arrow_red.tex", GameFileLocs.GUI);
            rightArrowR = UITextureManager.Instance.CreateInstance(fl);


            fl = FileSystem.Instance.Locate("ig_leftArrow_green.tex", GameFileLocs.GUI);
            leftArrowG = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_rightArrow_green.tex", GameFileLocs.GUI);
            rightArrowG = UITextureManager.Instance.CreateInstance(fl);


            fl = FileSystem.Instance.Locate("ig_progressbar.tex", GameFileLocs.GUI);
            Texture prgBg = UITextureManager.Instance.CreateInstance(fl);

            devBar = new ProgressBar();
            devBar.X = 630;
            devBar.Y = 620;
            devBar.Height = 25;
            devBar.Width = 110;
            devBar.ProgressImage = prgBg;

            popBar = new ProgressBar();
            popBar.X = 630;
            popBar.Y = 650;
            popBar.Height = 25;
            popBar.Width = 110;
            popBar.ProgressImage = prgBg;

            supBar = new ProgressBar();
            supBar.X = 630;
            supBar.Y = 680;
            supBar.Height = 25;
            supBar.Width = 110;
            supBar.ProgressImage = prgBg;


            disBar = new ProgressBar();
            disBar.X = 630;
            disBar.Y = 710;
            disBar.Height = 25;
            disBar.Width = 110;
            disBar.ProgressImage = prgBg;
        }

        public int PopulationDirective
        {
            get;
            private set;
        }

        public int DiseaseDirective
        {
            get;
            private set;
        }
        public int DevelopmentDir
        {
            get;
            private set;
        }

        public float Development
        {
            get;
            private set;
        }
        public float Disease
        {
            get;
            private set;
        }
        public float Population
        {
            get;
            private set;
        }

        int ClassifyDir(float v)
        {
            v *= 100;
            int sig = Math.Sign(v);
            int result = 0;
            v = Math.Abs(v);
            if (v > float.Epsilon)
            {
                if (v > 1)
                {
                    if (v > 2)
                    {
                        result = v > 3 ? 4 : 3;
                        result *= sig;
                        return result;
                    }
                    else
                    {
                        result = 2;
                        result *= sig;
                        return result;
                    }
                }
                else
                {
                    result = 1;
                    result *= sig;
                    return result;
                }
            }
            return result;
        }

        public void Update(GameTime time)
        {
            if (current != null)
            {
                float dev = current.Development / 10000f;
                float pop = current.Population / CityGrade.GetRefPopulation(current.Size);
                float dis = current.Disease / 2;
                Development = MathEx.Saturate(dev);
                Population = MathEx.Saturate(pop);
                Disease = MathEx.Saturate(dis);


                populationDir.Add((pop - lastPopulation) * 100);
                diseaseDir.Add(dis - lastDisease);
                devDir.Add(dev - lastDev);

                float v = populationDir.Result;
                PopulationDirective = ClassifyDir(v);
                v = diseaseDir.Result;
                DiseaseDirective = ClassifyDir(v);
                v = devDir.Result;
                DevelopmentDir = ClassifyDir(v);

                lastPopulation = pop;
                lastDisease = dis;
                lastDev = dev;

            }
        }

        public void Render(Sprite sprite)
        {
            devBar.Value = Development;
            devBar.Render(sprite);

            popBar.Value = Population;
            popBar.Render(sprite);

            disBar.Value = Disease;
            disBar.Render(sprite);

            if (PopulationDirective < 0)
            {
                Rectangle rect;

                rect.Y = 680;
                rect.Width = 16;
                rect.Height = 16;
                for (int i = PopulationDirective; i < 0; i++)
                {
                    rect.X = 620 + i * 16;
                    sprite.Draw(leftArrowR, rect, ColorValue.White);
                }
            }

        }
    }
    class CityFactoryPluginMeasure
    {

        City current;

        RenderSystem renderSys;


        FastList<CityPlugin> woodFactory = new FastList<CityPlugin>();
        FastList<CityPlugin> oilRefinary = new FastList<CityPlugin>();
        FastList<CityPlugin> school = new FastList<CityPlugin>();
        FastList<CityPlugin> hospital = new FastList<CityPlugin>();



        public City Current
        {
            get { return current; }
            set
            {
                if (!object.ReferenceEquals(current, value))
                {
                    current = value;

                    UpdateInfo();
                }
            }
        }

        public void UpdateInfo()
        {
            woodFactory.Clear();
            oilRefinary.Clear();
            school.Clear();
            hospital.Clear();

            if (current != null)
            {
                for (int i = 0; i < current.PluginCount; i++)
                {
                    switch (current[i].TypeId)
                    {
                        case CityPluginTypeId.WoodFactory:
                            woodFactory.Add(current[i]);
                            break;
                        case CityPluginTypeId.OilRefinary:
                            oilRefinary.Add(current[i]);
                            break;
                        case CityPluginTypeId.BiofuelFactory:
                            oilRefinary.Add(current[i]);
                            break;
                        case CityPluginTypeId.EducationOrg:
                            school.Add(current[i]);
                            break;
                        case CityPluginTypeId.Hospital:
                            hospital.Add(current[i]);
                            break;
                    }

                }
            }

            woodFactory.Trim();
            oilRefinary.Trim();
            school.Trim();
            hospital.Trim();
        }


        public bool HasWoodFactory
        {
            get { return woodFactory.Count > 0; }
        }
        public bool HasOilRefinary
        {
            get { return oilRefinary.Count > 0; }
        }

        public bool HasHospital
        {
            get { return hospital.Count > 0; }
        }

        public bool HasSchool
        {
            get { return school.Count > 0; }
        }

        public CityFactoryPluginMeasure(RenderSystem rs)
        {
            renderSys = rs;
        }

        public void RenderWoodFactory(Sprite sprite, Font font)
        {
            if (current != null)
            {
                font.DrawString(sprite, "this city has " + woodFactory.Count.ToString() + "wood factorries", 630, 620, 14, DrawTextFormat.Left, -1);

                float averE = 0;
                for (int i = 0; i < woodFactory.Count; i++)
                {
                    averE += woodFactory[i].LRPConvRate;
                }
                averE /= (float)woodFactory.Count;

                font.DrawString(sprite, "effeciency: " + averE.ToString(), 630, 660, 14, DrawTextFormat.Left, -1);

            }
        }

        public void Update(GameTime time)
        {

        }
    }

    /// <summary>
    ///  用于显示游戏中已选择物体的信息
    /// </summary>
    class InGameUI2 : UIComponent
    {


        GameScene scene;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;
        Font font;
        Player player;

        GameState gameLogic;

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


        Texture selimglarge;
        //Texture earthGlow;


        RoundButton btnInfo;
        RoundButton btnEduorg;
        RoundButton btnHosp;
        RoundButton btnOilref;
        RoundButton btnWood;


        Button captureBtn;
        Button buildBtn;



        Texture co2meterBg;

        RtsCamera camera;


        ISelectableObject selected;
        CityObject city;
        CityMeasure cityMeasure;
        CityFactoryPluginMeasure pluginMeasure;

        Point selectedProjPos;
        bool isCapturable;


        public ISelectableObject SelectedObject
        {
            get { return selected; }
            set
            {
                if (!object.ReferenceEquals(selected, value))
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

                        if (city != null)
                        {
                            Vector3 ppos = renderSys.Viewport.Project(city.Position, camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);

                            selectedProjPos.X = (int)ppos.X;
                            selectedProjPos.Y = (int)ppos.Y;

                            isCapturable = city.CanCapture(player);
                            cityMeasure.Current = city.City;
                        }
                        else
                        {
                            cityMeasure.Current = null;
                        }
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

            FileLocation fl = FileSystem.Instance.Locate("def.fnt", GameFileLocs.GUI);
            font = FontManager.Instance.CreateInstance(renderSys, fl, "default");
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

            fl = FileSystem.Instance.Locate("ig_earthGlow.tex", GameFileLocs.GUI);
            //earthGlow = UITextureManager.Instance.CreateInstance(fl);

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




            #region 占领按钮
            fl = FileSystem.Instance.Locate("ig_btn_capture.tex", GameFileLocs.GUI);
            btnbg = UITextureManager.Instance.CreateInstance(fl);

            captureBtn = new Button();
            captureBtn.X = 440;
            captureBtn.Y = 630;

            captureBtn.Width = 256;
            captureBtn.Height = 128;
            captureBtn.Image = btnbg;
            captureBtn.ImageMouseOver = btnbg;
            captureBtn.IsValid = true;
            captureBtn.Enabled = true;

            captureBtn.MouseClick += this.CaptureBtn_Click;
            #endregion

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
            
        }

        void BuildBtn_Click(object sender, MouseButtonFlags btn)
        {
            switch (page)
            {
                case PanelPage.WoodFactory:
                    city.City.Add(gameLogic.PluginFactory.MakeWoodFactory());

                    pluginMeasure.UpdateInfo();
                    break;
            }
        }

        void CaptureBtn_Click(object sender, MouseButtonFlags btn)
        {
            if (btn == MouseButtonFlags.Left)
            {
                city.Capture.SetCapture(player, player.Area.GetNearestCity(city.City));
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
            for (int i = 0; i < scene.VisibleCityCount; i++)
            {
                CityObject cc = scene.GetVisibleCity(i);

                Vector3 tangy = PlanetEarth.GetTangentY(MathEx.Degree2Radian(cc.Longitude), MathEx.Degree2Radian(cc.Latitude));

                Vector3 ppos = renderSys.Viewport.Project(cc.Position - tangy * (CityStyleTable.CityRadius[(int)cc.Size] + 5),
                    camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);
                Point scrnPos = new Point((int)ppos.X, (int)ppos.Y);

                Size strSize = font.MeasureString(cc.Name, 20, DrawTextFormat.Center);

                //scrnPos.Y += strSize.Height;
                scrnPos.X -= strSize.Width / 2;

                font.DrawString(sprite, cc.Name, scrnPos.X, scrnPos.Y, 20, DrawTextFormat.Center, -1);
            }

            if (!object.ReferenceEquals(city, null))
            {
                switch (page)
                {
                    case PanelPage.Info:
                        sprite.Draw(yellowpanel, 401, 580, ColorValue.White);
                        sprite.Draw(ico_info, 394, 563, ColorValue.White);

                        font.DrawString(sprite, city.Name, 457, 600, 14, DrawTextFormat.Center, (int)ColorValue.Black.PackedValue);
                        font.DrawString(sprite, city.Size.ToString() + " City", 470, 672, 14, DrawTextFormat.Center, (int)ColorValue.Black.PackedValue);

                        if (object.ReferenceEquals(city.Owner, player))
                        {
                            cityMeasure.Render(sprite);
                        }
                        else if (!object.ReferenceEquals(city.Owner, null))
                        {
                            // 城市是别人的 
                        }
                        else
                        {
                            if (isCapturable)
                            {
                                captureBtn.Render(sprite);
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

                        buildBtn.Render(sprite);

                        break;
                    case PanelPage.WoodFactory:
                        sprite.Draw(bluePanel, 401, 580, ColorValue.White);
                        sprite.Draw(ico_wood, 394, 563, ColorValue.White);


                        font.DrawString(sprite,
                            pluginMeasure.HasWoodFactory ? "wood factory of " + city.Name : "the city have no wood factory",
                            457, 600, 14, DrawTextFormat.Center, (int)ColorValue.Black.PackedValue);

                        buildBtn.Render(sprite);

                        if (pluginMeasure.HasWoodFactory)
                        {
                            pluginMeasure.RenderWoodFactory(sprite, font);
                        }

                        break;
                    case PanelPage.OilRefinary:
                        sprite.Draw(greenPanel, 401, 580, ColorValue.White);
                        sprite.Draw(ico_oil, 394, 563, ColorValue.White);

                        font.DrawString(sprite,
                            pluginMeasure.HasSchool ? "oil producer of " + city.Name : "the city have no oil producer",
                            457, 600, 14, DrawTextFormat.Center, (int)ColorValue.Black.PackedValue);

                        buildBtn.Render(sprite);
                        break;
                    case PanelPage.Hospital:
                        sprite.Draw(redPanel, 401, 580, ColorValue.White);
                        sprite.Draw(ico_hosp, 394, 563, ColorValue.White);

                        font.DrawString(sprite,
                            pluginMeasure.HasSchool ? "hospital of " + city.Name : "the city have no hospital",
                            457, 600, 14, DrawTextFormat.Center, (int)ColorValue.Black.PackedValue);

                        buildBtn.Render(sprite);
                        break;
                }

                sprite.Draw(selimglarge, 785, 575, ColorValue.White);

                if (object.ReferenceEquals(city.Owner, player))
                {
                    btnInfo.Render(sprite);
                    btnEduorg.Render(sprite);
                    btnHosp.Render(sprite);
                    btnOilref.Render(sprite);
                    btnWood.Render(sprite);
                }
            }

            sprite.Draw(statusBar, 188, -8, ColorValue.White);
            sprite.Draw(co2meterBg, 437, -5, ColorValue.White);
        }

        public override void Update(GameTime time)
        {
            if (!object.ReferenceEquals(city, null))
            {
                if (object.ReferenceEquals(city.Owner, player))
                {
                    switch (page)
                    {
                        case PanelPage.Info:
                            if (city.Owner == null)
                            {
                                cityMeasure.Update(time);
                            }
                            break;
                        default:
                            if (city.Owner != null)
                            {
                                buildBtn.Update(time);
                            }
                            break;
                    }

                    btnInfo.Update(time);
                    btnEduorg.Update(time);
                    btnHosp.Update(time);
                    btnOilref.Update(time);
                    btnWood.Update(time);
                }
                else if (!object.ReferenceEquals(city.Owner, null))
                {
                    // 城市是别人的
                }
                else
                {
                    page = PanelPage.Info;
                    captureBtn.Update(time);
                }

            }

        }
    }
}
