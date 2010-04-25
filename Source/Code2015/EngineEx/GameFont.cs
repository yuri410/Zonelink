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

        GameFont large;

        private GameFontManager(RenderSystem rs)
        {
            large = new GameFont("");
        }

        public GameFont LargeFont
        {
            get { return large; }
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

        }

        public GameFont(string name)
        {
            FileLocation fl = FileSystem.Instance.Locate(name + ".tex", GameFileLocs.GUI);
            font = UITextureManager.Instance.CreateInstance(fl);

            charHeight = font.Height / 7;

            fl = FileSystem.Instance.Locate(name + ".xml", GameFileLocs.GUI);
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
                    if (ch >= 'A' && ch <= 'Z')
                    {
                        int idx = ch - 'A';

                        srect.X = idx % 7;
                        srect.Y = charHeight * (idx / 7);
                        srect.Width = charHeight;
                        srect.Height = charHeight;

                    }
                    else if (ch >= '0' && ch <= '9')
                    {
                        int idx = ch - '0';

                        srect.X = idx % 7;
                        srect.Y = charHeight * (idx / 7);
                        srect.Width = charHeight;
                        srect.Height = charHeight;
                    }
                    else 
                    {
                        srect.X = 0;
                        srect.Y = 0;
                        srect.Width = 1;
                        srect.Height = 1;
                    }

                    sprite.Draw(font, rect, srect, color);
                    x += (int)(charWidth[ch]);
                }
                else
                {
                    x = 0;
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
