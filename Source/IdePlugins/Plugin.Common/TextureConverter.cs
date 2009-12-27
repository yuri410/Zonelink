using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Ide;
using Apoc3D.Ide.Converters;
using Apoc3D.Vfs;

namespace Plugin.Common
{
    unsafe class TextureConverter : ConverterBase
    {
        public override void ShowDialog(object sender, EventArgs e)
        {
            string[] files;
            string path;
            if (ConvDlg.Show("转换器", GetOpenFilter(), out files, out path) == DialogResult.OK)
            {
                ProgressDlg pd = new ProgressDlg(DevStringTable.Instance["GUI:Converting"]);

                pd.MinVal = 0;
                pd.Value = 0;
                pd.MaxVal = files.Length;

                pd.Show();
                for (int i = 0; i < files.Length; i++)
                {
                    string dest = Path.Combine(path, Path.GetFileNameWithoutExtension(files[i]) + ".tex");

                    Convert(new DevFileLocation(files[i]), new DevFileLocation(dest));

                    pd.Value = i;
                    Application.DoEvents();
                }
                pd.Close();
                pd.Dispose();
            }
        }

        //Apoc3D.Media.ImagePixelFormat Convert(Format fmt)
        //{
        //    switch (fmt)
        //    {
        //        case Format.A16B16G16R16:
        //            return Apoc3D.Media.ImagePixelFormat.A16B16G16R16;
        //        case Format.A16B16G16R16F:
        //            return Apoc3D.Media.ImagePixelFormat.A16B16G16R16F;
        //        case Format.A1R5G5B5:
        //            return Apoc3D.Media.ImagePixelFormat.A1R5G5B5;
        //        case Format.A2B10G10R10:
        //            return Apoc3D.Media.ImagePixelFormat.A2B10G10R10;
        //        case Format.A2R10G10B10:
        //            return Apoc3D.Media.ImagePixelFormat.A2R10G10B10;
        //        case Format.A32B32G32R32F:
        //            return Apoc3D.Media.ImagePixelFormat.A32B32G32R32F;
        //        case Format.A4L4:
        //            return Apoc3D.Media.ImagePixelFormat.A4L4;
        //        case Format.A4R4G4B4:
        //            return Apoc3D.Media.ImagePixelFormat.A4R4G4B4;
        //        case Format.A8:
        //            return Apoc3D.Media.ImagePixelFormat.Alpha8;
        //        case Format.A8B8G8R8:
        //            return Apoc3D.Media.ImagePixelFormat.A8B8G8R8;
        //        case Format.A8L8:
        //            return Apoc3D.Media.ImagePixelFormat.A8L8;
        //        case Format.A8R8G8B8:
        //            return Apoc3D.Media.ImagePixelFormat.A8R8G8B8;
        //        case Format.D15S1:
        //        case Format.D16:
        //        case Format.D16Lockable:
        //        case Format.D24S8:
        //        case Format.D24SingleS8:
        //        case Format.D24X4S4:
        //        case Format.D24X8:
        //        case Format.D32:
        //        case Format.D32Lockable:
        //        case Format.D32SingleLockable:
        //            return Apoc3D.Media.ImagePixelFormat.Depth;
        //        case Format.Dxt1:
        //            return Apoc3D.Media.ImagePixelFormat.DXT1;
        //        case Format.Dxt2:
        //            return Apoc3D.Media.ImagePixelFormat.DXT2;
        //        case Format.Dxt3:
        //            return Apoc3D.Media.ImagePixelFormat.DXT3;
        //        case Format.Dxt4:
        //            return Apoc3D.Media.ImagePixelFormat.DXT4;
        //        case Format.Dxt5:
        //            return Apoc3D.Media.ImagePixelFormat.DXT5;
        //        case Format.G16R16:
        //            return Apoc3D.Media.ImagePixelFormat.G16R16;
        //        case Format.G16R16F:
        //            return Apoc3D.Media.ImagePixelFormat.G16R16F;
        //        case Format.G32R32F:
        //            return Apoc3D.Media.ImagePixelFormat.G32R32F;
        //        case Format.L16:
        //            return Apoc3D.Media.ImagePixelFormat.Luminance16;
        //        case Format.L8:
        //            return Apoc3D.Media.ImagePixelFormat.Luminance8;
        //        case Format.P8:
        //            return Apoc3D.Media.ImagePixelFormat.Palette8;
        //        case Format.A8P8:
        //            return Apoc3D.Media.ImagePixelFormat.Palette8Alpha8;
        //        case Format.R16F:
        //            return Apoc3D.Media.ImagePixelFormat.R16F;
        //        case Format.R32F:
        //            return Apoc3D.Media.ImagePixelFormat.R32F;
        //        case Format.R3G3B2:
        //            return Apoc3D.Media.ImagePixelFormat.R3G3B2;
        //        case Format.R5G6B5:
        //            return Apoc3D.Media.ImagePixelFormat.R5G6B5;
        //        case Format.R8G8B8:
        //            return Apoc3D.Media.ImagePixelFormat.R8G8B8;
        //        case Format.Unknown:
        //            return Apoc3D.Media.ImagePixelFormat.Unknown;
        //        case Format.X8B8G8R8:
        //            return Apoc3D.Media.ImagePixelFormat.X8B8G8R8;
        //        case Format.X8R8G8B8:
        //            return Apoc3D.Media.ImagePixelFormat.X8R8G8B8;
        //        default:
        //            return Apoc3D.Media.ImagePixelFormat.Unknown;
        //    }
        //}

        public override void Convert(ResourceLocation source, ResourceLocation dest)
        {
            //Direct3D d3d = new Direct3D();

            //PresentParameters pm = new PresentParameters();
            //pm.Windowed = true;
            //Device dev = new Device(d3d, 0, DeviceType.Reference, IntPtr.Zero, CreateFlags.SoftwareVertexProcessing, pm);

            //ImageInformation info;
            //PaletteEntry[] palEntry;

            //Texture tex = Texture.FromStream(dev, source.GetStream,
            //    0, D3DX.DefaultNonPowerOf2, D3DX.DefaultNonPowerOf2, D3DX.DefaultNonPowerOf2,
            //    Usage.None, Format.Unknown, Pool.Managed, Filter.None, Filter.None, 0, out info, out palEntry);

            //Apoc3D.Graphics.TextureData texData;
            //texData.Depth = 1;
            //texData.Width = info.Width;
            //texData.Height = info.Height;
            //texData.LevelCount = info.MipLevels;
            //texData.Type = Apoc3D.Graphics.TextureType.Texture2D;
            //texData.Format = Convert(info.Format);

            //byte[] buffer = new byte[Apoc3D.Media.PixelFormat.GetMemorySize(texData.Width, texData.Height, 1, texData.Format) * 2];
            //int size = 0;
            //for (int i = 0; i < texData.LevelCount; i++)
            //{
            //    DataRectangle rect = tex.LockRectangle(i, LockFlags.ReadOnly);

            //    DataStream ds = rect.Data;

            //    texData.LevelSize[i] = (int)ds.Length;

            //    ds.Read(buffer, 0, texData.LevelSize[i]);

            //    size += texData.LevelSize[i];

            //    tex.UnlockRectangle(i);
            //}
            //texData.ContentSize = size;
            //texData.Content = new byte[size];
            //Array.Copy(buffer, texData.Content, size);

            //texData.Save(dest.GetStream);
            //tex.Dispose();

            //dev.Dispose();
            //d3d.Dispose();
        }

        public override string Name
        {
            get { return "纹理转换器"; }
        }

        public override string[] SourceExt
        {
            get { return new string[] { ".png", ".bmp", ".tga", ".tif" }; }
        }

        public override string[] DestExt
        {
            get { return new string[] { ".tex" }; }
        }

        public override string SourceDesc
        {
            get { return "纹理贴图"; }
        }

        public override string DestDesc
        {
            get { return "纹理贴图"; }
        }
    }
}
