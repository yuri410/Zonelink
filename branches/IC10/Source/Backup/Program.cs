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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Apoc3D;

namespace Apoc3D.Ide
{
    public class DevStringTable : Singleton
    {
        static Apoc3D.StringTable singleton;

        public static Apoc3D.StringTable Instance
        {
            get { return singleton; }
        }

        public static void Initialize()
        {
            singleton = (new StringTableCsfFormat()).Load(
                new DevFileLocation(
                    Path.Combine(Application.StartupPath, "VBIDE.csf")
                    ));
        }

        protected override void dispose()
        {
            singleton = null;
        }
    }

    public static class Program
    {
        static Icon defaultIcon;
        //static StringTable strTable;
        static MainForm form;

        public static MainForm MainForm
        {
            get { return form; }
        }
        public static Icon DefaultIcon
        {
            get { return defaultIcon; }
        }


        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            DevStringTable.Initialize();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            defaultIcon = new Icon(typeof(Form), "wfc.ico");
            form = new MainForm();
            Application.Run(form);
        }


    }
}
