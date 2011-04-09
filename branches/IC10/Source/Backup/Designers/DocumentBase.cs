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
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Apoc3D.Ide.Tools;
using Apoc3D.Vfs;
using WeifenLuo.WinFormsUI.Docking;

namespace Apoc3D.Ide.Designers
{
    public delegate void PropertyUpdateHandler(object sender, object[] allObjects);
    public delegate void SaveStateChangedHandler(object sender);
    public delegate void ToolBoxItemsChangedHandler(ToolBoxItem[] items, ToolBoxCategory[] cates);

    public class DocumentBase : DockContent, IDocument
    {
        protected PropertyUpdateHandler propertyUpdated;
        protected SaveStateChangedHandler saveChanged;
        protected ToolBoxItemsChangedHandler tbitemsChanged;

        bool activated;

        [Browsable(false)]
        [ReadOnly(true)]
        public event ToolBoxItemsChangedHandler TBItemsChanged
        {
            add { tbitemsChanged += value; }
            remove { tbitemsChanged -= value; }
        }

        protected virtual void OnTBItemsChanged(ToolBoxItem[] items, ToolBoxCategory[] cates)
        {
            if (tbitemsChanged != null)
            {
                tbitemsChanged(items, cates);
            }
        }


        /// <summary>
        /// 更新属性窗格
        /// </summary>
        [Browsable(false)]
        [ReadOnly(true)]
        public event PropertyUpdateHandler PropertyUpdate
        {
            add { propertyUpdated += value; }
            remove { propertyUpdated -= value; }
        }

        protected virtual void OnPropertyUpdated(object[] objects)
        {
            if (propertyUpdated != null)
            {
                propertyUpdated(this, objects);
            }
        }

        [Browsable(false)]
        [ReadOnly(true)]
        public event SaveStateChangedHandler SavedStateChanged
        {
            add { saveChanged += value; }
            remove { saveChanged -= value; }
        }

        public virtual Icon GetIcon()
        {
            return Program.DefaultIcon;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [ReadOnly(true)]
        public virtual DesignerAbstractFactory Factory
        {
            get { throw new NotImplementedException(); }
        }
        public virtual bool LoadRes()
        {
            throw new NotImplementedException();
        }
        public virtual bool SaveRes()
        {
            throw new NotImplementedException();
        }
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [ReadOnly(true)]
        public virtual ToolStrip[] ToolStrips
        {
            get { throw new NotImplementedException(); }
        }
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [ReadOnly(true)]
        public virtual ResourceLocation ResourceLocation
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [ReadOnly(true)]
        public virtual bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [ReadOnly(true)]
        public virtual bool Saved
        {
            get { throw new NotImplementedException(); }
            protected set
            {
                throw new NotImplementedException();
            }
        }

        protected virtual void active() { }
        protected virtual void deactive() { }

        void IDocument.DocActivate()
        {
            if (!activated)
            {
                active();
                activated = true;
            }
        }

        void IDocument.DocDeactivate()
        {
            if (activated)
            {
                deactive();
                activated = false;
            }
        }
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [ReadOnly(true)]
        bool IDocument.IsActivated
        {
            get { return activated; }
        }
    }
}
