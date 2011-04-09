/*
-----------------------------------------------------------------------------
This source file is part of Apoc3D Engine

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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Apoc3D.Ide.Designers;
using Apoc3D.Ide.Templates;

namespace Apoc3D.Ide
{
    public partial class NewFileForm : Form
    {
        class Platform
        {
            public string name;
            public int fieldCode;

            public Platform(string n, int fc)
            {
                name = n;
                fieldCode = fc;
            }

            public override string ToString()
            {
                return name;
            }
        }

        static DialogResult dr;
        static DocumentBase resDoc;

        int CateComparision(FileTemplateBase a, FileTemplateBase b)
        {
            return a.CategoryPath.CompareTo(b.CategoryPath);
        }

        public NewFileForm()
        {
            InitializeComponent();

            LanguageParser.ParseLanguage(DevStringTable.Instance, this);

            KeyValuePair<int, string>[] platforms = PlatformManager.Instance.GetPlatforms();

            for (int i = 0; i < platforms.Length; i++)
            {
                comboBox1.Items.Add(new Platform(platforms[i].Value, platforms[i].Key));
            }
            comboBox1.SelectedIndex = platforms.Length - 1;


            FileTemplateBase[] temps = TemplateManager.Instance.GetFileTemplates();
            Array.Sort<FileTemplateBase>(temps, CateComparision);

            int pos;
            string lastTemp = null;
            List<FileTemplateBase> current = new List<FileTemplateBase>();
            for (int i = 0; i < temps.Length; i++)
            {
                string cp = temps[i].CategoryPath;

                if (lastTemp != null && cp != lastTemp)
                {
                    TreeNode node = new TreeNode();

                    pos = cp.IndexOf('\\');
                    node.Text = pos < 0 ? cp : cp.Substring(pos);
                    node.Tag = current;
                    treeView1.Nodes.Add(node);

                    current = new List<FileTemplateBase>();
                }
                else
                {
                    current.Add(temps[i]);
                }
            }

            TreeNode lastNode = new TreeNode();
            string lastCp = temps[temps.Length - 1].CategoryPath;
            pos = lastCp.IndexOf('\\');
            lastNode.Text = pos < 0 ? lastCp : lastCp.Substring(pos);
            lastNode.Tag = current;
            treeView1.Nodes.Add(lastNode);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = Path.Combine(Application.StartupPath, textBox2.Text);
            saveFileDialog1.Filter = ((FileTemplateBase)listView1.SelectedItems[0].Tag).Filter;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = saveFileDialog1.FileName;
            }

        }

        public static DialogResult ShowDlg(IWin32Window owner, bool fileNamed, out DocumentBase doc)
        {

            NewFileForm f = new NewFileForm();
            f.button3.Visible = fileNamed;
            f.textBox2.Visible = fileNamed;
            f.label3.Visible = fileNamed;

            f.ShowDialog(owner);
            f.Dispose();

            doc = resDoc;
            resDoc = null;

            return dr;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null)
            {
                listView1.Clear();

                Platform pl = (Platform)comboBox1.SelectedItem;
                int platform = pl.fieldCode;

                List<FileTemplateBase> tmps = (List<FileTemplateBase>)e.Node.Tag;
                for (int i = 0; i < tmps.Count; i++)
                {
                    if ((tmps[i].Platform & platform) != 0)
                    {
                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = tmps[i].Name;
                        lvi.Tag = tmps[i];
                        
                        listView1.Items.Add(lvi);
                    }
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            treeView1_AfterSelect(null, new TreeViewEventArgs(treeView1.SelectedNode));
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                button3.Enabled = true;
                FileTemplateBase tmp = (FileTemplateBase)listView1.SelectedItems[0].Tag;

                textBox2.Text = tmp.DefaultFileName;
                textBox1.Text = tmp.Description;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dr = DialogResult.Cancel;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dr = DialogResult.OK;
            resDoc = ((FileTemplateBase)listView1.SelectedItems[0].Tag).CreateInstance(textBox2.Visible ? textBox2.Text : null);
            if (resDoc == null)
            {
                dr = DialogResult.Cancel;
            }

            this.Close();
        }

    }
}
