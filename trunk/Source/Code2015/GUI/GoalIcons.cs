using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Code2015.World.Screen;
using XI = Microsoft.Xna.Framework.Input;

namespace Code2015.GUI
{
    class GoalIcons : UIComponent
    {
        MdgResourceManager resources;

        ScreenPhysicsWorld physWorld;

        Point lastMousePos;
        bool lastMouseLeft;
        IMdgSelection selectedItem;

        public GoalIcons(ScreenPhysicsWorld physWorld)
        {
            this.physWorld = physWorld;
            this.physWorld.BodyCollision += Phys_Collision;
            resources = new MdgResourceManager();

            #region test

            MdgResource res = new MdgResource(physWorld, MdgType.ChildMortality, new Vector2(300, 300), 1);
            resources.Add(res);
            res = new MdgResource(physWorld, MdgType.Environment, new Vector2(600, 300), 0);
            resources.Add(res);

            MdgPiece pie = new MdgPiece(resources, physWorld, MdgType.Diseases, 1, new Vector2(200, 300), 0);
            resources.Add(pie);
            pie = new MdgPiece(resources, physWorld, MdgType.Diseases, 2, new Vector2(400, 300), 0);
            resources.Add(pie);

            #endregion
        }

        public override void Render(Sprite sprite)
        {
            for (MdgType i = MdgType.Hunger; i < MdgType.Count; i++)
            {
                int cnt = resources.GetResourceCount(i);
                for (int j = 0; j < cnt; j++)
                {
                    MdgResource res = resources.GetResource(i, j);
                    res.IsPrimary = j == 0;
                    res.Render(sprite);
                }

                for (int k = 1; k < 8; k++)
                {
                    cnt = resources.GetPieceCount(i, k);
                    for (int j = 0; j < cnt; j++)
                    {
                        MdgPiece piece = resources.GetPiece(i, k, j);
                        piece.IsPrimary = j == 0;
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
                    if (res != selectedItem && res.HitTest(x, y))
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
                        if (res != selectedItem && res.HitTest(x, y))
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
                (object.ReferenceEquals(left, selectedItem) || object.ReferenceEquals(right, selectedItem)))
            {
                if (left.CheckMerge(right))
                {
                    object result = left.Merge(right);

                    MdgPiece r1 = result as MdgPiece;
                    if (r1 != null)
                    {
                        resources.Add(r1);
                        selectedItem = r1;
                    }

                    MdgResource r2 = result as MdgResource;
                    if (r2 != null)
                    {
                        resources.Add(r2);
                        selectedItem = r2;
                    }

                    return true;
                }
            }
            return false;
        }

        public override void Update(GameTime time)
        {
            XI.MouseState state = XI.Mouse.GetState();

            if (state.LeftButton == XI.ButtonState.Pressed)
            {
                if (!lastMouseLeft)//是否刚按下
                {
                    lastMouseLeft = true;

                    selectedItem = HitTest(state.X, state.Y);
                }

                if (selectedItem != null)
                {
                    selectedItem.Velocity = Vector2.Zero;
                    selectedItem.Position += new Vector2(state.X - lastMousePos.X, state.Y - lastMousePos.Y);
                }
            }
            else if (state.LeftButton == XI.ButtonState.Released) 
            {
                if (lastMouseLeft) //是否刚松开
                {
                    if (selectedItem != null)
                    {
                        float dt = time.ElapsedGameTimeSeconds;
                        if (dt > float.Epsilon)
                            selectedItem.Velocity = new Vector2(state.X - lastMousePos.X, state.Y - lastMousePos.Y) / (2 * dt);

                        selectedItem = null;
                    }
                    lastMouseLeft = false;
                }
            }

            lastMousePos.X = state.X;
            lastMousePos.Y = state.Y;

        }
    }
}
