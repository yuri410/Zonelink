using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Apoc3D.Ide;

namespace Plugin.CsfTools
{
    public class Plugin : IPlugin
    {
        #region IPlugin 成员

        CsfDesignerFactory csfDesigner;
        CsfTemplate csfTemplate;

        public void Load()
        {
            if (csfDesigner == null)
            {
                csfDesigner = new CsfDesignerFactory();
            }
            if (csfTemplate == null)
            {
                csfTemplate = new CsfTemplate();
            }
            DesignerManager.Instance.RegisterDesigner(csfDesigner);
            TemplateManager.Instance.RegisterTemplate(csfTemplate);
        }

        public void Unload()
        {
            DesignerManager.Instance.UnregisterDesigner(csfDesigner);
            TemplateManager.Instance.UnregisterTemplate(csfTemplate);
        }

        public string Name
        {
            get { return "Csf Plugin"; }
        }

        public Icon PluginIcon
        {
            get { return null; }
        }

        public bool IsListed
        {
            get { return true; }
        }

        #endregion
    }
}
