using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;

namespace Code2015.GUI
{
    class GameUI
    {
        public UIComponent CurrentComponent
        {
            get;
            set;
        }

        public void Render()
        {
            if (CurrentComponent != null)
            {
                CurrentComponent.Render();
            }
        }
        public void Update(GameTime time) 
        {
            if (CurrentComponent != null)
            {
                CurrentComponent.Update(time);
            }
        }
    }
}
