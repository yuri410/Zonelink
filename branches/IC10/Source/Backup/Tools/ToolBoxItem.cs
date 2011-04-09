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
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Apoc3D.Ide.Tools
{
    public delegate void ToolBoxItemActivatedHandler();

    public abstract class ToolBoxItem
    {
        public abstract Image Icon
        {
            get;
        }

        public abstract string Name
        {
            get;
        }

        public abstract ToolBoxCategory Category
        {
            get;
        }

        public abstract void NotifyMouseDown(MouseEventArgs e);
        public abstract void NotifyMouseUp(MouseEventArgs e);
        public abstract void NotifyMouseMove(MouseEventArgs e);
        public abstract void NotifyMouseClick(MouseEventArgs e);
        public abstract void NotifyMouseDoubleClick(MouseEventArgs e);
        public abstract void NotifyMouseWheel(MouseEventArgs e);


        public event ToolBoxItemActivatedHandler Activated;

        public virtual void NotifyActivated()
        {
            if (Activated != null)
            {
                Activated();
            }
        }
    }
}
