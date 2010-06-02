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
http://www.gnu.org/copyleft/lesser.txt.

-----------------------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Code2015.BalanceSystem;
using Code2015.Logic;
using Code2015.World;
using Code2015.World.Screen;

namespace Code2015.GUI
{
    class GoalIcons : UIComponent
    {
        MdgResourceManager resources;
        CityInfoDisplay cityInfo;

        ScreenPhysicsWorld physWorld;

        GameScene scene;
        Player player;

        IMdgSelection selectedItem;

        IMdgSelection SelectedItem
        {
            get { return selectedItem; }
            set
            {
                if (selectedItem != null)
                    selectedItem.IsSelected = false;

                selectedItem = value;

                if (selectedItem != null)
                    selectedItem.IsSelected = true;
            }
        }

        Brackets brackets;

        ScreenStaticBody panel;
        ScreenStaticBody statBar;

        InGameUI parent;
        PieceContainer container;

        public MdgResourceManager Manager
        {
            get { return resources; }
        }

        public ScreenPhysicsWorld PhysicsWorld
        {
            get { return physWorld; }
        }

        public void SetPieceContainer(PieceContainer cont) 
        {
            this.container = cont;

        }

        public GoalIcons(Game game, InGameUI parent, CityInfoDisplay cityInfo, GameScene scene, ScreenPhysicsWorld physWorld, Brackets brackets)
        {
            this.brackets = brackets;
            this.cityInfo = cityInfo;
            this.parent = parent;
            this.physWorld = physWorld;
            this.physWorld.BodyCollision += Phys_Collision;
            this.resources = new MdgResourceManager();
            this.player = game.HumanPlayer;
            this.scene = scene;

            MdgResource res = new MdgResource(resources, physWorld, (MdgType.Environment), new Vector2(778, 669), 0);
            resources.Add(res);

            res = new MdgResource(resources, physWorld, (MdgType.Diseases), new Vector2(1084, 669), 0);
            resources.Add(res);

            res = new MdgResource(resources, physWorld, (MdgType.Hunger), new Vector2(984, 669), 0);
            resources.Add(res);

            res = new MdgResource(resources, physWorld, (MdgType.Education), new Vector2(881, 669), 0);
            resources.Add(res);





            panel = new ScreenStaticBody();

            statBar = new ScreenStaticBody();

        }

        public override void Render(Sprite sprite)
        {
            for (MdgType i = MdgType.Hunger; i < MdgType.Count; i++)
            {
                int cnt = resources.GetResourceCount(i);
                for (int j = 0; j < cnt; j++)
                {
                    MdgResource res = resources.GetResource(i, j);
                    res.Render(sprite);
                }

                cnt = resources.GetPieceCount(i);
                for (int j = 0; j < cnt; j++)
                {
                    MdgPiece piece = resources.GetPiece(i, j);
                    piece.Render(sprite);
                }
            }
        }


        IMdgSelection TrySelect(int x, int y)
        {
            IMdgSelection result = null;
            bool passed = false;
            for (MdgType i = MdgType.Hunger; i < MdgType.Count && !passed; i++)
            {
                int cnt = resources.GetResourceCount(i);
                for (int j = 0; j < cnt; j++)
                {
                    MdgResource res = resources.GetResource(i, j);
                    if (res != SelectedItem && res.HitTest(x, y))
                    {
                        result = res;
                        passed = true;
                        break;
                    }
                }

                if (passed)
                    break;

                cnt = resources.GetPieceCount(i);
                for (int j = 0; j < cnt; j++)
                {
                    MdgPiece res = resources.GetPiece(i, j);
                    if (res != SelectedItem && res.HitTest(x, y))
                    {
                        result = res;
                        passed = true;
                        break;
                    }
                }
            }
            return result;
        }

        bool Phys_Collision(ScreenRigidBody a, ScreenRigidBody b)
        {
            MdgPiece left = a.Tag as MdgPiece;
            MdgPiece right = b.Tag as MdgPiece;

            if (left != null && right != null)
            {
                if (left.CheckMerge(right))
                {
                    object result = left.Merge(right);

                    MdgPiece r1 = result as MdgPiece;
                    if (r1 != null)
                    {
                        //SelectedItem = r1;
                        return true;
                    }

                    MdgResource r2 = result as MdgResource;
                    if (r2 != null)
                    {
                        //SelectedItem = r2;
                        return true;
                    }
                }
            }
            return false;
        }


        void MouseSelectItem()
        {
            SelectedItem = TrySelect(MouseInput.X, MouseInput.Y);

            //IsMouseOver = SelectedItem != null;
        }
        void MouseDrag()
        {
            if (SelectedItem != null)
            {
                SelectedItem.Velocity = Vector2.Zero;
                SelectedItem.Position += new Vector2(MouseInput.DX, MouseInput.DY);

                MdgResource piece = SelectedItem as MdgResource;
                if (piece != null)
                {
                    for (int i = 0; i < scene.VisibleCityCount; i++)
                    {
                        CityObject cc = scene.GetVisibleCity(i);
                        if (cc.Owner == player)
                        {
                            if (brackets.CheckAutoStick(cc, piece))
                                break;
                        }
                    }
                }
                //IsMouseOver = true;
            }

        }
        void MouseDragEnd(GameTime time)
        {
            if (SelectedItem != null)
            {
                MdgResource piece = SelectedItem as MdgResource;

                if (piece != null)
                {
                    if (PieceContainer.IsInRange(piece.Position))
                    {
                        container.Exchange(piece);
                    }
                    else
                    {
                        for (int i = 0; i < scene.VisibleCityCount; i++)
                        {
                            CityObject cc = scene.GetVisibleCity(i);
                            if (cc.Owner == player)
                            {
                                if (brackets.Accept(cc, piece))
                                {
                                    resources.Remove(piece);
                                    cc.Flash(60);
                                    return;
                                }
                            }
                        }
                    }
                }

                float dt = time.ElapsedGameTimeSeconds;
                if (dt > float.Epsilon)
                    SelectedItem.Velocity = new Vector2(MouseInput.DX, MouseInput.DY) / (2 * dt);

                SelectedItem = null;

            }
        }
        public override bool HitTest(int x, int y)
        {
            return SelectedItem != null || TrySelect(x, y) != null;
        }
        public override int Order
        {
            get
            {
                return 9;
            }
        }
        public override string ToString()
        {
            return base.ToString();
        }
        public override void UpdateInteract(GameTime time)
        {
            if (MouseInput.IsMouseDownLeft)
            {
                MouseSelectItem();
            }

            if (MouseInput.IsLeftPressed)
            {
                MouseDrag();
            }
            else if (MouseInput.IsMouseUpLeft)
            {
                MouseDragEnd(time);
            }
        }

        public override void Update(GameTime time)
        {
            resources.Update(time);


        }
    }
}
