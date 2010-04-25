using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Code2015.Logic;
using Code2015.World;
using Apoc3D.Vfs;
using Code2015.EngineEx;

namespace Code2015.GUI
{
    class CO2Graph : UIComponent
    {
        const float MaxCO2 = 0.6f;
        const float MaxCO2Hoz = 0.2f;

        const int HozTop = 104;
        const int HozLeft = 56;
        const int HozWidth = 209;

        RenderSystem renderSys;
        GameScene scene;
        Player player;
        Texture hozbar;
        Texture curbar;
        GameState state;
        GameFont font;

        float hozPrg;
        float vertPrg;


        public CO2Graph(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.scene = scene;
            this.renderSys = game.RenderSystem;
            this.player = parent.HumanPlayer;
            this.state = gamelogic;

            FileLocation fl = FileSystem.Instance.Locate("", GameFileLocs.GUI);
        }

        public override int Order
        {
            get { return 7; }
        }

        public override void Render(Sprite sprite)
        {
            int hl = (int)(HozWidth * hozPrg);

            int left = HozLeft + hl;
            Rectangle drect = new Rectangle(996 + left, 18 + HozTop, hl, 13);
            Rectangle srect = new Rectangle(left, HozTop, hl, 13);

            sprite.Draw(hozbar, drect, srect, ColorValue.White);


        }

        public override void Update(GameTime time)
        {
            Dictionary<Player, float> ratios = state.SLGWorld.EnergyStatus.GetCarbonRatios();

            float r;
            ratios.TryGetValue(player, out r);

            hozPrg = MathEx.Saturate(r / MaxCO2Hoz);
            vertPrg = MathEx.Saturate((r - MaxCO2Hoz) / (MaxCO2 - MaxCO2Hoz));
        }
    }
}
