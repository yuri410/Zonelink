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
        [Flags]
        enum MouseCursor
        {
            Normal = 0,
            LeftArrow = 1,
            RightArrow = 2,
            UpArrow = 4,
            DownArrow = 8,
            UpRightArrow = UpArrow | RightArrow,
            DownRightArrow = DownArrow | RightArrow,
            UpLeftArrow = UpArrow | LeftArrow,
            DownLeftArrow = DownArrow | LeftArrow,
        }

        GameScene scene;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;
        GameState logic;
        Font font;
        MouseCursor cursorState;

        LoadingScreen loadScreen;
        ScoreScreen scoreScreen;

        GoalIcons icons;
        GoalPieceMaker pieceMaker;

        ScreenPhysicsWorld physWorld;

        Texture cursor;
        Texture cursor_up;
        Texture cursor_down;
        Texture cursor_left;
        Texture cursor_right;
        Texture cursor_ul;
        Texture cursor_ur;
        Texture cursor_dl;
        Texture cursor_dr;
        
        PieceContainer container;
        InfoUI infoUI;
        MiniMap miniMap;
        NoticeBar noticeBar;
        DevelopmentMeter playerProgress;
        Picker picker;
        CityEditPanel cityEdit;
        Brackets brackets;

        CO2Graph co2graph;
        Player player;



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

            fl = FileSystem.Instance.Locate("cursor_u.tex", GameFileLocs.GUI);
            cursor_up = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("cursor_l.tex", GameFileLocs.GUI);
            cursor_left = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("cursor_d.tex", GameFileLocs.GUI);
            cursor_down = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("cursor_r.tex", GameFileLocs.GUI);
            cursor_right = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("cursor_lu.tex", GameFileLocs.GUI);
            cursor_ul = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("cursor_ru.tex", GameFileLocs.GUI);
            cursor_ur = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("cursor_ld.tex", GameFileLocs.GUI);
            cursor_dl = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("cursor_rd.tex", GameFileLocs.GUI);
            cursor_dr = UITextureManager.Instance.CreateInstance(fl);
          
            
            
            cursorState = MouseCursor.Normal;



            picker = new Picker(game, parent, scene, gamelogic);
            AddElement(picker);


            this.cityEdit = new CityEditPanel(game, parent, scene, gamelogic);
            AddElement(cityEdit);

            this.infoUI = new InfoUI(game, parent, scene, gamelogic);
            AddElement(infoUI);

            this.brackets = new Brackets(game, parent, scene, gamelogic, picker);
            AddElement(brackets);

            this.icons = new GoalIcons(parent, this, infoUI.CityInfoDisplay, scene, physWorld, brackets);
            AddElement(icons);

            brackets.SetGoalIcons(icons);

            this.pieceMaker = new GoalPieceMaker(player.Area, renderSys, scene.Camera, icons);

            this.loadScreen = new LoadingScreen(renderSys);

            this.playerProgress = new DevelopmentMeter(game, parent, scene, gamelogic);
            AddElement(playerProgress);
            this.noticeBar = new NoticeBar(game, parent, scene, gamelogic);
            AddElement(noticeBar);
            this.miniMap = new MiniMap(game, parent, scene, gamelogic);
            AddElement(miniMap);
            this.container = new PieceContainer(game, parent, scene, gamelogic, icons);
            AddElement(container);
            PieceContainerOverlay overlay = new PieceContainerOverlay(game, parent, scene, gamelogic);
            AddElement(overlay);

            icons.SetPieceContainer(container);
            //co2graph = new CO2Graph(game, parent, scene, gamelogic);
            //AddElement(co2graph);
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

                    Point hsp = new Point();// GetHotSpot(cursorState);
                    Texture ctex = cursor;
                    switch (cursorState)
                    {
                        case MouseCursor.Normal:
                            hsp = new Point(6, 6);
                            ctex = cursor;
                            break;
                        case MouseCursor.LeftArrow:
                            hsp = new Point(5, 24);
                            ctex = cursor_left;
                            break;
                        case MouseCursor.DownArrow:
                            hsp = new Point(24, 26);
                            ctex = cursor_down;
                            break;
                        case MouseCursor.RightArrow:
                            hsp = new Point(31, 24);
                            ctex = cursor_right;
                            break;
                        case MouseCursor.UpArrow:
                            hsp = new Point(25, 4);
                            ctex = cursor_up;
                            break;
                        case MouseCursor.DownLeftArrow:
                            hsp = new Point(8, 34);
                            ctex = cursor_dl;
                            break;
                        case MouseCursor.DownRightArrow:
                            hsp = new Point(34, 34);
                            ctex = cursor_dr;
                            break;
                        case MouseCursor.UpLeftArrow:
                            hsp = new Point(7, 7);
                            ctex = cursor_ul;
                            break;
                        case MouseCursor.UpRightArrow:
                            hsp = new Point(35, 8);
                            ctex = cursor_ur;
                            break;
                    }

                    sprite.Draw(ctex, MathEx.Clamp(0, Program.ScreenWidth, MouseInput.X) - hsp.X, 
                        MathEx.Clamp(0, Program.ScreenHeight, MouseInput.Y) - hsp.Y, ColorValue.White);
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
                    #region 屏幕边缘滚动视野
                    const int ScrollPadding = 3;
                    RtsCamera camera = parent.Scene.Camera;

                    camera.Height += MouseInput.DScrollWheelValue * 0.05f;
                    cursorState = MouseCursor.Normal;
                    if (MouseInput.X <= ScrollPadding)
                    {
                        camera.MoveLeft();
                        cursorState |= MouseCursor.LeftArrow;
                    }
                    if (MouseInput.X >= Program.Window.ClientSize.Width - ScrollPadding)
                    {
                        camera.MoveRight();
                        cursorState |= MouseCursor.RightArrow;
                    }
                    if (MouseInput.Y <= ScrollPadding)
                    {
                        camera.MoveFront();
                        cursorState |= MouseCursor.UpArrow;
                    }
                    if (MouseInput.Y >= Program.Window.ClientSize.Height - ScrollPadding)
                    {
                        camera.MoveBack();
                        cursorState |= MouseCursor.DownArrow;
                    }

                    #endregion

                    pieceMaker.Update(time);

                    physWorld.Update(time);
                    infoUI.SelectedObject = picker.SelectedObject;
                    cityEdit.SelectedCity = picker.SelectedCity;

                    base.Update(time);
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
