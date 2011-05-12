using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        RenderSystem renderSys;


        public MainForm(RenderSystem rs)
        {
            InitializeComponent();
            renderSys = rs;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Program.Viewer.CurrentTexture = TextureManager.Instance.CreateInstance(new FileLocation(openFileDialog1.FileName));
            }
        }
    }
}
