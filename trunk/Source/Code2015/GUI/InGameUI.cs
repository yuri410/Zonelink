using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
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
        Font font;

        GoalIcons icons;

        ScreenPhysicsWorld physWorld;
        Texture background;
        Texture progressBarImp;
        Texture progressBarCmp;


        LinkUI linkUI;

        Texture cursor;
        Texture lds_ball;
        Point mousePosition;

        InGameUI2 ingameui2;

        Player player;

        CityObject mouseHoverCity;

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

            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;
            this.physWorld = new ScreenPhysicsWorld();

            this.player = parent.HumanPlayer;

            this.icons = new GoalIcons(parent, this, physWorld);

            FileLocation fl = FileSystem.Instance.Locate("def.fnt", GameFileLocs.GUI);
            font = FontManager.Instance.CreateInstance(renderSys, fl, "default");

            fl = FileSystem.Instance.Locate("cursor.tex", GameFileLocs.GUI);
            cursor = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("lds_bg.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("lds_prgcmp.tex", GameFileLocs.GUI);
            progressBarCmp = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("lds_prgimp.tex", GameFileLocs.GUI);
            progressBarImp = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("lds_ball.tex", GameFileLocs.GUI);
            lds_ball = UITextureManager.Instance.CreateInstance(fl);


            this.ingameui2 = new InGameUI2(game, parent, scene, gamelogic);
            this.linkUI = new LinkUI(game, parent, scene);
        }

        public override void Render(Sprite sprite)
        {
            if (!parent.IsLoaded)
            {
                sprite.Draw(background, 0, 0, ColorValue.LightGray);

                font.DrawString(sprite, "Loading", 0, 0, 34, DrawTextFormat.Center, (int)ColorValue.Black.PackedValue);


                sprite.Draw(progressBarImp, 15, 692, ColorValue.White);


                Rectangle srect = new Rectangle(0, 0, (int)(progressBarCmp.Width * parent.LoadingProgress), progressBarCmp.Height);
                Rectangle drect = new Rectangle(15, 692, srect.Width, progressBarCmp.Height);
               
                
                sprite.Draw(progressBarCmp, drect, srect, ColorValue.White);
                int x = srect.Width + 15 - 60;
                ColorValue c = ColorValue.White;
                c.A = 189;
                sprite.Draw(lds_ball, x, 657, c);

            }
            else
            {

                icons.Render(sprite);
                sprite.SetTransform(Matrix.Identity);
                ingameui2.Render(sprite);


                sprite.Draw(cursor, mousePosition.X, mousePosition.Y, ColorValue.White);
            }
        }

        public override void Update(GameTime time)
        {
            physWorld.Update(time);

            mousePosition.X = MouseInput.X;
            mousePosition.Y = MouseInput.Y;

            icons.Update(time);
            ingameui2.Update(time);

            RtsCamera camera = parent.Scene.Camera;

            camera.Height += MouseInput.DScrollWheelValue * 0.05f;


            if (MouseInput.X <= 0)
            {
                camera.MoveLeft();
            }
            if (MouseInput.X >= Program.Window.ClientSize.Width)
            {
                camera.MoveRight();
            }
            if (MouseInput.Y <= 0)
            {
                camera.MoveFront();
            }
            if (MouseInput.Y >= Program.Window.ClientSize.Height)
            {
                camera.MoveBack();
            }

            Vector3 mp = new Vector3(mousePosition.X, mousePosition.Y, 0);
            Vector3 start = renderSys.Viewport.Unproject(mp, camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);
            mp.Z = 1;
            Vector3 end = renderSys.Viewport.Unproject(mp, camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);
            Vector3 dir = end - start;
            dir.Normalize();

            SceneObject obj = parent.Scene.Scene.FindObject(new Ray(start, dir), SelFilter.Instance);
            if (obj != null)
            {
                ISelectableObject sel = obj as ISelectableObject;

                MouseHoverCity = sel as CityObject;

                if (MouseInput.IsMouseDownLeft)
                {
                    ingameui2.SelectedObject = sel;
                    linkUI.SelectedCity = MouseHoverCity;
                }
                else if (MouseInput.IsLeftPressed)
                {
                    linkUI.HoverCity = MouseHoverCity;

                    BoundingSphere earthSphere = new BoundingSphere(new Vector3(), PlanetEarth.PlanetRadius);

                    Vector3 intersect;
                    if (BoundingSphere.Intersects(earthSphere, new Ray(start, dir), out intersect))
                    {
                        linkUI.HoverPoint = intersect;
                    }


                }
                else
                {
                    linkUI.HoverCity = null;
                }
            }
            else
            {
                linkUI.HoverCity = null;
                MouseHoverCity = null;
            }


            linkUI.Update(time);
        }
    }
}
