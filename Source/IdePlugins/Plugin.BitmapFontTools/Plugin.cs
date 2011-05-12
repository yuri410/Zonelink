using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Apoc3D.Ide;

namespace Plugin.BitmapFontTools
{
    public class Plugin : IPlugin
    {
        #region IPlugin 成员

        FontConverter fntConverter;

        public void Load()
        {
            if (fntConverter == null)
            {
                fntConverter = new FontConverter();
            }

            ConverterManager.Instance.Register(fntConverter);
        }

        public void Unload()
        {
            ConverterManager.Instance.Unregister(fntConverter);
        }

        public string Name
        {
            get { return "位图字体工具"; }
        }

        public Icon PluginIcon
        {
            get { return Properties.Resources.font; }
        }

        public bool IsListed
        {
            get { return true; }
        }

        #endregion
    }
}
