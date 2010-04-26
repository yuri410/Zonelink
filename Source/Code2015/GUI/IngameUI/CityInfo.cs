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
    class PluginInfo : UIComponent
    {
        RenderSystem renderSys;

        CityObject city;
        CityInfo parent;
        CityInfoDisplay display;

        Texture woodFacbg;
        Texture oilRefbg;
        Texture hospbg;
        Texture edubg;


        //ProgressBar upgrade;

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

            FileLocation fl = FileSystem.Instance.Locate("ig_edu.tex", GameFileLocs.GUI);
            edubg = UITextureManager.Instance.CreateInstance(fl);


            fl = FileSystem.Instance.Locate("ig_wood.tex", GameFileLocs.GUI);
            woodFacbg = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_oilref.tex", GameFileLocs.GUI);
            oilRefbg = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_hospital.tex", GameFileLocs.GUI);
            hospbg = UITextureManager.Instance.CreateInstance(fl);


            //FileLocation fl = FileSystem.Instance.Locate("ig_prgbar_vert_cmp.tex", GameFileLocs.GUI);
            //Texture prgBg = UITextureManager.Instance.CreateInstance(fl);

            //fl = FileSystem.Instance.Locate("ig_prgbar_vert_imp.tex", GameFileLocs.GUI);
            //Texture prgBg1 = UITextureManager.Instance.CreateInstance(fl);

            //upgrade = new ProgressBar();

            //upgrade.Height = 117;
            //upgrade.Width = 18;
            //upgrade.Direction = ControlDirection.Vertical;
            //upgrade.ProgressImage = prgBg;
            //upgrade.Background = prgBg1;
        }

        public override void Render(Sprite sprite)
        {
            CityPlugin cplug = city.GetPlugin(Plugin);

            Vector3 plpos;
            Vector3 ppofs = new Vector3(60, 0, 0);
            ppofs += city.GetPluginPosition(Plugin);

            Vector3.TransformSimple(ref ppofs, ref city.Transformation, out plpos);

            plpos = renderSys.Viewport.Project(plpos, display.Projection, display.View, Matrix.Identity);

            int x = (int)plpos.X;
            int y = (int)plpos.Y;

            switch (cplug.TypeId)
            {
                case CityPluginTypeId.BiofuelFactory:
                case CityPluginTypeId.OilRefinary:
                    sprite.Draw(oilRefbg, x - oilRefbg.Width / 2, y - oilRefbg.Height / 2, ColorValue.White);
                    break;
                case CityPluginTypeId.WoodFactory:
                    sprite.Draw(woodFacbg, x - woodFacbg.Width / 2, y - woodFacbg.Height / 2, ColorValue.White);
                    break;
                case CityPluginTypeId.EducationOrg:
                    sprite.Draw(edubg, x - edubg.Width / 2, y - edubg.Height / 2, ColorValue.White);
                    break;
                case CityPluginTypeId.Hospital:
                    sprite.Draw(hospbg, x - hospbg.Width / 2, y - hospbg.Height / 2, ColorValue.White);
                    break;
            }
            //upgrade.X = (int)plpos.X;
            //upgrade.Y = (int)plpos.Y - 50;

            //upgrade.ModulateColor = parent.DistanceMod;
            //upgrade.Value = cplug.IsBuilding ? cplug.BuildProgress : cplug.UpgradePoint;
            //upgrade.Render(sprite);

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

        PluginInfo[] pluginInfo = new PluginInfo[CityGrade.LargePluginCount];

        public ColorValue DistanceMod;

        public CityInfo(CityInfoDisplay info, RenderSystem rs, CityObject city, Player player)
        {
            this.font = FontManager.Instance.GetFont("default");

            this.parent = info;
            this.city = city;
            //this.brackets = new Brackets(info, this, rs, city);
            this.renderSys = rs;
            this.player = player;


            //FileLocation fl = FileSystem.Instance.Locate("ig_prgbar_cmp.tex", GameFileLocs.GUI);
            //Texture prgBg = UITextureManager.Instance.CreateInstance(fl);

            //fl = FileSystem.Instance.Locate("ig_prgbar_imp.tex", GameFileLocs.GUI);
            //Texture prgBg1 = UITextureManager.Instance.CreateInstance(fl);

            //fl = FileSystem.Instance.Locate("happy.tex", GameFileLocs.GUI);
            //happy = UITextureManager.Instance.CreateInstance(fl);
            //fl = FileSystem.Instance.Locate("angry.tex", GameFileLocs.GUI);
            //angry = UITextureManager.Instance.CreateInstance(fl);
            //fl = FileSystem.Instance.Locate("human.tex", GameFileLocs.GUI);
            //popu = UITextureManager.Instance.CreateInstance(fl);

            //satisfy = new ProgressBar();

            //satisfy.Height = 18;
            //satisfy.Width = 117;
            //satisfy.ProgressImage = prgBg;
            //satisfy.Background = prgBg1;


            for (int i = 0; i < pluginInfo.Length; i++)
            {
                pluginInfo[i] = new PluginInfo(info, this, rs, city);
            }
            //linkArr = new CityLinkableMark(rs);

        }

        //public Brackets Bracket
        //{
        //    get { return brackets; }
        //}

        public override void Render(Sprite sprite)
        {
            Vector3 tangy = PlanetEarth.GetTangentY(MathEx.Degree2Radian(city.Longitude), MathEx.Degree2Radian(city.Latitude));

            Vector3 ppos = renderSys.Viewport.Project(city.Position - tangy * (CityStyleTable.CityRadius + 5),
                parent.Projection, parent.View, Matrix.Identity);
            Point scrnPos = new Point((int)ppos.X, (int)ppos.Y);

            Size strSize = font.MeasureString(city.Name, 20, DrawTextFormat.Center);

            //scrnPos.Y += strSize.Height;
            scrnPos.X -= strSize.Width / 2;

            font.DrawString(sprite, city.Name, scrnPos.X + 1, scrnPos.Y + 1, 20, DrawTextFormat.Center, (int)ColorValue.Black.PackedValue);
            font.DrawString(sprite, city.Name, scrnPos.X, scrnPos.Y, 20, DrawTextFormat.Center, -1);

            if (city.IsCaptured)
            {
                //satisfy.X = scrnPos.X;
                //satisfy.Y = scrnPos.Y - 30;
                //satisfy.Value = city.Satisfaction;
                //satisfy.Render(sprite);

                //sprite.Draw(angry, scrnPos.X - angry.Width, scrnPos.Y - 45, ColorValue.White);
                //sprite.Draw(happy, scrnPos.X + satisfy.Width, scrnPos.Y - 45, ColorValue.White);

                float mult = city.City.AdditionalDevMult;
                if (mult > 1)
                {
                    //font.DrawString(sprite, mult.ToString("F1") + "X", scrnPos.X + satisfy.Width + 32, scrnPos.Y - 45, 20, DrawTextFormat.Center, -1);
                }

                float count = (5 * city.City.Population / CityGrade.GetRefPopulation(city.Size));
                int count2 = (int)Math.Truncate(count);
                float rem = count - count2;

                //int i;
                //for (i = 0; i < count2; i++)
                //{
                //    sprite.Draw(popu, scrnPos.X + i * 20, scrnPos.Y - 64, ColorValue.White);
                //}
                //if (rem > float.Epsilon)
                //{
                //    sprite.Draw(popu,
                //        new Rectangle(scrnPos.X + i * 20, scrnPos.Y - 64 + popu.Height - (int)(rem * popu.Height), (int)(rem * popu.Width), (int)(rem * popu.Height)), ColorValue.White);
                //}
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

                //brackets.Render(sprite);
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
                //brackets.Update(time);
            }
        }
    }
}
