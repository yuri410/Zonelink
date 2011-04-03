﻿/*
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
using Apoc3D.Config;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.GUI;

namespace Code2015.EngineEx
{
    class GameFontManager
    {
        static GameFontManager singleton;

        public static GameFontManager Instance
        {
            get
            {
                return singleton;
            }
        }

        public static void Initiaize(RenderSystem rs)
        {
            singleton = new GameFontManager(rs);
        }

        GameFont f201;
        GameFont f18;
        GameFont f14;
        GameFont f20ig1;
        GameFont f18g1;

        private GameFontManager(RenderSystem rs)
        {
            f20ig1 = new GameFont("f20ig1");
            f18g1 = new GameFont("f18g1"); 
            f201 = new GameFont("f20i");
            f14 = new GameFont("f14");
            f18 = new GameFont("f18");
        }
        public GameFont F18G1 
        {
            get { return f18g1; }
        }
        public GameFont F20IG1
        {
            get { return f20ig1; }
        }
        public GameFont F14
        {
            get { return f18; }
        }

        public GameFont F18
        {
            get { return f18; }
        }
        public GameFont F20I
        {
            get { return f201; }
        }
    }
    class GameFont
    {        
        Texture font;
        int[] charWidth;
        int charHeight;


        static readonly int[] IndexCast;
        static GameFont() 
        {
            int idx =0;
            IndexCast = new int[byte.MaxValue];
            for (char c = 'A'; c <= 'Z'; c++)
            {
                IndexCast[c] = idx++;
            }
            for (char c = '0'; c <= '9'; c++)
            {
                IndexCast[c] = idx++;
            }
            IndexCast['.'] = idx++;
            IndexCast[':'] = idx++;
            IndexCast[';'] = idx++;
            IndexCast[','] = idx++;
            IndexCast['?'] = idx++;
            IndexCast['!'] = idx++;
            IndexCast['\''] = idx++;
            IndexCast['('] = idx++;
            IndexCast[')'] = idx++;
            IndexCast[' '] = idx++;

        }

        public GameFont(string name)
        {
            FileLocation fl = FileSystem.Instance.Locate(name + ".tex", GameFileLocs.GUI);
            font = UITextureManager.Instance.CreateInstance(fl);

            charHeight = font.Height / 7;

            fl = FileSystem.Instance.Locate(name + ".xml", GameFileLocs.Config);
            Configuration config = ConfigurationManager.Instance.CreateInstance(fl);

            charWidth = new int[byte.MaxValue];

            foreach (KeyValuePair<string, ConfigurationSection> e in config)
            {
                ConfigurationSection sect = e.Value;

                char ch = (char)sect.GetInt("Char");

                charWidth[ch] = sect.GetInt("Width");
            }
        }

        public void DrawString(Sprite sprite, string text, int x, int y, ColorValue color) 
        {
            int std = x;
            for (int i = 0; i < text.Length; i++)
            {
                char ch = text[i];
                if (ch != '\n')
                {

                    Rectangle rect;
                    rect.X = x;
                    rect.Y = y;
                    rect.Width = charHeight;
                    rect.Height = charHeight;

                    Rectangle srect;


                    int idx = IndexCast[ch];

                    srect.X = charHeight * (idx % 7);
                    srect.Y = charHeight * (idx / 7);
                    srect.Width = charHeight;
                    srect.Height = charHeight;



                    sprite.Draw(font, rect, srect, color);
                    x += charWidth[ch] - 3;
                }
                else
                {
                    x = std;
                    y += charHeight;
                }
            }
        }
        public Size MeasureString(string text)
        {
            Size result = new Size(0, charHeight);

            int x = 0, y = 0;
            for (int i = 0; i < text.Length; i++)
            {
                char ch = text[i];
                if (ch != '\n')
                {
                    x += (int)(charWidth[ch]);
                }
                else
                {
                    x = 0;
                    y += charHeight;
                }

                if (result.Width < x)
                    result.Width = x;
                if (result.Height < y)
                    result.Height = y;
            }
            
            return result;
        }
    }
}