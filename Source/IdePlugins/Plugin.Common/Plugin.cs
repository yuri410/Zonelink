using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Apoc3D.Ide;
using DevIl;

namespace Plugin.Common
{
    class Plugin : IPlugin
    {
        TextureConverter tex2dConv;
        IndexMapMixer idxMapMixer;

        #region IPlugin 成员

        public Icon PluginIcon
        {
            get { return null; }
        }

        public bool IsListed
        {
            get { return true; }
        }

        public string Name
        {
            get { return "通用插件"; }
        }

        public void Load()
        {
            Il.ilInit();

            if (tex2dConv == null)
            {
                tex2dConv = new TextureConverter();
            }
            if (idxMapMixer == null) 
            {
                idxMapMixer = new IndexMapMixer();
            }
            ConverterManager.Instance.Register(tex2dConv);
            ConverterManager.Instance.Register(idxMapMixer);
        }

        public void Unload()
        {
            ConverterManager.Instance.Unregister(tex2dConv);
            ConverterManager.Instance.Unregister(idxMapMixer);
        }

        #endregion
    }
}
