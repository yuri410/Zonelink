using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Animation;
using Apoc3D.MathLib;
using Apoc3D.Media;
using Apoc3D.Vfs;

namespace ModelStudio
{
    public unsafe partial class MainForm : Form
    {
        class EditableModel : ModelBase<MeshData>, IDisposable
        {
            public EditableModel() { }

            protected override MeshData LoadMesh(BinaryDataReader data)
            {
                throw new NotSupportedException();
            }

            protected override BinaryDataWriter SaveMesh(MeshData mesh)
            {
                return mesh.Save();
            }

            protected override void unload()
            {

            }

        }


        RenderSystem renderSys;
        public MainForm(RenderSystem rs)
        {
            InitializeComponent();
            renderSys = rs;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Program.Viewer.CurrentModel = new Model(ModelManager.Instance.CreateInstance(renderSys, new FileLocation(openFileDialog1.FileName)));
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Model cm = Program.Viewer.CurrentModel;
            if (cm != null) 
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK) 
                {
                    ModelData data = cm.GetData();
                    ModelData.ToFile(data, saveFileDialog1.FileName);
                }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
 
            }

        }


    }
}
