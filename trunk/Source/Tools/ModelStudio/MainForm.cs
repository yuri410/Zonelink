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


                ModelData data = Program.Viewer.CurrentModel.GetData();
                GameMesh[] meshes = data.Entities;
                for (int i = 0; i < meshes.Length; i++)
                {
                    comboBox3.Items.Add(meshes[i]);
                }
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

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            GameMesh mesh = (GameMesh)comboBox3.SelectedItem;
            Material[][] mats = mesh.Materials;

            for (int i = 0; i < mats.Length; i++)
            {
                comboBox1.Items.Add(mats[i][0]);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Material mtrl = (Material)comboBox1.SelectedItem;

            button1.BackColor = Color.FromArgb(
                (int)(mtrl.Ambient.Alpha * 255), 
                (int)(mtrl.Ambient.Red * 255), 
                (int)(mtrl.Ambient.Green * 255), 
                (int)(mtrl.Ambient.Blue * 255));

            button2.BackColor = Color.FromArgb(
                (int)(mtrl.Diffuse.Alpha * 255),
                (int)(mtrl.Diffuse.Red * 255),
                (int)(mtrl.Diffuse.Green * 255),
                (int)(mtrl.Diffuse.Blue * 255));

            button3.BackColor = Color.FromArgb(
                (int)(mtrl.Specular.Alpha * 255),
                (int)(mtrl.Specular.Red * 255),
                (int)(mtrl.Specular.Green * 255),
                (int)(mtrl.Specular.Blue * 255));

            button4.BackColor = Color.FromArgb(
                (int)(mtrl.Emissive.Alpha * 255),
                (int)(mtrl.Emissive.Red * 255),
                (int)(mtrl.Emissive.Green * 255),
                (int)(mtrl.Emissive.Blue * 255));

            numericUpDown2.Value = (decimal)mtrl.Power;

            textBox1.Text = mtrl.GetTextureFile(0);
            textBox2.Text = mtrl.GetTextureFile(1);

            checkBox1.Checked = mtrl.IsTransparent;
            checkBox4.Checked = mtrl.ZWriteEnabled;
            checkBox2.Checked = mtrl.IsVegetation;
            checkBox3.Checked = mtrl.ZEnabled;

            switch (mtrl.CullMode) 
            {
                case CullMode.Clockwise:
                    comboBox2.SelectedIndex = 1;
                    break;
                case CullMode.CounterClockwise:
                    comboBox2.SelectedIndex = 2;
                    break;
                default:
                    comboBox2.SelectedIndex = 0;
                    break;
            }
            numericUpDown1.Value = (decimal)mtrl.AlphaRef;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Material mtrl = (Material)comboBox1.SelectedItem;

            Color clr = button1.BackColor;
            mtrl.Ambient = new Color4F(clr.A, clr.R, clr.G, clr.B);

            clr = button2.BackColor;
            mtrl.Diffuse = new Color4F(clr.A, clr.R, clr.G, clr.B);

            clr = button3.BackColor;
            mtrl.Specular = new Color4F(clr.A, clr.R, clr.G, clr.B);

            clr = button4.BackColor;
            mtrl.Emissive = new Color4F(clr.A, clr.R, clr.G, clr.B);

            mtrl.Power = (float)numericUpDown2.Value;

            if (textBox1.Text != mtrl.GetTextureFile(0) ||
                textBox2.Text != mtrl.GetTextureFile(1))
            {
                mtrl.SetTextureFile(0, textBox1.Text);
                mtrl.SetTextureFile(1, textBox2.Text);
                mtrl.ReloadTextures();

            }
            mtrl.IsTransparent = checkBox1.Checked;
            mtrl.ZWriteEnabled = checkBox4.Checked;
            mtrl.IsVegetation = checkBox2.Checked;
            mtrl.ZEnabled = checkBox3.Checked;

            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    mtrl.CullMode = CullMode.None;
                    break;
                case 1:
                    mtrl.CullMode = CullMode.Clockwise;
                    break;
                case 2:
                    mtrl.CullMode = CullMode.CounterClockwise;
                    break;
                default:
                    mtrl.CullMode = CullMode.None;
                    break;
            }

            mtrl.AlphaRef = (float)numericUpDown1.Value;
        }


    }
}
