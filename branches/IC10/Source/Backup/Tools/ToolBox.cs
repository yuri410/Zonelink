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
using WeifenLuo.WinFormsUI.Docking;

namespace Apoc3D.Ide.Tools
{
    public partial class ToolBox : DockContent, ITool
    {
        ToolBoxItem[] items;
        ToolBoxCategory[] cates;

        public ToolBox()
        {
            InitializeComponent();
            LanguageParser.ParseLanguage(DevStringTable.Instance, this);
        }

        #region ITool 成员

        public bool IsVisibleInMenu
        {
            get { return true; }
        }

        public DockContent Form
        {
            get { return this; }
        }

        #endregion

        public void SetToolItems(ToolBoxItem[] items, ToolBoxCategory[] cates)
        {
            this.treeView1.Nodes.Clear();
            this.imageList1.Images.Clear();

            this.items = items;
            this.cates = cates;

            if (items != null)
            {
                if (cates == null)
                {
                    cates = new ToolBoxCategory[0];
                }

                Dictionary<ToolBoxCategory, TreeNode> table = new Dictionary<ToolBoxCategory, TreeNode>(cates.Length);

                for (int i = 0; i < cates.Length; i++)
                {
                    this.imageList1.Images.Add(cates[i].Icon);

                    string name = cates[i].Name;
                    int imgIndex = imageList1.Images.Count - 1;

                    TreeNode treeNode = treeView1.Nodes.Add(name, name, imgIndex, imgIndex);

                    table.Add(cates[i], treeNode);
                }

                for (int i = 0; i < items.Length; i++)
                {
                    this.imageList1.Images.Add(items[i].Icon);

                    ToolBoxCategory category = items[i].Category;

                    string name = items[i].Name;
                    int imgIndex = imageList1.Images.Count - 1;

                    if (category != null)
                    {
                        TreeNode parent = table[category];

                        TreeNode node = parent.Nodes.Add(name, name, imgIndex, imgIndex);

                        node.Tag = items[i];
                    }
                    else
                    {
                        TreeNode node = treeView1.Nodes.Add(name, name, imgIndex, imgIndex);

                        node.Tag = items[i];
                    }
                }
            }
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                if (treeView1.SelectedNode.Tag != null)
                {
                    ToolBoxItem item = (ToolBoxItem)treeView1.SelectedNode.Tag;

                    item.NotifyActivated();
                }
            }
        }


    }
}
