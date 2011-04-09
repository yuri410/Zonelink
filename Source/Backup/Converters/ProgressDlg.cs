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
    public partial class ProgressDlg : Form
    {

        public ProgressDlg(string message)
        {
            InitializeComponent();
            Message = message;
        }

        public string Message
        {
            get { return label1.Text; }
            set
            {
                label1.Text = value;
                Application.DoEvents();
            }
        }

        public int Value
        {
            get { return progressBar1.Value; }
            set
            {
                progressBar1.Value = value;
                Application.DoEvents();
            }
        }
        public int MaxVal
        {
            get { return progressBar1.Maximum; }
            set { progressBar1.Maximum = value; }
        }
        public int MinVal
        {
            get { return progressBar1.Minimum; }
            set { progressBar1.Minimum = value; }
        }
    }
}
