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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Apoc3D.Vfs;
using WeifenLuo.WinFormsUI.Docking;

namespace Apoc3D.Ide.Designers
{
    
    public interface IDocument
    {
        //int GetHashCode();
        //Icon GetIcon();
        //string ToString();
        void DocActivate();
        void DocDeactivate();
        bool IsActivated { get; }
    }
    public abstract class DesignerAbstractFactory
    {
        public abstract DocumentBase CreateInstance(ResourceLocation res);

        public abstract Type CreationType { get; }

        public abstract string Description { get; }

        public virtual string Filter
        {
            get { return DevUtils.GetFilter(Description, Filters); }
        }

        public abstract string[] Filters { get; }
    }
}
