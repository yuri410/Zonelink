using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Code2015.EngineEx;
using Code2015.World;

namespace Code2015.GUI
{
    class Picker : UIComponent
    {
        GameScene scene;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;
        GameState logic;
        RtsCamera camera;

        CityObject mouseHoverCity;

        ISelectableObject selectedCity;


        Ray selectRay;

        public Ray SelectionRay
        {
            get { return selectRay; }
        }

        public ISelectableObject SelectedObject
        {
            get { return selectedCity; }
            private set 
            {
                if (selectedCity != value) 
                {
                    if (selectedCity != null)
                    {
                        selectedCity.IsSelected = false;
                    }

                    selectedCity = value;

                    if (selectedCity != null)
                    {
                        selectedCity.IsSelected = true;
                    }
                }
            }
        }
        public CityObject SelectedCity
        {
            get;
            private set;
        }
        public ISelectableObject MouseHoverObject
        {
            get;
            private set;

        }
        public CityObject MouseHoverCity
        {
            get { return mouseHoverCity; }
            private set
            {
                mouseHoverCity = value;
            }
        }

        public Picker(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.parent = parent;
            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;
            this.logic = gamelogic;
            this.camera = scene.Camera;
        }

        public override bool HitTest(int x, int y)
        {
            return true;
        }
        public override int Order
        {
            get { return 0; }
        }

        public override void Render(Sprite sprite)
        {

        }
        public override void Update(GameTime time)
        {
            Vector3 mp = new Vector3(MouseInput.X, MouseInput.Y, 0);
            Vector3 start = renderSys.Viewport.Unproject(mp, camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);
            mp.Z = 1;
            Vector3 end = renderSys.Viewport.Unproject(mp, camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);
            Vector3 dir = end - start;
            dir.Normalize();

            selectRay = new Ray(start, dir);
        }

        Point mouseRightPosition;

        public override void UpdateInteract(GameTime time)
        {
            SceneObject obj = parent.Scene.Scene.FindObject(selectRay, SelFilter.Instance);
            MouseHoverObject = obj as ISelectableObject;
            MouseHoverCity = MouseHoverObject as CityObject;

            if (MouseInput.IsMouseDownLeft)
            {
                SelectedObject = MouseHoverObject;
                SelectedCity = MouseHoverCity;
            }
            if (MouseInput.IsMouseDownRight)
            {
                mouseRightPosition.X = MouseInput.X;
                mouseRightPosition.Y = MouseInput.Y;
            }

            if (MouseInput.IsRightPressed)
            {
                if (MouseInput.X != mouseRightPosition.X && MouseInput.Y != mouseRightPosition.Y)
                {
                    int dx = MouseInput.X - mouseRightPosition.X;
                    int dy = MouseInput.Y - mouseRightPosition.Y;

                    if (dx > 10) dx = 10;
                    if (dx < -10) dx = -10;
                    if (dy > 10) dy = 10;
                    if (dy < -10) dy = -10;

                    camera.Move(dx * -0.1f, dy * -0.1f);
                }
            }
        }
    }
}
