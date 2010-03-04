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
        /// <summary>
        ///  上一次更新时鼠标的状态
        /// </summary>
        static XI.MouseState oldState;
        /// <summary>
        ///  当前鼠标状态
        /// </summary>
        static XI.MouseState currentState;


        /// <summary>
        ///  鼠标任一按键由按下变为松开时引发此事件
        /// </summary>
        public static event MouseClickHandler MouseUp;

        /// <summary>
        ///  鼠标任一按键由松开变为按下时引发此事件
        /// </summary>
        public static event MouseClickHandler MouseDown;


        public static int X 
        {
            get { return currentState.X; }
        }

        public static int Y
        {
            get { return currentState.Y; }
        }

        public static int ScrollWheelValue 
        {
            get { return currentState.ScrollWheelValue; }
        }


            
        public static void Update(GameTime time)
        {
            currentState = XI.Mouse.GetState();

            if (MouseDown != null)
            {
                if (currentState.LeftButton == XI.ButtonState.Pressed &&
                    oldState.LeftButton == XI.ButtonState.Released)
                {
                    MouseDown(MouseButton.Left, currentState.X, currentState.Y);
                }
                if (currentState.MiddleButton == XI.ButtonState.Pressed &&
                    oldState.MiddleButton == XI.ButtonState.Released)
                {
                    MouseDown(MouseButton.Middle, currentState.X, currentState.Y);
                } 
                if (currentState.RightButton == XI.ButtonState.Pressed &&
                     oldState.RightButton == XI.ButtonState.Released)
                {
                    MouseDown(MouseButton.Right, currentState.X, currentState.Y);
                }
            }

            if (MouseUp != null)
            {
                if (currentState.LeftButton == XI.ButtonState.Released &&
                    oldState.LeftButton == XI.ButtonState.Pressed)
                {
                    MouseUp(MouseButton.Left, currentState.X, currentState.Y);
                }
                if (currentState.MiddleButton == XI.ButtonState.Released &&
                    oldState.MiddleButton == XI.ButtonState.Pressed)
                {
                    MouseUp(MouseButton.Middle, currentState.X, currentState.Y);
                }
                if (currentState.RightButton == XI.ButtonState.Released &&
                    oldState.RightButton == XI.ButtonState.Pressed)
                {
                    MouseUp(MouseButton.Right, currentState.X, currentState.Y);
                }
            }

            oldState = currentState;
        }
    }
}
