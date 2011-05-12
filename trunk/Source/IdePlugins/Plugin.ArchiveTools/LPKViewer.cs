using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Apoc3D;
using Apoc3D.Ide;
using Apoc3D.Ide.Designers;
using Apoc3D.Vfs;

namespace Plugin.ArchiveTools
{
    public partial class LPKViewer : GeneralDocumentBase
    {
        public static readonly string Extension = ".lpk";

        LpkArchive arc;
        LpkArchiveEntry[] entries;

        public LPKViewer(DesignerAbstractFactory fac, ResourceLocation res)
        {
            InitializeComponent();

            LanguageParser.ParseLanguage(DevStringTable.Instance, this);
            LanguageParser.ParseLanguage(DevStringTable.Instance, listView1);
            Init(fac, res);

            Saved = true;

        }

        public override ToolStrip[] ToolStrips
        {
            get
            {
                return new ToolStrip[0];
            }
        }

        public override bool LoadRes()
        {
            FileLocation fl = ResourceLocation as FileLocation;

            if (fl != null)
            {
                arc = new LpkArchive(fl);

                entries = arc.GetEntries();

                ListView.ListViewItemCollection col = listView1.Items;
                for (int i = 0; i < entries.Length; i++)
                {
                    ListViewItem item = col.Add(entries[i].Name);
                    item.SubItems.Add(entries[i].Size.ToString());
                    item.SubItems.Add(entries[i].CompressedSize.ToString());
                    item.SubItems.Add(entries[i].Offset.ToString());
                    item.SubItems.Add(entries[i].Flag.ToString());
                }
            }

            return true;
        }
        public override bool SaveRes()
        {
            return true;
        }

        public override bool IsReadOnly
        {
            get
            {
                return true;
            }
        }
    }
}
