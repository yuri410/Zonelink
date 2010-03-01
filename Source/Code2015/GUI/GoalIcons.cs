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



        public GoalIcons(ScreenPhysicsWorld physWorld)
        {
            this.physWorld = physWorld;
            this.physWorld.BodyCollision += Phys_Collision;
            resources = new MdgResourceManager();

            #region test

            MdgResource res = new MdgResource(resources, physWorld, MdgType.ChildMortality, new Vector2(200, 300), 1);
            resources.Add(res);
            res = new MdgResource(resources, physWorld, MdgType.Environment, new Vector2(200, 500), 0);
            resources.Add(res);
            res = new MdgResource(resources, physWorld, MdgType.Diseases, new Vector2(200, 600), 0);
            resources.Add(res);
            res = new MdgResource(resources, physWorld, MdgType.Education, new Vector2(200, 400), 0);
            resources.Add(res);
            res = new MdgResource(resources, physWorld, MdgType.GenderEquality, new Vector2(200, 700), 0);
            resources.Add(res);
            res = new MdgResource(resources, physWorld, MdgType.Hunger, new Vector2(500, 300), 0);
            resources.Add(res);
            res = new MdgResource(resources, physWorld, MdgType.MaternalHealth, new Vector2(500, 400), 0);
            resources.Add(res);



            MdgPiece pie = new MdgPiece(resources, physWorld, MdgType.Diseases, 1, new Vector2(300, 300), 0);
            resources.Add(pie);

            pie = new MdgPiece(resources, physWorld, MdgType.Diseases, 2, new Vector2(400, 300), 0);
            resources.Add(pie);

            pie = new MdgPiece(resources, physWorld, MdgType.Diseases, 4, new Vector2(600, 300), 0);
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
                        r1.Position = new Vector2(lastMousePos.X, lastMousePos.Y);
                        SelectedItem = r1;
                        return true;
                    }

                    MdgResource r2 = result as MdgResource;
                    if (r2 != null)
                    {
                        r2.Position = new Vector2(lastMousePos.X, lastMousePos.Y);
                        SelectedItem = r2;
                        return true;
                    }
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

                    SelectedItem = HitTest(state.X, state.Y);
                    
                }

                if (SelectedItem != null)
                {   
                    SelectedItem.Velocity = Vector2.Zero;
                    SelectedItem.Position += new Vector2(state.X - lastMousePos.X, state.Y - lastMousePos.Y);
                }
            }
            else if (state.LeftButton == XI.ButtonState.Released) 
            {
                if (lastMouseLeft) //是否刚松开
                {
                    if (SelectedItem != null)
                    {
                        float dt = time.ElapsedGameTimeSeconds;
                        if (dt > float.Epsilon)
                            SelectedItem.Velocity = new Vector2(state.X - lastMousePos.X, state.Y - lastMousePos.Y) / (2 * dt);

                        SelectedItem = null;
                    }
                    lastMouseLeft = false;
                }
            }

            lastMousePos.X = state.X;
            lastMousePos.Y = state.Y;

        }
    }
}
