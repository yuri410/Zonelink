using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Code2015.World;
using Code2015.World.Screen;
using Code2015.EngineEx;

namespace Code2015.GUI
{
    class Brackets : UIComponent
    {
        RenderSystem renderSys;
        GameScene scene;

        public Brackets(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.scene = scene;
        }

        Point GetSiteProjPosition(CityObject city, int i)
        {
            Vector3 ppofs = CityStyleTable.SiteTransform[i].TranslationValue;

            Vector3 plpos;
            Vector3.TransformSimple(ref ppofs, ref city.Transformation, out plpos);

            RtsCamera camera = scene.Camera;

            plpos = renderSys.Viewport.Project(plpos, camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);
            return new Point((int)plpos.X, (int)plpos.Y);
        }
        public override int Order
        {
            get
            {
                return 1;
            }
        }

        public void CheckAutoStick(CityObject city, MdgResource res)
        {
            for (int i = 0; i < CityGoalSite.SiteCount; i++)
            {
                Point pt = GetSiteProjPosition(city, i);
                Vector2 pos = res.Position;
                float dx = pos.X - pt.X;
                float dy = pos.Y - pt.Y;

                float len = (float)Math.Sqrt(dx * dx + dy * dy);

                if (len < MdgPhysicsParams.BallRadius)
                {
                    if (res.AutoStick == null)
                        res.AutoStick = new PieceAutoStick(new Vector2(pt.X, pt.Y));
                    break;
                }
                else if (res.AutoStick != null)
                {
                    res.AutoStick = null;
                }
            }
        }
        public bool Accept(CityObject city, MdgResource res)
        {
            CityGoalSite site = city.GoalSite;
            for (int i = 0; i < CityGoalSite.SiteCount; i++)
            {
                Point pt = GetSiteProjPosition(city, i);

                Vector2 pos = res.Position;
                float dx = pos.X - pt.X;
                float dy = pos.Y - pt.Y;

                float len = (float)Math.Sqrt(dx * dx + dy * dy);

                if (len < MdgPhysicsParams.BallRadius)
                {

                    if (site.MatchPiece(i, res.Type))
                    {
                        site.SetPiece(i, res.Type);
                        return true;
                    }
                }
                //else if (res.AutoStick != null) { res.AutoStick = null; }
            }
            return false;
        }
    }
}
