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
using Apoc3D.Graphics.Effects;
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

        class MeshWrapper 
        {
            int index;
            public Mesh Mesh
            {
                get;
                private set;
            }

            public MeshWrapper(Mesh mesh, int idx) { Mesh = mesh; index = idx; }

            public override string ToString()
            {
                return index.ToString() + ".Mesh " + Mesh.Name.ToString();
            }
        }
        class PartMaterialWrapper 
        {
            int partIndex;
            Material[] mtrls;
            Mesh mesh;

            public Material[] Materials
            {
                get { return mtrls; }
                set
                {
                    mtrls = value;
                    mesh.Materials[partIndex] = mtrls;
                }
            }
            public PartMaterialWrapper(Mesh mesh, int partId, Material[] mtrl)
            {
                this.partIndex = partId;
                this.mtrls = mtrl;
                this.mesh = mesh;
            }

            public override string ToString()
            {
                return "Part " + partIndex.ToString();
            }
        }

        RenderSystem renderSys;
        int selectedKeyFrame;

        public MainForm(RenderSystem rs)
        {
            InitializeComponent();
            renderSys = rs;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            for (RenderPriority i = RenderPriority.First; i <= RenderPriority.Last; i++)
            {
                comboBox4.Items.Add(i.ToString());
            }
            for (MaterialFlags f = MaterialFlags.None; f <= MaterialFlags.BlendBright_Color; f++)
            {
                comboBox5.Items.Add(f.ToString());
            }

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                OpenModel();
            }
        }

        void OpenModel()
        {
            comboBox3.Items.Clear();

            Program.Viewer.CurrentModel = new Model(ModelManager.Instance.CreateInstance(renderSys, new FileLocation(openFileDialog1.FileName)));

            Program.Viewer.CurrentModel.PlayAnimation();
            Program.Viewer.CurrentModel.AutoLoop = true;

            ModelData data = Program.Viewer.CurrentModel.GetData();
            Mesh[] meshes = data.Entities;
            for (int i = 0; i < meshes.Length; i++)
            {
                comboBox3.Items.Add(new MeshWrapper(meshes[i], i));
            }
            toolStripButton7.Checked = true;
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
            comboBox1.Items.Clear();

            Mesh mesh = ((MeshWrapper)comboBox3.SelectedItem).Mesh;
            Material[][] mats = mesh.Materials;

            for (int i = 0; i < mats.Length; i++)
            {
                comboBox1.Items.Add(new PartMaterialWrapper(mesh, i, mats[i]));
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            tabControl1.TabPages.Clear();
            Material[] mtrls = ((PartMaterialWrapper)comboBox1.SelectedItem).Materials;
            for (int i = 0; i < mtrls.Length; i++)
            {
                TabPage page = new TabPage("Material Frame " + i.ToString());

                page.Tag = mtrls[i];
                tabControl1.TabPages.Add(page);
            }
            if (tabControl1.TabPages.Count > 0)
            {
                tabControl1.TabPages[0].Select();
                tabControl1_Selected(null,new TabControlEventArgs (tabControl1.TabPages[0], 0, TabControlAction.Selected));
            }
            //tabControl1_Selected(null, new TabControlEventArgs (
        }
        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage != null)
            {
                panel2.Parent = e.TabPage;

                Material mtrl = (Material)e.TabPage.Tag;
                

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
                textBox6.Text = mtrl.GetTextureFile(2);
                textBox7.Text = mtrl.GetTextureFile(3);
                textBox8.Text = mtrl.GetTextureFile(4);
                textBox9.Text = mtrl.GetTextureFile(5);
                textBox3.Text = mtrl.GetEffectName();// mtrl.Effect == null ? string.Empty : mtrl.Effect.Name;

                checkBox1.Checked = mtrl.IsTransparent;
                checkBox4.Checked = mtrl.ZWriteEnabled;
                checkBox2.Checked = mtrl.IsVegetation;
                checkBox3.Checked = mtrl.ZEnabled;

                comboBox4.SelectedIndex = (int)mtrl.PriorityHint;
                comboBox5.SelectedIndex = (int)mtrl.Flags;

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
        }



        private void button5_Click(object sender, EventArgs e)
        {
            Material mtrl = (Material)tabControl1.SelectedTab.Tag;

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
                textBox2.Text != mtrl.GetTextureFile(1) ||
                textBox6.Text != mtrl.GetTextureFile(2) ||
                textBox7.Text != mtrl.GetTextureFile(3) ||
                textBox8.Text != mtrl.GetTextureFile(4) ||
                textBox9.Text != mtrl.GetTextureFile(5))
            {
                mtrl.SetTextureFile(0, textBox1.Text);
                mtrl.SetTextureFile(1, textBox2.Text);
                mtrl.SetTextureFile(2, textBox6.Text);
                mtrl.SetTextureFile(3, textBox7.Text);
                mtrl.SetTextureFile(4, textBox8.Text);
                mtrl.SetTextureFile(5, textBox9.Text);

                mtrl.ReloadTextures();

            }

            mtrl.SetEffectName(textBox3.Text);
            

            mtrl.IsTransparent = checkBox1.Checked;
            mtrl.ZWriteEnabled = checkBox4.Checked;
            mtrl.IsVegetation = checkBox2.Checked;
            mtrl.ZEnabled = checkBox3.Checked;
            mtrl.PriorityHint = (RenderPriority)comboBox4.SelectedIndex;
            mtrl.Flags = (MaterialFlags)comboBox5.SelectedIndex;

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

        private void button6_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                PartMaterialWrapper pmw = (PartMaterialWrapper)comboBox1.SelectedItem;
                Material[] mtrls = pmw.Materials;

                
                Array.Resize<Material>(ref mtrls, mtrls.Length + 1);

                mtrls[mtrls.Length - 1] = mtrls[mtrls.Length - 2].Clone();

                pmw.Materials = mtrls;



                comboBox1_SelectedIndexChanged(null, EventArgs.Empty);
            }
        }


        private void button10_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null && tabControl1.SelectedTab!=null)
            {
                PartMaterialWrapper pmw = (PartMaterialWrapper)comboBox1.SelectedItem;
                Material[] mtrls = pmw.Materials;
                if (mtrls.Length > 1) 
                {
                    int idx = Array.IndexOf<Material>(mtrls, (Material)tabControl1.SelectedTab.Tag);


                    if (idx != -1) 
                    {
                        List<Material> temp = new List<Material>(mtrls);
                        temp.RemoveAt(idx);
                        mtrls = temp.ToArray();
                        pmw.Materials = mtrls;
                    }
                }


                comboBox1_SelectedIndexChanged(null, EventArgs.Empty);
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = button1.BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                button1.BackColor = colorDialog1.Color;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = button2.BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                button2.BackColor = colorDialog1.Color;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = button3.BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                button3.BackColor = colorDialog1.Color;
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = button4.BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                button4.BackColor = colorDialog1.Color;
            }

        }

        float Time2TimeLineX(float time, float total)
        {
            return time / total * (panel1.Width - 50) + 25;
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (Program.Viewer.CurrentModel != null) 
            {
                MaterialAnimationClip clip=  Program.Viewer.CurrentModel.GetData().MaterialAnimationClip;

                if (clip != null)
                {
                    Graphics g = e.Graphics;
                    g.Clear(Color.White);

                    float total = clip.Duration;

                    float step = total / 50.0f;

                    float stepLen = panel1.Width / 50.0f;

                    for (int i = 0; i < 50; i++)
                    {
                        PointF pa = new PointF(Time2TimeLineX(i * step, total), 0);
                        PointF pb = new PointF(Time2TimeLineX(i * step, total), panel1.Height);
                        g.DrawLine(Pens.Black, pa, pb);
                    }

                    for (int i = 0; i < clip.Keyframes.Count; i++)
                    {
                        float time = clip.Keyframes[i].Time;
                        g.DrawString(TimeSpan.FromSeconds(time).ToString(),
                            new System.Drawing.Font("S", 8), Brushes.Black, new PointF(Time2TimeLineX(time,total), 0));
                    }

                    g.DrawString(TimeSpan.FromSeconds(total).ToString(),
                           new System.Drawing.Font("S", 8), Brushes.Black, new PointF(panel1.Width - 25, 0));

                    for (int i = 0; i < clip.Keyframes.Count; i++)
                    {
                        float time = clip.Keyframes[i].Time;
                        PointF pa = new PointF(Time2TimeLineX(time, total), 15);
                        //PointF pd = new PointF(Time2TimeLineX(time, total) + stepLen, panel1.Height);

                        System.Drawing.RectangleF rect = new System.Drawing.RectangleF(pa.X, pa.Y, stepLen, panel1.Height);
                        g.FillRectangle(Brushes.Red, rect);
                    }
                }
            }
        }


        #region 播放控制
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (Program.Viewer.CurrentModel != null)
                Program.Viewer.CurrentModel.PlayAnimation();
        }


        private void toolStripButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (Program.Viewer.CurrentModel != null)
            {
                if (toolStripButton6.Checked)
                {
                    Program.Viewer.CurrentModel.PauseAnimation();
                }
                else
                {
                    Program.Viewer.CurrentModel.ResumeAnimation();
                }
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            if (Program.Viewer.CurrentModel != null)
            {

                Program.Viewer.CurrentModel.StopAnimation();

            }
        }

        private void toolStripButton7_CheckedChanged(object sender, EventArgs e)
        {
            if (Program.Viewer.CurrentModel != null)
            {
                Program.Viewer.CurrentModel.AutoLoop = toolStripButton7.Checked;
              
            }
        }
        #endregion


        void SelectKeyFrame(int idx, MaterialAnimationKeyFrame kf)
        {
            selectedKeyFrame = idx;

            if (selectedKeyFrame != -1)
            {
                textBox4.Text = kf.Time.ToString();
                textBox5.Text = kf.MaterialIndex.ToString();
            }
            else
            {
                textBox4.Text = "";
                textBox5.Text = "";
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (Program.Viewer.CurrentModel != null)
            {
                MaterialAnimationClip clip = Program.Viewer.CurrentModel.GetData().MaterialAnimationClip;

                if (clip != null)
                {
                    List<MaterialAnimationKeyFrame> kfs = clip.Keyframes;

                    float time = kfs[kfs.Count - 1].Time + 0.5f;
                    int mid = kfs[kfs.Count - 1].MaterialIndex + 1;
                    kfs.Add(new MaterialAnimationKeyFrame(time, mid));


                    for (int i = 0; i < kfs.Count; i++) 
                    {
                        if (kfs[i].Time > time)
                            time = kfs[i].Time;
                    }
                    clip.SetDuration(time);
                }
                else 
                {
                    List<MaterialAnimationKeyFrame> kfs = new List<MaterialAnimationKeyFrame>();
                    kfs.Add(new MaterialAnimationKeyFrame(0, 0));
                    kfs.Add(new MaterialAnimationKeyFrame(1, 1));

                    clip = new MaterialAnimationClip(1, kfs);
                    Program.Viewer.CurrentModel.GetData().SetMaterialAnimationClip(clip);
                }
                Program.Viewer.CurrentModel.ReloadMaterialAnimation();
                Program.Viewer.CurrentModel.PlayAnimation();
            }
            panel1.Invalidate();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (selectedKeyFrame != -1)
            {
                if (Program.Viewer.CurrentModel != null)
                {
                    MaterialAnimationClip clip = Program.Viewer.CurrentModel.GetData().MaterialAnimationClip;

                    if (clip != null)
                    {
                        List<MaterialAnimationKeyFrame> kfs = clip.Keyframes;

                        float time = float.Parse(textBox4.Text);
                        kfs[selectedKeyFrame] = new MaterialAnimationKeyFrame(time, int.Parse(textBox5.Text));


                        for (int i = 0; i < kfs.Count; i++)
                        {
                            if (kfs[i].Time > time)
                                time = kfs[i].Time;
                        }
                        clip.SetDuration(time);
                    }
                    Program.Viewer.CurrentModel.PlayAnimation();
                }
                panel1.Invalidate();
                
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (Program.Viewer.CurrentModel != null)
            {
                MaterialAnimationClip clip = Program.Viewer.CurrentModel.GetData().MaterialAnimationClip;

                if (clip != null)
                {
                    if (clip.Keyframes.Count <= 2)
                    {
                        Program.Viewer.CurrentModel.GetData().SetMaterialAnimationClip(null);
                    }
                    else 
                    {
                        clip.Keyframes.RemoveAt(selectedKeyFrame);
                    }
                }
                Program.Viewer.CurrentModel.ReloadMaterialAnimation();
                Program.Viewer.CurrentModel.PlayAnimation();
            }
            panel1.Invalidate();
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            if (Program.Viewer.CurrentModel != null)
            {
                MaterialAnimationClip clip = Program.Viewer.CurrentModel.GetData().MaterialAnimationClip;

                if (clip != null)
                {
                    float total = clip.Duration;

                    float step = total / 50.0f;
                    float stepLen = panel1.Width / 50.0f;
                   
                    

                    for (int i = 0; i < clip.Keyframes.Count; i++)
                    {
                        float time = clip.Keyframes[i].Time;
                        PointF pa = new PointF(Time2TimeLineX(time, total), 15);

                        System.Drawing.RectangleF rect = new System.Drawing.RectangleF(pa.X, pa.Y, stepLen, panel1.Height);

                        int x = e.X;
                        int y = e.Y;

                        if (x > rect.Left && x < rect.Right && y > rect.Top && y < rect.Bottom)
                        {
                            SelectKeyFrame(i, clip.Keyframes[i]);
                            return;
                        }
                    }
                }
            }
            SelectKeyFrame(-1, new MaterialAnimationKeyFrame());
        }


    }
}
