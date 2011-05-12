using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Apoc3D.Ide;

namespace Plugin.ArchiveTools
{
    public class Plugin : Apoc3D.Ide.IPlugin
    {
        PakArchivePacker conv = new PakArchivePacker();
        PAKViewerFactory pakviewer = new PAKViewerFactory();
        LpkArchivePacker conv2 = new LpkArchivePacker();
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
            get { return "文件包工具"; }
        }

        public void Load()
        {
            ConverterManager.Instance.Register(conv);
            DesignerManager.Instance.RegisterDesigner(pakviewer);
            ConverterManager.Instance.Register(conv2);
            DesignerManager.Instance.RegisterDesigner(lpkviewer);
        }

        public void Unload()
        {
            ConverterManager.Instance.Unregister(conv);
            DesignerManager.Instance.UnregisterDesigner(pakviewer);
            ConverterManager.Instance.Unregister(conv2);
            DesignerManager.Instance.UnregisterDesigner(lpkviewer);
        }

        #endregion
    }
}
