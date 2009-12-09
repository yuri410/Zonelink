using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Apoc3D.Ide;

namespace Plugin.ArchiveTools
{
    public class Plugin : Apoc3D.Ide.IPlugin
    {
        LpkArchivePacker conv = new LpkArchivePacker();
        LPKViewerFactory lpkviewer = new LPKViewerFactory();

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
            get { return "Archive Tools"; }
        }

        public void Load()
        {
            ConverterManager.Instance.Register(conv);
            DesignerManager.Instance.RegisterDesigner(lpkviewer);
        }

        public void Unload()
        {
            ConverterManager.Instance.Unregister(conv);
            DesignerManager.Instance.UnregisterDesigner(lpkviewer);
        }

        #endregion
    }
}
