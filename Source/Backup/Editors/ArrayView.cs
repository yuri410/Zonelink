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

namespace Apoc3D.Ide.Editors
{
    public partial class ArrayView<T> : Form
    {
        static int[] res;
        static DialogResult dr;
        public ArrayView()
        {
            InitializeComponent();

            LanguageParser.ParseLanguage(DevStringTable.Instance, this);
        }

        public static DialogResult ShowDialog(string text, T[] array, out int[] result)
        {
            ArrayView<T> f = new ArrayView<T>();

            f.label1.Text = text;
            if (array != null)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    f.listBox1.Items.Add(i.ToString() + ' ' + array[i].ToString());
                }
            }

            f.ShowDialog();

            result = res;
            res = null;
            return dr;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dr = DialogResult.OK;
            res = new int[listBox1.CheckedItems.Count];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = listBox1.CheckedIndices[i];
            }
            Array.Sort<int>(res);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dr = DialogResult.Cancel;
            this.Close();
        }
    }
}
