using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Ide;
using System.Drawing;

namespace Plugin.Common
{
    class Plugin : IPlugin
    {


        #region IPlugin 成员

        public Icon PluginIcon
        {
            get { return null; }
        }

        public bool IsListed
        {
            get { return true; }
        }

        #endregion

        #region IPlugin 成员

        public string Name
        {
            get { return "通用插件"; }
        }

        public void Load()
        {
            
        }

        public void Unload()
        {
           
        }

        #endregion
    }
}
