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
http://www.gnu.org/copyleft/lesser.txt.

-----------------------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.Effects.Post;
using Code2015.EngineEx;
using Code2015.Logic;
using Code2015.World;

namespace Code2015.GUI
{
    class CO2Graph : UIComponent
    {
        const int X = 996;
        const int Y = 20;
        const float MaxCO2 = 0.6f;
        const float WarningThreshold = 0.6f;

        RenderSystem renderSys;
        GameScene scene;
        Player player;

        GameState state;
        Texture co2bar;
        Texture co2bar_glow;

        float prgress;

        float flash;
        GeomentryData quad;
        CO2PieProgressEffect pieEffect;

        public CO2Graph(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.scene = scene;
            this.renderSys = game.RenderSystem;
            this.player = parent.HumanPlayer;
            this.state = gamelogic;

            pieEffect = new CO2PieProgressEffect(renderSys);

            FileLocation fl = FileSystem.Instance.Locate("ig_co2.tex", GameFileLocs.GUI);
            co2bar = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ig_co2_glow.tex", GameFileLocs.GUI);
            co2bar_glow = UITextureManager.Instance.CreateInstance(fl);
            
            //fl = FileSystem.Instance.Locate("ig_co2bar_cur", GameFileLocs.GUI);
            //curbar = UITextureManager.Instance.CreateInstance(fl);
            BuildQuad(renderSys);
        }
        void BuildQuad(RenderSystem rs)
        {
            ObjectFactory fac = rs.ObjectFactory;
            VertexDeclaration vtxDecl = fac.CreateVertexDeclaration(VertexPT1.Elements);

            VertexBuffer vb = fac.CreateVertexBuffer(4, vtxDecl, BufferUsage.Static);

            const int W = 265;
            const int H = 113;
            VertexPT1[] vtx = new VertexPT1[4];
            vtx[0].pos = new Vector3(X, Y, 0);
            vtx[0].u1 = 0; vtx[0].v1 = 0;

            vtx[1].pos = new Vector3(X, Y + H, 0);
            vtx[1].u1 = 0; vtx[1].v1 = 1;

            vtx[2].pos = new Vector3(X + W, Y, 0);
            vtx[2].u1 = 1; vtx[2].v1 = 0;


            vtx[3].pos = new Vector3(X + W, Y + H, 0);
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

        public override int Order
        {
            get { return 8; }
        }

        public override void Render(Sprite sprite)
        {
            sprite.End();

            pieEffect.SetTexture("texDif", co2bar);
            pieEffect.SetValue("hozsep", 0.25f);
           
            pieEffect.SetValue("weight", prgress);
            
            sprite.DrawQuad(quad, pieEffect);

            sprite.Begin();



            int alpha = (int)(byte.MaxValue * Math.Sin(flash));
            ColorValue color = ColorValue.White;

            if (alpha > byte.MaxValue)
                alpha = byte.MaxValue;
            if (alpha < 0)
                alpha = 0;

            color.A = (byte)alpha;
            sprite.Draw(co2bar_glow, X, Y, color);

        }

        public override void Update(GameTime time)
        {
            Dictionary<Player, float> ratios = state.SLGWorld.EnergyStatus.GetCarbonRatios();

            float r;
            ratios.TryGetValue(player, out r);

            prgress = MathEx.Saturate(r / MaxCO2);
            //prgress += 0.0025f;

            float over = prgress - WarningThreshold;
            if (over > 0)
            {
                flash += over * 0.5f;
            }
            else
            {
                flash *= 0.99f;
            }

            if (flash > MathEx.PIf)
                flash -= MathEx.PIf;
            //vertPrg = MathEx.Saturate((r - MaxCO2Hoz) / (MaxCO2 - MaxCO2Hoz));
        }
    }
}
