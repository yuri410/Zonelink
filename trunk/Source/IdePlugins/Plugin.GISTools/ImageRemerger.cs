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

namespace Plugin.GISTools
{
    unsafe class ImageRemerger : ConverterBase
    {
        string srcDir;

        public override void ShowDialog(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "选择输入目录";

            if (dlg.ShowDialog()== DialogResult.OK)
            {
                SaveFileDialog fdlg = new SaveFileDialog();

                if (fdlg.ShowDialog() == DialogResult.OK) 
                {
                    srcDir = dlg.SelectedPath;
                    Convert(null, new DevFileLocation(fdlg.FileName));
                }
            }
        }

        public override void Convert(ResourceLocation source, ResourceLocation dest)
        {
            Bitmap bmp = new Bitmap(512 * 10, 512 * 5);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);


            int ofsX = 0;
            int ofsY = 0;
            for (int i = 4; i >= 0; i--)
            {
                string[] files = Directory.GetFiles(Path.Combine(srcDir, "000" + i.ToString() + "\\"), "*.*");

                for (int j = 0; j < 10; j++)
                {
                    Bitmap b2 = new Bitmap(files[j]);
                    BitmapData d2 = b2.LockBits(new Rectangle(0, 0, b2.Width, b2.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                    int* src = (int*)d2.Scan0;
                    int* dst = (int*)data.Scan0;

                    for (int y = 0; y < b2.Height; y++)
                    {
                        for (int x = 0; x < b2.Width; x++)
                        {
                            dst[(y + ofsY) * bmp.Width + x + ofsX] = src[y * b2.Width + x];
                        }
                    }


                    ofsX += b2.Width;
                    if (ofsX >= bmp.Width)
                    {
                        ofsX = 0;
                        ofsY += b2.Height;
                    }

                    b2.UnlockBits(d2);
                    b2.Dispose();
                }
            }

            bmp.UnlockBits(data);

            Stream stm = dest.GetStream;
            bmp.Save(stm, ImageFormat.Png);
            stm.Close();

            bmp.Dispose();
        }

        public override string Name
        {
            get { return "卫星图像拼接器"; }
        }

        public override string[] SourceExt
        {
            get { return new string[] { ".jpg" }; }
        }

        public override string[] DestExt
        {
            get { return new string[] { ".png" }; }
        }

        public override string SourceDesc
        {
            get { return "Image"; }
        }

        public override string DestDesc
        {
            get { return "Image"; }
        }
    }
}
