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
using Apoc3D.Ide.Tools;

namespace Apoc3D.Ide
{
    public class ToolManager
    {
        static ToolManager singleton;

        public static ToolManager Instance
        {
            get
            {
                if (singleton == null)
                    singleton = new ToolManager();
                return singleton;
            }
        }

        Dictionary<Type, IToolAbstractFactory> factories;
        Dictionary<string, IToolAbstractFactory> factories2;

        

        private ToolManager()
        {
            factories = new Dictionary<Type, IToolAbstractFactory>();
            factories2 = new Dictionary<string, IToolAbstractFactory>();
        }

        public ITool CreateTool(string typeName)
        {
            IToolAbstractFactory fac;
            if (factories2.TryGetValue(typeName, out fac))
            {
                return fac.CreateInstance();
            }
            else
                throw new NotSupportedException();
        }
        public ITool CreateTool(Type type)
        {
            IToolAbstractFactory fac;
            if (factories.TryGetValue(type, out fac))
            {
                return fac.CreateInstance();
            }
            else
                throw new NotSupportedException();
        }

        public void RegisterToolType(IToolAbstractFactory fac)
        {
            factories.Add(fac.CreationType, fac);
            factories2.Add(fac.CreationType.ToString(), fac);
        }
        public void UnregisterToolType(IToolAbstractFactory fac)
        {
            factories.Remove(fac.CreationType);
            factories2.Remove(fac.CreationType.ToString());

        }
    }
}
