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
using Code2015.Effects.Post;

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
        Texture ring;

        GeomentryData quad;
        PieProgressEffect pieEffect;

        public int Plugin
        {
            get;
            set;
        }

        public PluginInfo(CityInfoDisplay info, CityInfo parent, RenderSystem rs, CityObject city, PieProgressEffect pieEffect)
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

            fl = FileSystem.Instance.Locate("ig_ring.tex", GameFileLocs.GUI);
            ring = UITextureManager.Instance.CreateInstance(fl);

            this.pieEffect = pieEffect;
            BuildQuad(renderSys);
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

        void BuildQuad(RenderSystem rs)
        {
            ObjectFactory fac =rs.ObjectFactory;
            VertexDeclaration vtxDecl = fac.CreateVertexDeclaration(VertexPT1.Elements );

            VertexBuffer vb = fac.CreateVertexBuffer(4, vtxDecl, BufferUsage.Static);

            VertexPT1[] vtx = new VertexPT1[4];
            vtx[0].pos = new Vector3(-142 / 4, -142 / 4, 0);
            vtx[0].u1 = 0; vtx[0].v1 = 0;

            vtx[1].pos = new Vector3(-142 / 4, 142 / 4, 0);
            vtx[1].u1 = 0; vtx[1].v1 = 1;

            vtx[2].pos = new Vector3(142 / 4, -142 / 4, 0);
            vtx[2].u1 = 1; vtx[2].v1 = 0;


            vtx[3].pos = new Vector3(142 / 4, 142 / 4, 0);
            vtx[3].u1 = 1; vtx[3].v1 = 1;

            

            vb.SetData(vtx);

            quad = new GeomentryData();
            quad.VertexBuffer = vb;
            quad.PrimCount = 2;
            quad.PrimitiveType = RenderPrimitiveType.TriangleStrip;
            quad.VertexCount = 4;
            quad.VertexDeclaration = vtxDecl;
            quad.VertexSize = vtxDecl.GetVertexSize();
        }

        public override void Render(Sprite sprite)
        {
            CityPlugin cplug = city.GetPlugin(Plugin);

            Vector3 plpos;
            Vector3 ppofs = new Vector3(0, 150, 0);
            ppofs += city.GetPluginPosition(Plugin);

            Vector3.TransformSimple(ref ppofs, ref city.Transformation, out plpos);

            plpos = renderSys.Viewport.Project(plpos, display.Projection, display.View, Matrix.Identity);

            int x = (int)plpos.X;
            int y = (int)plpos.Y;

            switch (cplug.TypeId)
            {
                case CityPluginTypeId.BiofuelFactory:
                case CityPluginTypeId.OilRefinary:
                    Rectangle rect = new Rectangle(x - oilRefbg.Width / 4, y - oilRefbg.Height / 4, oilRefbg.Width / 2, oilRefbg.Height / 2);

                    sprite.Draw(oilRefbg, rect, parent.DistanceMod);
                    break;
                case CityPluginTypeId.WoodFactory:
                    rect = new Rectangle(x - woodFacbg.Width / 4, y - woodFacbg.Height / 4, woodFacbg.Width / 2, woodFacbg.Height / 2);
                    sprite.Draw(woodFacbg, rect, parent.DistanceMod);
                    break;
                case CityPluginTypeId.EducationOrg:
                    rect = new Rectangle(x - edubg.Width / 4, y - edubg.Height / 4, edubg.Width / 2, edubg.Height / 2);
                    sprite.Draw(edubg, rect, parent.DistanceMod);
                    break;
                case CityPluginTypeId.Hospital:
                    rect = new Rectangle(x - hospbg.Width / 4, y - hospbg.Height / 4, hospbg.Width / 2, hospbg.Height / 2);
                    sprite.Draw(hospbg, rect, parent.DistanceMod);
                    break;
            }

            sprite.End();

            Vector2 p = new Vector2(plpos.X , plpos.Y );
            pieEffect.SetVSValue(0, ref p);
            pieEffect.SetTexture("texDif", ring);
            if (cplug.IsBuilding || cplug.IsSelling)
            {
                pieEffect.SetValue("weight", cplug.BuildProgress);
            }
            else
            {
                pieEffect.SetValue("weight", cplug.UpgradePoint);
            }
            pieEffect.SetValue("alpha", parent.DistanceMod.A / 255f);
            sprite.DrawQuad(quad, pieEffect);


            sprite.Begin();


            if (city.IsUpgraded)
            {
                display.AddLVPopup(x, y);
            }
            if (cplug.IsSelling && !cplug.IsSold && cplug.BuildProgress < 0.1f) 
            {
                display.AddSellPopup(x, y);
                cplug.IsSold = true;
            }
        }

        public override void Update(GameTime time)
        {
        }
    }

    class CityInfo : UIComponent
    {
        RenderSystem renderSys;
        GameFont f18ig1;

        CityInfoDisplay parent;
        CityObject city;
        Player player;

        PluginInfo[] pluginInfo = new PluginInfo[CityGrade.LargePluginCount];

        public ColorValue DistanceMod;

        public CityInfo(CityInfoDisplay info, RenderSystem rs, CityObject city, Player player, PieProgressEffect pieEff)
        {
            this.f18ig1 = GameFontManager.Instance.F18IG1;

            this.parent = info;
            this.city = city;

            this.renderSys = rs;
            this.player = player;

            for (int i = 0; i < pluginInfo.Length; i++)
            {
                pluginInfo[i] = new PluginInfo(info, this, rs, city, pieEff);
            }

        }

        public override void Render(Sprite sprite)
        {
            Vector3 tangy = PlanetEarth.GetTangentY(MathEx.Degree2Radian(city.Longitude), MathEx.Degree2Radian(city.Latitude));

            Vector3 ppos = renderSys.Viewport.Project(city.Position - tangy * (CityStyleTable.CityRadius + 5),
                parent.Projection, parent.View, Matrix.Identity);
            Point scrnPos = new Point((int)ppos.X, (int)ppos.Y);

            string name = city.Name.ToUpperInvariant();
            Size strSize = f18ig1.MeasureString(name);

            f18ig1.DrawString(sprite, name, scrnPos.X - strSize.Width / 2, scrnPos.Y, ColorValue.White);
            //f18.DrawString(sprite, city.Name, scrnPos.X, scrnPos.Y,  DrawTextFormat.Center, -1);

            if (city.Owner == player)
            {
                float dist = Vector3.Distance(city.Position, parent.CameraPosition);
                dist = 1 - MathEx.Saturate((dist - 2000) / 750);
                DistanceMod = ColorValue.White;
                DistanceMod.A = (byte)(dist * byte.MaxValue);

                if (city.IsLinked)
                {
                    parent.AddLinkPopup(scrnPos.X, scrnPos.Y);
                    city.IsLinked = false;
                }
                if (city.IsScaleIncreased) 
                {
                    parent.AddGrowPopup(scrnPos.X, scrnPos.Y);
                    city.IsScaleIncreased = false;
                }

                if (((ISelectableObject)city).IsSelected)
                {
                    for (int i = 0; i < city.PluginCount; i++)
                    {
                        pluginInfo[i].Plugin = i;
                        pluginInfo[i].Render(sprite);
                    }
                }

                if (city.IsUpgraded)
                {
                    city.IsUpgraded = false;
                }

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
            }
        }
    }
}
