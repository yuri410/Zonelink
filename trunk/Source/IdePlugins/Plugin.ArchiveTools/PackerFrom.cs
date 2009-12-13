using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Apoc3D;
using Apoc3D.Ide;
using Apoc3D.Vfs;
using SevenZip;

namespace Plugin.ArchiveTools
{
    public unsafe partial class PackerFrom : Form
    {
        const UInt32 kAdditionalSize = (6 << 20);
        const UInt32 kCompressedAdditionalSize = (1 << 10);
        const UInt32 kMaxLzmaPropSize = 10;

        public PackerFrom()
        {
            InitializeComponent();

            LanguageParser.ParseLanguage(DevStringTable.Instance, this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string[] files = openFileDialog1.FileNames;

                for (int i = 0; i < files.Length; i++)
                {
                    ListViewItem item = listView1.Items.Add(files[i]);
                    item.SubItems.Add((new FileInfo(files[i]).Length.ToString()));
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = listView1.SelectedItems.Count - 1; i >= 0; i--)
            {
                listView1.SelectedItems[i].Remove();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = saveFileDialog1.FileName;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            
            DevFileLocation fl = new DevFileLocation(textBox1.Text);

            ContentBinaryWriter bw = new ContentBinaryWriter(fl);

            bw.Write(LpkArchive.FileId);

            int count = listView1.Items.Count;
            bw.Write(count);
            progressBar1.Value = 0; 
            progressBar1.Maximum = count;

            int oldPos = (int)bw.BaseStream.Position;

            ListView.ListViewItemCollection coll = listView1.Items;

           
            LpkArchiveEntry[] entries = new LpkArchiveEntry[count];


            for (int i = 0; i < count; i++)
            {   
                entries[i].Name = Path.GetFileName(coll[i].Text);

                bw.WriteStringUnicode(entries[i].Name);
                bw.Write(entries[i].CompressedSize);
                bw.Write(entries[i].Offset);
                bw.Write(entries[i].Size);
                bw.Write(entries[i].Flag);
            }

            CoderPropID[] propIDs = 
			{ 
				CoderPropID.DictionarySize,
                CoderPropID.Algorithm
			};
            object[] properties = 
			{
				1048576*8,
                2
			};
            SevenZip.Compression.LZMA.Encoder encoder = new SevenZip.Compression.LZMA.Encoder();

            encoder.SetCoderProperties(propIDs, properties);

            System.IO.MemoryStream propms = new System.IO.MemoryStream();
            encoder.WriteCoderProperties (propms);
            bw.Write((int)propms.Length);
            propms.Close();
            bw.Write(propms.ToArray());

            for (int i = 0; i < count; i++)
            {
                entries[i].Offset = (int)bw.BaseStream.Position;

                FileStream fs = new FileStream(coll[i].Text, FileMode.Open, FileAccess.Read);
                System.IO.MemoryStream ms = new System.IO.MemoryStream((int)fs.Length / 2);

                encoder.Code(fs, ms, -1, -1, null);

                entries[i].Size = (int)fs.Length;
                               
                fs.Close();
                ms.Close();

                byte[] buffer = ms.ToArray();
                bw.Write(buffer);
                entries[i].CompressedSize = buffer.Length;
                entries[i].Flag = 0;

                Application.DoEvents();
                progressBar1.Value = i + 1;
            }

            bw.Seek(oldPos, SeekOrigin.Begin);

            for (int i = 0; i < count; i++)
            {
                bw.WriteStringUnicode(entries[i].Name);
                bw.Write(entries[i].CompressedSize);
                bw.Write(entries[i].Offset);
                bw.Write(entries[i].Size);
                bw.Write(entries[i].Flag);
            }

            bw.Close();

            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
        }

    }
}
