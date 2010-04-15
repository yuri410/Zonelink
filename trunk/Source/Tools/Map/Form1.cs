using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace Map
{
    public partial class Form1 : Form
    {   
        private Graphics g = null;
        private MouseEventArgs mouseE = null;
        //Point P;
        public Form1()
        {
            InitializeComponent();
            this.ClientSize = new Size(800, 500);

            g = this.CreateGraphics();

            //P = System.Windows.Forms.Control.MousePosition;
        }

     
        private void pictureBox1_Click(object sender, EventArgs e)
        {
           
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Load("world.png");
            //pictureBox1.Visible = false;

        }

        private void DrawMap(Graphics g)
        {
            g.DrawImage(Image.FromFile(@"G:\lrvbsvnicg\Source\Tools\Map\world.png"), new Point(0, 0));
            
        }

        private void DrawGridding(Graphics g)//画网格
        {
            for (int i = 0; i <= 27; i++)
            {
                g.DrawLine(new Pen(Color.Black), new Point(i * 30-10, 0), new Point(i *30-10, 432));
            }
            for (int j = 0; j <= 14; j++)
            {
                g.DrawLine(new Pen(Color.Black), new Point(0, j * 30+10), new Point(828, j * 30+10));
            }
       
        }


       

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;
            //pictureBox2.Location = P;
            textBox1.Text = x.ToString() + ", " + y.ToString();

            float lat = 90f -y * 180f / 400f;
            float lon =x * 360f / 800f - 180f;
            textBox2.Text =  lon.ToString() + ", " + lat.ToString();


        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] v = textBox1.Text.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (v.Length == 0 || v.Length != 2)
            {
                MessageBox.Show("不合法输入");
            }
            else
            {
                int x = int.Parse(v[0]);
                float lon = x * 360f / 800f - 180f;

                int y = int.Parse(v[1]);
                float lat = 90f - y * 180f / 400;

                calll.Text = lon.ToString() + "," + lat.ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string[] v = textBox2.Text.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (v.Length==0|| v.Length != 2)
            {
                MessageBox.Show("不合法输入");
            }
            else
            {
                float lon = float.Parse(v[0]);
                int x = (int)((lon + 180f) / 360f * 800);
                float lat = float.Parse(v[1]);
                int y = (int)((90f - lat) / 180f * 400);

                calxy.Text = x.ToString() + "," + y.ToString();
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {

            this.DrawMap(e.Graphics);
            this.DrawGridding(e.Graphics);
            
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.pictureBox1.Visible = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.pictureBox1.Visible = false;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
      
            int x = e.X;
            int y = e.Y;
            pictureBox2.Location = new Point(x, y);
            textBox1.Text = x.ToString() + ", " + y.ToString();

            float lat = 90f - y * 180f / 400f;
            float lon = x * 360f / 800f - 180f;
            textBox2.Text = lon.ToString() + ", " + lat.ToString();
        }
    }
}
