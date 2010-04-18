using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;

namespace Code2015.GUI
{
    public abstract class UIComponent
    {
        public virtual int Order
        {
            get { return 0; }
        }

        public virtual void Update(GameTime time)
        {

        }
        public virtual void UpdateInteract(GameTime time)
        {

        }
        public virtual void Render(Sprite sprite)
        {

        }

        public virtual bool HitTest(int x, int y)
        {
            return false;
        }
    }
}
