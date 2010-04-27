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
using Code2015.GUI.Controls;

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
        enum SelectedPluginType
        {
            None,
            Wood,
            Oil,
            Hospital,
            Education
        }

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

        float blendWeight1;
        float blendWeight2;
        float blendWeight3;
        float blendWeight4;

        GameScene scene;
        GameState gameLogic;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;

        GameFont f18;
        GameFont f14;

        RoundButton btnEduorg;
        RoundButton btnHosp;
        RoundButton btnOilref;
        RoundButton btnWood;

        Button build;
        Button sell;

        Player player;

        AnimState state;
        AnimState state2;

        float cx;
        float cx2;

        CityObject anySelCity;
        CityObject selectCity;

        #region 左
        Texture background;
        Texture eduHover;
        Texture hospHover;
        Texture oilHover;
        Texture woodHover;

        Texture infobg;
        Texture costbg;
        #endregion

        #region 文本中
        Texture woodSign;
        Texture oilSign;
        #endregion



        SelectedPluginType selectedType;




        private SelectedPluginType SelectedType
        {
            get { return selectedType; }
            set
            {
                if (selectedType != value)
                {
                    selectedType = value;
                    if (selectedType == SelectedPluginType.None)
                        state2 = AnimState.In;
                    else
                        state2 = AnimState.Out;
                }                
            }
        }

        Texture cityInfoBg;
        Texture woodResSign;
        Texture oilResSign;
        Texture foodResSign;

        ProgressBar cityDev;

        Texture smCitySign;
        Texture medCitySign;
        Texture lgCitySign;

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
                anySelCity = value;

                if (selectCity != null && state != AnimState.Outside)
                {
                    state = AnimState.Out;
                }
                else if (selectCity == null && state != AnimState.Inside)
                {
                    state = AnimState.In;
                    SelectedType = SelectedPluginType.None;
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

            fl = FileSystem.Instance.Locate("ig_info.tex", GameFileLocs.GUI);
            infobg = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_cost.tex", GameFileLocs.GUI);
            costbg = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_sign_oil_sm.tex", GameFileLocs.GUI);
            oilSign = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_sign_wood_sm.tex", GameFileLocs.GUI);
            woodSign = UITextureManager.Instance.CreateInstance(fl);

            #region 建造变卖
            
            build = new Button();
            fl = FileSystem.Instance.Locate("ig_btn_build_down.tex", GameFileLocs.GUI);
            build.ImageMouseDown = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_btn_build_mon.tex", GameFileLocs.GUI);
            build.ImageMouseOver = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_btn_build.tex", GameFileLocs.GUI);
            build.Image = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_btn_build_disabled.tex", GameFileLocs.GUI);
            build.ImageDisabled = UITextureManager.Instance.CreateInstance(fl);

            build.Width = 64;
            build.Height = 35;
            build.Enabled = true;
            build.IsValid = true;

            build.MouseClick += Build_Click;


            sell = new Button();
            fl = FileSystem.Instance.Locate("ig_btn_sell_down.tex", GameFileLocs.GUI);
            sell.ImageMouseDown = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_btn_sell_mon.tex", GameFileLocs.GUI);
            sell.ImageMouseOver = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_btn_sell.tex", GameFileLocs.GUI);
            sell.Image = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_btn_sell_disabled.tex", GameFileLocs.GUI);
            sell.ImageDisabled = UITextureManager.Instance.CreateInstance(fl);

            sell.Width = 55;
            sell.Height = 34;
            sell.Enabled = true;
            sell.IsValid = true;

            sell.MouseClick += Sell_Click;

            #endregion



            fl = FileSystem.Instance.Locate("ig_name.tex", GameFileLocs.GUI);
            cityInfoBg = UITextureManager.Instance.CreateInstance(fl);
           
            fl = FileSystem.Instance.Locate("ig_sign_oil.tex", GameFileLocs.GUI);
            oilResSign = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_sign_wood.tex", GameFileLocs.GUI);
            woodResSign = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_sign_food.tex", GameFileLocs.GUI);
            foodResSign = UITextureManager.Instance.CreateInstance(fl);


            fl = FileSystem.Instance.Locate("ig_sign_small.tex", GameFileLocs.GUI);
            smCitySign = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_sign_medium.tex", GameFileLocs.GUI);
            medCitySign = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_sign_large.tex", GameFileLocs.GUI);
            lgCitySign = UITextureManager.Instance.CreateInstance(fl);






            cityDev = new ProgressBar();
            cityDev.X = 386;
            cityDev.Y = 637;
            fl = FileSystem.Instance.Locate("ig_namebar.tex", GameFileLocs.GUI);
            cityDev.ProgressImage = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_namebar1.tex", GameFileLocs.GUI);
            cityDev.Background = UITextureManager.Instance.CreateInstance(fl);


            cx = -PanelWidth;
            cx2 = -PanelWidth;

            f18 = GameFontManager.Instance.F18;
            f14 = GameFontManager.Instance.F14;
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
            Rectangle rect2 = new Rectangle((int)cx, 452, 358, 93);
            Rectangle rect3 = new Rectangle(332, 657, 379, 66);
            return Control.IsInBounds(x, y, ref rect) || 
                Control.IsInBounds(x, y, ref rect2) || 
                Control.IsInBounds(x, y, ref rect3);
        }
        public override int Order
        {
            get { return 5; }
        }

        void Sell_Click(object sender, MouseButtonFlags btn)
        {
            CityPlugin bestPlugin = null;
            float min = float.MaxValue;

            switch (selectedType)
            {
                case SelectedPluginType.Oil:
                    for (int i = 0; i < selectCity.PluginCount; i++)
                    {
                        CityPlugin cp = selectCity.GetPlugin(i);
                        if (cp.IsBuilding || cp.IsSelling)
                            continue;

                        if (cp.TypeId == CityPluginTypeId.OilRefinary)
                        {
                            if (cp.HRPConvRate < min)
                            {
                                min = cp.HRPConvRate;
                            }
                            bestPlugin = cp;
                        }
                        else if (cp.TypeId == CityPluginTypeId.BiofuelFactory)
                        {
                            if (cp.FoodConvRate + 1 < min)
                            {
                                min = cp.FoodConvRate + 1;
                            }
                            bestPlugin = cp;
                        }

                    }
                    break;
                case SelectedPluginType.Wood:
                    for (int i = 0; i < selectCity.PluginCount; i++)
                    {
                        CityPlugin cp = selectCity.GetPlugin(i);
                        if (cp.IsBuilding || cp.IsSelling)
                            continue;
                        if (cp.TypeId == CityPluginTypeId.WoodFactory)
                        {
                            if (cp.LRPConvRate < min)
                            {
                                min = cp.LRPConvRate;
                                bestPlugin = cp;
                            }
                        }
                    }
                    break;
                case SelectedPluginType.Education:
                case SelectedPluginType.Hospital:
                    for (int i = 0; i < selectCity.PluginCount; i++)
                    {
                        CityPlugin cp = selectCity.GetPlugin(i);
                        if (cp.IsBuilding || cp.IsSelling)
                            continue;
                        if (cp.TypeId == CityPluginTypeId.EducationOrg || 
                            cp.TypeId == CityPluginTypeId.Hospital)
                        {
                            if (cp.UpgradePoint < min)
                            {
                                min = cp.UpgradePoint;
                                bestPlugin = cp;
                            }
                        }
                    }
                    break;
            }

            if (bestPlugin != null)
                bestPlugin.Sell();

        }
        void Build_Click(object sender, MouseButtonFlags btn) 
        {
            switch (SelectedType)
            {
                case SelectedPluginType.Education:
                    selectCity.City.Add(gameLogic.PluginFactory.MakeEducationAgent());
                    break;
                case SelectedPluginType.Hospital:
                    selectCity.City.Add(gameLogic.PluginFactory.MakeHospital());
                    break;
                case SelectedPluginType.Oil:
                    selectCity.City.Add(gameLogic.PluginFactory.MakeOilRefinary());
                    break;
                case SelectedPluginType.Wood:
                    selectCity.City.Add(gameLogic.PluginFactory.MakeWoodFactory());
                    break;
            }
        }
        void EduBtn_Click(object sender, MouseButtonFlags btn)
        {
            SelectedType = SelectedPluginType.Education;
        }
        void OilBtn_Click(object sender, MouseButtonFlags btn)
        {
            SelectedType = SelectedPluginType.Oil;
        }
        void WoodBtn_Click(object sender, MouseButtonFlags btn)
        {
            SelectedType = SelectedPluginType.Wood;
        }
        void HospBtn_Click(object sender, MouseButtonFlags btn)
        {
            SelectedType = SelectedPluginType.Hospital;
        }

        public override void Render(Sprite sprite)
        {
            if (state != AnimState.Inside)
            {
                const float AnimStep = 0.33f;
                if (btnEduorg.IsMouseOver)
                {
                    blendWeight1 += AnimStep;
                    blendWeight2 -= AnimStep;
                    blendWeight3 -= AnimStep;
                    blendWeight4 -= AnimStep;
                }
                else if (btnHosp.IsMouseOver)
                {
                    blendWeight1 -= AnimStep;
                    blendWeight2 += AnimStep;
                    blendWeight3 -= AnimStep;
                    blendWeight4 -= AnimStep;
                }
                else if (btnOilref.IsMouseOver)
                {
                    blendWeight1 -= AnimStep;
                    blendWeight2 -= AnimStep;
                    blendWeight3 += AnimStep;
                    blendWeight4 -= AnimStep;
                }
                else if (btnWood.IsMouseOver)
                {
                    blendWeight1 -= AnimStep;
                    blendWeight2 -= AnimStep;
                    blendWeight3 -= AnimStep;
                    blendWeight4 += AnimStep;
                }
                else
                {
                    blendWeight1 -= AnimStep;
                    blendWeight2 -= AnimStep;
                    blendWeight3 -= AnimStep;
                    blendWeight4 -= AnimStep;
                }

                sprite.Draw(background, (int)cx, PanelY, ColorValue.White);


                blendWeight1 = MathEx.Saturate(blendWeight1);
                blendWeight2 = MathEx.Saturate(blendWeight2);
                blendWeight3 = MathEx.Saturate(blendWeight3);
                blendWeight4 = MathEx.Saturate(blendWeight4);

                int alpha = (int)(blendWeight1 * byte.MaxValue);
                if (alpha < 0) alpha = 0;
                if (alpha > byte.MaxValue) alpha = byte.MaxValue;

                ColorValue color = ColorValue.White;
                if (alpha > 0)
                {
                    color.A = (byte)alpha;
                    sprite.Draw(eduHover, (int)cx, PanelY, color);
                }

                alpha = (int)(blendWeight2 * byte.MaxValue);
                if (alpha < 0) alpha = 0;
                if (alpha > byte.MaxValue) alpha = byte.MaxValue;
                if (alpha > 0)
                {
                    color.A = (byte)alpha;
                    sprite.Draw(hospHover, (int)cx, PanelY, color);
                }

                alpha = (int)(blendWeight3 * byte.MaxValue);
                if (alpha < 0) alpha = 0;
                if (alpha > byte.MaxValue) alpha = byte.MaxValue;
                if (alpha > 0)
                {
                    color.A = (byte)alpha;
                    sprite.Draw(oilHover, (int)cx, PanelY, color);
                }

                alpha = (int)(blendWeight4 * byte.MaxValue);
                if (alpha < 0) alpha = 0;
                if (alpha > byte.MaxValue) alpha = byte.MaxValue;
                if (alpha > 0)
                {
                    color.A = (byte)alpha;
                    sprite.Draw(woodHover, (int)cx, PanelY, color);
                }




                f18.DrawString(sprite, "CONSTRUCT", (int)cx + 30, PanelY + 10, ColorValue.White);

                switch (selectedType) 
                {
                    case SelectedPluginType.Oil:

                        f18.DrawString(sprite, "OIL REFINARY", (int)cx + 233, PanelY + 10, ColorValue.White);
                        break;
                    case SelectedPluginType.Wood:

                        f18.DrawString(sprite, "WOOD FACTORY", (int)cx + 203, PanelY + 10, ColorValue.White);
                        break;
                    case SelectedPluginType.Hospital:

                        f18.DrawString(sprite, "HOSIPTAL", (int)cx + 270, PanelY + 10, ColorValue.White);
                        break;
                    case SelectedPluginType.Education:

                        f18.DrawString(sprite, "SCHOOL", (int)cx + 270, PanelY + 10, ColorValue.White);
                        break;
                }
                
            }
            if (state2 != AnimState.Inside)
            {
                sprite.Draw(infobg, (int)cx2, 449, ColorValue.White);
                sprite.Draw(costbg, (int)cx2, 486, ColorValue.White);
               
                sell.Render(sprite);
                build.Render(sprite);

                switch (selectedType)
                {
                    case SelectedPluginType.Wood:
                        string msg = "GATHERS ";
                        string msg2 = "FROM NEARBY FOREST";


                        f14.DrawString(sprite, msg, (int)cx2 + 30, 461, ColorValue.White);
                        sprite.Draw(woodSign, (int)cx2 + 30 + 80, 455, ColorValue.White);
                        f14.DrawString(sprite, msg2, (int)cx2 + 30 + 120, 461, ColorValue.White);

                        break;
                    case SelectedPluginType.Oil:
                        msg = "GATHERS ";
                        msg2 = "FROM NEARBY OILFIELD";

                        f14.DrawString(sprite, msg, (int)cx2 + 30, 461, ColorValue.White);
                        sprite.Draw(oilSign, (int)cx2 + 30 + 80, 455, ColorValue.White);
                        f14.DrawString(sprite, msg2, (int)cx2 + 30 + 120, 461, ColorValue.White);

                        break;
                    case SelectedPluginType.Hospital:
                        msg = "IMPROVES CITY'S HEALTHCARE";

                        f14.DrawString(sprite, msg, (int)cx2 + 30, 461, ColorValue.White);

                        break;
                    case SelectedPluginType.Education:
                        msg = "BOOSTS CITY'S DEVELOPMENT";

                        f14.DrawString(sprite, msg, (int)cx2 + 30, 461, ColorValue.White);

                        break;
                }
                
                f14.DrawString(sprite, "COST", (int)cx2 + 37, 502, ColorValue.White);

            }

            sprite.Draw(cityInfoBg, 333, 601, ColorValue.White);

            if (anySelCity != null)
            {
                if (anySelCity.IsCaptured)
                {
                    cityDev.Render(sprite);
                }

                f18.DrawString(sprite, anySelCity.Name.ToUpperInvariant(), 345, 670, ColorValue.White);

                switch (anySelCity.Size)
                {
                    case UrbanSize.Small:
                        sprite.Draw(smCitySign, 0, 0, ColorValue.White);
                        break;
                    case UrbanSize.Medium:
                        sprite.Draw(medCitySign, 0, 0, ColorValue.White);
                        break;
                    case UrbanSize.Large:
                        sprite.Draw(lgCitySign, 0, 0, ColorValue.White);
                        break;
                }
            }
            if (selectCity != null)
            {
                sprite.Draw(oilResSign, 0, 0, ColorValue.White);
                sprite.Draw(woodResSign, 0, 0, ColorValue.White);
                sprite.Draw(foodResSign, 0, 0, ColorValue.White);

                f18.DrawString(sprite, selectCity.City.LocalLR.Current.ToString("F0"), 398, 689, ColorValue.White);
                f18.DrawString(sprite, selectCity.City.LocalHR.Current.ToString("F0"), 487, 689, ColorValue.White);
                f18.DrawString(sprite, selectCity.City.LocalFood.Current.ToString("F0"), 600, 689, ColorValue.White);

                float mult = selectCity.City.AdditionalDevMult;

                mult = selectCity.City.HealthCare;

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
            if (state2 == AnimState.Outside)
            {
                build.Update(time);
                sell.Update(time);
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
            if (state2 == AnimState.Out)
            {
                cx2 += (PanelWidth + PanelX - cx2 + PopBaseSpeed) * time.ElapsedGameTimeSeconds * 2;
                if (cx2 >= PanelX)
                {
                    cx2 = PanelX;
                    state2 = AnimState.Outside;
                }

            }
            else if (state2 == AnimState.In)
            {
                cx2 -= (PanelWidth + PanelX - cx2 + PopBaseSpeed) * time.ElapsedGameTimeSeconds * 2;
                if (cx2 < -PanelWidth)
                {
                    cx2 = -PanelWidth;
                    state2 = AnimState.Inside;
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



            build.X = (int)(cx2 + 292);
            build.Y = 492;

            sell.X = (int)(cx2 + 236);
            sell.Y = 492;

            build.Enabled = selectCity != null ? selectCity.City.CanAddPlugins : false;
            sell.Enabled = false;

            if (anySelCity != null)
            {
                cityDev.Value = anySelCity.Satisfaction;
            }

            if (selectCity != null && selectedType != SelectedPluginType.None)
            {
                for (int i = selectCity.PluginCount -1; i >=0;i--)
                {
                    CityPlugin cp = selectCity.GetPlugin(i);

                    if (cp.IsSelling || cp.IsBuilding)
                        continue;

                    switch (cp.TypeId)
                    {
                        case CityPluginTypeId.WoodFactory:
                            if (selectedType == SelectedPluginType.Wood)
                            {
                                sell.Enabled = true;
                                i = -1;
                            }
                            break;
                        case CityPluginTypeId.OilRefinary:                            
                        case CityPluginTypeId.BiofuelFactory:
                            if (selectedType == SelectedPluginType.Oil)
                            {
                                sell.Enabled = true;
                                i = -1;
                            }
                            break;
                        case CityPluginTypeId.EducationOrg:
                            if (selectedType == SelectedPluginType.Education)
                            {
                                sell.Enabled = true;
                                i = -1;
                            }
                            break;
                        case CityPluginTypeId.Hospital:
                            if (selectedType == SelectedPluginType.Hospital)
                            {
                                sell.Enabled = true;
                                i = -1;
                            }
                            break;
                    }
                }
            }

        }
    }
}
