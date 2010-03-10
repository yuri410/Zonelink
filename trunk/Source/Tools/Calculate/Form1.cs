using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Calculate
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int X = int.Parse(x.Text);
            int Y = int.Parse(y.Text);
            int width = int.Parse(Width.Text);
            int height = int.Parse(Height.Text);
            float input = float.Parse(inputHlat.Text);

            float lat = 90f - Y * 180f / height;
            latitude.Text = lat.ToString();

          
            float lon = X * 360f / width - 180f;
            longtitude.Text = lon.ToString();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //cal X
            int width = int.Parse(Width.Text);
            float input = float.Parse(inputHlat.Text);

            
            int X = (int)((float.Parse(longtitude.Text) + 180f) / 360f * width);
          
            x.Text = X.ToString();
            //cal Y
            int height = int.Parse(Height.Text);
           
            int Y = (int)((90f - float.Parse(latitude.Text)) / 180f * height);
         
            y.Text = Y.ToString();
        }
    }
}
