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
using System.IO;
using System.Text;
using Apoc3D.Vfs;

namespace Apoc3D.Ide.Designers
{
    public class GeneralDocumentBase : DocumentBase
    {
        DesignerAbstractFactory factory;

        ResourceLocation resLoc;

        bool saved;

        protected virtual string NewFileName
        {
            get { return "新文件"; }
        }


        protected void Init(DesignerAbstractFactory fac, ResourceLocation rl)
        {
            factory = fac;
            resLoc = rl;

            if (resLoc != null)
            {
                this.Text = resLoc.ToString();

                if (resLoc is FileLocation)
                    this.Text = Path.GetFileName(this.Text);

                if (resLoc.IsReadOnly)
                    this.Text += "只读";
            }
            else
            {
                this.Text = NewFileName;
            }

            this.TabText = this.Text;

        }

        public override bool IsReadOnly
        {
            get { return resLoc == null ? false : resLoc.IsReadOnly; }
        }

        public override int GetHashCode()
        {
            if (resLoc != null && resLoc.Name != null)
            {
                return resLoc.GetHashCode();
            }
            return 0;
        }
        public override string ToString()
        {
            if (resLoc != null)
                return resLoc.ToString();
            return Text;
        }
        public override ResourceLocation ResourceLocation
        {
            get { return resLoc; }
            set { resLoc = value; }
        }
        public override bool Saved
        {
            get { return saved; }
            protected set
            {
                if (value != saved)
                {
                    saved = value;
                    if (saveChanged != null)
                    {
                        saveChanged(this);
                    }
                }
            }
        }
        public override DesignerAbstractFactory Factory
        {
            get { return factory; }
        }
    }
}
