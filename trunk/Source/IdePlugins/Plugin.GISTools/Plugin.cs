using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Apoc3D.Ide;

namespace Plugin.GISTools
{
    public class Plugin : IPlugin
    {
        #region IPlugin 成员

        DemConverter demConverter;
        TDmp32To8Converter tdmp32to8;
        TDmp32To16Converter tdmp32to16;

        public void Load()
        {
            if (demConverter == null)
            {
                demConverter = new DemConverter();
            }
            if (tdmp32to8 == null)
            {
                tdmp32to8 = new TDmp32To8Converter();
            }
            if (tdmp32to16 == null)
            {
                tdmp32to16 = new TDmp32To16Converter();
            }

            ConverterManager.Instance.Register(demConverter);
            ConverterManager.Instance.Register(tdmp32to8);
            ConverterManager.Instance.Register(tdmp32to16);
        }

        public void Unload()
        {
            ConverterManager.Instance.Unregister(demConverter);
            ConverterManager.Instance.Unregister(tdmp32to8);
            ConverterManager.Instance.Unregister(tdmp32to16);
        }

        public string Name
        {
            get { return "GIS Converter"; }
        }

        public Icon PluginIcon
        {
            get { return Properties.Resources.PluginIco; }
        }

        public bool IsListed
        {
            get { return true; }
        }

        #endregion
    }
}
