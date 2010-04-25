using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Code2015.EngineEx;
using Code2015.Logic;
using Code2015.World;
using Code2015.World.Screen;

namespace Code2015.GUI
{
    class Brackets : UIComponent
    {
        RenderSystem renderSys;
        GameScene scene;
        Player player;
        Picker picker;
        GoalIcons icons;

        bool hitting;
        CityObject hittingCity;

        public void SetGoalIcons(GoalIcons icos) 
        {
            icons = icos;
        }
        public Brackets(Code2015 game, Game parent, GameScene scene, GameState gamelogic, Picker picker)
        {
            this.scene = scene;
            this.renderSys = game.RenderSystem;
            this.player = parent.HumanPlayer;
            this.picker = picker;
        }

        BoundingSphere GetBoundingSphere(CityObject city, int i)
        {
            Vector3 ppofs = CityStyleTable.SiteTransform[i].TranslationValue;

            Vector3 plpos;
            Vector3.TransformSimple(ref ppofs, ref city.Transformation, out plpos);
            return new BoundingSphere(plpos, 40);
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

        public bool CheckAutoStick(CityObject city, MdgResource res)
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
                    return true;
                }
                else if (res.AutoStick != null)
                {
                    res.AutoStick = null;
                }
            }
            return false;
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


        public bool IntersectsMouseHas(CityObject city)
        {
            CityGoalSite site = city.GoalSite;
            for (int i = 0; i < CityGoalSite.SiteCount; i++)
            {
                site.SetHighlight(i, false);
                if (site.HasPiece(i))
                {
                    BoundingSphere sphere = GetBoundingSphere(city, i);
                    Ray ray = picker.SelectionRay;
                    bool r = MathEx.BoundingSphereIntersects(ref sphere, ref ray);
                    site.SetHighlight(i, r);
                    return r;
                }
            }
            return false;
        }

        public override void UpdateInteract(GameTime time)
        {
            if (MouseInput.IsMouseUpLeft)
            {
                MdgResourceManager manager = icons.Manager;

                // 检查取下
                if (hittingCity != null)
                {
                    CityGoalSite site = hittingCity.GoalSite;
                    for (int i = 0; i < CityGoalSite.SiteCount; i++)
                    {
                        if (site.HasPiece(i))
                        {
                            BoundingSphere sphere = GetBoundingSphere(hittingCity, i);
                            Ray ray = picker.SelectionRay;
                            if (MathEx.BoundingSphereIntersects(ref sphere, ref ray))
                            {
                                Point pt = GetSiteProjPosition(hittingCity, i);

                                MdgResource resource = new MdgResource(manager, icons.PhysicsWorld, site.GetPieceType(i), new Vector2(pt.X, pt.Y), 0);
                                manager.Add(resource);

                                site.ClearAt(i);
                                return;
                            }
                        }
                    }
                }
            }
        }
        public override bool HitTest(int x, int y)
        {
            return hitting;
        }

        public override void Update(GameTime time)
        {
            hittingCity = null;
            hitting = false;
            for (int i = 0; i < scene.VisibleCityCount; i++) 
            {
                 CityObject cc = scene.GetVisibleCity(i);
                 if (cc.Owner == player)
                 {
                     if (IntersectsMouseHas(cc) && hittingCity == null)
                     {
                         hittingCity = cc;
                         hitting = true;
                     }
                 }
            }

        }
    }
}
