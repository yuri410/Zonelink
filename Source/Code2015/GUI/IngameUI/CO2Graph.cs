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
        const float MaxCO2 = 0.6f;

        const int HozTop = 104;
        const int HozLeft = 56;
        const int HozWidth = 209;

        RenderSystem renderSys;
        GameScene scene;
        Player player;
        //Texture hozbar;
        //Texture curbar;
        GameState state;
        Texture co2bar;

        float prgress;
        //float hozPrg;
        //float vertPrg;
        GeomentryData quad;
        CO2PieProgressEffect pieEffect;

        public CO2Graph(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.scene = scene;
            this.renderSys = game.RenderSystem;
            this.player = parent.HumanPlayer;
            this.state = gamelogic;

            FileLocation fl = FileSystem.Instance.Locate("ig_co2_debug.tex", GameFileLocs.GUI);
            co2bar = UITextureManager.Instance.CreateInstance(fl);
            
            //fl = FileSystem.Instance.Locate("ig_co2bar_cur", GameFileLocs.GUI);
            //curbar = UITextureManager.Instance.CreateInstance(fl);
        }
        void BuildQuad(RenderSystem rs)
        {
            ObjectFactory fac = rs.ObjectFactory;
            VertexDeclaration vtxDecl = fac.CreateVertexDeclaration(VertexPT1.Elements);

            VertexBuffer vb = fac.CreateVertexBuffer(4, vtxDecl, BufferUsage.Static);

            VertexPT1[] vtx = new VertexPT1[4];
            vtx[0].pos = new Vector3(-142 / 4, -142 / 4, 0);
            vtx[0].u1 = 0; vtx[0].v1 = 0;

            vtx[1].pos = new Vector3(-142 / 4, 142 / 4, 0);
            vtx[1].u1 = 1; vtx[1].v1 = 0;

            vtx[2].pos = new Vector3(142 / 4, -142 / 4, 0);
            vtx[2].u1 = 0; vtx[2].v1 = 1;


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

        public override int Order
        {
            get { return 7; }
        }

        public override void Render(Sprite sprite)
        {
            //int hl = (int)(HozWidth * hozPrg);

            //int left = HozLeft + hl;
            //Rectangle drect = new Rectangle(996 + left, 18 + HozTop, hl, 13);
            //Rectangle srect = new Rectangle(left, HozTop, hl, 13);

            //sprite.Draw(hozbar, drect, srect, ColorValue.White);


        }

        public override void Update(GameTime time)
        {
            Dictionary<Player, float> ratios = state.SLGWorld.EnergyStatus.GetCarbonRatios();

            float r;
            ratios.TryGetValue(player, out r);

            prgress = MathEx.Saturate(r / MaxCO2);
            //vertPrg = MathEx.Saturate((r - MaxCO2Hoz) / (MaxCO2 - MaxCO2Hoz));
        }
    }
}
