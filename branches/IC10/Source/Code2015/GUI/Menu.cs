/*
-----------------------------------------------------------------------------
This source file is part of Zonelink

Copyright (c) 2009+ Tao Games

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  if not, write to the Free Software Foundation, 
Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA, or go to
http://www.gnu.org/copyleft/gpl.txt.

-----------------------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Effects;
using Apoc3D.Graphics.Geometry;
using Apoc3D.GUI.Controls;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.Effects;
using Code2015.EngineEx;
using Code2015.Logic;
using Code2015.World;

namespace Code2015.GUI
{

    /// <summary>
    ///  表示游戏菜单
    /// </summary>
    class Menu : UIComponent, IGameComponent
    {
        //const float RotSpeed = 3;
        //Vector3 RotAxis = new Vector3(2505.168f, 4325.199f, 4029.689f);
        //float angle;
        //float rotScale;

        //class MenuScene : StaticModelObject
        //{

        //    public MenuScene(RenderSystem rs)
        //    {
        //        FileLocation fl = FileSystem.Instance.Locate("start.mesh", GameFileLocs.Model);

        //        ModelL0 = new Model(ModelManager.Instance.CreateInstance(rs, fl));
        //        BoundingSphere.Radius = PlanetEarth.PlanetRadius;
        //        Transformation = Matrix.Identity;

        //    }

        //    public override bool IsSerializable
        //    {
        //        get { return false; }
        //    }

        //    public override RenderOperation[] GetRenderOperation()
        //    {
        //        return base.GetRenderOperation();
        //    }
        //}

        //class MenuCamera : Camera
        //{
        //    public MenuCamera(float aspect)
        //        : base(aspect)
        //    {
        //        FieldOfView = 35;
        //        Position = new Vector3(-5260.516f, 6214.899f, -15371.574f);
        //        NearPlane = 100;
        //        FarPlane = 25000;
        //    }
        //    public override void UpdateProjection()
        //    {
        //        float fovy = MathEx.Degree2Radian(23.5f);
        //        NearPlaneHeight = (float)(Math.Tan(fovy * 0.5f)) * NearPlane * 2;
        //        NearPlaneWidth = NearPlaneHeight * AspectRatio;

        //        Frustum.proj = Matrix.PerspectiveRH(NearPlaneWidth, NearPlaneHeight, NearPlane, FarPlane);

        //    }
        //    public override void Update(GameTime time)
        //    {
        //        //UpdateProjection();

        //        //x->z y->x z->y 
        //        Vector3 target = new Vector3(-3151.209f, 6214.899f, 325.246f);

        //        base.Update(time);
        //        Frustum.view = Matrix.LookAtRH(Position, target, Vector3.UnitY);
        //        Frustum.Update();

        //        orientation = Quaternion.RotationMatrix(Frustum.view);

        //        Matrix m = Matrix.Invert(Frustum.view);
        //        front = m.Forward;// MathEx.GetMatrixFront(ref m);
        //        top = m.Up;// MathEx.GetMatrixUp(ref m);
        //        right = m.Right;// MathEx.GetMatrixRight(ref m);
        //    }
        //    public override float GetSMScale()
        //    {
        //        return 20;
        //    }
        //    public override Matrix GetSMTrans()
        //    {
        //        //Vector3 pos = new Vector3(-10799.082f, -3815.834f, 6951.33f);
        //        Vector3 pos = new Vector3(-5009.926f, 8066.071f, -16341.605f);//-6522.938f, 8066.071f, -12065.895f);// new Vector3(-3815.834f, 6951.33f, -10799.082f);
        //        Vector3 target = new Vector3(-2578.986f, 4845.344f, -2702.878f);// new Vector3(-3151.209f, 6214.899f, 325.246f);//-2702.878  -2578.986  4845.344


        //        return Matrix.LookAtRH(pos, target, Vector3.UnitY);
        //    }
        //}
        //class MenuWater : StaticModelObject
        //{
        //    Sphere oceanSphere;

        //    public MenuWater(RenderSystem renderSys)
        //    {

        //        Material[][] mats = new Material[1][];
        //        mats[0] = new Material[1];
        //        mats[0][0] = new Material(renderSys);

        //        FileLocation fl = FileSystem.Instance.Locate("WaterNormal.tex", GameFileLocs.Nature);
        //        ResourceHandle<Texture> map = TextureManager.Instance.CreateInstance(fl);
        //        mats[0][0].SetTexture(1, map);

        //        fl = FileSystem.Instance.Locate("WaterDudv.tex", GameFileLocs.Nature);
        //        map = TextureManager.Instance.CreateInstance(fl);
        //        mats[0][0].SetTexture(0, map);

        //        mats[0][0].SetEffect(EffectManager.Instance.GetModelEffect(MMWaterEffectFactory.Name));
        //        mats[0][0].IsTransparent = true;
        //        mats[0][0].ZWriteEnabled = false;
        //        mats[0][0].ZEnabled = true;
        //        mats[0][0].CullMode = CullMode.CounterClockwise;
        //        mats[0][0].PriorityHint = RenderPriority.Third;


        //        oceanSphere = new Sphere(renderSys, PlanetEarth.PlanetRadius + 15,
        //            PlanetEarth.ColTileCount * 4, PlanetEarth.LatTileCount * 4, mats);

        //        base.ModelL0 = oceanSphere;

        //        BoundingSphere.Radius = PlanetEarth.PlanetRadius;
        //    }

        //    public override bool IsSerializable
        //    {
        //        get { return false; }
        //    }
        //}

        //RenderSystem renderSys;
        //SceneRenderer renderer;
        //MenuScene earth;
        //MenuWater water;

        Tutorial tutorial;
        Intro intro;
        //LoadingScreen loadScreen;
        //ScoreScreen2 scoreScreen;
        CreditScreen credits;
        LoadingOverlay loadingOverlay;

        Code2015 game;
        MainMenu mainMenu;
        //SelectScreen sideSelect;
        //RenderTarget renderTarget;

        //Texture logolgt1;
        //Texture logolgt2;
        //Texture logolgt3;
        //float light1;
        //float light2;
        //float light3;

        Texture logo;
        Texture overlay34;
        float overlayAlpha;

        public UIComponent CurrentScreen
        {
            get;
            set;
        }
        public CreditScreen GetCredits()
        {
            return credits;
        }
        public LoadingOverlay GetOverlay() { return loadingOverlay; }


        public Tutorial GetTutorial()
        {
            return tutorial;
        }

        //public Texture Earth
        //{
        //    get { return renderTarget.GetColorBufferTexture(); }
        //}

        public Menu(Code2015 game, RenderSystem rs)
        {
            this.game = game;
            this.mainMenu = new MainMenu(game, this);
            //this.sideSelect = new SelectScreen(game, this);
            //this.renderSys = rs;

            //CreateScene(rs);
            //this.loadScreen = new LoadingScreen(this, rs);
            this.intro = new Intro(rs);
            this.credits = new CreditScreen(rs, this);
            this.tutorial = new Tutorial(this);

            this.CurrentScreen = null;

            FileLocation fl = FileSystem.Instance.Locate("overlay34.tex", GameFileLocs.GUI);
            overlay34 = UITextureManager.Instance.CreateInstance(fl);
            this.loadingOverlay = new LoadingOverlay(game, overlay34);

            //light1 = Randomizer.GetRandomSingle() * MathEx.PIf * 2;
            //light2 = Randomizer.GetRandomSingle() * MathEx.PIf * 2;
            //light3 = Randomizer.GetRandomSingle() * MathEx.PIf * 2;


            fl = FileSystem.Instance.Locate("mm_logo.tex", GameFileLocs.GUI);
            logo = UITextureManager.Instance.CreateInstance(fl);

            //fl = FileSystem.Instance.Locate("mm_logo_l1.tex", GameFileLocs.GUI);
            //logolgt1 = UITextureManager.Instance.CreateInstance(fl);

            //fl = FileSystem.Instance.Locate("mm_logo_l2.tex", GameFileLocs.GUI);
            //logolgt2 = UITextureManager.Instance.CreateInstance(fl);

            //fl = FileSystem.Instance.Locate("mm_logo_l3.tex", GameFileLocs.GUI);
            //logolgt3 = UITextureManager.Instance.CreateInstance(fl);

        }
        public void RenderLogo(Sprite sprite)
        {
            sprite.Draw(logo, 0, 0, ColorValue.White);

            //ColorValue color = ColorValue.White;
            //color.A = (byte)(byte.MaxValue * MathEx.Saturate((float)Math.Cos(light1) * 0.5f + 1));
            //sprite.Draw(logolgt1, (int)(Math.Cos(light1) * 10), 0, color);
            //color.A = (byte)(byte.MaxValue * MathEx.Saturate((float)Math.Cos(light2) * 0.5f + 1));
            //sprite.Draw(logolgt2, (int)(Math.Cos(light2) * 10), 0, color);
            //color.A = (byte)(byte.MaxValue * MathEx.Saturate((float)Math.Cos(light3) * 0.5f + 1));
            //sprite.Draw(logolgt3, (int)(Math.Cos(light3) * 10), 0, color);
        }
        //void CreateScene(RenderSystem rs)
        //{
        //    SceneRendererParameter sm = new SceneRendererParameter();
        //    sm.SceneManager = new OctreeSceneManager(new OctreeBox(PlanetEarth.PlanetRadius * 4f), PlanetEarth.PlanetRadius / 75f);
        //    sm.PostRenderer = new GamePostRenderer(rs, null);
        //    sm.UseShadow = true;

        //    MenuCamera camera = new MenuCamera(Program.ScreenWidth / (float)Program.ScreenHeight);

        //    renderTarget = rs.ObjectFactory.CreateRenderTarget(Program.ScreenWidth, Program.ScreenHeight, Apoc3D.Media.ImagePixelFormat.A8R8G8B8);

        //    camera.RenderTarget = renderTarget;
        //    renderer = new SceneRenderer(rs, sm);
        //    renderer.ClearColor = new ColorValue(205, 244, 161, 0);// ColorValue.TransparentWhite;
        //    renderer.RegisterCamera(camera);


        //    renderer.ClearScreen = true;

        //    earth = new MenuScene(rs);
        //    renderer.SceneManager.AddObjectToScene(earth);

        //    water = new MenuWater(rs);
        //    renderer.SceneManager.AddObjectToScene(water);


        //    RotAxis.Normalize();
        //}

        public void Render()
        {

            if (!game.IsIngame)
            {
                //EffectParams.LightDir = -renderer.CurrentCamera.Front;
                //renderer.RenderScene();
            }
        }


        //void ShowScore(ScoreEntry[] entries)
        //{
        //    if (scoreScreen != null)
        //    {
        //        scoreScreen.Clear();
        //    }
        //    else
        //    {
        //        scoreScreen = new ScoreScreen2(game, this);
        //    }
        //    for (int i = 0; i < entries.Length; i++)
        //    {
        //        scoreScreen.Add(entries[i]);
        //    }

        //    CurrentScreen = scoreScreen;
        //}

        public override void Render(Sprite sprite)
        {

            if (!game.IsIngame)
            {
                mainMenu.Render(sprite);

                if (CurrentScreen != null)
                {
                    if (overlayAlpha < 1)
                        overlayAlpha += 0.1f;
                    else
                        overlayAlpha = 1;

                    ColorValue color = ColorValue.White;
                    color.A = (byte)(87 * MathEx.Saturate(overlayAlpha));

                    sprite.Draw(overlay34, 0, 0, color);
                    CurrentScreen.Render(sprite);
                }
                else
                {
                    if (overlayAlpha > 0)
                    {
                        overlayAlpha -= 0.1f;
                        ColorValue color = ColorValue.White;
                        color.A = (byte)(87 * MathEx.Saturate(overlayAlpha));

                        sprite.Draw(overlay34, 0, 0, color);
                    }
                    else
                        overlayAlpha = 0;
                }
            }


            loadingOverlay.Render(sprite);


            if (intro != null)
            {
                intro.Render(sprite);
            }
        }
        //void UpdateScene(GameTime time)
        //{
        //    #region 地球
        //    if (angle > MathEx.Degree2Radian(140) && angle < MathEx.Degree2Radian(250))
        //    {
        //        rotScale += 0.01f;

        //        if (rotScale > 3.5f)
        //            rotScale = 3.5f;
        //    }
        //    else if (rotScale < 1)
        //    {
        //        rotScale += 0.01f;
        //    }
        //    else
        //    {
        //        rotScale -= 0.01f;
        //        //if (rotScale < 1)
        //        //    rotScale = 1;
        //    }
        //    angle += MathEx.Degree2Radian(RotSpeed * time.ElapsedGameTimeSeconds) * rotScale;

        //    if (angle > MathEx.PIf * 2)
        //        angle -= MathEx.PIf * 2;


        //    Matrix rot = Matrix.RotationAxis(RotAxis, -angle);
        //    earth.Transformation = rot;
        //    water.Transformation = rot;


        //    renderer.Update(time);

        //    #endregion
        //}
        public override void Update(GameTime time)
        {
            if (intro != null)
            {
                intro.Update(time);
                if (intro.IsOver)
                {
                    intro.Dispose();
                    intro = null;
                }
            }

            //light1 += time.ElapsedGameTimeSeconds * (0.5f + Randomizer.GetRandomSingle());
            //if (light1 > MathEx.PIf * 2)
            //    light1 -= MathEx.PIf * 2;

            //light2 += time.ElapsedGameTimeSeconds * (0.5f + Randomizer.GetRandomSingle());
            //if (light2 > MathEx.PIf * 2)
            //    light2 -= MathEx.PIf * 2;
            //light3 += time.ElapsedGameTimeSeconds * (0.5f + Randomizer.GetRandomSingle());
            //if (light3 > MathEx.PIf * 2)
            //    light3 -= MathEx.PIf * 2;


            loadingOverlay.Update(time);
            if (!game.IsIngame)
            {
                //UpdateScene(time);
                if (CurrentScreen != null)
                {
                    CurrentScreen.Update(time);
                }
                else
                {
                    mainMenu.Update(time);
                }
            }
            else
            {
                if (game.CurrentGame.IsOver)
                {
                    //ShowScore(game.CurrentGame.ResultScore);
                    game.Back();
                }
            }
        }

    }

}
