using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Plugin.BitmapFontTools
{
    partial class FontDlg : Form
    {
        public FontDlg(FontFamily[] fonts)
        {
            InitializeComponent();

            for (int i = 0; i < fonts.Length; i++)
            {
                checkedListBox1.Items.Add(fonts[i]);
            }

            DialogResult = DialogResult.Cancel;
        }

        public FontFamily[] SelectedFonts() 
        {
            CheckedListBox.CheckedItemCollection items = checkedListBox1.CheckedItems;

            FontFamily[] result = new FontFamily[items.Count];

            for (int i = 0; i < result.Length; i++) 
            {
                result[i] = (FontFamily)items[i];
            }
            return result;
        }
        public string DestPath 
        {
            get { return textBox1.Text; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

    }
}
