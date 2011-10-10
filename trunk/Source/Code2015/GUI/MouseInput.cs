/*
-----------------------------------------------------------------------------
This source file is part of Zonelink

Copyright (c) 2009+ Tao Games

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  if not, write to the Free Software Foundation, 
Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA, or go to
http://www.gnu.org/copyleft/gpl.txt.

-----------------------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using XI = Microsoft.Xna.Framework.Input;
using Apoc3D.MathLib;

namespace Code2015.GUI
{
    public enum MouseButtonFlags
    {
        None,
        Left = 1,
        Middle = 1 << 1,
        Right = 1 << 2
    };

    public delegate void MouseClickHandler(MouseButtonFlags mouse, int x, int y);
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

        static ValueSmoother wheel = new ValueSmoother(20);

        /// <summary>
        ///  鼠标任一按键由按下变为松开时引发此事件
        /// </summary>
        public static event MouseClickHandler MouseUp;

        /// <summary>
        ///  鼠标任一按键由松开变为按下时引发此事件
        /// </summary>
        public static event MouseClickHandler MouseDown;


        public static int DX
        {
            get { return currentState.X - oldState.X; }
        }
        public static int DY 
        {
            get { return currentState.Y - oldState.Y; }
        }

        public static int X 
        {
            get { return currentState.X; }
        }

        public static int Y
        {
            get { return currentState.Y; }
        }

        public static float ScrollWheelValue
        {
            get;
            private set;
        }
        static float OldScrollWheelValue
        {
            get;
            set;
        }
        public static float DScrollWheelValue 
        {
            get { return ScrollWheelValue - OldScrollWheelValue; }
        }
        public static bool IsMouseUpLeft
        {
            get;
            private set;
        }
        public static bool IsMouseMoving
        {
            get { return DX != 0 || DY != 0; }
        }

        public static bool IsMouseDownLeft
        {
            get;
            private set;
        }
        public static bool IsMouseUpRight
        {
            get;
            private set;
        }
        public static bool IsMouseDownRight
        {
            get;
            private set;
        }

        public static bool IsMouseUpMiddle
        {
            get;
            private set;
        }
        public static bool IsLeftPressed 
        {
            get { return currentState.LeftButton == XI.ButtonState.Pressed; }
        }
        public static bool IsRightPressed
        {
            get { return currentState.RightButton == XI.ButtonState.Pressed; }
        }

        public static void Update(GameTime time)
        {
            IsMouseUpLeft = false;
            IsMouseUpRight = false;
            IsMouseUpMiddle = false;
            IsMouseDownLeft = false;
            IsMouseDownRight = false;
            
            oldState = currentState;
            OldScrollWheelValue = ScrollWheelValue;
            
            currentState = XI.Mouse.GetState();

            wheel.Add(currentState.ScrollWheelValue);
            ScrollWheelValue = wheel.Result;



            if (currentState.LeftButton == XI.ButtonState.Pressed &&
                oldState.LeftButton == XI.ButtonState.Released)
            {
                if (MouseDown != null)
                {
                    MouseDown(MouseButtonFlags.Left, currentState.X, currentState.Y);
                }
                IsMouseDownLeft = true;
            }
            if (currentState.MiddleButton == XI.ButtonState.Pressed &&
                oldState.MiddleButton == XI.ButtonState.Released)
            {
                if (MouseDown != null)
                {
                    MouseDown(MouseButtonFlags.Middle, currentState.X, currentState.Y);
                }
            }
            if (currentState.RightButton == XI.ButtonState.Pressed &&
                 oldState.RightButton == XI.ButtonState.Released)
            {
                if (MouseDown != null)
                {
                    MouseDown(MouseButtonFlags.Right, currentState.X, currentState.Y);
                }
                IsMouseDownRight = true;
            }




            if (currentState.LeftButton == XI.ButtonState.Released &&
                oldState.LeftButton == XI.ButtonState.Pressed)
            {
                if (MouseUp != null)
                {
                    MouseUp(MouseButtonFlags.Left, currentState.X, currentState.Y);
                }
                IsMouseUpLeft = true;
            }
            if (currentState.MiddleButton == XI.ButtonState.Released &&
                oldState.MiddleButton == XI.ButtonState.Pressed)
            {
                if (MouseUp != null)
                {
                    MouseUp(MouseButtonFlags.Middle, currentState.X, currentState.Y);
                }
                IsMouseUpMiddle = true;
            }
            if (currentState.RightButton == XI.ButtonState.Released &&
                oldState.RightButton == XI.ButtonState.Pressed)
            {
                if (MouseUp != null)
                {
                    MouseUp(MouseButtonFlags.Right, currentState.X, currentState.Y);
                }
                IsMouseUpRight = true;
            }

        }
    }
}
