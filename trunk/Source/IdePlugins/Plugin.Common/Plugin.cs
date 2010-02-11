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
        IndexMapMixer4 idxMapMixer4;

        

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
            Ilu.iluInit();

            if (tex2dConv == null)
            {
                tex2dConv = new TextureConverter();
            }
            if (idxMapMixer == null) 
            {
                idxMapMixer = new IndexMapMixer();
            }
            if (idxMapMixer4 == null) 
            {
                idxMapMixer4 = new IndexMapMixer4();
            }
            ConverterManager.Instance.Register(tex2dConv);
            ConverterManager.Instance.Register(idxMapMixer);
            ConverterManager.Instance.Register(idxMapMixer4);
        }

        public void Unload()
        {
            ConverterManager.Instance.Unregister(tex2dConv);
            ConverterManager.Instance.Unregister(idxMapMixer);
            ConverterManager.Instance.Unregister(idxMapMixer4);
        }

        #endregion
    }
}
