using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Vfs;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D;

namespace Code2015.GUI
{
    class LoadingOverlay : UIComponent, IDisposable
    {
        float alpha;
        float dir;
        Texture overlay34;
        Code2015 parent; 
        float perdelay;
        public LoadingOverlay(Code2015 parent, Texture overlay34)
        {
            this.parent = parent;
            this.overlay34 = overlay34;
        }

        public void In() { dir = 1; }
        public void Out(int preDelay)
        {
            dir = -1; this.perdelay = preDelay;
        }
        public override void Render(Sprite sprite)
        {
            ColorValue color = ColorValue.White;
            color.A = (byte)(byte.MaxValue * MathEx.Saturate(alpha));
            sprite.Draw(overlay34, 0, 0, color);
        }

        public override void Update(GameTime time)
        {
            

            if (perdelay > 0)
            {
                perdelay -= time.ElapsedGameTimeSeconds;
                return;
            }

            if (parent.IsIngame && parent.CurrentGame.IsLoaded && alpha > 0)
            {
                Out(0);
            }

            alpha += dir * time.ElapsedGameTimeSeconds;
            
            if (alpha > 1)
            {
                alpha = 1;
                dir = 0;
            }
            else if (alpha < 0)
            {
                alpha = 0;
                dir = 0;
            }
        }
    }
}
