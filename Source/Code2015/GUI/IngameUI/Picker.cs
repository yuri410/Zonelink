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

        City mouseHoverCity;

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
        public City SelectedCity
        {
            get;
            private set;
        }
        public ISelectableObject MouseHoverObject
        {
            get;
            private set;

        }
        public City MouseHoverCity
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

        public override void UpdateInteract(GameTime time)
        {
            SceneObject obj = parent.Scene.Scene.FindObject(selectRay, SelFilter.Instance);
            MouseHoverObject = obj as ISelectableObject;
            MouseHoverCity = MouseHoverObject as City;

            if (MouseInput.IsMouseDownLeft)
            {
                SelectedObject = MouseHoverObject;
                SelectedCity = MouseHoverCity;
            }
        }
    }
}
