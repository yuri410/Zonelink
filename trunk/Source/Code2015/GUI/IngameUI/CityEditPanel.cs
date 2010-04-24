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
using Code2015.Logic;
using Code2015.World;

namespace Code2015.GUI
{
    enum AnimState
    {
        Inside,
        Outside,
        In,
        Out
    }
    class CityPluginPanel : UIComponent
    {
        GameScene scene;
        GameState gameLogic;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;

        public CityPluginPanel(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.parent = parent;
            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;
            this.gameLogic = gamelogic;


        }
    }

    class CityEditPanel : UIComponent
    {
        const int PanelX = 0;
        const int PanelY = 300;
        const int PanelWidth = 380;
        const int PanelHeight = 156;
        const float PopBaseSpeed = 10;


        const int Con1X = 3;
        const int Con1Y = 358 - PanelY;
        const int Con2X = 97;
        const int Con2Y = 338 - PanelY;
        const int Con3X = 190;
        const int Con3Y = 360 - PanelY;
        const int Con4X = 285;
        const int Con4Y = 347 - PanelY;
        const int HotRadius = 92 / 2;


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
        Texture eduHover;
        Texture hospHover;
        Texture oilHover;
        Texture woodHover;



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

            FileLocation fl = FileSystem.Instance.Locate("ig_construct.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_construct_hover4.tex", GameFileLocs.GUI);
            eduHover = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_construct_hover2.tex", GameFileLocs.GUI);
            hospHover = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_construct_hover3.tex", GameFileLocs.GUI);
            oilHover = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_construct_hover1.tex", GameFileLocs.GUI);
            woodHover = UITextureManager.Instance.CreateInstance(fl);

            #region 教育机构按钮
            btnEduorg = new RoundButton();
            btnEduorg.Radius = HotRadius;

            btnEduorg.IsValid = true;
            btnEduorg.Enabled = true;
            btnEduorg.ResizeImage = true;
            btnEduorg.MouseClick += this.EduBtn_Click;
            #endregion

            #region 医院按钮
            btnHosp = new RoundButton();

            btnHosp.Radius = HotRadius;
            btnHosp.IsValid = true;
            btnHosp.Enabled = true;
            btnHosp.ResizeImage = true;
            btnHosp.MouseClick += this.HospBtn_Click;
            #endregion

            #region 石油加工按钮
            btnOilref = new RoundButton();
            btnOilref.Radius = HotRadius;

            btnOilref.IsValid = true;
            btnOilref.Enabled = true;
            btnOilref.ResizeImage = true;
            btnOilref.MouseClick += this.OilBtn_Click;
            #endregion

            #region 木材厂按钮
            btnWood = new RoundButton();
            btnWood.Radius = HotRadius;

            btnWood.IsValid = true;
            btnWood.Enabled = true;
            btnWood.ResizeImage = true;
            btnWood.MouseClick += this.WoodBtn_Click;
            #endregion

            cx = -PanelWidth;
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
            {
                background.Dispose();

                eduHover.Dispose();
                hospHover.Dispose();
                oilHover.Dispose();
                woodHover.Dispose();
            }
            eduHover = null;
            woodHover = null;
            oilHover = null;
            hospHover = null;
            background = null;

            selectCity = null;
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

                if (btnEduorg.IsMouseOver)
                {
                    sprite.Draw(eduHover, (int)cx, PanelY, ColorValue.White);
                }
                else if (btnHosp.IsMouseOver)
                {
                    sprite.Draw(hospHover, (int)cx, PanelY, ColorValue.White);
                }
                else if (btnOilref.IsMouseOver)
                {
                    sprite.Draw(oilHover, (int)cx, PanelY, ColorValue.White);
                }
                else if (btnWood.IsMouseOver)
                {
                    sprite.Draw(woodHover, (int)cx, PanelY, ColorValue.White);
                }
                else
                {
                    sprite.Draw(background, (int)cx, PanelY, ColorValue.White);
                }
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

            btnEduorg.X = (int)(cx + Con4X);
            btnEduorg.Y = PanelY + Con4Y;

            btnHosp.X = (int)(cx + Con2X);
            btnHosp.Y = PanelY + Con2Y;

            btnOilref.X = (int)(cx + Con3X);
            btnOilref.Y = PanelY + Con3Y;

            btnWood.X = (int)(cx + Con1X);
            btnWood.Y = PanelY + Con1Y;


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
