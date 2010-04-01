using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;

namespace Code2015.GUI
{
    class ScoreScreen : UIComponent
    {
        RenderSystem renderSys;

        public ScoreScreen(RenderSystem rs)
        {
            this.renderSys = rs;
        }

        public override void Render(Sprite sprite)
        {
            base.Render(sprite);
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
        }
    }
}
