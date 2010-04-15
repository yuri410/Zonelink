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
using System.Xml;

namespace MapEdit
{
    public partial class MainForm : Form
    {
        #region joujia
        Image currentImage;

        List<MapObject> objectList = new List<MapObject>();

        MapObject selectedObject;
        #endregion

        List<Image> bgImages = new List<Image>();

        Graphics g = null;
        Image City, ResWood, ResOil, Sound, Scene;
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

             City = Image.FromFile("City");
             ResWood = Image.FromFile("ResWood");
             ResOil = Image.FromFile("ResOil");
             Sound = Image.FromFile("Sound");
             Scene = Image.FromFile("Scene");

            
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            MapObject.MapWidth = pictureBox1.Width;
            MapObject.MapHeight = pictureBox1.Height;

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                
                StreamWriter sw = new StreamWriter(
                    File.Open(Path.Combine(folderBrowserDialog1.SelectedPath, "cities.xml"), FileMode.OpenOrCreate), 
                    Encoding.UTF8);
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                sw.WriteLine("<cities>");
                for (int i = 0; i < objectList.Count; i++)
                {
                    MapObject obj = objectList[i];

                    if (objectList[i].Type == ObjectType.City)
                    {
                        City city = (City)objectList[i].Tag;
                        sw.Write("    "); sw.Write("<"); sw.Write(objectList[i].SectionName); sw.WriteLine(">");

                        sw.Write("        ");
                        sw.Write("<Name>"); sw.Write(city.Name); sw.WriteLine(@"</Name>");

                        sw.Write("        "); 
                        sw.Write("<Longitude>"); sw.Write(city.Longitude); sw.WriteLine("</Longitude>");

                        sw.Write("        "); 
                        sw.Write("<Latitude>"); sw.Write(city.Latitude); sw.WriteLine("</Latitude>");

                        sw.Write("        ");
                        sw.Write("<Size>"); sw.Write(city.Size.ToString()); sw.WriteLine("</Size>");

                        sw.Write("        ");
                        sw.Write("<"); sw.Write(objectList[i].SectionName); sw.WriteLine(">");

                        
                        sw.Write("    "); sw.Write("<"); sw.Write(objectList[i]..SectionName); sw.WriteLine(@"/>");

                    }
                }
                sw.WriteLine("</cities>");
                sw.Close();

                sw = new StreamWriter(
                    File.Open(Path.Combine(folderBrowserDialog1.SelectedPath, "resources.xml"), FileMode.OpenOrCreate),
                    Encoding.UTF8);
                for (int i = 0; i < objectList.Count; i++)
                {

                }
                sw.Close();
                sw = new StreamWriter(
                    File.Open(Path.Combine(folderBrowserDialog1.SelectedPath, "sceneObjects.xml"), FileMode.OpenOrCreate), 
                    Encoding.UTF8);
                for (int i = 0; i < objectList.Count; i++)
                {

                }
                sw.Close();
                sw = new StreamWriter(
                    File.Open(Path.Combine(folderBrowserDialog1.SelectedPath, "soundObject.xml"), FileMode.OpenOrCreate), 
                    Encoding.UTF8);
                for (int i = 0; i < objectList.Count; i++)
                {

                }
                sw.Close();
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }

        private void DrawAll()
        {
            for (int i = 0; i < objectList.Count; i++)
            {
                switch (objectList[i].Type)
                { 
                    case ObjectType.City:
                        g.DrawImage(City,new Point(objectList[i].X,objectList[i].Y));
                        break;
                    case ObjectType.ResWood:
                        g.DrawImage(ResWood, new Point(objectList[i].X, objectList[i].Y));
                        break;
                    case ObjectType.ResOil:
                        g.DrawImage(ResOil, new Point(objectList[i].X, objectList[i].Y));
                        break;
                    case ObjectType.Sound:
                        g.DrawImage(Sound, new Point(objectList[i].X, objectList[i].Y));
                        break;
                    case ObjectType.Scene:
                        g.DrawImage(Scene, new Point(objectList[i].X, objectList[i].Y));
                        break;

                }
                
            }
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
                    obj.SectionName = sect.Name;
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
                    obj.SectionName = sect.Name; 
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
                    obj.SectionName = sect.Name;
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
                    obj.SectionName = sect.Name; 
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

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < objectList.Count; i++)
            {
                MapObject obj = objectList[i];
                if (obj.Intersects(e.X, e.Y))
                {
                    selectedObject = obj;
                    return;
                }
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            if (selectedObject != null)
            {
                objectList.Remove(selectedObject);
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            objectList.Add(new MapObject());
        }

    }
}
