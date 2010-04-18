using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.GUI.Controls;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.Logic;
using Code2015.World;
using Code2015.BalanceSystem;

namespace Code2015.GUI
{
    enum AnimState
    {
        Inside,
        Outside,
        In,
        Out
    }
    class CityEditPanel : UIComponent
    {
        const int PanelX = 0;
        const int PanelY = 385;
        const int PanelWidth = 364;
        const int PanelHeight = 145;
        const float PopBaseSpeed = 10;


        GameScene scene;
        GameState gameLogic;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;

        RoundButton btnEduorg;
        RoundButton btnHosp;
        RoundButton btnOilref;
        RoundButton btnWood;

        Player player;

        AnimState state;

        float cx;

        CityObject selectCity;
        Texture background;




        public CityObject SelectedCity
        {
            get { return selectCity; }
            set
            {

                if (selectCity != value)
                {
                    selectCity = value;

                    if (selectCity != null)
                    {
                        if (selectCity.Owner != player)
                        {
                            selectCity = null;
                        }
                    }
                }

                if (selectCity != null && state != AnimState.Outside)
                {
                    state = AnimState.Out;
                }
                else if (selectCity == null && state != AnimState.Inside)
                {
                    state = AnimState.In;
                }
            }
        }

        public CityEditPanel(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.parent = parent;
            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;
            this.gameLogic = gamelogic;
            this.player = parent.HumanPlayer;

            FileLocation fl = FileSystem.Instance.Locate("ig_edit.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl);


            #region 教育机构按钮
            fl = FileSystem.Instance.Locate("ig_circle1.tex", GameFileLocs.GUI);
            Texture btnbg = UITextureManager.Instance.CreateInstance(fl);
            btnEduorg = new RoundButton();
            btnEduorg.Radius = 32;

            btnEduorg.Image = btnbg;
            btnEduorg.ImageMouseOver = btnbg;
            btnEduorg.IsValid = true;
            btnEduorg.Enabled = true;
            btnEduorg.ResizeImage = true;
            btnEduorg.MouseClick += this.EduBtn_Click;
            #endregion

            #region 医院按钮

            fl = FileSystem.Instance.Locate("ig_circle2.tex", GameFileLocs.GUI);
            btnbg = UITextureManager.Instance.CreateInstance(fl);
            btnHosp = new RoundButton();

            btnHosp.Radius = 32;

            btnHosp.Image = btnbg;
            btnHosp.ImageMouseOver = btnbg;
            btnHosp.IsValid = true;
            btnHosp.Enabled = true;
            btnHosp.ResizeImage = true;
            btnHosp.MouseClick += this.HospBtn_Click;
            #endregion

            #region 石油加工按钮

            fl = FileSystem.Instance.Locate("ig_circle3.tex", GameFileLocs.GUI);
            btnbg = UITextureManager.Instance.CreateInstance(fl);
            btnOilref = new RoundButton();
            btnOilref.Radius = 32;

            btnOilref.Image = btnbg;
            btnOilref.ImageMouseOver = btnbg;
            btnOilref.IsValid = true;
            btnOilref.Enabled = true;
            btnOilref.ResizeImage = true;
            btnOilref.MouseClick += this.OilBtn_Click;
            #endregion

            #region 木材厂按钮
            fl = FileSystem.Instance.Locate("ig_circle4.tex", GameFileLocs.GUI);
            btnbg = UITextureManager.Instance.CreateInstance(fl);

            btnWood = new RoundButton();
            btnWood.Radius = 32;

            btnWood.Image = btnbg;
            btnWood.ImageMouseOver = btnbg;
            btnWood.IsValid = true;
            btnWood.Enabled = true;
            btnWood.ResizeImage = true;
            btnWood.MouseClick += this.WoodBtn_Click;
            #endregion

            cx = -PanelWidth;
        }



        public override bool HitTest(int x, int y)
        {
            Rectangle rect = new Rectangle((int)cx, PanelY, PanelWidth, PanelHeight);
            return Control.IsInBounds(x, y, ref rect);
        }
        public override int Order
        {
            get { return 5; }
        }


        void EduBtn_Click(object sender, MouseButtonFlags btn)
        {
            selectCity.City.Add(gameLogic.PluginFactory.MakeEducationAgent());

        }
        void OilBtn_Click(object sender, MouseButtonFlags btn)
        {
            selectCity.City.Add(gameLogic.PluginFactory.MakeOilRefinary());

        }
        void WoodBtn_Click(object sender, MouseButtonFlags btn)
        {
            selectCity.City.Add(gameLogic.PluginFactory.MakeWoodFactory());
        }
        void HospBtn_Click(object sender, MouseButtonFlags btn)
        {
            selectCity.City.Add(gameLogic.PluginFactory.MakeHospital());

        }

        public override void Render(Sprite sprite)
        {
            if (state != AnimState.Inside)
            {
                sprite.Draw(background, (int)cx, PanelY, ColorValue.White);

                btnEduorg.Render(sprite);
                btnHosp.Render(sprite);
                btnOilref.Render(sprite);
                btnWood.Render(sprite);
            }
        }

        public override void UpdateInteract(GameTime time)
        {
            if (state == AnimState.Outside)
            {
                btnEduorg.Update(time);
                btnHosp.Update(time);
                btnOilref.Update(time);
                btnWood.Update(time);
            }
        }
        public override void Update(GameTime time)
        {
            if (state == AnimState.Out)
            {
                cx += (PanelWidth + PanelX - cx + PopBaseSpeed) * time.ElapsedGameTimeSeconds * 2;
                if (cx >= PanelX)
                {
                    cx = PanelX;
                    state = AnimState.Outside;
                }

            }
            else if (state == AnimState.In)
            {
                cx -= (PanelWidth + PanelX - cx + PopBaseSpeed) * time.ElapsedGameTimeSeconds * 2;
                if (cx < -PanelWidth)
                {
                    cx = -PanelWidth;
                    state = AnimState.Inside;
                }
            }

            btnEduorg.X = (int)(cx + 46);
            btnEduorg.Y = PanelY + 48;

            btnHosp.X = (int)(cx + 122);
            btnHosp.Y = PanelY + 67;

            btnOilref.X = (int)(cx + 200);
            btnOilref.Y = PanelY + 62;

            btnWood.X = (int)(cx + 277);
            btnWood.Y = PanelY + 41;


            bool enable;

            if (selectCity != null)
            {
                enable = selectCity.City.CanAddPlugins;
            }
            else
            {
                enable = false;
            }

            btnEduorg.Enabled = enable;
            btnHosp.Enabled = enable;
            btnOilref.Enabled = enable;
            btnWood.Enabled = enable;
        }
    }
}
