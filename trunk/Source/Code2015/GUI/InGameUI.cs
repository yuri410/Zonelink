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
    class InGameUI : GUIScreen
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
      
        Texture cursor;

        PieceContainer container;
        InfoUI infoUI;
        MiniMap miniMap;
        NoticeBar noticeBar;
        DevelopmentMeter playerProgress;
        Picker picker;
        CityEditPanel cityEdit;

        Player player;


        //public Ray SelectionRay
        //{
        //    get { return picker.SelectionRay; }
        //}

        public ScreenPhysicsWorld PhysicsWorld
        {
            get { return physWorld; }
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



            picker = new Picker(game, parent, scene, gamelogic);
            AddElement(picker);


            this.cityEdit = new CityEditPanel(game, parent, scene, gamelogic);
            AddElement(cityEdit);

            this.infoUI = new InfoUI(game, parent, scene, gamelogic);
            AddElement(infoUI);
            this.icons = new GoalIcons(parent, this, infoUI.CityInfoDisplay, scene, physWorld);
            AddElement(icons); 
            this.pieceMaker = new GoalPieceMaker(player.Area, renderSys, scene.Camera, icons);
          
            this.loadScreen = new LoadingScreen(renderSys);

            this.playerProgress = new DevelopmentMeter(game, parent, scene, gamelogic);
            AddElement(playerProgress); 
            this.noticeBar = new NoticeBar(game, parent, scene, gamelogic);
            AddElement(noticeBar); 
            this.miniMap = new MiniMap(game, parent, scene, gamelogic);
            AddElement(miniMap); 
            this.container = new PieceContainer(game, parent, scene, gamelogic);
            AddElement(container);
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

                    base.Render(sprite);
                    //icons.Render(sprite);
                    //sprite.SetTransform(Matrix.Identity);
                    //cityUI.Render(sprite);

                    //playerProgress.Render(sprite);
                    //miniMap.Render(sprite);
                    //noticeBar.Render(sprite);
                    //container.Render(sprite);

                    sprite.Draw(cursor, MouseInput.X, MouseInput.Y, ColorValue.White);
                }
            }
        }

        //public void Interact(GameTime time)
        //{
        //    // 交互检查
        //    //  界面
        //    //  图标
        //    //  场景

        //    //linkUI.Update(time);
        //    cityUI.Update(time);
        //    icons.Update(time);

        //    if (cityUI.HitTest(MouseInput.X, MouseInput.Y))
        //    {
        //        cityUI.Interact(time);
        //    }
        //    else //if (icons.MouseHitTest(mousePosition.X,mousePosition.Y))
        //    {
        //        icons.Interact(time);

        //        //if (!icons.MouseHitTest(mousePosition.X, mousePosition.Y))
        //        {
        //            //}
        //            //else
        //            //{
                  

        //            //linkUI.Interact(time);
        //        }
        //    }

        //}

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

                    pieceMaker.Update(time);

                    physWorld.Update(time);
                    infoUI.SelectedObject = picker.SelectedObject;
                    
                    base.Update(time);


                    //playerProgress.Update(time);
                    //miniMap.Update(time);
                    //noticeBar.Update(time);
                    //container.Update(time);

                    //Interact(time);

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
