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
        MdgResource selectedItem;


        public GoalIcons(ScreenPhysicsWorld physWorld) 
        {
            this.physWorld = physWorld;
            resources = new MdgResourceManager();

            // test
            MdgResource res = new MdgResource(physWorld, MdgType.ChildMortality, new Vector2(300, 300), 1);
            resources.Add(res);
            res = new MdgResource(physWorld, MdgType.Environment, new Vector2(600, 300), 0);
            resources.Add(res);
        }

        public override void Render(Sprite sprite)
        {
            for (MdgType i = MdgType.Hunger; i < MdgType.Count; i++)
            {
                int cnt = resources.GetResourceCount(i);
                for (int j = 0; j < cnt; j++)
                {
                    if (j == 0)
                    {

                    }
                    resources.GetResource(i, j).Render(sprite);
                }

                for (int k = 1; k < 8; k++)
                {
                    cnt = resources.GetPieceCount(i, k);
                    for (int j = 0; j < cnt; j++)
                    {
                        resources.GetPiece(i, k, j).Render(sprite);
                    }
                }
            }
        }
        public override void Update(GameTime time)
        {
            XI.MouseState state = XI.Mouse.GetState();

            if (state.LeftButton == XI.ButtonState.Pressed)
            {
                if (!lastMouseLeft)//是否刚按下
                {
                    lastMouseLeft = true;

                    for (MdgType i = MdgType.Hunger; i < MdgType.Count; i++)
                    {
                        int cnt = resources.GetResourceCount(i);
                        for (int j = 0; j < cnt; j++)
                        {
                            MdgResource res = resources.GetResource(i, j);
                            if (res.HitTest(state.X, state.Y))
                            {
                                i = MdgType.Count;
                                selectedItem = res;
                                break;
                            }
                        }
                    }
                }

                if (selectedItem != null)
                {
                    selectedItem.Velocity = Vector2.Zero;
                    selectedItem.Position += new Vector2(state.X - lastMousePos.X, state.Y - lastMousePos.Y);
                }

                //bool passed = false;
                //for (MdgType i = MdgType.Hunger; i < MdgType.Count; i++)
                //{
                //    int cnt = resources.GetResourceCount(i);
                //    for (int j = 0; j < cnt; j++)
                //    {
                //        MdgResource res = resources.GetResource(i, j);
                //        if (res.HitTest(state.X, state.Y))
                //        {
                //            res.Position += new Vector2(state.X - lastMousePos.X, state.Y - lastMousePos.Y);
                //            passed = true;
                //            break;
                //        }
                //    }

                //    if (passed)
                //        break;

                //    for (int k = 1; k < 8; k++)
                //    {
                //        cnt = resources.GetPieceCount(i, k);
                //        for (int j = 0; j < cnt; j++)
                //        {
                //            MdgPiece res = resources.GetPiece(i, k, j);
                //            passed = true;
                //            break;
                //        }
                //    }
                //    if (passed)
                //        break;
                //}
            }
            else if (state.LeftButton == XI.ButtonState.Released) 
            {
                if (lastMouseLeft) //是否刚松开
                {
                    if (selectedItem != null)
                    {
                        float dt = time.ElapsedRealTime;
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
