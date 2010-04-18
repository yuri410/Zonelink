﻿using System;
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
    class CityUI : UIComponent
    {
        GameScene scene;
        GameState gameLogic;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;
        Font font;

        Font algerFont;
        Player player;

        //侧边栏图标
        Texture ico_box1;
        Texture ico_box2;
        Texture ico_box3;
        Texture ico_box4;
        Texture ico_exchange;

        CityInfoDisplay cityInfoDisplay;
        ResInfoDisplay resInfoDisplay;

        RtsCamera camera;

        CityLinkableMark linkArrow;
        CityEditPanel cityEdit;

        ISelectableObject selected;
        CityObject city;
       

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

                        if (city != null)
                        {
                            Vector3 ppos = renderSys.Viewport.Project(city.Position, camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);

                            selectedProjPos.X = (int)ppos.X;
                            selectedProjPos.Y = (int)ppos.Y;

                            City cc = city.City;
                            CityObject[] nearby = new CityObject[cc.LinkableCityCount];

                            for (int i = 0; i < cc.LinkableCityCount; i++)
                            {
                                nearby[i] = cc.GetLinkableCity(i).Parent;
                            }

                            linkArrow.SetCity(city, nearby);
                        }
                    }
                    else
                    {
                        city = null;
                    }

                    if (city == null)
                    {
                        linkArrow.SetCity(null, null);
                    }

                    cityEdit.SelectedCity = city;
                }
            }
        }

        public CityUI(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
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
            fl = FileSystem.Instance.Locate("ico_box1.tex", GameFileLocs.GUI);
            ico_box1 = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ico_box2.tex", GameFileLocs.GUI);
            ico_box2 = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ico_box3.tex", GameFileLocs.GUI);
            ico_box3 = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ico_box4.tex", GameFileLocs.GUI);
            ico_box4 = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_changeBox.tex", GameFileLocs.GUI);
            ico_exchange = UITextureManager.Instance.CreateInstance(fl);


            //fl = FileSystem.Instance.Locate("ico_oilref.tex", GameFileLocs.GUI);
            //ico_oil = UITextureManager.Instance.CreateInstance(fl);

            //fl = FileSystem.Instance.Locate("ico_hosp.tex", GameFileLocs.GUI);
            //ico_hosp = UITextureManager.Instance.CreateInstance(fl);

            //fl = FileSystem.Instance.Locate("ico_school.tex", GameFileLocs.GUI);
            //ico_edu = UITextureManager.Instance.CreateInstance(fl);

            //fl = FileSystem.Instance.Locate("ico_woodfac.tex", GameFileLocs.GUI);
            //ico_wood = UITextureManager.Instance.CreateInstance(fl);

            cityEdit = new CityEditPanel(game, parent, scene, gamelogic);
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
        //void EduBtn_Click(object sender, MouseButtonFlags btn)
        //{
        //    //page = PanelPage.EduOrg;
        //}
        //void OilBtn_Click(object sender, MouseButtonFlags btn)
        //{
        //    //page = PanelPage.OilRefinary;
        //}
        //void WoodBtn_Click(object sender, MouseButtonFlags btn)
        //{
        //    //page = PanelPage.WoodFactory;
        //}
        //void HospBtn_Click(object sender, MouseButtonFlags btn)
        //{
        //    //page = PanelPage.Hospital;
        //}


        public override void Render(Sprite sprite)
        {
            cityInfoDisplay.Render(sprite);
            resInfoDisplay.Render(sprite);

            #region 渲染城市信息
            if (city != null)
            {
                //sprite.Draw(ico_edu, 394, 563, ColorValue.White);
                //sprite.Draw(ico_wood, 394, 563, ColorValue.White);
                //sprite.Draw(ico_oil, 394, 563, ColorValue.White);
                //sprite.Draw(ico_hosp, 394, 563, ColorValue.White);


                //if (city.Owner == player)
                //{
                //    btnEduorg.Render(sprite);
                //    btnHosp.Render(sprite);
                //    btnOilref.Render(sprite);
                //    btnWood.Render(sprite);
                //}
            }
            #endregion


            sprite.Draw(ico_exchange, 1177, 619, ColorValue.White);
            sprite.Draw(ico_box1, 614, 606, ColorValue.White);
            sprite.Draw(ico_box2, 729, 606, ColorValue.White);
            sprite.Draw(ico_box3, 844, 606, ColorValue.White);
            sprite.Draw(ico_box4, 959, 606, ColorValue.White);

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
            //if (city != null)
            //{
                //if (y > 595 && x > 420)
                //{
                //    return true;
                //}
                //if (y > 168 && x < 155)
                //{
                //    return true;
                //}
            //}
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
                        //btnEduorg.Update(time);
                        //btnHosp.Update(time);
                        //btnOilref.Update(time);
                        //btnWood.Update(time);
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