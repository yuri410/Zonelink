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
        TDmp32To12Converter tdmp32to12;

        TDmpLodGen tdmplod16;
        TextureColorCalculator texClrCalc;
        ImageRemerger imgRmg;

        TDmpMerger dmpMerger;
        TDmpResizer dmpResizer;
        TDmpBlur dmpBlur;

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
            if (tdmp32to12 == null)
            {
                tdmp32to12 = new TDmp32To12Converter();
            }
            if (tdmplod16 == null) 
            {
                tdmplod16 = new TDmpLodGen();
            }
            if (texClrCalc == null) 
            {
                texClrCalc = new TextureColorCalculator();
            }
            if (imgRmg == null)
            {
                imgRmg = new ImageRemerger();
            } 
            if (dmpMerger == null)
            {
                dmpMerger = new TDmpMerger();
            }
            if (dmpResizer == null)
            {
                dmpResizer = new TDmpResizer();
            }
            if (dmpBlur == null)
            {
                dmpBlur = new TDmpBlur();
            }
            ConverterManager.Instance.Register(tdmplod16);
            ConverterManager.Instance.Register(demConverter);
            ConverterManager.Instance.Register(tdmp32to8);
            ConverterManager.Instance.Register(tdmp32to16);
            ConverterManager.Instance.Register(texClrCalc);
            ConverterManager.Instance.Register(imgRmg);
            ConverterManager.Instance.Register(dmpMerger);
            ConverterManager.Instance.Register(dmpResizer);
            ConverterManager.Instance.Register(dmpBlur);
            ConverterManager.Instance.Register(tdmp32to12);
        }

        public void Unload()
        {
            ConverterManager.Instance.Unregister(demConverter);
            ConverterManager.Instance.Unregister(tdmp32to8);
            ConverterManager.Instance.Unregister(tdmp32to16);
            ConverterManager.Instance.Unregister(tdmplod16);
            ConverterManager.Instance.Unregister(texClrCalc);
            ConverterManager.Instance.Unregister(imgRmg);
            ConverterManager.Instance.Unregister(dmpMerger);
            ConverterManager.Instance.Unregister(dmpResizer);
            ConverterManager.Instance.Unregister(tdmp32to12);
            ConverterManager.Instance.Unregister(dmpBlur);
        }

        public string Name
        {
            get { return "GIS 转换器"; }
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
