using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.Ide;
using Apoc3D.Ide.Converters;
using Apoc3D.Media;
using Apoc3D.Vfs;
using DevIl;

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

        public ImagePixelFormat ConvertFormat(int format, int elementType, int bpp)
        {
            switch (format)
            {
                case Il.IL_BGR:
                    switch (elementType) 
                    {
                        case Il.IL_BYTE:
                        case Il.IL_UNSIGNED_BYTE:
                            if (bpp ==4)
                                return ImagePixelFormat.X8R8G8B8;
                            return ImagePixelFormat.R8G8B8;
                    }
                    break;
                case Il.IL_BGRA:
                    switch (elementType)
                    {
                        case Il.IL_BYTE:
                        case Il.IL_UNSIGNED_BYTE:
                            return ImagePixelFormat.A8R8G8B8;
                    }
                    break;
                case Il.IL_COLOUR_INDEX:
                    switch (elementType)
                    {
                        case Il.IL_BYTE:
                        case Il.IL_UNSIGNED_BYTE:
                            return ImagePixelFormat.Palette8;
                    }
                    break;
                case Il.IL_LUMINANCE:
                    switch (elementType)
                    {
                        case Il.IL_BYTE:
                        case Il.IL_UNSIGNED_BYTE :
                            return ImagePixelFormat.Luminance8;
                        case Il.IL_SHORT:
                        case Il.IL_UNSIGNED_SHORT:
                            return ImagePixelFormat.Luminance16;
                    }

                    break;
                case Il.IL_LUMINANCE_ALPHA:
                    switch (elementType)
                    {
                        case Il.IL_BYTE:
                        case Il.IL_UNSIGNED_BYTE:
                            return ImagePixelFormat.A8L8;
                    }
                    break;
                case Il.IL_RGB:
                    switch (elementType)
                    {
                        case Il.IL_BYTE:
                        case Il.IL_UNSIGNED_BYTE:
                            if (bpp == 4)
                                return ImagePixelFormat.X8B8G8R8;
                            return ImagePixelFormat.B8G8R8;
                    }
                    break;
                case Il.IL_RGBA:
                    switch (elementType)
                    {
                        case Il.IL_BYTE:
                        case Il.IL_UNSIGNED_BYTE:
                            return ImagePixelFormat.A8B8G8R8;
                        case Il.IL_SHORT:
                        case Il.IL_UNSIGNED_SHORT:
                            return ImagePixelFormat.A16B16G16R16;
                        case Il.IL_FLOAT:
                            if (bpp == 2)
                                return ImagePixelFormat.A16B16G16R16F;
                            return ImagePixelFormat.A32B32G32R32F;
                    }
                    break;
                // hacks
                case Il.IL_DXT1:
                    return ImagePixelFormat.DXT1;
                case Il.IL_DXT2:
                    return ImagePixelFormat.DXT2;
                case Il.IL_DXT3:
                    return ImagePixelFormat.DXT3;
                case Il.IL_DXT4:
                    return ImagePixelFormat.DXT4;
                case Il.IL_DXT5:
                    return ImagePixelFormat.DXT5;
            }
            return ImagePixelFormat.Unknown;
        }

        public override void Convert(ResourceLocation source, ResourceLocation dest)
        {
            FileLocation rl = source as FileLocation;

            if (rl == null)
            {
                MessageBox.Show("Not supported", "");
                return;
            }

            int image = Il.ilGenImage();

            Il.ilBindImage(image);
            Il.ilSetInteger(Il.IL_KEEP_DXTC_DATA, Il.IL_TRUE);

            Il.ilLoadImage(rl.Path);

            int mipCount = Il.ilGetInteger(Il.IL_NUM_MIPMAPS) + 1;

            int ilFormat = Il.ilGetInteger(Il.IL_IMAGE_FORMAT);
            int dataType = Il.ilGetInteger(Il.IL_IMAGE_TYPE);
            int bytePP = Il.ilGetInteger(Il.IL_IMAGE_BYTES_PER_PIXEL);

            bool cubeFlags = (Il.ilGetInteger(Il.IL_IMAGE_CUBEFLAGS) > 0);

            int depth0 = Il.ilGetInteger(Il.IL_IMAGE_DEPTH);

            TextureData texData;
            texData.LevelCount = mipCount;
            texData.Levels = new TextureLevelData[mipCount];
            texData.ContentSize = 0;
            texData.Format = ConvertFormat(ilFormat, dataType, bytePP);

          
            if (cubeFlags)
                texData.Type = TextureType.CubeTexture;
            else 
                texData.Type = depth0 > 1 ? TextureType.Texture3D : TextureType.Texture2D;

            int dxtFormat = Il.ilGetInteger(Il.IL_DXTC_DATA_FORMAT);
            if (dxtFormat != Il.IL_DXT_NO_COMP)
            {
                texData.Format = ConvertFormat(dxtFormat, 0, 0);
            }
            //else
            //{
            //    if (texData.Format == ImagePixelFormat.A8B8G8R8)
            //    {
            //        Ilu.iluSwapColours();
            //        texData.Format = ImagePixelFormat.A8R8G8B8;
            //    }
            //}

            for (int i = 0; i < mipCount; i++)
            {
                Il.ilBindImage(image);
                Il.ilActiveMipmap(i);

                texData.Levels[i].Width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
                texData.Levels[i].Height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
                texData.Levels[i].Depth = Il.ilGetInteger(Il.IL_IMAGE_DEPTH);

                if (dxtFormat != Il.IL_DXT_NO_COMP)
                {
                    int dxtSize = Il.ilGetDXTCData(IntPtr.Zero, 0, dxtFormat);
                    texData.Levels[i].Content = new byte[dxtSize];
                    texData.Levels[i].LevelSize = dxtSize;

                    fixed (byte* dst = &texData.Levels[i].Content[0])
                    {
                        Il.ilGetDXTCData(new IntPtr(dst), dxtSize, dxtFormat);
                    }

                    texData.ContentSize += dxtSize;
                }
                else
                {
                    int numImagePasses = cubeFlags ? 6 : 1;
                    int imageSize = Il.ilGetInteger(Il.IL_IMAGE_SIZE_OF_DATA);

                    texData.Levels[i].LevelSize = imageSize;
                    byte[] buffer = new byte[numImagePasses * imageSize];
                    texData.Levels[i].Content = buffer;

                    for (int j = 0, offset = 0; j < numImagePasses; j++, offset += imageSize)
                    {
                        if (cubeFlags)
                        {
                            Il.ilBindImage(image);
                            Il.ilActiveImage(j);
                            Il.ilActiveMipmap(i);
                        }

                        if (texData.Format == ImagePixelFormat.A8B8G8R8)
                        {
                            fixed (byte* dst = &buffer[offset])
                            {
                                Il.ilCopyPixels(0, 0, 0, texData.Levels[i].Width, texData.Levels[i].Height,
                                    texData.Levels[i].Depth, Il.IL_BGRA, Il.IL_UNSIGNED_BYTE, new IntPtr(dst));
                            }
                        }
                        else
                        {
                            IntPtr ptr = Il.ilGetData();
                            fixed (byte* dst = &buffer[offset])
                            {
                                Memory.Copy(ptr.ToPointer(), dst, imageSize);
                            }

                        }
                    }

                    texData.ContentSize += imageSize;
                }

            }
            Il.ilDeleteImage(image);

            if (texData.Format == ImagePixelFormat.A8B8G8R8)
            {
                texData.Format = ImagePixelFormat.A8R8G8B8;
            }
            texData.Save(dest.GetStream);

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
