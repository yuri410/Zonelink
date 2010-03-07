using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
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
            devBar.Y = 640;
            devBar.Height = 25;
            devBar.Width = 110;
            devBar.ProgressImage = prgBg;

            popBar = new ProgressBar();
            popBar.X = 630;
            popBar.Y = 680;
            popBar.Height = 25;
            popBar.Width = 110;
            popBar.ProgressImage = prgBg;

            supBar = new ProgressBar();
            supBar.X = 630;
            supBar.Y = 720;
            supBar.Height = 25;
            supBar.Width = 110;
            supBar.ProgressImage = prgBg;


            disBar = new ProgressBar();
            disBar.X = 630;
            disBar.Y = 760;
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

    /// <summary>
    ///  用于显示游戏中已选择物体的信息
    /// </summary>
    class InGameUI2 : UIComponent
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

        GameScene scene;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;
        Font font;
        Player player;

        //Texture cursor;
        Texture statusBar;
        Texture yellowpanel;
        Texture selimglarge;
        Texture earthGlow;
        Texture btninfo;
        Texture btneduorg;
        Texture btnhosp;
        Texture btnoilref;
        Texture btnwoodfac;


        RtsCamera camera;

        Texture[] earth;

        const int EarthFrameCount = 100;
        const float RoundTime = 30;

        int currentFrameIdx;

        float cycleTime;

        ISelectableObject selected;
        CityObject city;
        CityMeasure cityMeasure;

        Point selectedProjPos;
        bool isCapturable;


        Button captureBtn;

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

        public InGameUI2(Code2015 game, Game parent, GameScene scene)
        {
            this.parent = parent;
            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;

            this.camera = scene.Camera;
            this.player = parent.HumanPlayer;

            this.cityMeasure = new CityMeasure(renderSys);

            FileLocation fl = FileSystem.Instance.Locate("def.fnt", GameFileLocs.GUI);
            font = FontManager.Instance.CreateInstance(renderSys, fl, "default");
            ////读取纹理
            //fl = FileSystem.Instance.Locate("cursor.tex", GameFileLocs.GUI);
            //cursor = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_statusBar.tex", GameFileLocs.GUI);
            statusBar = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_yellow_panel.tex", GameFileLocs.GUI);
            yellowpanel = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_selimg_large_1.tex", GameFileLocs.GUI);
            selimglarge = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_earthGlow.tex", GameFileLocs.GUI);
            earthGlow = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_btn_info.tex", GameFileLocs.GUI);
            btninfo = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_btn_eduorg.tex", GameFileLocs.GUI);
            btneduorg = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_btn_hosp.tex", GameFileLocs.GUI);
            btnhosp = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_btn_oilref.tex", GameFileLocs.GUI);
            btnoilref = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_btn_woodfac.tex", GameFileLocs.GUI);
            btnwoodfac = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_btn_capture.tex", GameFileLocs.GUI);
            Texture helpBg = UITextureManager.Instance.CreateInstance(fl);
            captureBtn = new Button();
            captureBtn.X = 440;
            captureBtn.Y = 630;

            captureBtn.Width = 256;
            captureBtn.Height = 128;
            captureBtn.Image = helpBg;
            captureBtn.ImageMouseOver = helpBg;
            captureBtn.IsValid = true;
            captureBtn.Enabled = true;

            captureBtn.MouseClick += this.CaptureBtn_Click;


            earth = new Texture[EarthFrameCount];
            for (int i = 0; i < EarthFrameCount; i++)
            {
                fl = FileSystem.Instance.Locate("earth" + i.ToString("D4") + ".tex", GameFileLocs.Earth);

                earth[i] = UITextureManager.Instance.CreateInstance(fl);

            }
        }

        void CaptureBtn_Click(object sender, MouseButtonFlags btn)
        {
            if (btn == MouseButtonFlags.Left)
            {
                city.Capture.SetCapture(player, player.Area.GetNearestCity(city.City));
            }
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

            if (city != null)
            {

                sprite.Draw(yellowpanel, 401, 580, ColorValue.White);
                sprite.Draw(selimglarge, 785, 575, ColorValue.White);

                sprite.Draw(btninfo, 734, 590, ColorValue.White);
                sprite.Draw(btneduorg, 885, 531, ColorValue.White);
                sprite.Draw(btnhosp, 931, 672, ColorValue.White);
                sprite.Draw(btnoilref, 936, 595, ColorValue.White);
                sprite.Draw(btnwoodfac, 795, 528, ColorValue.White);




                font.DrawString(sprite, city.Name, 427, 600, 14, DrawTextFormat.Center, (int)ColorValue.Black.PackedValue);
                font.DrawString(sprite, city.Size.ToString() + " City", 470, 672, 14, DrawTextFormat.Center, (int)ColorValue.Black.PackedValue);

                if (object.ReferenceEquals(city.Owner, player))
                {
                    cityMeasure.Render(sprite);

                    //supBar.Value = MathEx.Saturate(city.City.SelfHRCRatio);
                    //disBar.Render(sprite);
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
            }

            //sprite.SetTransform(Matrix.Identity);
            //sprite.Draw(cursor, mousePosition.X, mousePosition.Y, ColorValue.White);
            
            sprite.Draw(statusBar, 130, 0, ColorValue.White);
            sprite.Draw(earth[currentFrameIdx], 448, -3, ColorValue.White);
            //if (currentFrameIdx >= EarthFrameCount)
            //    currentFrameIdx = 0;

            sprite.Draw(earthGlow, 423, -30, ColorValue.White);


        }

        public override void Update(GameTime time)
        {
            cycleTime += time.ElapsedGameTimeSeconds;
            if (cycleTime >= RoundTime)
                cycleTime = 0;

            currentFrameIdx = (int)(EarthFrameCount * (cycleTime / RoundTime));

            if (city != null)
            {
                captureBtn.Update(time);
                cityMeasure.Update(time);
            }

        }
    }
}
