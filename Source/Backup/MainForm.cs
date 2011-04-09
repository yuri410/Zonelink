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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using Apoc3D;
using Apoc3D.Config;
using Apoc3D.Graphics.Effects;
using Apoc3D.Ide.Converters;
using Apoc3D.Ide.Designers;
using Apoc3D.Ide.Projects;
using Apoc3D.Ide.Templates;
using Apoc3D.Ide.Tools;
using Apoc3D.MathLib;
using Apoc3D.Media;
using Apoc3D.Vfs;
using WeifenLuo.WinFormsUI.Docking;

namespace Apoc3D.Ide
{
    public partial class MainForm : Form
    {
        readonly static string ConfigFile = Path.Combine("Configs", "environment.xml");
        readonly static string DockingConfigFile = Path.Combine("Configs", "docking.xml");

        BasicConfigs configs;

        #region Helpers
        /// <summary>
        /// 检查
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        bool DocumentCanSave(DocumentBase doc)
        {
            return !doc.Saved && !doc.IsReadOnly;
        }
        bool DocumentCanSave(IDockContent doc)
        {
            DocumentBase d = (DocumentBase)doc;
            return !d.Saved && !d.IsReadOnly;
        }
        #endregion

        PropertyWindow propertyWnd;
        ExplorerWindow explorerWnd;
        ToolBox toolBox;

        ProjectBase currentProject;

        public MainForm()
        {
            InitializeComponent();


            SplashForm splash = new SplashForm();
            splash.Show(this);
            Application.DoEvents();


            LanguageParser.ParseLanguage(DevStringTable.Instance, this);
            LanguageParser.ParseLanguage(DevStringTable.Instance, menuStripMain);
            LanguageParser.ParseLanguage(DevStringTable.Instance, toolStripMain);


            if (File.Exists(ConfigFile))
            {
                configs = Serialization.XmlDeserialize<BasicConfigs>(ConfigFile);
            }
            else
            {
                configs = new BasicConfigs();
            }

            FileSystem.Instance.AddWorkingDir(Application.StartupPath);
            FileSystem.Instance.AddWorkingDir(@"..\..\..\Content");
            PlatformManager.Instance.RegisterPlatform(PresetedPlatform.VirtualBike, PresetedPlatform.VirtualBikeName);

            PluginManager.Initiailze(null, splash.PluginProgressCallBack);


            ToolManager.Instance.RegisterToolType(new PropertyWndFactory());
            ToolManager.Instance.RegisterToolType(new ExplorerWndFactory());
            ToolManager.Instance.RegisterToolType(new ToolBoxFactory());

            //ConverterManager.Instance.Register(new DispMapConverter());
            ConverterManager.Instance.Register(new NormalMapConverter());

            //if (configs.GamePath.Length > 0)
            //{
            //    FileSystem.Instance.AddWorkingDir(configs.GamePath);
            //}
            //if (configs.RA2YRPath.Length > 0)
            //{
            //    Ra2FileSystem.Instance.AddWorkingDir(configs.RA2YRPath);
            //}



            //EffectManager.Instance.LoadEffects();

            ConverterBase[] converters = ConverterManager.Instance.GetAllConverters();

            for (int i = 0; i < converters.Length; i++)
            {
                ToolStripMenuItem mi = new ToolStripMenuItem(converters[i].Name, converters[i].GetIcon.ToBitmap(), new EventHandler(converters[i].ShowDialog));
                converterMenuItem.DropDownItems.Add(mi);
            }

            splash.Close();

            if (File.Exists(DockingConfigFile))
            {
                dockPanel.LoadFromXml(DockingConfigFile, LoadToolPanels);
            }
        }

       

        ProjectBase CurrentProject
        {
            get { return currentProject; }
            set { currentProject = value; }
        }

        IDockContent LoadToolPanels(string name)
        {
            try
            {
                ITool tool = ToolManager.Instance.CreateTool(name);

                if (tool.GetType() == typeof(PropertyWindow))
                {
                    propertyWnd = (PropertyWindow)tool;
                }
                else if (tool.GetType() == typeof(ExplorerWindow))
                {
                    explorerWnd = (ExplorerWindow)tool;
                }
                else if (tool.GetType() == typeof(ToolBox))
                {
                    toolBox = (ToolBox)tool;
                }

                return tool.Form;
            }
            catch (NotSupportedException)
            {
                return null;
            }
        }

        public void AddDocumentTab(DocumentBase des)
        {
            //docs.Add(des);

            des.LoadRes();

            des.Show(dockPanel, DockState.Document);

            SetLayout(des);
        }

        void SetLayout(DocumentBase des)
        {
            int x = toolStripMain.Width + toolStripMain.Left;
            int y = toolStripMain.Top;
            ToolStrip[] list = des.ToolStrips;

            for (int i = 0; i < list.Length; i++)
            {
                if (x > toolStripContainer1.TopToolStripPanel.Width)
                {
                    x = 0;
                    y += toolStripMain.Height;
                }

                list[i].Location = new System.Drawing.Point(x, y);

                x += list[i].Width;
            }
        }

        void docSaveStateChanged(object sender)
        {
            bool state = DocumentCanSave((DocumentBase)sender);// !saved && !((IDocument)sender).IsReadOnly;
            saveMenuItem.Enabled = state;
            saveTool.Enabled = state;
        }
        void docPropertyUpdateNeeded(object sender, object[] allObjs)
        {
            if (propertyWnd != null)
            {
                propertyWnd.SetObjects(sender, allObjs);
            }
        }
        void docClosing(object sender, FormClosingEventArgs e)
        {
            CloseDocumentResult res = CloseDocuments(new IDockContent[] { (IDockContent)sender });
            if (res == CloseDocumentResult.Cancel)
            {
                e.Cancel = true;
            }
            else
            {
                SwichDocumentTab(null);
            }
        }

        void docToolBoxItemsChanged(ToolBoxItem[] items, ToolBoxCategory[] cates)
        {
            if (toolBox != null)
            {
                toolBox.SetToolItems(items, cates);
            }
        }

        void SwichDocumentTab(DocumentBase des)
        {
            for (int i = toolStripContainer1.TopToolStripPanel.Controls.Count - 1; i >= 0; i--)
            {
                string n = toolStripContainer1.TopToolStripPanel.Controls[i].Name;
                if (n != toolStripMain.Name && n != menuStripMain.Name)
                    toolStripContainer1.TopToolStripPanel.Controls.RemoveAt(i);
            }

            foreach (IDockContent con in dockPanel.Documents)
            {
                DocumentBase doc = (DocumentBase)con;
                if (((IDocument)doc).IsActivated)
                {
                    doc.SavedStateChanged -= docSaveStateChanged;
                    doc.PropertyUpdate -= docPropertyUpdateNeeded;
                    doc.FormClosing -= docClosing;
                    doc.TBItemsChanged -= docToolBoxItemsChanged;
                    docPropertyUpdateNeeded(null, null);

                    ((IDocument)doc).DocDeactivate();
                }
            }

            if (des != null)
            {
                ToolStrip[] list = des.ToolStrips;
                for (int i = 0; i < list.Length; i++)
                    toolStripContainer1.TopToolStripPanel.Controls.Add(list[i]);

                des.SavedStateChanged += docSaveStateChanged;
                des.PropertyUpdate += docPropertyUpdateNeeded;
                des.FormClosing += docClosing;
                des.TBItemsChanged += docToolBoxItemsChanged;

                docPropertyUpdateNeeded(null, null);
                docSaveStateChanged(des);

                ((IDocument)des).DocActivate();
            }
        }
        private void dockPanel_ActiveDocumentChanged(object sender, EventArgs e)
        {
            SwichDocumentTab((DocumentBase)dockPanel.ActiveDocument);
        }


        /// <summary>
        /// 保存需要保存的文档
        /// </summary>
        /// <param name="doc"></param>
        void SaveDocument(DocumentBase doc)
        {
            if (!doc.Saved)
            {
                if (doc.ResourceLocation == null || doc.IsReadOnly)
                {
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        saveFileDialog1.Filter = doc.Factory.Filter;
                        doc.ResourceLocation = new DevFileLocation(saveFileDialog1.FileName);
                        doc.SaveRes();
                        statusLabel.Text = DevStringTable.Instance["STATUS:SAVEDDOC"];
                    }
                }
                else
                {
                    doc.SaveRes();
                    statusLabel.Text = DevStringTable.Instance["STATUS:SAVEDDOC"];
                }

            }
        }


        public IDockContent CurrentDocument
        {
            get { return dockPanel.ActiveDocument; }
        }


        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            IDockContent[] docs = dockPanel.DocumentsToArray();
            CloseDocumentResult res = CloseDocuments(docs);

            if (res == CloseDocumentResult.Saved)
            {
                for (int i = 0; i < docs.Length; i++)
                    docs[i].DockHandler.Close();

                DocumentConfigBase.SaveAll();

                dockPanel.SaveAsXml(DockingConfigFile);

                Serialization.XmlSerialize<BasicConfigs>(configs, ConfigFile);
            }
            else if (res == CloseDocumentResult.Cancel)
            {
                e.Cancel = true;
            }
            //Engine.Release();
        }

        public CloseDocumentResult CloseDocuments(IDockContent[] docFrms)
        {
            //IDocument[] docs = new IDocument[docs.Length];
            List<DocumentBase> docs = new List<DocumentBase>(docFrms.Length);

            for (int i = 0; i < docFrms.Length; i++)
            {
                DocumentBase doc = (DocumentBase)docFrms[i];
                if (!doc.IsReadOnly && !doc.Saved)
                {
                    docs.Add(doc);
                }
            }

            if (docs.Count > 0)
            {
                Pair<DialogResult, DocumentBase[]> res = SaveConfirmationDlg.Show(this, docs.ToArray());
                if (res.first == DialogResult.Yes)
                {
                    for (int i = 0; i < res.second.Length; i++)
                    {
                        res.second[i].SaveRes();
                    }
                    return CloseDocumentResult.Saved;
                }
                else if (res.first == DialogResult.No)
                {
                    return CloseDocumentResult.NotSaved;
                }

                return CloseDocumentResult.Cancel;
            }
            return CloseDocumentResult.Saved;

        }



        private void saveMenuItem_Click(object sender, EventArgs e)
        {
            if (dockPanel.ActiveDocument != null)
            {
                DocumentBase doc = (DocumentBase)dockPanel.ActiveDocument;
                SaveDocument(doc);
            }
        }

        private void openFileMenuItem_Click(object sender, EventArgs e)
        {
            //StringBuilder sb = new StringBuilder();

            //Pair<string, string>[] fmts = DesignerManager.Instance.GetAllFormats();
            //for (int i = 0; i < fmts.Length; i++)
            //{
            //    sb.Append(fmts[i].a);
            //    sb.Append('(' + fmts[i].b + ')');
            //    sb.Append('|');
            //    sb.Append(fmts[i].b);
            //    sb.Append('|');
            //}
            //sb.Append(Program.StringTable["DOCS:ALLFILES"]);
            //sb.Append(" (*.*)|");
            //sb.Append("*.*");

            openFileDialog1.Filter = DesignerManager.Instance.GetFilter();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string[] files = openFileDialog1.FileNames;
                for (int i = 0; i < files.Length; i++)
                {
                    FileLocation rl;
                    if (openFileDialog1.ReadOnlyChecked)
                    {
                        rl = new FileLocation(files[i]);
                    }
                    else
                    {
                        rl = new DevFileLocation(files[i]);
                    }

                    AddDocumentTab(DesignerManager.Instance.CreateDocument(rl, Path.GetExtension(rl.Path)));
                }
            }
        }

        private void saveAsMenuItem_Click(object sender, EventArgs e)
        {
            if (dockPanel.ActiveDocument != null)
            {
                DocumentBase doc = (DocumentBase)dockPanel.ActiveDocument;

                if (doc.IsReadOnly)
                {
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        saveFileDialog1.Filter = doc.Factory.Filter;
                        doc.ResourceLocation = new DevFileLocation(saveFileDialog1.FileName);
                        doc.SaveRes();
                    }

                    statusLabel.Text = DevStringTable.Instance["STATUS:SAVEDDOC"];
                }
                else
                {
                    statusLabel.Text = DevStringTable.Instance["STATUS:READONLYDOC"];
                }
            }

        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveAllMenuItem_Click(object sender, EventArgs e)
        {
            IDockContent[] docs = dockPanel.DocumentsToArray();
            for (int i = 0; i < docs.Length; i++)
            {
                SaveDocument((DocumentBase)docs[i]);
            }
        }

        private void propertyWndMenuItem_Click(object sender, EventArgs e)
        {
            if (propertyWnd != null)
            {
                propertyWnd.Show(dockPanel, propertyWnd.DockState);
            }
            else
            {
                propertyWnd = (PropertyWindow)ShowTool<PropertyWindow>(DockState.DockRight);

            }
        }

        private void toolBoxMenuItem_Click(object sender, EventArgs e)
        {
            if (toolBox != null)
            {
                toolBox.Show(dockPanel, toolBox.DockState);
            }
            else
            {
                toolBox = (ToolBox)ShowTool<ToolBox>(DockState.DockLeft);

            }
        }

        private void managerWndMenuItem_Click(object sender, EventArgs e)
        {
            if (explorerWnd != null)
            {
                explorerWnd.Show(dockPanel, explorerWnd.DockState);
            }
            else
            {
                explorerWnd = (ExplorerWindow)ShowTool<ExplorerWindow>(DockState.DockRight);
            }
        }

        ITool ShowTool<T>(DockState ds) where T : ITool
        {
            ITool tool = ToolManager.Instance.CreateTool(typeof(T));
            tool.Form.Show(dockPanel, ds);
            return tool;
        }

        private void newFileMenuItem_Click(object sender, EventArgs e)
        {
            DocumentBase doc;
            if (NewFileForm.ShowDlg(this, false, out doc) == DialogResult.OK)
            {
                AddDocumentTab(doc);
            }
        }

    }

    public enum CloseDocumentResult
    {
        Saved,
        NotSaved,
        Cancel
    }
}
