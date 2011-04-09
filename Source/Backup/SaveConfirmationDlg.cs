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
using System.Text;
using System.Windows.Forms;
using Apoc3D;
using Apoc3D.Ide.Designers;
using Apoc3D.MathLib;

namespace Apoc3D.Ide
{
    public partial class SaveConfirmationDlg : Form
    {
        public SaveConfirmationDlg()
        {
            InitializeComponent();
            LanguageParser.ParseLanguage(DevStringTable.Instance, this);
            dr = DialogResult.Cancel;
        }


        static DialogResult dr;
        static DocumentBase[] savingDocs;

        static Dictionary<string, DocumentBase> docs;

        public static Pair<DialogResult, DocumentBase[]> Show(IWin32Window parent, DocumentBase[] allDocs)
        {
            docs = new Dictionary<string, DocumentBase>(allDocs.Length);
            SaveConfirmationDlg f = new SaveConfirmationDlg();
            for (int i = 0; i < allDocs.Length; i++)
            {
                docs.Add(allDocs[i].ToString(), allDocs[i]);
                f.listBox1.Items.Add(allDocs[i].ToString());
            }

            f.ShowDialog(parent);

            Pair<DialogResult, DocumentBase[]> res;
            res.first = dr;
            res.second = savingDocs;

            dr = DialogResult.Cancel;
            savingDocs = null;
            docs = null;
            return res;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<DocumentBase> res = new List<DocumentBase>(listBox1.Items.Count);
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                res.Add(docs[(string)listBox1.Items[i]]);
            }
            savingDocs = res.ToArray();
            dr = DialogResult.Yes;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dr = DialogResult.No;
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
