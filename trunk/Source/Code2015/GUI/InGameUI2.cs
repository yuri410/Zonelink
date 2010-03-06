using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.GUI.Controls;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.World;
using Code2015.World.Screen;
using Code2015.Logic;

namespace Code2015.GUI
{
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
                city.Capture.SetCapture(player);
            }
        }

        public override void Render(Sprite sprite)
        {
            for (int i = 0; i < scene.VisibleCityCount; i++)
            {
                CityObject cc = scene.GetVisibleCity(i);
                Vector3 ppos = renderSys.Viewport.Project(cc.Position, camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);
                Point scrnPos = new Point((int)ppos.X, (int)ppos.Y);

                font.DrawString(sprite, cc.Name, scrnPos.X, scrnPos.Y, 34, DrawTextFormat.Center, -1);
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
            }

        }
    }
}
