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
using Apoc3D.Graphics;
using Apoc3D.GUI.Controls;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.GUI.IngameUI;

namespace Code2015.GUI
{
    class Tutorial : UIComponent
    {
//        back_btn	498,557
//help_bg		0,0
//help_next	655,535
//help_panel	97,24
        const int TotalPages = 14;
        Menu parent;
        Texture background;
        Texture panel;
        Texture[] help;
        Texture[] helpText;

        Texture nextBtn;
        Texture prevBtn;
        Texture exitBtn;

        Texture cursor;
        Point mousePosition;

        int currentPage;

        Button nextButton;
        Button prevButton;        
        Button exitButton;

        NormalSoundObject mouseHover;
        NormalSoundObject mouseDown;


        float showPrg;
        NIGDialogState state;


        public Tutorial(Menu parent)
        {
            this.parent = parent;
            help = new Texture[TotalPages];
            helpText = new Texture[TotalPages];

            for (int i = 0; i < 14; i++)
            {
                FileLocation fl = FileSystem.Instance.Locate("sh" + (i + 1).ToString() + ".tex", GameFileLocs.Help);
                help[i] = UITextureManager.Instance.CreateInstance(fl);
                fl = FileSystem.Instance.Locate("sh" + (i + 1).ToString() + "t.tex", GameFileLocs.Help);
                helpText[i] = UITextureManager.Instance.CreateInstance(fl);
            }


            FileLocation fl2 = FileSystem.Instance.Locate("help_bg.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl2);

            fl2 = FileSystem.Instance.Locate("cursor.tex", GameFileLocs.GUI);
            cursor = UITextureManager.Instance.CreateInstance(fl2);



            fl2 = FileSystem.Instance.Locate("help_next.tex", GameFileLocs.GUI);
            nextBtn = UITextureManager.Instance.CreateInstance(fl2);
            fl2 = FileSystem.Instance.Locate("help_back.tex", GameFileLocs.GUI);
            prevBtn = UITextureManager.Instance.CreateInstance(fl2);
            fl2 = FileSystem.Instance.Locate("nig_m_btn_back.tex", GameFileLocs.GUI);
            exitBtn = UITextureManager.Instance.CreateInstance(fl2);

            fl2 = FileSystem.Instance.Locate("help_panel.tex", GameFileLocs.GUI);
            panel = UITextureManager.Instance.CreateInstance(fl2);


            nextButton = new Button();
            nextButton.Enabled = true;
            nextButton.IsValid = true;
            nextButton.X = 655;
            nextButton.Y = 535;
            nextButton.Width = nextBtn.Width;
            nextButton.Height = nextBtn.Height;
            nextButton.MouseClick += Continue_Click;
            nextButton.MouseEnter += Button_MouseIn;
            nextButton.MouseDown += Button_DownSound;

            prevButton = new Button();
            prevButton.Enabled = true;
            prevButton.IsValid = true;
            prevButton.X = 498;
            prevButton.Y = 540;
            prevButton.Width = prevBtn.Width;
            prevButton.Height = prevBtn.Height;
            prevButton.MouseClick += Continue_Click;
            prevButton.MouseEnter += Button_MouseIn;
            prevButton.MouseDown += Button_DownSound;

            exitButton = new Button();
            exitButton.Enabled = true;
            exitButton.IsValid = true;
            exitButton.X = 990;
            exitButton.Y = 560;
            exitButton.Width = exitBtn.Width;
            exitButton.Height = exitBtn.Height;
            exitButton.MouseClick += Exit_Click;
            exitButton.MouseEnter += Button_MouseIn;
            exitButton.MouseDown += Button_DownSound;


            mouseHover = (NormalSoundObject)SoundManager.Instance.MakeSoundObjcet("buttonHover", null, 0);
            mouseDown = (NormalSoundObject)SoundManager.Instance.MakeSoundObjcet("buttonDown", null, 0);

            state = NIGDialogState.Hiding;
        }
        public void Reset() { currentPage = 0; }
        public bool Advance()
        {
            currentPage++;
            return currentPage < TotalPages;
        }
        public bool Back()
        {
            currentPage--;
            return currentPage >= 0;
        }

        public bool IsFinished
        {
            get { return currentPage >= TotalPages; }
        }


        public void NotifyShown() 
        {
            state = NIGDialogState.MovingIn;
        }
        void Close() 
        {

            state = NIGDialogState.MovingOut;

        }
        void Button_MouseIn(object sender, MouseButtonFlags btn)
        {
            mouseHover.Fire();
        }
        void Button_DownSound(object sender, MouseButtonFlags btn)
        {
            if (btn == MouseButtonFlags.Left)
            {
                mouseDown.Fire();
            }
        }
        void Prev_Click(object sender, MouseButtonFlags btn)
        {
            if (btn == MouseButtonFlags.Left)
            {
                if (!Back())
                {
                }
            }
        }
        void Continue_Click(object sender,MouseButtonFlags btn)
        {
            if (btn == MouseButtonFlags.Left) 
            {
                if (!Advance())
                {
                    
                }
            }
        }
        void Exit_Click(object sender, MouseButtonFlags btn)
        {
            if (btn == MouseButtonFlags.Left)
            {
                state = NIGDialogState.MovingOut;
            }
        }

        public override void Render(Sprite sprite)
        {
            sprite.Draw(background, 0, 0, ColorValue.White);

            if (showPrg > 0.5f)
            {
                Matrix trans = Matrix.Translation(-background.Width / 2, -background.Height / 2, 0) *
                   Matrix.Scaling((showPrg - 0.5f) * 2 * 0.8f + 0.2f, 1, 1) *
                   Matrix.Translation(97 + background.Width / 2, 24 + background.Height / 2, 0);

                sprite.SetTransform(trans);
                sprite.Draw(panel, 0, 0, ColorValue.White);
                sprite.SetTransform(Matrix.Identity);
            }

            int idx = currentPage;
            if (idx >= TotalPages)
                idx = TotalPages - 1;

            if (state == NIGDialogState.Showing)
            {
                sprite.Draw(nextBtn, nextButton.X, nextButton.Y, ColorValue.White);
                if (nextButton.IsMouseOver && !nextButton.IsPressed)
                {
                    sprite.Draw(nextBtn, nextButton.X, nextButton.Y - 1, ColorValue.White);
                }


                sprite.Draw(prevBtn, prevButton.X, prevButton.Y, ColorValue.White);
                if (prevButton.IsMouseOver && !exitButton.IsPressed)
                {
                    sprite.Draw(prevBtn, prevButton.X, prevButton.Y - 1, ColorValue.White);
                }


                sprite.Draw(exitBtn, exitButton.X, exitButton.Y, ColorValue.White);
                if (exitButton.IsMouseOver && !exitButton.IsPressed)
                {
                    sprite.Draw(exitBtn, exitButton.X, exitButton.Y - 3, ColorValue.White);
                }
            }

            Rectangle rect = new Rectangle(62, 186, 616, 365);
            sprite.Draw(help[idx], rect, ColorValue.White);
            sprite.Draw(helpText[idx], 84, 15, ColorValue.White);

            sprite.Draw(cursor, mousePosition.X, mousePosition.Y, ColorValue.White);
        }
        public override void Update(GameTime time)
        {
            mousePosition.X = MouseInput.X;
            mousePosition.Y = MouseInput.Y;


            if (state == NIGDialogState.MovingIn)
            {
                showPrg += time.ElapsedGameTimeSeconds * 4;
                if (showPrg > 1)
                {
                    showPrg = 1;
                    state = NIGDialogState.Showing;
                }
            }
            else if (state == NIGDialogState.MovingOut)
            {
                showPrg -= time.ElapsedGameTimeSeconds * 4;
                if (showPrg < 0)
                {
                    showPrg = 0;
                    state = NIGDialogState.Hiding;
                    parent.Back();
                }
            }

            if (state == NIGDialogState.Showing)
            {
                nextButton.Update(time);
                prevButton.Update(time);
                exitButton.Update(time);
            }
            
        }
    }
}
