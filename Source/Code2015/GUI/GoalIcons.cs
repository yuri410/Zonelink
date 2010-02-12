using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Code2015.World.Screen;

namespace Code2015.GUI
{
    class GoalIcons : UIComponent
    {
        MdgResourceManager resources;

        ScreenPhysicsWorld physWorld;


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
            //resources.Update(time);
        }
    }
}
