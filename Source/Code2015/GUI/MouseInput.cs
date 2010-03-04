using System;
using System.Collections.Generic;
using System.Text;
using XI = Microsoft.Xna.Framework.Input;
using Apoc3D;

namespace Code2015.GUI
{
    public enum MouseButton { Left, Middle, Right };

    public delegate void MouseClickHandler(MouseButton mouse, int x, int y);
    static class MouseInput
    {

        public static event MouseClickHandler MouseUp;
        public static event MouseClickHandler MouseDown;
        
        public static void Update(GameTime time)
        {
            XI.MouseState mstate = XI.Mouse.GetState();
            
            if (MouseDown != null)
            { 
                
            }
        }
    }
}
