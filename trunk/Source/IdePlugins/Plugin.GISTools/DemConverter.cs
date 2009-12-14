using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows.Forms;
using Apoc3D.Ide;
using Apoc3D.Ide.Converters;
using Apoc3D.Vfs;
using Apoc3D.MathLib;

namespace Plugin.GISTools
{
    class DemParameters
    {
        const int DefaultRescaleSize = 3000;
        const float DefaultCellSize = 2;
        const float DefaultHeightScale = 5500;
        const int DefaultZeroLevel = 100;

        public DemParameters()
        {
            CellSize = DefaultCellSize;
            HeightScale = DefaultHeightScale;

            GenereateNoise = false;

            RescaleSize = DefaultRescaleSize;
            ZeroLevel = DefaultZeroLevel;
        }

        [DefaultValue(DefaultCellSize)]
        public float CellSize
        {
            get;
            set;
        }

        [DefaultValue(DefaultRescaleSize)]
        public int RescaleSize
        {
            get;
            set;
        }

        [DefaultValue(DefaultHeightScale)]
        public float HeightScale
        {
            get;
            set;
        }

        [DefaultValue(DefaultZeroLevel)]
        public int ZeroLevel
        {
            get;
            set;
        }

        [DefaultValue(false)]
        public bool GenereateNoise
        {
            get;
            set;
        }
    }

    interface IItemProgressCallback 
    {
        void Invoke(int current, int total);
    }

    class DemConverter : ConverterBase
    {
        const int NoValue = -9999;

        public IItemProgressCallback ProgressCBK
        {
            get;
            set;
        }
        public DemParameters Parameters
        {
            get;
            set;
        }

        public override void ShowDialog(object sender, EventArgs e)
        {
            DemConvDlg dlg = new DemConvDlg(this);
            dlg.ShowDialog();
        }


        int ParseInt(string s, int begin, int l, int nodVal)
        {
            int result = 0;

            int sign = s[begin] == '-' ? -1 : 1;

            for (int i = sign == -1 ? 1 : 0; i < l; i++)
            {
                char c = s[i + begin];
                if (c >= '0' && c <= '9')
                {
                    result *= 10;

                    result += c - '0';
                }
            }

            result *= sign;

            if (result == nodVal)
            {
                result = 0;
            }
            else
            {
                result += Parameters.ZeroLevel;
            }
            return result;
        }

        unsafe void Split(string str, int[] array, int nodVal)
        {
            int lastSepPos = 0;
            int len;
            int i;
            int j = 0;

            for (i = 0; i < str.Length; i++)
            {
                if (str[i] == ' ')
                {
                    len = i - lastSepPos;
                    if (len > 0)
                    {
                        array[j++] = ParseInt(str, lastSepPos, len, nodVal);
                    }

                    lastSepPos = i + 1;
                }
            }

            len = i - lastSepPos;
            if (len > 0)
            {
                array[j++] = ParseInt(str, lastSepPos, len, nodVal);
            }

            if (j != 6001 && j != 6000)
            {
                MessageBox.Show("Bad data");
                throw new Exception();
            }
        }

        unsafe void ConvertDem(Stream srcStm, Stream dstStm)
        {
            StreamReader sr = new StreamReader(srcStm, Encoding.Default);
            
            int width = -1;
            int height = -1;

            int nodVal = NoValue;

            float cellSize = 0.00083333333333333f;

            float xllcorner = 0;
            float yllcorner = 0;

            int[][] heightMap = null;

            int row = 0;

            char[] sep = new char[] { ' ' };

            string[] emptyV = new string[1] { string.Empty };

            List<char> charLine = new List<char>(24000);


            bool beginData = false;
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();

                string[] v;
                if (beginData)
                {
                    v = emptyV;
                }
                else
                {
                    v = line.Split(sep, StringSplitOptions.RemoveEmptyEntries);
                }

                switch (v[0])
                {
                    case "ncols":
                        width = int.Parse(v[1]);
                        break;
                    case "nrows":
                        height = int.Parse(v[1]);

                        heightMap = new int[height][];
                        for (int i = 0; i < height; i++)
                        {
                            heightMap[i] = new int[width];
                        }
                        break;
                    case "xllcorner":
                        xllcorner = float.Parse(v[1]);
                        break;
                    case "yllcorner":
                        yllcorner = float.Parse(v[1]);
                        break;
                    case "cellsize":
                        cellSize = float.Parse(v[1]);
                        break;
                    case "NODATA_value":
                        nodVal = int.Parse(v[1]);

                        if (ProgressCBK != null)
                        {
                            ProgressCBK.Invoke(0, width + 100);
                        }
                        beginData = true;
                        break;
                    default:

                        Split(line, heightMap[row], nodVal);
                        //for (int i = 0; i < width; i++)
                        //{
                        //    heightMap[row][i] = int.Parse(v[i]);
                        //    if (heightMap[row][i] == nodVal)
                        //    {
                        //        heightMap[row][i] = 0;
                        //    }
                        //    else
                        //    {
                        //        heightMap[row][i] += (int)Parameters.ZeroLevel;
                        //    }

                        //}
                        if (ProgressCBK != null)
                        {
                            ProgressCBK.Invoke(row, width + 100);
                        }
                        row++;

                        break;
                }
            }

            sr.Close();

            if (ProgressCBK != null)
            {
                ProgressCBK.Invoke(width, width + 100);
            }

            float invHeightScale = 1.0f / Parameters.HeightScale;

            float[] demData = new float[Parameters.RescaleSize * Parameters.RescaleSize];

            float wzoom = width / (float)Parameters.RescaleSize;
            float hzoom = height / (float)Parameters.RescaleSize;

            int[] buffer = new int[16];
            float[] afu = new float[4];
            float[] afv = new float[4];

            for (int i = 0; i < Parameters.RescaleSize; i++)
            {
                float srcy = (i + 0.5f) * hzoom - 0.5f;
                int y0 = (int)srcy; if (y0 > srcy) --y0;
  
                for (int j = 0; j < Parameters.RescaleSize; j++)
                {
                    float srcx = (j + 0.5f) * wzoom - 0.5f;

                    int x0 = (int)srcx; if (x0 > srcx) --x0;

                    float fv = srcx - x0;
                    float fu = srcy - y0;

                    for (int ii = 0; ii < 4; ii++)
                    {
                        for (int jj = 0; jj < 4; jj++)
                        {
                            int x = x0 + jj - 1;
                            int y = y0 + ii - 1;

                            if (x < 0) x = 0;
                            if (y < 0) y = 0;
                            if (x >= width) x = width - 1;
                            if (y >= height) y = height - 1;

                            buffer[ii * 4 + jj] = heightMap[y][x];
                        }
                    }

                    afu[0] = MathEx.Sinc(1 + fu);
                    afu[1] = MathEx.Sinc(fu);
                    afu[2] = MathEx.Sinc(1 - fu);
                    afu[3] = MathEx.Sinc(2 - fu);
                    afv[0] = MathEx.Sinc(1 + fv);
                    afv[1] = MathEx.Sinc(fv);
                    afv[2] = MathEx.Sinc(1 - fv);
                    afv[3] = MathEx.Sinc(2 - fv);

                    float s = 0;
                    for (int ii = 0; ii < 4; ii++)
                    {
                        float a = 0;
                        for (int jj = 0; jj < 4; jj++)
                        {
                            a += afu[jj] * buffer[ii * 4 + jj];
                        }
                        s += a * afv[ii];
                    }

                    demData[i * Parameters.RescaleSize + j] = s * invHeightScale;
                }
            }

            if (ProgressCBK != null)
            {
                ProgressCBK.Invoke(width + 100, width + 100);
            }

            
            //float[] demData = new float[Parameters.RescaleSize * Parameters.RescaleSize];

            //int halfWidth = width / 2;
            //int halfHeight = height / 2;

            //for (int i = 0; i < Parameters.RescaleSize; i++)
            //{
            //    for (int j = 0; j < Parameters.RescaleSize; j++)
            //    {
            //        int tempHeight = i - halfHeight;
            //        int tempWidth = j - halfWidth;
            //        float tempX = tempWidth / wzoom;
            //        float tempY = tempHeight / hzoom;

            //        int xz = 0;
            //        int yz = 0;

            //        if (tempWidth > 0)
            //        {
            //            xz = (int)(tempWidth / x + 0.5f);
            //        }
            //        else
            //        {
            //            xz = (int)(tempWidth / x - 0.5f);
            //        }
            //        if (tempHeight > 0)
            //        {
            //            yz = (int)(tempHeight / y + 0.5f);
            //        }
            //        else
            //        {
            //            yz = (int)(tempHeight / y - 0.5f);
            //        }
            //        float p = tempX - xz;
            //        float q = tempY - yz;

            //        tempWidth = xz + halfWidth;
            //        tempHeight = yz + halfHeight;

            //        if (tempWidth < 0)
            //        {
            //            if (tempHeight < 0)
            //            {
            //                demData[i * Parameters.RescaleSize + j] = heightMap[0][0];
            //            }
            //            else if (tempHeight + 1 >= height)
            //            {
            //                demData[i * Parameters.RescaleSize + j] = heightMap[height - 1][0];
            //            }
            //            else
            //            {
            //                int a = heightMap[tempHeight][0];
            //                int b = heightMap[tempHeight][1];
            //                int c = heightMap[tempHeight + 1][0];
            //                int d = heightMap[tempHeight + 1][1];

            //                demData[i * Parameters.RescaleSize + j] =
            //                    (1 - q) * ((1 - p) * a + p * b) +
            //                    q * ((1 - p) * c + p * d);
            //            }
            //        }
            //        else if (tempWidth + 1 >= width)
            //        {
            //            if (tempHeight < 0)
            //            {
            //                demData[i * Parameters.RescaleSize + j] = heightMap[0][width - 1];
            //            }
            //            else if (tempHeight + 1 >= height)
            //            {
            //                demData[i * Parameters.RescaleSize + j] = heightMap[height - 1][width - 1];
            //            }
            //            else
            //            {
            //                int a = heightMap[tempHeight][width - 1];
            //                int b = heightMap[tempHeight + 1][width - 1];

            //                demData[i * Parameters.RescaleSize + j] = (1 - q) * a + q * b;
            //            }
            //        }
            //        else
            //        {
            //            demData[i * Parameters.RescaleSize + j] =
            //                (1 - q) * ((1 - p) * heightMap[tempHeight][tempWidth] + p * heightMap[tempHeight][tempWidth + 1]) +
            //                q * ((1 - p) * heightMap[tempHeight + 1][tempWidth] + p * heightMap[tempHeight + 1][tempWidth + 1]);
            //        }
            //    }
            //}

            //Bitmap bmp = new Bitmap(Parameters.RescaleSize, Parameters.RescaleSize);

            //BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            //int* dst = (int*)data.Scan0;

            //for (int i = 0; i < demData.Length; i++)
            //{
            //    int g = (int)(255 * demData[i]);

            //    if (g < 0) g = 0;
            //    if (g > 255) g = 255;

            //    dst[i] = Color.FromArgb(g, 255 - g, 0).ToArgb();
            //}

            //bmp.UnlockBits(data);
            //bmp.Save(dstStm, ImageFormat.Png);

            //bmp.Dispose();

            //dstStm.Close();


            BinaryDataWriter result = new BinaryDataWriter();

            result.AddEntry("xllcorner", xllcorner);
            result.AddEntry("yllcorner", yllcorner);
            result.AddEntry("width", Parameters.RescaleSize);
            result.AddEntry("height", Parameters.RescaleSize);

            Stream dataStream = result.AddEntryStream("data");

            ContentBinaryWriter bw = new ContentBinaryWriter(dataStream);
            for (int i = 0; i < demData.Length; i++)
            {
                bw.Write(demData[i]);
            }

            bw.Close();

            bw = new ContentBinaryWriter(dstStm);
            bw.Write(result);
            bw.Close();
        }

        public override void Convert(ResourceLocation source, ResourceLocation dest)
        {
            ConvertDem(source.GetStream, dest.GetStream);
        }

        public override string Name
        {
            get { return DevStringTable.Instance["GUI:DemConverter"]; }
        }

        public override string[] SourceExt
        {
            get { return new string[] { ".asc" }; }
        }

        public override string[] DestExt
        {
            get { return new string[] { ".tdmp" }; }
        }

        public override string SourceDesc
        {
            get { return "Elevation Data"; }
        }

        public override string DestDesc
        {
            get { return "Displacement Map"; }
        }
    }
}
