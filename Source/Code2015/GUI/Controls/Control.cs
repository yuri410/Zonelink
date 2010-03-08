
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Apoc3D.Config;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Code2015.GUI;

namespace Apoc3D.GUI.Controls
{
    public delegate void MouseEventHandler(object sender, MouseButtonFlags btn);
    public delegate void MouseWheelHandler(object sender, float value);
    /// <summary>
    /// 
    /// </summary>
    public abstract class Control : UIComponent
    {
        protected ColorValue backColor;
        protected ColorValue foreColor = ColorValue.White;
        protected ColorValue modColor = ColorValue.White;

        protected object tag;

        //protected KeyHandler hotkeyInvoked;
        //protected KeyEventHandler keyPressed;

        //bool hasFocus;


        List<Control> controls;

        ///// <summary>
        ///// Occurs when the control's hotkey is invoked.
        ///// </summary>
        //public event KeyHandler HotkeyInvoked
        //{
        //    add { hotkeyInvoked += value; }
        //    remove { hotkeyInvoked -= value; }
        //}

        //public event KeyEventHandler KeyPressed
        //{
        //    add { keyPressed += value; }
        //    remove { keyPressed -= value; }
        //}
        public event MouseEventHandler MouseClick;
        public event MouseEventHandler MouseUp;
        public event MouseEventHandler MouseDown;

        public event MouseEventHandler MouseMove;
        public event MouseEventHandler MouseEnter;
        public event MouseEventHandler MouseLeave;

        public event MouseWheelHandler MouseWheel;
        

        protected Control()
        {

            //txtHint = TextRenderingHint.ClearTypeGridFit;
            modColor = ColorValue.White;

            controls = new List<Control>();

            //InputEngine = gameUI.InputEngine;

            //InputEngine.KeyStateChanged += this._OnKeyStateChanged;
            //InputEngine.MouseDown += this._OnMouseDown;
            //InputEngine.MouseMove += this._OnMouseMove;
            //InputEngine.MouseUp += this._OnMouseUp;
            //InputEngine.MouseWheel += this._OnMouseWheel;
        }

        public void AddControl(Control ctl)
        {
            ctl.Parent = this;
            controls.Add(ctl);
        }

        public void RemoveControl(Control ctl)
        {
            ctl.Parent = null;
            controls.Remove(ctl);
        }
        public Control GetControl(int index)
        {
            return controls[index];
        }
        public int ControlCount
        {
            get { return controls.Count; }
        }
        //public Input InputEngine
        //{
        //    get;
        //    private set;
        //}
        public bool HasHotKey
        {
            get;
            set;
        }
        public bool IsInputControl
        {
            get;
            set;
        }
        

        /// <summary>
        /// Gets or sets the parent dialog.
        /// </summary>
        /// <value>The parent dialog.</value>
        public Control Parent
        {
            get;
            set;
        }


        #region Properities



        public bool Enabled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the mouse is over the control.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the mouse is over the control; otherwise, <c>false</c>.
        /// </value>
        public bool IsMouseOver
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is pressed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is pressed; otherwise, <c>false</c>.
        /// </value>
        public bool IsPressed
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the X position.
        /// </summary>
        /// <value>The X position.</value>
        public int X
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Y position.
        /// </summary>
        /// <value>The Y position.</value>
        public int Y
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public Size Size
        {
            get { return new Size(Width, Height); }
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        public Point Location
        {
            get { return new Point(X, Y); }
            set { X = value.X; Y = value.Y; }
        }

        /// <summary>
        /// Gets or sets the bounds.
        /// </summary>
        /// <value>The bounds.</value>
        public Rectangle Bounds
        {
            get { return new Rectangle(X, Y, Width, Height); }
            set
            {
                X = value.X; Y = value.Y; Width = value.Width; Height = value.Height;
            }
        }

        public ColorValue BackColor
        {
            get { return backColor; }
            set { backColor = value; }
        }

        public ColorValue ForeColor
        {
            get { return foreColor; }
            set { foreColor = value; }
        }

        #endregion

        public ColorValue ModulateColor
        {
            get { return modColor; }
            set { modColor = value; }
        }

        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        ///   Updates the control.
        /// </summary>
        /// <param name="dt">The elapsed time.</param>
        public override void Update(GameTime dt)
        {
            OnUpdate(dt);
            for (int i = 0; i < controls.Count; i++)
            {
                controls[i].Update(dt);
            }
        }

        public override void Render(Sprite sprite)
        {
            OnPaint(sprite);
            for (int i = 0; i < controls.Count; i++)
            {
                controls[i].Render(sprite);
            }
        }

        protected virtual bool IsInBounds(int x, int y) { return false; }

        public static bool IsInBounds(int x, int y, ref Rectangle rect)
        {
            return (x >= rect.X) && (y >= rect.Y) && (x <= rect.Width + rect.X) && (y <= rect.Height + rect.Y);
        }
        public static bool IsInBounds(int x, int y, int cx, int cy, int r)
        {
            Vector2 d = new Vector2(x - cx, y - cy);
            return d.Length() <= r;
        }
        //protected virtual void OnKeyPressed(KeyPressEventArgs e) { }

        protected virtual void OnPaint(Sprite sprite) 
        { }
        protected virtual void OnUpdate(GameTime time)
        {
            MouseButtonFlags flag = MouseButtonFlags.None;
            if (MouseInput.IsLeftPressed)
                flag |= MouseButtonFlags.Left;
            if (MouseInput.IsRightPressed)
                flag |= MouseButtonFlags.Right;
            

            //if (MouseInput.DX != 0 && MouseInput.DY != 0)
            //{
            bool inBounds = IsInBounds(MouseInput.X, MouseInput.Y);

            if (!IsMouseOver && inBounds)
            {
                OnMouseEnter(flag);
                //IsMouseOver = inBounds;
            }

            if (IsMouseOver && !inBounds)
            {
                OnMouseLeave(flag);
                //IsMouseOver = inBounds;
            }


            if (inBounds)
            {
                OnMouseMove(flag);

                flag = MouseButtonFlags.None;
                if (MouseInput.IsMouseDownLeft)
                    flag |= MouseButtonFlags.Left;
                if (MouseInput.IsMouseDownRight)
                    flag |= MouseButtonFlags.Right;
                if (flag != MouseButtonFlags.None)
                    OnMouseDown(flag);

                flag = MouseButtonFlags.None;
                if (MouseInput.IsMouseUpLeft)
                    flag |= MouseButtonFlags.Left;
                if (MouseInput.IsMouseUpRight)
                    flag |= MouseButtonFlags.Right;
                if (flag != MouseButtonFlags.None)
                    OnMouseUp(flag);

                if (MouseInput.DScrollWheelValue != 0)
                {
                    OnMouseWheel();
                }
            }
            //}
        }

        protected virtual void OnMouseMove(MouseButtonFlags flag)
        {
            if (MouseMove != null)
                MouseMove(this, flag);
            //if (!IsMouseOver)
            //{
            //    OnMouseEnter();
            //}
            //else if (IsMouseOver)
            //{
            //    OnMouseLeave();
            //}

        }

        /// <summary>
        ///   Raises the <see cref="E:MouseEnter"/> event.
        /// </summary>
        protected virtual void OnMouseEnter(MouseButtonFlags flag)
        {
            // mouse is over
            IsMouseOver = true;
            if (MouseEnter != null)
                MouseEnter(this, flag);
        }

        /// <summary>
        ///   Raises the <see cref="E:MouseLeave"/> event.
        /// </summary>
        protected virtual void OnMouseLeave(MouseButtonFlags flag)
        {
            // mouse is not over
            IsMouseOver = false;
            if (MouseLeave != null)
                MouseLeave(this, flag);
        }

        /// <summary>
        ///   Raises the <see cref="E:MouseDown"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event sounds.</param>
        protected virtual void OnMouseDown(MouseButtonFlags btn)
        {
            if (IsMouseOver)
            {
                // the mouse is now down
                if ((btn & MouseButtonFlags.Left) == MouseButtonFlags.Left)
                {
                    IsPressed = true;
                    if (MouseDown != null)
                        MouseDown(this, btn);
                }

            }
        }

        /// <summary>
        ///   Raises the <see cref="E:MouseUp"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event sounds.</param>
        protected virtual void OnMouseUp(MouseButtonFlags btn)
        {
            // if the mouse was down, we just got a click
            if (IsPressed && (btn & MouseButtonFlags.Left) == MouseButtonFlags.Left)
            {
                IsPressed = false;

                if (Enabled)
                {
                    // raise the event
                    if (MouseUp != null)
                        MouseUp(this, btn);
                    OnMouseClick(btn);
                }
                //IsMouseOver = false;
            }
        }

        protected virtual void OnMouseClick(MouseButtonFlags e)
        {
            if (MouseClick != null)
                MouseClick(this, e);
        }


        protected virtual void OnMouseWheel()
        {
            if (MouseWheel != null)
                MouseWheel(this, MouseInput.DScrollWheelValue);
        }


        public virtual void Dispose(bool disposing)
        {
            
            if (disposing)
            {
                for (int i = 0; i < controls.Count; i++)
                {
                    controls[i].Parent = null;
                }
                controls.Clear();
            }
        }

        //public void InvokeMouseMove(MouseEventArgs e)
        //{
        //    OnMouseMove(e);
        //    for (int i = 0; i < controls.Count; i++)
        //    {
        //        controls[i].InvokeMouseMove(e);
        //    }
        //}
        //public void InvokeMouseUp(MouseEventArgs e)
        //{
        //    OnMouseUp(e);
        //    for (int i = 0; i < controls.Count; i++)
        //    {
        //        controls[i].InvokeMouseUp(e);
        //    }
        //}
        //public void InvokeMouseDown(MouseEventArgs e)
        //{
        //    OnMouseDown(e);
        //    for (int i = 0; i < controls.Count; i++)
        //    {
        //        controls[i].InvokeMouseDown(e);
        //    }
        //}
        //public void InvokeMouseWheel(MouseEventArgs e)
        //{
        //    OnMouseWheel(e);
        //    for (int i = 0; i < controls.Count; i++)
        //    {
        //        controls[i].InvokeMouseWheel(e);
        //    }
        //}

        //public void InvokeKeyPressed(KeyPressEventArgs e)
        //{
        //    if (FocusControl != null)
        //    {
        //        FocusControl.InvokeKeyPressed(e);
        //    }
        //    else
        //    {
        //        this.OnKeyPressed(e);
        //    }
            
            
        //}
    

    }
}
