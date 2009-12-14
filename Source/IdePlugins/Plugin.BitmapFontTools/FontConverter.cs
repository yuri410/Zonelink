using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Apoc3D.Ide;
using Apoc3D.Ide.Converters;
using Apoc3D.Vfs;

namespace Plugin.BitmapFontTools
{
    class FontConverter : ConverterBase
    {        
        FontFamily currentFont;

        public FontConverter() 
        {
            FontSize = 10;
        }

        public float FontSize
        {
            get;
            set;
        }

        public override void ShowDialog(object sender, EventArgs e)
        {
            FontDlg dlg = new FontDlg(FontFamily.Families);

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string path = dlg.DestPath;
                FontFamily[] fonts = dlg.SelectedFonts();

                ProgressDlg pd = new ProgressDlg(DevStringTable.Instance["GUI:Converting"]);

                pd.MinVal = 0;
                pd.Value = 0;
                pd.MaxVal = fonts.Length;

                pd.Show();
                for (int i = 0; i < fonts.Length; i++)
                {
                    string dest = Path.Combine(path, Path.GetFileNameWithoutExtension(fonts[i].Name) + ".fnt");
                    currentFont = fonts[i];
                    Convert(null, new DevFileLocation(dest));
                    pd.Value = i;
                }
                pd.Close();
                pd.Dispose();

            }
        }

        public unsafe override void Convert(ResourceLocation source, ResourceLocation dest)
        {
            const int origWidth = 64;
            const int origHeight = 64;
            const int Id = 'S' << 24 | 'F' << 16 | 'N' << 8 | 'T';

            Font font = new Font(currentFont, FontSize);

            ContentBinaryWriter bw = new ContentBinaryWriter(dest);
            bw.Write(Id);
            bw.Write((int)0);
            bw.Write(byte.MaxValue);
            bw.Write((int)origWidth);
            bw.Write((int)origHeight);
            
            for (char c = '\0'; c < 256; c++)
            {
                Bitmap bmp = new Bitmap(origWidth, origHeight);
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);

                Size size = TextRenderer.MeasureText(c.ToString(), font);
                TextRenderer.DrawText(g, c.ToString(), font, Point.Empty, Color.White);

                g.Dispose();

                BitmapData data = bmp.LockBits(new Rectangle(0, 0, origWidth, origHeight), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                byte* src = (byte*)data.Scan0;

                bw.Write(c);
                bw.Write(size.Width);
                for (int i = 0; i < origHeight; i++)
                {
                    for (int j = 0; j < origWidth; j++)
                    {
                        src++;
                        byte red = *src++;
                        byte gr = *src++;
                        byte bl = *src++;

                        float lum = (red * 0.3f + gr * 0.59f + bl * 0.11f);
                        bw.Write((byte)lum);
                    }
                }
               
                bmp.UnlockBits(data);
                bmp.Dispose();
            }
            bw.Close();
        }

        public override string Name
        {
            get { return "位图字体生成器"; }
        }

        public override string[] SourceExt
        {
            get { return new string[0]; }
        }

        public override string[] DestExt
        {
            get { return new string[] { ".fnt" }; }
        }

        public override string SourceDesc
        {
            get { return "System font"; }
        }

        public override string DestDesc
        {
            get { return "Bitmap font"; }
        }
    }
}
