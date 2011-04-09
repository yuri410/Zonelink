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
using Apoc3D.Ide.Designers;
using Apoc3D.Ide.Projects;

namespace Apoc3D.Ide.Templates
{
    /// <summary>
    /// 模板控制着文档和项目的创建。Factory method
    /// </summary>
    public abstract class TemplateBase
    {
        protected Icon icon;

        public Icon GetIcon
        {
            get
            {
                if (icon == null)
                {
                    return Program.DefaultIcon;
                }
                return icon;
            }
        }

        public virtual string CategoryPath
        {
            get { return DevStringTable.Instance["GUI:DEFCATE"]; }
        }

        public abstract string Description
        {
            get; 
        }

        public abstract int Platform
        {
            get;
        }

        public abstract string Name
        {
            get;
        }
    }

    /// <summary>
    /// 文件模板控制着文档的创建。性质类似抽象工厂。
    /// </summary>
    public abstract class FileTemplateBase : TemplateBase
    {
        public abstract DocumentBase CreateInstance(string fileName);

        public abstract string Filter
        {
            get;
        }

        public virtual string DefaultFileName
        {
            get { return DevStringTable.Instance["GUI:DefFileName"]; }
        }
    }
    public abstract class ProjectTemplateBase : TemplateBase     
    {
        public abstract ProjectBase CreateInstance();
    }

}
