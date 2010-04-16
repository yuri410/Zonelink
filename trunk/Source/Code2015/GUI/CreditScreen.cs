using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D;

namespace Code2015.GUI
{
    class CreditScreen : UIComponent, IDisposable
    {
        RenderSystem renderSys;

        public CreditScreen(RenderSystem rs)
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
        #region IDisposable 成员

        public bool Disposed
        {
            get;
            private set;
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
