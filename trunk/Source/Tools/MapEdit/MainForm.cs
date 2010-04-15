using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Apoc3D.Config;
using Apoc3D.Vfs;
using Code2015.BalanceSystem;

namespace MapEdit
{
    public partial class MainForm : Form
    {
        Image currentImage;

        List<Image> bgImages = new List<Image>();

        List<MapObject> objectList = new List<MapObject>();
        Graphics g = null;

        public MainForm()
        {
            InitializeComponent();

            g = this.CreateGraphics();
            ConfigurationManager.Initialize();


        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            pictureBox1.Load("City");
            pictureBox1.Load("ResWood");
            pictureBox1.Load("ResOil");
            pictureBox1.Load("Sound");
            pictureBox1.Load("Scene");
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            MapObject.MapWidth = pictureBox1.Width;
            MapObject.MapHeight = pictureBox1.Height;

        }


        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }


        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            SimulationWorld sim = new SimulationWorld();

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string dir = folderBrowserDialog1.SelectedPath;

                Configuration config = ConfigurationManager.Instance.CreateInstance(new FileLocation(Path.Combine(dir, "cities.xml")));

                foreach (KeyValuePair<string, ConfigurationSection> s in config)
                {
                    ConfigurationSection sect = s.Value;
                    City city = new City(sim);
                    city.Parse(sect);

                    MapObject obj = new MapObject();
                    obj.Longitude = city.Longitude;
                    obj.Latitude = city.Latitude;
                    obj.Tag = city;
                    obj.Type = ObjectType.City;

                    objectList.Add(obj);
                }

                config = ConfigurationManager.Instance.CreateInstance(new FileLocation(Path.Combine(dir, "sceneObjects.xml")));
                foreach (KeyValuePair<string, ConfigurationSection> s in config)
                {
                    ConfigurationSection sect = s.Value;

                    MapObject obj = new MapObject();
                    obj.Longitude = sect.GetSingle("Longitude");
                    obj.Latitude = sect.GetSingle("Latitude");
                    obj.Type = ObjectType.Scene;
                    objectList.Add(obj);
                }

                config = ConfigurationManager.Instance.CreateInstance(new FileLocation(Path.Combine(dir, "resources.xml")));

                foreach (KeyValuePair<string, ConfigurationSection> s in config)
                {
                    ConfigurationSection sect = s.Value;
                    string type = sect["Type"];

                    NaturalResource res = null;

                    MapObject obj = new MapObject();
                    switch (type)
                    {
                        case "petro":
                            OilField oil = new OilField(sim);
                            oil.Parse(sect);
                            res = oil;
                            break;
                        case "wood":
                            Forest fores = new Forest(sim);
                            fores.Parse(sect);
                            res = fores;
                            break;
                    }

                    obj.Longitude = res.Longitude;
                    obj.Latitude = res.Latitude;
                    obj.Tag = res;
                    if (res.Type == NaturalResourceType.Wood)
                    {
                        obj.Type = ObjectType.ResWood;
                    }
                    else if (res.Type == NaturalResourceType.Petro)
                    {
                        obj.Type = ObjectType.ResOil;
                    }

                    objectList.Add(obj);
                }

                config = ConfigurationManager.Instance.CreateInstance(new FileLocation(Path.Combine(dir, "soundObjects.xml")));
                foreach (KeyValuePair<string, ConfigurationSection> s in config)
                {
                    ConfigurationSection sect = s.Value;

                    MapObject obj = new MapObject();
                    obj.Longitude = sect.GetSingle("Longitude");
                    obj.Latitude = sect.GetSingle("Latitude");

                    obj.Type = ObjectType.Sound;
                    objectList.Add(obj);
                }
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Image img = (Image.FromFile(openFileDialog1.FileName));
                checkedListBox1.Items.Add(img);
                bgImages.Add(img);
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentImage = (Image)checkedListBox1.SelectedItem;
        }

    }
}
