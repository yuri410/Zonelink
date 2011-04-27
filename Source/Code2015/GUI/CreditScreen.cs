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
using Apoc3D.Graphics;
using Apoc3D;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Apoc3D.GUI.Controls;

namespace Code2015.GUI
{
    class CreditScreen : UIComponent
    {
        RenderSystem renderSys;
        Menu parent;

        Texture crdRuan;
        Texture crdXin;
        Texture crdYb;
        Texture crdZj;
        Texture crdMusic;

        Button backBtn;

        float ruanProgress;
        float xinProgress;
        float zjProgress;
        float ybProgress;
        float musicProgress;


        Texture cursor;
        Texture bg;
        //Texture list;
        Point mousePosition;

        NormalSoundObject mouseHover;
        NormalSoundObject mouseDown;
        float roll;
        public CreditScreen(RenderSystem rs, Menu parent)
        {
            this.renderSys = rs;
            this.parent = parent;

            FileLocation fl = FileSystem.Instance.Locate("cursor.tex", GameFileLocs.GUI);
            cursor = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("credits_bg.tex", GameFileLocs.GUI);
            bg = UITextureManager.Instance.CreateInstance(fl);
            //fl = FileSystem.Instance.Locate("crd_list.tex", GameFileLocs.GUI);
            //list = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("crd_ruan.tex", GameFileLocs.GUI);
            crdRuan = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("crd_xin.tex", GameFileLocs.GUI);
            crdXin = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("crd_yb.tex", GameFileLocs.GUI);
            crdYb = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("crd_zj.tex", GameFileLocs.GUI);
            crdZj = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("crd_music.tex", GameFileLocs.GUI);
            crdMusic = UITextureManager.Instance.CreateInstance(fl);

            backBtn = new Button();
            fl = FileSystem.Instance.Locate("back_btn.tex", GameFileLocs.GUI);
            backBtn.Image = UITextureManager.Instance.CreateInstance(fl);
            backBtn.X = 1000;
            backBtn.Y = 600;
            backBtn.Width = backBtn.Image.Width;
            backBtn.Height = backBtn.Image.Height;
            backBtn.Enabled = true;
            backBtn.IsValid = true;
            backBtn.MouseClick += Back_Click;

            Reset();

            mouseHover = (NormalSoundObject)SoundManager.Instance.MakeSoundObjcet("buttonHover", null, 0);
            mouseDown = (NormalSoundObject)SoundManager.Instance.MakeSoundObjcet("buttonDown", null, 0);
        }


        private void Reset()
        {
            const float interval = 0.33f;

            ruanProgress = -interval;
            ybProgress = -interval * 2;
            xinProgress = -interval * 3;
            zjProgress = -interval * 4;
            musicProgress = -interval * 5;
        }

        public override void Render(Sprite sprite)
        {
            sprite.Draw(bg, 0, 0, ColorValue.White);

            //int panelHeight = 538;
            //int h = (int)(roll * (list.Height + panelHeight));

            //if (h < panelHeight)
            //{
            //    Rectangle drect = new Rectangle(658, 46 + panelHeight - h, list.Width, h);
            //    Rectangle srect = new Rectangle(0, 0, list.Width, h);
            //    sprite.Draw(list, drect, srect, ColorValue.White);
            //}
            //else if (h > list.Height - panelHeight)
            //{
            //    Rectangle drect = new Rectangle(658, 46, list.Width, list.Height * 2 - h + panelHeight);
            //    Rectangle srect = new Rectangle(0, h - panelHeight, list.Width, list.Height * 2 - h + panelHeight);
            //    sprite.Draw(list, drect, srect, ColorValue.White);
            //}

            if (ruanProgress > 0)
            {
                float rolescale = -MathEx.Sqr(MathEx.Saturate(ruanProgress * 6) * 1.5f - 1) + 1;
                rolescale = (1.0f / 0.75f) * rolescale;
                Matrix roletrans = Matrix.Scaling(rolescale, rolescale, 1) *
                        Matrix.Translation(179 + crdRuan.Width / 2, 292 + crdRuan.Height / 2, 0);
                sprite.SetTransform(roletrans);
                sprite.Draw(crdRuan, -crdRuan.Width / 2, -crdRuan.Height / 2, ColorValue.White);
                sprite.SetTransform(Matrix.Identity);
            }


            if (zjProgress > 0)
            {
                float rolescale = -MathEx.Sqr(MathEx.Saturate(zjProgress * 6) * 1.5f - 1) + 1;
                rolescale = (1.0f / 0.75f) * rolescale;
                Matrix roletrans = Matrix.Scaling(rolescale, rolescale, 1) *
                        Matrix.Translation(701 + crdZj.Width / 2, 465 + crdZj.Height / 2, 0);
                sprite.SetTransform(roletrans);
                sprite.Draw(crdZj, -crdZj.Width / 2, -crdZj.Height / 2, ColorValue.White);
                sprite.SetTransform(Matrix.Identity);
            }

            if (xinProgress > 0)
            {
                float rolescale = -MathEx.Sqr(MathEx.Saturate(xinProgress * 6) * 1.5f - 1) + 1;
                rolescale = (1.0f / 0.75f) * rolescale;
                Matrix roletrans = Matrix.Scaling(rolescale, rolescale, 1) *
                        Matrix.Translation(230 + crdXin.Width / 2, 460 + crdXin.Height / 2, 0);
                sprite.SetTransform(roletrans);
                sprite.Draw(crdXin, -crdXin.Width / 2, -crdXin.Height / 2, ColorValue.White);
                sprite.SetTransform(Matrix.Identity);
            }

            if (ybProgress > 0)
            {
                float rolescale = -MathEx.Sqr(MathEx.Saturate(ybProgress * 6) * 1.5f - 1) + 1;
                rolescale = (1.0f / 0.75f) * rolescale;
                Matrix roletrans = Matrix.Scaling(rolescale, rolescale, 1) *
                        Matrix.Translation(695 + crdYb.Width / 2, 288 + crdYb.Height / 2, 0);
                sprite.SetTransform(roletrans);
                sprite.Draw(crdYb, -crdYb.Width / 2, -crdYb.Height / 2, ColorValue.White);
                sprite.SetTransform(Matrix.Identity);
            }

            if (musicProgress > 0)
            {
                float rolescale = -MathEx.Sqr(MathEx.Saturate(musicProgress * 6) * 1.5f - 1) + 1;
                rolescale = (1.0f / 0.75f) * rolescale;
                Matrix roletrans = Matrix.Scaling(rolescale, rolescale, 1) *
                        Matrix.Translation(218 + crdMusic.Width / 2, 649 + crdMusic.Height / 2, 0);
                sprite.SetTransform(roletrans);
                sprite.Draw(crdMusic, -crdMusic.Width / 2, -crdMusic.Height / 2, ColorValue.White);
                sprite.SetTransform(Matrix.Identity);
            }

       
            sprite.Draw(backBtn.Image, backBtn.X, backBtn.Y, ColorValue.White);
            if (backBtn.IsMouseOver)
            {
                sprite.Draw(backBtn.Image, backBtn.X, backBtn.Y - 4, ColorValue.White);
            }
            
            sprite.Draw(cursor, mousePosition.X, mousePosition.Y, ColorValue.White);
        }


        public override void Update(GameTime time)
        {
            mousePosition.X = MouseInput.X;
            mousePosition.Y = MouseInput.Y;

            backBtn.Update(time);

            roll += 0.1f * time.ElapsedGameTimeSeconds;
            if (roll > 1)
                roll = 0;


            float dt = time.ElapsedGameTimeSeconds;

            ruanProgress += dt * 1.5f;
            if (ruanProgress > 1)
            {
                ruanProgress = 1;
            }

            xinProgress += dt * 1.5f;
            if (xinProgress > 1)
            {
                xinProgress = 1;
            }

            zjProgress += dt * 1.5f;
            if (zjProgress > 1)
            {
                zjProgress = 1;
            }

            ybProgress += dt * 1.5f;
            if (ybProgress > 1)
            {
                ybProgress = 1;
            }

            musicProgress += dt * 1.5f;
            if (musicProgress > 1)
            {
                musicProgress = 1;
            }


        }


        void Back_Click(object sender, MouseButtonFlags btn)
        {
            Reset();

            parent.Back();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
