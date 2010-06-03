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
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.BalanceSystem;
using Code2015.EngineEx;
using Code2015.GUI.Controls;
using Code2015.World;

namespace Code2015.GUI
{
    class ResourceInfo : UIComponent
    {
        RenderSystem renderSys;

        ResInfoDisplay parent;

        IResourceObject resource;

        ProgressBar amountBar;
        Texture woodOverlay;

        public ResourceInfo(ResInfoDisplay info, RenderSystem rs, IResourceObject res)
        {
            this.renderSys = rs;

            this.parent = info;
            this.resource = res;

            string imp = res.Type == NaturalResourceType.Wood ? "ig_prgbar_wood_imp.tex" : "ig_prgbar_oil_imp.tex";
            string cmp = res.Type == NaturalResourceType.Wood ? "ig_prgbar_wood_cmp.tex" : "ig_prgbar_oil_cmp.tex";
            string text = res.Type == NaturalResourceType.Wood ? "ig_prgbar_wood.tex" : "ig_prgbar_oil.tex";
            string gray = res.Type == NaturalResourceType.Wood ? "ig_prgbar_wood_gray.tex" : "ig_prgbar_oil_gray.tex";


            amountBar = new ProgressBar();

            amountBar.Height = 30;
            amountBar.Width = 150;
            FileLocation fl = FileSystem.Instance.Locate(cmp, GameFileLocs.GUI);
            amountBar.ProgressImage = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate(imp, GameFileLocs.GUI);
            amountBar.Background = UITextureManager.Instance.CreateInstance(fl);
            amountBar.LeftPadding = 7;
            amountBar.RightPadding = 9;

            fl = FileSystem.Instance.Locate(gray, GameFileLocs.GUI);
            amountBar.Medium = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate(text, GameFileLocs.GUI);
            woodOverlay = UITextureManager.Instance.CreateInstance(fl);
        }

        public override void Render(Sprite sprite)
        {
            Vector3 tangy = PlanetEarth.GetTangentY(MathEx.Degree2Radian(resource.Longitude), MathEx.Degree2Radian(resource.Latitude));

            Vector3 pos = resource.Position;
            Vector3 ppos = renderSys.Viewport.Project(pos - tangy * (resource.Radius + 5),
                parent.Projection, parent.View, Matrix.Identity);

            float dist = Vector3.Distance(ref pos, ref parent.CameraPosition);
            //float d1 = 1 - MathEx.Saturate((dist - 1000) / 1500);
            float d2 = 1 - MathEx.Saturate((dist - 2000) / 1000);

            byte alpha = (byte)(d2 * byte.MaxValue);

            if (alpha > 5)
            {
                ColorValue modColor = new ColorValue(byte.MaxValue, byte.MaxValue, byte.MaxValue, alpha);

                Point scrnPos = new Point((int)ppos.X, (int)ppos.Y);
                scrnPos.X -= amountBar.Width / 2;
                scrnPos.Y -= amountBar.Height / 2;

                amountBar.ModulateColor = modColor;
                amountBar.X = scrnPos.X;
                amountBar.Y = scrnPos.Y;
                amountBar.Value = resource.AmountPer;
                amountBar.MediumValue = resource.MaxValue;
                amountBar.Render(sprite);

                sprite.Draw(woodOverlay, scrnPos.X, scrnPos.Y, modColor);
            }
        }

        public override void Update(GameTime time)
        {
        }
    }
}
