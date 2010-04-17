using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.Logic;
using Code2015.World;
using Code2015.World.Screen;

namespace Code2015.GUI
{
    class SelFilter : IObjectFilter
    {
        static SelFilter singleton;

        public static SelFilter Instance
        {
            get
            {
                if (singleton == null)
                    singleton = new SelFilter();
                return singleton;
            }
        }

        #region IObjectFilter 成员

        public bool Check(SceneObject obj)
        {
            return obj is ISelectableObject;
        }

        public bool Check(OctreeSceneNode node)
        {
            return true;
        }

        #endregion
    }

    /// <summary>
    ///  表示游戏过程中的界面
    /// </summary>
    class InGameUI : UIComponent
    {
        GameScene scene;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;
        GameState logic;
        Font font;

        LoadingScreen loadScreen;
        ScoreScreen scoreScreen;

        GoalIcons icons;
        GoalPieceMaker pieceMaker;

        ScreenPhysicsWorld physWorld;
      
        //LinkUI linkUI;

        Texture cursor;
        Point mousePosition;

        CityUI ingameui2;

        Player player;

        CityObject mouseHoverCity;

        Ray selectRay;

        public Ray SelectionRay
        {
            get { return selectRay; }
        }

        public ScreenPhysicsWorld PhysicsWorld
        {
            get { return physWorld; }
        }
        public CityObject MouseHoverCity
        {
            get { return mouseHoverCity; }
            private set
            {
                mouseHoverCity = value;
            }
        }


        public InGameUI(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.parent = parent;
            this.logic = gamelogic;

            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;
            this.physWorld = new ScreenPhysicsWorld();

            this.player = parent.HumanPlayer;


            FileLocation fl = FileSystem.Instance.Locate("def.fnt", GameFileLocs.GUI);
            font = FontManager.Instance.CreateInstance(renderSys, fl, "default");

            fl = FileSystem.Instance.Locate("cursor.tex", GameFileLocs.GUI);
            cursor = UITextureManager.Instance.CreateInstance(fl);

           

            this.ingameui2 = new CityUI(game, parent, scene, gamelogic);

            this.icons = new GoalIcons(parent, this, ingameui2.CityInfoDisplay, scene, physWorld);
            this.pieceMaker = new GoalPieceMaker(player.Area, renderSys, scene.Camera, icons);
            this.loadScreen = new LoadingScreen(renderSys);
        }

        public override void Render(Sprite sprite)
        {
            if (scoreScreen != null)
            {
                scoreScreen.Render(sprite);
            }
            else
            {
                if (!parent.IsLoaded)
                {
                    loadScreen.Progress = parent.LoadingProgress;
                    loadScreen.Render(sprite);

                }
                else
                {
                    if (loadScreen != null)
                    {
                        loadScreen.Dispose();
                        loadScreen = null;
                    }

                    icons.Render(sprite);
                    sprite.SetTransform(Matrix.Identity);
                    ingameui2.Render(sprite);

                    sprite.Draw(cursor, mousePosition.X, mousePosition.Y, ColorValue.White);
                }
            }
        }

        public void Interact(GameTime time)
        {
            #region 屏幕边缘滚动视野
            const int ScrollPadding = 3;
            RtsCamera camera = parent.Scene.Camera;

            camera.Height += MouseInput.DScrollWheelValue * 0.05f;

            if (MouseInput.X <= ScrollPadding)
            {
                camera.MoveLeft();
            }
            if (MouseInput.X >= Program.Window.ClientSize.Width - ScrollPadding)
            {
                camera.MoveRight();
            }
            if (MouseInput.Y <= ScrollPadding)
            {
                camera.MoveFront();
            }
            if (MouseInput.Y >= Program.Window.ClientSize.Height - ScrollPadding)
            {
                camera.MoveBack();
            }
            #endregion

            // 交互检查
            //  界面
            //  图标
            //  场景

            //linkUI.Update(time);
            ingameui2.Update(time);
            icons.Update(time);

            if (ingameui2.HitTest(mousePosition.X, mousePosition.Y))
            {
                ingameui2.Interact(time);
            }
            else //if (icons.MouseHitTest(mousePosition.X,mousePosition.Y))
            {
                icons.Interact(time);

                if (!icons.MouseHitTest(mousePosition.X, mousePosition.Y))
                {
                    //}
                    //else
                    //{
                    Vector3 mp = new Vector3(mousePosition.X, mousePosition.Y, 0);
                    Vector3 start = renderSys.Viewport.Unproject(mp, camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);
                    mp.Z = 1;
                    Vector3 end = renderSys.Viewport.Unproject(mp, camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);
                    Vector3 dir = end - start;
                    dir.Normalize();

                    selectRay = new Ray(start, dir);

                    ISelectableObject sel = null;
                    SceneObject obj = parent.Scene.Scene.FindObject(selectRay, SelFilter.Instance);
                    sel = obj as ISelectableObject;
                    MouseHoverCity = sel as CityObject;

                    if (MouseInput.IsMouseDownLeft)
                    {
                        ingameui2.SelectedObject = sel;
                    }

                    //linkUI.Interact(time);
                }
            }

        }

        public override void Update(GameTime time)
        {
            if (scoreScreen != null)
            {
                scoreScreen.Update(time);
            }
            else
            {
                if (parent.IsLoaded)
                {
                    mousePosition.X = MouseInput.X;
                    mousePosition.Y = MouseInput.Y;

                    physWorld.Update(time);

                    pieceMaker.Update(time);

                    Interact(time);

                }
            }
        }

        public bool IsShowingScore
        {
            get { return scoreScreen != null; }
        }

        public void ShowScore(ScoreEntry[] entries) 
        {
            scoreScreen = new ScoreScreen(game);
            for (int i = 0; i < entries.Length; i++)
            {
                scoreScreen.Add(entries[i]);
            }
        }
    }
}
