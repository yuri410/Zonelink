using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Apoc3D.Ide;
using Apoc3D.Ide.Converters;
using Apoc3D.Vfs;
using Apoc3D.Core.MathLib;

namespace Plugin.GISTools
{
    class TDmp32To16Converter : ConverterBase
    {
        public override void ShowDialog(object sender, EventArgs e)
        {
            string[] files;
            string path;
            if (ConvDlg.Show("", GetOpenFilter(), out files, out path) == DialogResult.OK)
            {
                ProgressDlg pd = new ProgressDlg(DevStringTable.Instance["GUI:Converting"]);

                pd.MinVal = 0;
                pd.Value = 0;
                pd.MaxVal = files.Length;

                pd.Show();
                for (int i = 0; i < files.Length; i++)
                {
                    string dest = Path.Combine(path, Path.GetFileNameWithoutExtension(files[i]) + ".tdmp");

                    Convert(new DevFileLocation(files[i]), new DevFileLocation(dest));

                    pd.Value = i;
                    Application.DoEvents();
                }
                pd.Close();
                pd.Dispose();
            }
        }

        public override void Convert(ResourceLocation source, ResourceLocation dest)
        {
            ContentBinaryReader br = new ContentBinaryReader(source);

            BinaryDataReader data = br.ReadBinaryData();
            float xllcorner = data.GetDataSingle("xllcorner");
            float yllcorner = data.GetDataSingle("yllcorner");

            int width = data.GetDataInt32("width");
            int height = data.GetDataInt32("height");

            float[] demData = new float[height * width];


            int bits = data.GetDataInt32("bits", 32);

            ContentBinaryReader br2 = data.GetData("data");

            for (int i = 0; i < height; i++) 
            {
                for (int j = 0; j < width; j++) 
                {
                    demData[i * width + j] = br2.ReadSingle();
                }
            }

            br2.Close();

            data.Close();

            
            Half[] demData16 = Half.ConvertToHalf(demData);            

            // =========================================================


            BinaryDataWriter result = new BinaryDataWriter();

            result.AddEntry("xllcorner", xllcorner);
            result.AddEntry("yllcorner", yllcorner);
            result.AddEntry("width", width);
            result.AddEntry("height", height);

            result.AddEntry("bits", 16);

            Stream dataStream = result.AddEntryStream("data");

            ContentBinaryWriter bw = new ContentBinaryWriter(dataStream);
            for (int i = 0; i < demData.Length; i++)
            {
                bw.Write(demData16[i].InternalValue);
            }

            bw.Close();

            bw = new ContentBinaryWriter(dest);
            bw.Write(result);
            bw.Close();
        }

        public override string Name
        {
            get { return "TDmp 32 to 16 Converter"; }
        }

        public override string[] SourceExt
        {
            get { return new string[] { ".tdmp" }; }
        }

        public override string[] DestExt
        {
            get { return new string[] { ".tdmp" }; }
        }

        public override string SourceDesc
        {
            get { return "Terrain Displacement Map"; }
        }

        public override string DestDesc
        {
            get { return "Terrain Displacement Map"; }
        }
    }
}
