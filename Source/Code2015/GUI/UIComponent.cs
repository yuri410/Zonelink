using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;

namespace Code2015.GUI
{
    public abstract class UIComponent
    {
        public virtual bool MouseHitTest(int x, int y)
        {
            return false;
        }

        public virtual void Interact(GameTime time, bool action) { }

        public virtual void Update(GameTime time)
        {

        }

        public virtual void Render(Sprite sprite)
        {

        }
    }
}
