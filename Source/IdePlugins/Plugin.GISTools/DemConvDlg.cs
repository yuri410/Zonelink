using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Apoc3D.Ide;

namespace Plugin.GISTools
{
    partial class DemConvDlg : Form
    {
        class ProgressCBK : IItemProgressCallback
        {
            DemConvDlg parent;
            public ProgressCBK(DemConvDlg parent)
            {
                this.parent = parent;
            }

            #region IItemProgressCallback 成员

            public void Invoke(int current, int total)
            {
                if (current <= total)
                {
                    parent.progressBar2.Maximum = total;
                    parent.progressBar2.Value = current;
                    Application.DoEvents();
                }
            }

            #endregion
        }

        DemConverter converter;
        DemParameters conParams;

        public DemConvDlg(DemConverter converter)
        {
            InitializeComponent();

            this.converter = converter;
            this.conParams = new DemParameters();
            this.propertyGrid1.SelectedObject = conParams;
        }


        Point GetTileCoord(string fileName)
        {
            string[] v = fileName.Split('_');
            int len = v.Length;

            return new Point(int.Parse(v[len - 2]), int.Parse(v[len - 1]));
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = converter.GetOpenFilter();

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string[] files = openFileDialog1.FileNames;

                for (int i = 0; i < files.Length; i++)
                {
                    string fileName = Path.GetFileName(files[i]);

                    ListViewItem item = fileListView.Items.Add(fileName);

                    Point p = GetTileCoord(Path.GetFileNameWithoutExtension(fileName));

                    item.SubItems.Add(p.X.ToString() + ", " + p.Y.ToString());
                    item.Tag = files[i];
                }
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            propertyGrid1.Enabled = false;
            fileListView.Enabled = false;

            addButton.Enabled = false;
            removeButton.Enabled = false;
            okButton.Enabled = false;
            cancelButton.Enabled = false;

            converter.Parameters = conParams;
            converter.ProgressCBK = new ProgressCBK(this);

            progressBar1.Maximum = fileListView.Items.Count;

            for (int i = 0; i < fileListView.Items.Count; i++)
            {
                string fileName = (string)fileListView.Items[i].Tag;
                string destFileName = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + ".tdmp");

                DevFileLocation sfl = new DevFileLocation(fileName);
                DevFileLocation dfl = new DevFileLocation(destFileName);

                converter.Convert(sfl, dfl);
                progressBar1.Value = i + 1;

                Application.DoEvents();
            }

            propertyGrid1.Enabled = true;
            fileListView.Enabled = true;

            addButton.Enabled = true;
            removeButton.Enabled = true;
            okButton.Enabled = true;
            cancelButton.Enabled = true;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
