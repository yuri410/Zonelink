using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Code2015.Logic;
using Code2015.World.Screen;

namespace Code2015.GUI
{
    class GoalIcons : UIComponent
    {
        MdgResourceManager resources;

        ScreenPhysicsWorld physWorld;

        Player player;
        //Point lastMousePos;
        //bool lastMouseLeft;
        IMdgSelection selectedItem;

        IMdgSelection SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                if (value != null)
                {
                    resources.SetPrimary(value);
                }

            }
        }

        ScreenStaticBody panel;
        ScreenStaticBody statBar;

        InGameUI parent;

        public GoalIcons(Game game, InGameUI parent, ScreenPhysicsWorld physWorld)
        {
            this.parent = parent;
            this.physWorld = physWorld;
            this.physWorld.BodyCollision += Phys_Collision;
            this.resources = new MdgResourceManager();
            this.player = game.HumanPlayer;

            #region test


            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                {
                    int t = Randomizer.GetRandomInt((int)MdgType.Partnership);
                    MdgResource res = new MdgResource(resources, physWorld, (MdgType)t, new Vector2(i * 100 + 400, j * 100), 0);
                    resources.Add(res);
                }
           
            #endregion


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

                for (int k = 1; k < 8; k++)
                {
                    cnt = resources.GetPieceCount(i, k);
                    for (int j = 0; j < cnt; j++)
                    {
                        MdgPiece piece = resources.GetPiece(i, k, j);
                        piece.Render(sprite);
                    }
                }
            }
        }


        IMdgSelection HitTest(int x, int y)
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

                for (int k = 1; k < 8; k++)
                {
                    cnt = resources.GetPieceCount(i, k);
                    for (int j = 0; j < cnt; j++)
                    {
                        MdgPiece res = resources.GetPiece(i, k, j);
                        if (res != SelectedItem && res.HitTest(x, y))
                        {
                            result = res;
                            passed = true;
                            break;
                        }
                    }
                }
            }
            return result;
        }

        bool Phys_Collision(ScreenRigidBody a, ScreenRigidBody b)
        {
            MdgPiece left = a.Tag as MdgPiece;
            MdgPiece right = b.Tag as MdgPiece;

            if (left != null && right != null &&
                (object.ReferenceEquals(left, SelectedItem) || object.ReferenceEquals(right, SelectedItem)))
            {
                if (left.CheckMerge(right))
                {
                    object result = left.Merge(right);

                    MdgPiece r1 = result as MdgPiece;
                    if (r1 != null)
                    {
                        r1.Position = new Vector2(MouseInput.X, MouseInput.Y);
                        SelectedItem = r1;
                        return true;
                    }

                    MdgResource r2 = result as MdgResource;
                    if (r2 != null)
                    {
                        r2.Position = new Vector2(MouseInput.X, MouseInput.Y);
                        SelectedItem = r2;
                        return true;
                    }
                }
            }
            return false;
        }


        public override void Interact(GameTime time)
        {
            if (MouseInput.IsMouseDownLeft)
            {
                SelectedItem = HitTest(MouseInput.X, MouseInput.Y);
            }

            if (MouseInput.IsLeftPressed)
            {
                if (SelectedItem != null)
                {
                    SelectedItem.Velocity = Vector2.Zero;
                    SelectedItem.Position += new Vector2(MouseInput.DX, MouseInput.DY);
                }
            }
            else if (MouseInput.IsMouseUpLeft)
            {
                if (SelectedItem != null)
                {
                    // 检查是否在城市之上
                    if (!object.ReferenceEquals(parent.MouseHoverCity, null))
                    {
                        if (object.ReferenceEquals(parent.MouseHoverCity.Owner, player))
                        {

                            MdgResource piece = SelectedItem as MdgResource;
                            if (piece != null)
                            {
                                resources.Remove(piece);
                                parent.MouseHoverCity.Flash(60);
                                return;
                            }
                        }
                    }
                    else
                    {
                        float dt = time.ElapsedGameTimeSeconds;
                        if (dt > float.Epsilon)
                            SelectedItem.Velocity = new Vector2(MouseInput.DX, MouseInput.DY) / (2 * dt);

                        SelectedItem = null;
                    }
                }
            }
        }
        public override void Update(GameTime time)
        {
            resources.Update(time);

        }
    }
}
