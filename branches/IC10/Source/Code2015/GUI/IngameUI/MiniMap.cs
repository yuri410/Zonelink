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
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.GUI.Controls;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.World;

namespace Code2015.GUI
{
    class MiniMap : UIComponent
    {
        const int PanelX = 899;
        const int PanelY = 528;
        const int PanelWidth = 388;
        const int PanelHeight = 179;

        const float PopBaseSpeed = 0.75f;

        const int MapX = 20;
        const int MapY = 30;
        const int MapWidth = 360;
        const int MapHeight = 190;

        const float RotIn = -MathEx.PiOver2;

        GameScene scene;
        GameState gameLogic;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;
        RtsCamera camera;

        FastList<Popup2> marks = new FastList<Popup2>();

        Texture background;

        #region 内容显示
        Texture cameraView;

        Texture redDot;
        Texture greenDot;
        Texture blueDot;
        Texture yellowDot;

        Texture whiteDot;

        Texture redRing;
        Texture greenRing;
        Texture blueRing;
        Texture yellowRing;
        Dictionary<City, Point> positionBuffer = new Dictionary<City, Point>();
        #endregion

        //float rot;

        public MiniMap(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.parent = parent;
            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;
            this.gameLogic = gamelogic;
            this.camera = scene.Camera;

            FileLocation fl = FileSystem.Instance.Locate("nig_minimap.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl);


            #region 内容显示
            fl = FileSystem.Instance.Locate("ig_map_view.tex", GameFileLocs.GUI);
            cameraView = UITextureManager.Instance.CreateInstance(fl);


            fl = FileSystem.Instance.Locate("ig_mp_redpoint.tex", GameFileLocs.GUI);
            redDot = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ig_mp_greenpoint.tex", GameFileLocs.GUI);
            greenDot = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ig_mp_yellowpoint.tex", GameFileLocs.GUI);
            yellowDot = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ig_mp_bluepoint.tex", GameFileLocs.GUI);
            blueDot = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ig_mp_whitepoint.tex", GameFileLocs.GUI);
            whiteDot = UITextureManager.Instance.CreateInstance(fl);


            fl = FileSystem.Instance.Locate("ig_mp_red.tex", GameFileLocs.GUI);
            redRing  = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ig_mp_green.tex", GameFileLocs.GUI);
            greenRing = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ig_mp_yellow.tex", GameFileLocs.GUI);
            yellowRing = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ig_mp_blue.tex", GameFileLocs.GUI);
            blueRing = UITextureManager.Instance.CreateInstance(fl);

            #endregion

        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                background.Dispose();
                background = null;

            }

        }
   

        public override int Order
        {
            get { return 60; }
        }
        public override bool HitTest(int x, int y)
        {
            //if (state == AnimState.Outside)
            //{
                Rectangle rect = new Rectangle(PanelX, PanelY, PanelWidth, PanelHeight);
                return Control.IsInBounds(x, y, ref rect);
            //}
            //Rectangle rect2 = new Rectangle(51, 688, 111, 38);
            //return Control.IsInBounds(x, y, ref rect2);
        }

        Point GetPosition(float lng, float lat) 
        {
            int cx;
            int cy;
            float yspan = MathEx.PIf;

            cy = (int)(((yspan * 0.5f - lat) / yspan) * MapHeight);
            cx = (int)(((lng + MathEx.PIf) / (2 * MathEx.PIf)) * MapWidth);

            if (cy < 0) cy += MapHeight;
            if (cy >= MapHeight) cy -= MapHeight;

            if (cx < 0) cx += MapWidth;
            if (cx >= MapWidth) cx -= MapWidth;
            return new Point(cx, cy);
        }

        public void AddNotificationMark(float lnt, float lat, ColorValue color)
        {
            Point pt = GetPosition(MathEx.Degree2Radian(lnt), MathEx.Degree2Radian(lat));

            pt.X += MapX;
            pt.Y += MapY + PanelY;

            Texture tex = redRing;
            if (color == ColorValue.Yellow)
            {
                tex = yellowRing;
            }
            else if (color == ColorValue.Blue)
            {
                tex = blueRing;
            }
            else if (color == ColorValue.Green)
            {
                tex = greenRing;
            }

            Popup2 pop2 = new Popup2(renderSys, tex, pt.X, pt.Y, 3);

            marks.Add(pop2);
        }

        public override void Render(Sprite sprite)
        {
            sprite.SetTransform(Matrix.Translation(PanelX, PanelY + PanelHeight, 0));
            sprite.Draw(background, 0, -PanelHeight, ColorValue.White);

            //if (switchButton.IsPressed)
            //{
            //    sprite.Draw(switchButton.ImageMouseDown, switchButton.X, switchButton.Y - (PanelY + PanelHeight), switchButton.ModulateColor);
            //}
            //else if (switchButton.IsMouseOver)
            //{
            //    sprite.Draw(switchButton.ImageMouseOver, switchButton.X, switchButton.Y - (PanelY + PanelHeight), switchButton.ModulateColor);
            //}
            //else
            //{
            //    sprite.Draw(switchButton.Image, switchButton.X, switchButton.Y - (PanelY + PanelHeight), switchButton.ModulateColor);
            //}

            BattleField field = gameLogic.Field;

            #region 遍历战场上的城市，绘制地图上点
            for (int i = 0; i < field.CityCount; i++)
            {
                City cc = field.Cities[i];

                if (!cc.IsHomeCity)
                {
                    Texture dot;
                    if (cc.IsCaptured)
                    {
                        ColorValue color = cc.Owner.SideColor;
                        if (color == ColorValue.Red)
                        {
                            dot = redDot;
                        }
                        else if (color == ColorValue.Green)
                        {
                            dot = greenDot;
                        }
                        else if (color == ColorValue.Blue)
                        {
                            dot = blueDot;
                        }
                        else
                        {
                            dot = yellowDot;
                        }
                    }
                    else
                    {
                        dot = whiteDot;
                    }

                    Point pt;
                    if (!positionBuffer.TryGetValue(cc, out pt))
                    {
                        pt = GetPosition(MathEx.Degree2Radian(cc.Longitude), MathEx.Degree2Radian(cc.Latitude));

                        pt.X += MapX;
                        pt.Y = pt.Y + MapY - PanelHeight;

                        positionBuffer.Add(cc, pt);
                    }
                    sprite.Draw(dot, pt.X - 7, pt.Y - 7, ColorValue.White);
                }
                else
                {
                    Texture ring;

                    if (cc.IsCaptured)
                    {
                        ColorValue color = cc.Owner.SideColor;
                        if (color == ColorValue.Red)
                        {
                            ring = redRing;
                        }
                        else if (color == ColorValue.Green)
                        {
                            ring = greenRing;
                        }
                        else if (color == ColorValue.Blue)
                        {
                            ring = blueRing;
                        }
                        else
                        {
                            ring = yellowRing;
                        }

                        Point pt;
                        if (!positionBuffer.TryGetValue(cc, out pt))
                        {
                            pt = GetPosition(MathEx.Degree2Radian(cc.Longitude), MathEx.Degree2Radian(cc.Latitude));

                            pt.X += MapX;
                            pt.Y = pt.Y + MapY - PanelHeight;

                            positionBuffer.Add(cc, pt);
                        }
                        sprite.Draw(ring, pt.X - 12, pt.Y - 12, ColorValue.White);
                    }

                }
            }
            #endregion

            #region 绘制摄像机
            {
                Point pt = GetPosition(camera.Longitude, camera.Latitude);

                float ratio = camera.Height / 60f;
                Rectangle rect = new Rectangle(
                    pt.X + MapX - (int)(cameraView.Width * ratio * 0.5f),
                    pt.Y + MapY - (int)(cameraView.Height * ratio * 0.5f), (int)(cameraView.Width * ratio), (int)(cameraView.Height * ratio));
                rect.Y -= PanelHeight;

                sprite.Draw(cameraView, rect, ColorValue.White);
            }
            #endregion

            sprite.SetTransform(Matrix.Identity);

            #region 绘制事件标记
            for (int i = 0; i < marks.Count; i++)
            {
                marks[i].Render(sprite);
            }
            #endregion

            //sprite.Draw(compass, -21, 666, ColorValue.White);
        }


        public override void UpdateInteract(GameTime time)
        {
            //switchButton.Update(time);

            //if (state == AnimState.Inside)
            //{
            //    state = AnimState.Out;
            //}
            //if (state == AnimState.Outside)
            //{

            Rectangle rect = new Rectangle(PanelX + MapX - 20, PanelY + MapY - 20, MapWidth, MapHeight + 40);
            Rectangle rect2 = new Rectangle(PanelX + MapX + 67, PanelY + MapY, MapWidth, MapHeight);

            if (Control.IsInBounds(MouseInput.X, MouseInput.Y, ref rect))
            {
                if (MouseInput.IsLeftPressed)
                {
                    int x = MouseInput.X;
                    int y = MouseInput.Y;

                    if (x < rect2.Left)
                        x = rect2.Left;
                    if (y < rect2.Top)
                        y = rect2.Top;
                    if (x > rect2.Right)
                        x = rect2.Right;
                    if (y > rect2.Bottom)
                        y = rect2.Bottom;
                    x = x - MapX - PanelX;
                    y = y - MapY - PanelY;

                    float yspan = MathEx.PIf;

                    camera.Latitude = yspan * 0.5f - y * yspan / (float)MapHeight;
                    camera.Longitude = x * MathEx.PIf * 2 / (float)MapWidth - MathEx.PIf;
                }
            }
        }
        public override void Update(GameTime time)
        {
            const float StdRot = 0;
            //if (state == AnimState.Out)
            //{
            //    rot += (StdRot - rot + PopBaseSpeed) * time.ElapsedGameTimeSeconds * 2;
            //    if (rot >= StdRot)
            //    {
            //        rot = StdRot;
            //        state = AnimState.Outside;
            //    }

            //}
            //else if (state == AnimState.In)
            //{
            //    rot -= (StdRot - rot + PopBaseSpeed) * time.ElapsedGameTimeSeconds * 2;
            //    if (rot < RotIn)
            //    {
            //        rot = RotIn;
            //        state = AnimState.Inside;                    
            //    }
            //}
            for (int i = marks.Count - 1; i >= 0; i--)
            {
                if (marks[i].IsFinished)
                {
                    marks.RemoveAt(i);
                }
                else
                {
                    marks[i].Update(time);
                }
            }
        }
    }
}
