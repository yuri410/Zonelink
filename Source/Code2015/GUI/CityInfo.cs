using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.BalanceSystem;
using Code2015.EngineEx;
using Code2015.GUI.Controls;
using Code2015.Logic;
using Code2015.World;
using Code2015.World.Screen;

namespace Code2015.GUI
{
    class Brackets : UIComponent
    {
        RenderSystem renderSys;

        CityInfoDisplay display;
        CityInfo parent;
        CityObject city;

        Texture[] darkPieces;
        Texture[] pieces;

        public Brackets(CityInfoDisplay info, CityInfo parent, RenderSystem rs, CityObject city)
        {
            this.display = info;
            this.parent = parent;
            this.city = city;
            this.renderSys = rs;

            FileLocation fl = FileSystem.Instance.Locate("ig_bracket.tex", GameFileLocs.GUI);
            pieces = new Texture[4];
            pieces[0] = UITextureManager.Instance.CreateInstance(fl);

            darkPieces = new Texture[4];
            darkPieces[0] = UITextureManager.Instance.CreateInstance(fl);
        }

        Point GetPluginProjPosition(int i)
        {
            Vector3 ppofs = new Vector3(50, 250, 0);
            ppofs += city.GetPluginPosition(i);

            Vector3 plpos;
            Vector3.TransformSimple(ref ppofs, ref city.Transformation, out plpos);

            plpos = renderSys.Viewport.Project(plpos, display.Projection, display.View, Matrix.Identity);
            return new Point((int)plpos.X, (int)plpos.Y);
        }


        public override void Render(Sprite sprite)
        {
            for (int i = 0; i < city.PluginCount; i++)
            {
                Point pt = GetPluginProjPosition(i);

                sprite.Draw(darkPieces[0], pt.X, pt.Y, parent.DistanceMod);
            }
        }


        public bool Accept(MdgResource res)
        {
            for (int i = 0; i < city.PluginCount; i++)
            {
                Point pt = GetPluginProjPosition(i);

                Vector2 pos = res.Position;
                float dx = pos.X - pt.X + MdgPhysicsParams.BallRadius;
                float dy = pos.Y - pt.Y + MdgPhysicsParams.BallRadius;

                float len = (float)Math.Sqrt(dx * dx + dy * dy);

                if (len < MdgPhysicsParams.BallRadius)
                {
                    if (city.MatchPiece(i, res.Type))
                    {
                        city.SetPiece(i, res);
                        return true;
                    }
                }
            }
            return false;
        }

        public override void Update(GameTime time)
        {
        }
    }

    class PluginInfo : UIComponent
    {
        RenderSystem renderSys;

        CityObject city;
        CityInfo parent;
        CityInfoDisplay display;
        ProgressBar upgrade;

        public int Plugin
        {
            get;
            set;
        }

        public PluginInfo(CityInfoDisplay info, CityInfo parent, RenderSystem rs, CityObject city)
        {
            this.display = info;
            this.city = city;
            this.renderSys = rs;
            this.parent = parent;

            FileLocation fl = FileSystem.Instance.Locate("ig_prgbar_vert_cmp.tex", GameFileLocs.GUI);
            Texture prgBg = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_prgbar_vert_imp.tex", GameFileLocs.GUI);
            Texture prgBg1 = UITextureManager.Instance.CreateInstance(fl);

            upgrade = new ProgressBar();

            upgrade.Height = 117;
            upgrade.Width = 18;
            upgrade.Direction = ControlDirection.Vertical;
            upgrade.ProgressImage = prgBg;
            upgrade.Background = prgBg1;
        }

        public override void Render(Sprite sprite)
        {
            CityPlugin cplug = city.GetPlugin(Plugin);

            Vector3 plpos;
            Vector3 ppofs = new Vector3(60, 0, 0);
            ppofs += city.GetPluginPosition(Plugin);

            Vector3.TransformSimple(ref ppofs, ref city.Transformation, out plpos);

            plpos = renderSys.Viewport.Project(plpos, display.Projection, display.View, Matrix.Identity);

            upgrade.X = (int)plpos.X;
            upgrade.Y = (int)plpos.Y - 50;

            upgrade.ModulateColor = parent.DistanceMod;
            upgrade.Value = cplug.IsBuilding ? cplug.BuildProgress : cplug.GetLevelProgress();
            upgrade.Render(sprite);

        }

        public override void Update(GameTime time)
        {
        }
    }

    class CityInfo : UIComponent
    {
        RenderSystem renderSys;
        Font font;

        CityInfoDisplay parent;
        CityObject city;
        Player player;


        ProgressBar satisfy;
        PluginInfo[] pluginInfo = new PluginInfo[CityGrade.LargePluginCount];
        Brackets brackets;



        public ColorValue DistanceMod;

        public CityInfo(CityInfoDisplay info, RenderSystem rs, CityObject city, Player player)
        {
            this.font = FontManager.Instance.GetFont("default");

            this.parent = info;
            this.city = city;
            this.brackets = new Brackets(info, this, rs, city);
            this.renderSys = rs;
            this.player = player;


            FileLocation fl = FileSystem.Instance.Locate("ig_prgbar_cmp.tex", GameFileLocs.GUI);
            Texture prgBg = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_prgbar_imp.tex", GameFileLocs.GUI);
            Texture prgBg1 = UITextureManager.Instance.CreateInstance(fl);

            satisfy = new ProgressBar();

            satisfy.Height = 18;
            satisfy.Width = 117;
            satisfy.ProgressImage = prgBg;
            satisfy.Background = prgBg1;


            for (int i = 0; i < pluginInfo.Length; i++)
            {
                pluginInfo[i] = new PluginInfo(info, this, rs, city);
            }
        }

        public Brackets Bracket 
        {
            get { return brackets; }
        }

        public override void Render(Sprite sprite)
        {
            Vector3 tangy = PlanetEarth.GetTangentY(MathEx.Degree2Radian(city.Longitude), MathEx.Degree2Radian(city.Latitude));

            Vector3 ppos = renderSys.Viewport.Project(city.Position - tangy * (CityStyleTable.CityRadius[(int)city.Size] + 5),
                parent.Projection, parent.View, Matrix.Identity);
            Point scrnPos = new Point((int)ppos.X, (int)ppos.Y);

            Size strSize = font.MeasureString(city.Name, 20, DrawTextFormat.Center);

            //scrnPos.Y += strSize.Height;
            scrnPos.X -= strSize.Width / 2;

            font.DrawString(sprite, city.Name, scrnPos.X + 1, scrnPos.Y + 1, 20, DrawTextFormat.Center, (int)ColorValue.Black.PackedValue);
            font.DrawString(sprite, city.Name, scrnPos.X, scrnPos.Y, 20, DrawTextFormat.Center, -1);

            if (city.IsCaptured)
            {
                satisfy.X = scrnPos.X;
                satisfy.Y = scrnPos.Y - 30;
                satisfy.Value = city.Satisfaction; 
                satisfy.Render(sprite);
            }

            if (city.Owner == player)
            {
                float dist = Vector3.Distance(city.Position, parent.CameraPosition);
                dist = 1 - MathEx.Saturate((dist - 1500) / 750);
                DistanceMod = ColorValue.White;
                DistanceMod.A = (byte)(dist * byte.MaxValue);

                if (city.IsLinked)
                {
                    parent.AddPopup(new Popup(renderSys, "Congratulations  ", scrnPos.X, scrnPos.Y, 1));
                    city.IsLinked = false;
                }

                if (((ISelectableObject)city).IsSelected)
                {
                    for (int i = 0; i < city.PluginCount; i++)
                    {
                        pluginInfo[i].Plugin = i;
                        pluginInfo[i].Render(sprite);
                    }
                }

                brackets.Render(sprite);
            }
        }

        public override void Update(GameTime time)
        {
            if (city.Owner == player)
            {
                if (((ISelectableObject)city).IsSelected)
                {
                    for (int i = 0; i < city.PluginCount; i++)
                    {
                        pluginInfo[i].Update(time);
                    }
                }
                brackets.Update(time);
            }
        }
    }
}
