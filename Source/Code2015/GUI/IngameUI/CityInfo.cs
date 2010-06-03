/*
-----------------------------------------------------------------------------
This source file is part of Zonelink

Copyright (c) 2009+ Tao Games

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  if not, write to the Free Software Foundation, 
Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA, or go to
http://www.gnu.org/copyleft/gpl.txt.

-----------------------------------------------------------------------------
*/
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
            ObjectFactory fac = rs.ObjectFactory;
            VertexDeclaration vtxDecl = fac.CreateVertexDeclaration(VertexPT1.Elements );

            VertexBuffer vb = fac.CreateVertexBuffer(4, vtxDecl, BufferUsage.Static);

            const float QuadSize = 71;
            VertexPT1[] vtx = new VertexPT1[4];
            vtx[0].pos = new Vector3(-QuadSize / 2, -QuadSize / 2, 0);
            vtx[0].u1 = 0; vtx[0].v1 = 0;

            vtx[1].pos = new Vector3(-QuadSize / 2, QuadSize / 2, 0);
            vtx[1].u1 = 0; vtx[1].v1 = 1;

            vtx[2].pos = new Vector3(QuadSize / 2, -QuadSize / 2, 0);
            vtx[2].u1 = 1; vtx[2].v1 = 0;


            vtx[3].pos = new Vector3(QuadSize / 2, QuadSize / 2, 0);
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

        public Point GetProjectionPosition(int i)
        {
            Vector3 plpos;
            Vector3 ppofs = city.GetPluginPosition(Plugin);
            ppofs.Z += 45;

            Vector3.TransformSimple(ref ppofs, ref city.Transformation, out plpos);
            plpos = renderSys.Viewport.Project(plpos, display.Projection, display.View, Matrix.Identity);

            //plpos.Y -= 36;
            return new Point((int)plpos.X, (int)plpos.Y);
        }

        public override void Render(Sprite sprite)
        {
            CityPlugin cplug = city.GetPlugin(Plugin);

            if (cplug == null)
                return;
            Vector3 plpos;
            Vector3 ppofs = city.GetPluginPosition(Plugin);
            ppofs.Z += 45;

            Vector3.TransformSimple(ref ppofs, ref city.Transformation, out plpos);

            plpos = renderSys.Viewport.Project(plpos, display.Projection, display.View, Matrix.Identity);

            plpos.Y -= 36;

            int x = (int)plpos.X;
            int y = (int)plpos.Y;

            switch (cplug.TypeId)
            {
                case CityPluginTypeId.BiofuelFactory:
                case CityPluginTypeId.OilRefinary:
                    sprite.Draw(oilRefbg, x - oilRefbg.Width / 2, y - oilRefbg.Height / 2, parent.DistanceMod);
                    break;
                case CityPluginTypeId.WoodFactory:
                    sprite.Draw(woodFacbg, x - woodFacbg.Width / 2, y - woodFacbg.Height / 2, parent.DistanceMod);
                    break;
                case CityPluginTypeId.EducationOrg:
                    sprite.Draw(edubg, x - edubg.Width / 2, y - edubg.Height / 2, parent.DistanceMod);
                    break;
                case CityPluginTypeId.Hospital:
                    sprite.Draw(hospbg, x - hospbg.Width / 2, y - hospbg.Height / 2, parent.DistanceMod);
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

        Texture needFood;
        Texture needOil;
        Texture needWood;
        Texture needHelp;

        PluginInfo[] pluginInfo = new PluginInfo[CityGrade.LargePluginCount];

        float flashCounter;
        //ValueSmoother helper = new ValueSmoother(5);
        //int helperCounter;

        public ColorValue DistanceMod;

        public CityInfo(CityInfoDisplay info, RenderSystem rs, CityObject city, Player player, PieProgressEffect pieEff)
        {
            this.f18ig1 = GameFontManager.Instance.F18G1;

            this.parent = info;
            this.city = city;

            this.renderSys = rs;
            this.player = player;

            for (int i = 0; i < pluginInfo.Length; i++)
            {
                pluginInfo[i] = new PluginInfo(info, this, rs, city, pieEff);
            }

            FileLocation fl = FileSystem.Instance.Locate("ig_needfood.tex", GameFileLocs.GUI);
            needFood = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ig_needoil.tex", GameFileLocs.GUI);
            needOil = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ig_needwood.tex", GameFileLocs.GUI);
            needWood = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ig_needhelp.tex", GameFileLocs.GUI);
            needHelp = UITextureManager.Instance.CreateInstance(fl);
        }

        public override void Render(Sprite sprite)
        {
            Vector3 tangy = PlanetEarth.GetTangentY(MathEx.Degree2Radian(city.Longitude), MathEx.Degree2Radian(city.Latitude));

            Vector3 ppos = renderSys.Viewport.Project(city.Position - tangy * (CityStyleTable.CityRadius + 5),
                parent.Projection, parent.View, Matrix.Identity);
            Point scrnPos = new Point((int)ppos.X, (int)ppos.Y);

            string name = city.Name.ToUpperInvariant();
            Size strSize = f18ig1.MeasureString(name);
            
            f18ig1.DrawString(sprite, name, scrnPos.X - strSize.Width / 2 + 1, scrnPos.Y + 1, ColorValue.Black);
            f18ig1.DrawString(sprite, name, scrnPos.X - strSize.Width / 2, scrnPos.Y, ColorValue.White);

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
                    for (int i = 0; i < CityObject.MaxPlugin; i++)
                    {
                        pluginInfo[i].Plugin = i;
                        pluginInfo[i].Render(sprite);
                    }
                }

                if (city.IsUpgraded)
                {
                    for (int i = 0; i < CityObject.MaxPlugin; i++)
                    {
                        if (city.GetPlugin(i) == null)
                            continue;

                        pluginInfo[i].Plugin = i;
                        Point pt = pluginInfo[i].GetProjectionPosition(i);

                        parent.AddLVPopup(pt.X, pt.Y);
                    }
                    city.IsUpgraded = false;
                }



                ppos = renderSys.Viewport.Project(city.Position + tangy * (CityStyleTable.CityRadius + 25),
                   parent.Projection, parent.View, Matrix.Identity);
                scrnPos = new Point((int)ppos.X, (int)ppos.Y);
                scrnPos.X -= 40;
                scrnPos.Y -= 80;
                ColorValue color = ColorValue.White;
                if (city.City.IsLackFood)
                {
                    color.A = (byte)(byte.MaxValue * MathEx.Saturate((float)(0.5 * (1 - Math.Cos(flashCounter + Math.PI / 2)))));
                    sprite.Draw(needFood, scrnPos.X, scrnPos.Y, color);
                }

                if (city.City.IsLackOil)
                {
                    color.A = (byte)(byte.MaxValue * MathEx.Saturate((float)(0.5 * (1 - Math.Cos(flashCounter + Math.PI)))));
                    sprite.Draw(needOil, scrnPos.X, scrnPos.Y, color);
                }
                if (city.City.IsLackWood)
                {
                    color.A = (byte)(byte.MaxValue * MathEx.Saturate((float)(0.5 * (1 - Math.Cos(flashCounter + 3 * Math.PI / 2)))));
                    sprite.Draw(needWood, scrnPos.X, scrnPos.Y, color);
                }

            }
            else if (city.Capture.IsPlayerCapturing(player) &&
                (city.Capture.GetSpeed(player) < float.Epsilon))
            {
                ColorValue color = ColorValue.White;

                ppos = renderSys.Viewport.Project(city.Position + tangy * (CityStyleTable.CityRadius + 25),
                     parent.Projection, parent.View, Matrix.Identity);


                Rectangle rect = new Rectangle((int)ppos.X, (int)ppos.Y, needHelp.Width / 2, needHelp.Height / 2);

                rect.X -= rect.Width / 2;
                rect.Y -= needHelp.Height / 2;



                color.A = (byte)(byte.MaxValue * MathEx.Saturate((float)(0.5 * (1 - Math.Cos(flashCounter + 3 * Math.PI / 2)))));
                sprite.Draw(needHelp, rect, color);
            }


        }

        public override void Update(GameTime time)
        {
            if (city.Owner == player)
            {
                //if (((ISelectableObject)city).IsSelected)
                //{
                //    for (int i = 0; i < city.PluginCount; i++)
                //    {
                //        pluginInfo[i].Update(time);
                //    }
                //}
            }
            flashCounter += time.ElapsedGameTimeSeconds * 2;
            if (flashCounter > MathEx.PIf * 2)
                flashCounter -= MathEx.PIf * 2;
            //else if (city.Capture.IsPlayerCapturing(player))
            //{
            //    if (helperCounter++ == 60)
            //    {
            //        helper.Add(city.Capture.GetCaptureProgress(player));
            //        helperCounter = 0;
            //    }
            //}
        }
    }
}
