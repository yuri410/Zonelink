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
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Apoc3D.Ide
{
    public delegate void PluginErrorCallback(IPlugin plugin, Exception e);

    public delegate void PluginProgressCallBack(IPlugin plugin, int index, int count);

    public class PluginManager
    {
        static PluginManager singleton;

        public static bool IsInitialized
        {
            get { return singleton != null; }
        }
        public static PluginManager Instance
        {
            get { return singleton; }
        }

        Dictionary<string, IPlugin> plugins;

        public static void Initiailze(PluginErrorCallback errcbk, PluginProgressCallBack prgcbk)
        {
            singleton = new PluginManager();

            string pluginPath = Path.Combine(Application.StartupPath, Paths.PluginPath);
            if (Directory.Exists(pluginPath))
            {
                string[] files = Directory.GetFiles(pluginPath, "*.dll", SearchOption.TopDirectoryOnly);

                Type iplugin = typeof(IPlugin);

                for (int i = 0; i < files.Length; i++)
                {
                    IPlugin obj = null;

                    try
                    {
                        Assembly assembly = Assembly.LoadFile(files[i]);
                        
                        Type[] types = assembly.GetTypes();

                        bool found = false;
                        for (int j = 0; j < types.Length && !found; j++)
                        {
                            Type[] interfaces = types[j].GetInterfaces();

                            for (int k = 0; k < interfaces.Length && !found; k++)
                            {
                                if (interfaces[k] == iplugin)
                                {
                                    obj = (IPlugin)Activator.CreateInstance(types[j]);

                                    try
                                    {
                                        obj.Load();
                                    }
                                    catch (Exception e)
                                    {
                                        if (errcbk != null)
                                        {
                                            errcbk(obj, e);
                                        }
                                    }

                                    singleton.plugins.Add(obj.Name, obj);

                                    found = true;
                                }
                            }
                        }
                    }
                    //catch (FileLoadException)
                    //{

                    //}
                    catch (BadImageFormatException)
                    {

                    }
                    catch (Exception e)
                    {
                        Console.Write(e.Message);
                    }
                    if (prgcbk != null)
                    {
                        prgcbk(obj, i, files.Length);
                    }
                }
            }
        }

        private PluginManager()
        {
            plugins = new Dictionary<string, IPlugin>();
        }

        public int PluginCount
        {
            get { return plugins.Count; }
        }

        public IPlugin GetPlugin(string name) 
        {
            IPlugin result;
            plugins.TryGetValue(name, out result);
            return result;
        }

       
    }
}
