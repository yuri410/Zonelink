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
    class ScoreScreen2 : UIComponent
    {
        Code2015 parent;
        RenderSystem renderSys;
        Menu menu;
        Texture cursor;
        Point mousePosition;
        NormalSoundObject mouseHover;
        NormalSoundObject mouseDown;

        FastList<ScoreEntry> scores = new FastList<ScoreEntry>();

        Texture background;

        RoundButton countinue;
        GameFont font;
        GameFont font2;

        float coolDown;
        int index;
        Texture[] plates;

        public void Add(ScoreEntry entry)
        {
            scores.Add(entry);

            string color;

            if (entry.Player.SideColor == ColorValue.Red)
            {
                color = "r";
            }
            else if (entry.Player.SideColor == ColorValue.Green)
            {
                color = "g";
            }
            else if (entry.Player.SideColor == ColorValue.Yellow)
            {
                color = "y";
            }
            else
            {
                color = "b";
            }
            FileLocation fl = FileSystem.Instance.Locate("ss_" + (index + 1).ToString() + color + ".tex", GameFileLocs.GUI);
            plates[index++] = UITextureManager.Instance.CreateInstance(fl);
        }
        public void Clear()
        {
            coolDown = 2;
            index = 0;
            scores.Clear();
        }

        public ScoreScreen2(Code2015 parent, Menu menu)
        {
            this.menu = menu;
            this.parent = parent;
            this.renderSys = parent.RenderSystem;

            FileLocation fl = FileSystem.Instance.Locate("ss_bg.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl);



            font = GameFontManager.Instance.F18G1;
            font2 = GameFontManager.Instance.F20IG1;

            countinue = new RoundButton();
            countinue.Enabled = true;
            countinue.IsValid = true;
            //fl = FileSystem.Instance.Locate("ss_continue.tex", GameFileLocs.GUI);
            //countinue.Image = UITextureManager.Instance.CreateInstance(fl);

            countinue.X = 357;
            countinue.Y = 493;
            //countinue.Width = countinue.Image.Width;
            //countinue.Height = countinue.Image.Height;
            //fl = FileSystem.Instance.Locate("ss_continuehouver.tex", GameFileLocs.GUI);
            //countinue.ImageMouseOver = UITextureManager.Instance.CreateInstance(fl);


            fl = FileSystem.Instance.Locate("cursor.tex", GameFileLocs.GUI);
            cursor = UITextureManager.Instance.CreateInstance(fl);
            mouseHover = (NormalSoundObject)SoundManager.Instance.MakeSoundObjcet("buttonHover", null, 0);
            mouseDown = (NormalSoundObject)SoundManager.Instance.MakeSoundObjcet("buttonDown", null, 0);

            plates = new Texture[4];
        }
        void DrawString(Sprite sprite , string str, int x, int y)
        {
            font.DrawString(sprite, str, x + 1, y + 1, ColorValue.Black);
            font.DrawString(sprite, str, x, y, ColorValue.LightGray);


        }
        void DrawString2(Sprite sprite, string str, int x, int y)
        {
            font2.DrawString(sprite, str, x + 1, y + 1, ColorValue.Black);
            font2.DrawString(sprite, str, x, y, ColorValue.White);


        }

        public override void Render(Sprite sprite)
        {

            sprite.Draw(background, 0, 0, ColorValue.White);


            ColorValue ftc = scores.Elements[0].Player.SideColor;
            
            if (plates[0] != null) 
            {
                sprite.Draw(plates[0], 0, 0, ColorValue.White);
            }

            DrawString(sprite, scores.Elements[0].Player.Name.ToUpperInvariant(), 245, 300);

            string msg = ((int)Math.Round(scores.Elements[0].Development)).ToString("G");
            DrawString(sprite, msg, 351, 355);

            msg = ((int)Math.Round(scores.Elements[0].CO2)).ToString("G");
            DrawString(sprite, msg, 352, 384);

            msg = ((int)Math.Round(scores.Elements[0].Total)).ToString("G");
            DrawString2(sprite, msg, 274, 416);



            ftc = scores.Elements[1].Player.SideColor;

            if (plates[1] != null)
            {
                sprite.Draw(plates[1], 0, 0, ColorValue.White);
            }
            DrawString(sprite, scores.Elements[1].Player.Name.ToUpperInvariant(), 630, 326);

            msg = ((int)Math.Round(scores.Elements[1].Development)).ToString("G");
            DrawString(sprite, msg, 719, 371);

            msg = ((int)Math.Round(scores.Elements[1].CO2)).ToString("G");
            DrawString(sprite, msg, 719, 395); 

            msg = ((int)Math.Round(scores.Elements[1].Total)).ToString("G");
            DrawString2(sprite, msg, 655, 420);



            ftc = scores.Elements[2].Player.SideColor;

            if (plates[2] != null)
            {
                sprite.Draw(plates[2], 0, 0, ColorValue.White);
            }

            DrawString(sprite, scores.Elements[2].Player.Name.ToUpperInvariant(), 255, 521);

            msg = ((int)Math.Round(scores.Elements[2].Development)).ToString("G");
            DrawString(sprite, msg, 341, 571);
           
            msg = ((int)Math.Round(scores.Elements[2].CO2)).ToString("G");
            DrawString(sprite, msg, 341, 594);
           
            msg = ((int)Math.Round(scores.Elements[2].Total)).ToString("G");
            DrawString2(sprite, msg, 278, 621);


            ftc = scores.Elements[3].Player.SideColor;

            if (plates[3] != null)
            {
                sprite.Draw(plates[3], 0, 0, ColorValue.White);
            }

            DrawString(sprite, scores.Elements[3].Player.Name.ToUpperInvariant(), 631, 523);

            msg = ((int)Math.Round(scores.Elements[3].Development)).ToString("G");
            DrawString(sprite, msg, 715, 571);
            

            msg = ((int)Math.Round(scores.Elements[3].CO2)).ToString("G");
            DrawString(sprite, msg, 715, 594);
           
            msg = ((int)Math.Round(scores.Elements[3].Total)).ToString("G");
            DrawString2(sprite, msg, 652, 621);


            countinue.Render(sprite);

            sprite.Draw(cursor, mousePosition.X, mousePosition.Y, ColorValue.White);
        }

        public override void Update(GameTime time)
        {
            mousePosition.X = MouseInput.X;
            mousePosition.Y = MouseInput.Y;
            //countinue.Update(time);


            if (coolDown > 0)
                coolDown -= time.ElapsedGameTimeSeconds;
            else
            {
                if (MouseInput.IsMouseUpLeft)
                {
                    menu.CurrentScreen = null;
                }
            }
        }
    }
}
