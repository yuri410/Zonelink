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

namespace Apoc3D.Ide.Converters
{
    public partial class NormalMapConvDlg : Form
    {
        static NormalMapConvDlg()
        {
            ConversionType = NormalMapConverter.ConversionType.SwapYZ;
        }

        public static NormalMapConverter.ConversionType ConversionType
        {
            get;
            private set;
        }

        public NormalMapConvDlg()
        {
            InitializeComponent();

            LanguageParser.ParseLanguage(DevStringTable.Instance, this);

            this.DialogResult = DialogResult.Cancel;
        }

        public static DialogResult ShowDialog(string caption)
        {
            NormalMapConvDlg frm = new NormalMapConvDlg();

            if (!string.IsNullOrEmpty(caption))
            {
                frm.Text = caption;
            }

            frm.ShowDialog();

            return frm.DialogResult;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                ConversionType = NormalMapConverter.ConversionType.SwapXY;
            }
            else if (radioButton2.Checked)            
            {
                ConversionType = NormalMapConverter.ConversionType.SwapXZ;
            }
            else if (radioButton3.Checked)
            {
                ConversionType = NormalMapConverter.ConversionType.SwapYZ;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
