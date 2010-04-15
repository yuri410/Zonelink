﻿using System;
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
        #region joujia
        Image currentImage;

        List<MapObject> objectList = new List<MapObject>();

        MapObject selectedObject;

        ObjectType filter;
        #endregion

        

        List<Image> bgImages = new List<Image>();

        Graphics g = null;
        Image cityImage, resWoodImage, resOilImage, soundImage, sceneImage;

        MapObject SelectedObject
        {
            get { return selectedObject; }
            set
            {
                if (selectedObject != value)
                {
                    selectedObject = value;

                    if (selectedObject != null)
                    {
                        switch (selectedObject.Type)
                        {
                            case ObjectType.City:
                                
                                City city = (City)selectedObject.Tag;
                                textBox1.Text = city.Name;
                                numericUpDown2.Value = city.FarmLandCount;

                                numericUpDown1.Value = (decimal)city.ProblemHunger;
                                numericUpDown3.Value = (decimal)city.ProblemEducation;
                                numericUpDown4.Value = (decimal)city.ProblemGender;
                                numericUpDown5.Value = (decimal)city.ProblemChild;
                                numericUpDown5.Value = (decimal)city.ProblemMaternal;
                                numericUpDown6.Value = (decimal)city.ProblemDisease;
                                numericUpDown7.Value = (decimal)city.ProblemEnvironment;

                                switch (city.Size) 
                                {
                                    case UrbanSize.Small:
                                        radioButton5.Checked = true;
                                        break;
                                    case UrbanSize.Medium:
                                        radioButton6.Checked = true;
                                        break;
                                    case UrbanSize.Large:
                                        radioButton7.Checked = true;
                                        break;
                                }
                                panel1.Dock = DockStyle.Fill;
                                panel1.Visible = true;
                                break;
                            case ObjectType.ResOil:
                                OilField oil = (OilField)selectedObject.Tag;

                                numericUpDown9.Value = (decimal)oil.CurrentAmount;
                                numericUpDown10.Value = 0;
                                switch (oil.Type)
                                {
                                    case  NaturalResourceType.Wood:
                                        radioButton5.Checked = true;
                                        break;
                                    case NaturalResourceType.Petro:
                                        radioButton6.Checked = true;
                                        break;
                                }
                                panel2.Dock = DockStyle.Fill;
                                panel2.Visible = true;
                                break;
                            case ObjectType.ResWood:
                                Forest fore = (Forest)selectedObject.Tag;

                                numericUpDown9.Value = (decimal)fore.CurrentAmount;
                                numericUpDown10.Value = (decimal)fore.Radius;
                                switch (fore.Type)
                                {
                                    case NaturalResourceType.Wood:
                                        radioButton5.Checked = true;
                                        break;
                                    case NaturalResourceType.Petro:
                                        radioButton6.Checked = true;
                                        break;
                                }
                                panel2.Dock = DockStyle.Fill;
                                panel2.Visible = true;
                                break;
                            case ObjectType.Scene:
                                panel4.Dock = DockStyle.Fill;
                                panel4.Visible = true;
                                break;
                            case ObjectType.Sound:


                                panel3.Dock = DockStyle.Fill;
                                panel3.Visible = true;
                                break;
                        }
                    }
                }                
            }
        }
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

            cityImage = Image.FromFile("City");
            resWoodImage = Image.FromFile("ResWood");
            resOilImage = Image.FromFile("ResOil");
            soundImage = Image.FromFile("Sound");
            sceneImage = Image.FromFile("Scene");


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

                #region city info
                StreamWriter sw = new StreamWriter(
                    File.Open(Path.Combine(folderBrowserDialog1.SelectedPath, "cities.xml"), FileMode.OpenOrCreate),
                    Encoding.UTF8);
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                sw.WriteLine("<cities>");
                for (int i = 0; i < objectList.Count; i++)
                {
                    MapObject obj = objectList[i];
                    if (obj.Type == ObjectType.City)
                    {
                        City city = (City)obj.Tag;

                        sw.Write("    <"); sw.Write(obj.SectionName); sw.WriteLine(@">");

                        sw.Write("        ");
                        sw.Write("<Name>"); sw.Write(city.Name); sw.WriteLine(@"</Name>");

                        sw.Write("        ");
                        sw.Write("<Longitude>"); sw.Write(city.Longitude); sw.WriteLine("</Longitude>");

                        sw.Write("        ");
                        sw.Write("<Latitude>"); sw.Write(city.Latitude); sw.WriteLine("</Latitude>");

                        sw.Write("        ");
                        sw.Write("<Size>"); sw.Write(city.Size.ToString()); sw.WriteLine("</Size>");

                        if (city.ProblemChild != City.DefaultProblemWeight)
                        {
                            sw.Write("        ");
                            sw.Write("<Child>"); sw.Write(city.ProblemChild); sw.WriteLine("</Child>");
                        }
                        if (city.ProblemDisease != City.DefaultProblemWeight)
                        {
                            sw.Write("        ");
                            sw.Write("<Disease>"); sw.Write(city.ProblemDisease); sw.WriteLine("</Disease>");
                        }
                        if (city.ProblemEducation != City.DefaultProblemWeight)
                        {
                            sw.Write("        ");
                            sw.Write("<Education>"); sw.Write(city.ProblemEducation); sw.WriteLine("</Education>");
                        }
                        if (city.ProblemEnvironment != City.DefaultProblemWeight)
                        {
                            sw.Write("        ");
                            sw.Write("<Environment>"); sw.Write(city.ProblemEnvironment); sw.WriteLine("</Environment>");
                        }
                        if (city.ProblemGender != City.DefaultProblemWeight)
                        {
                            sw.Write("        ");
                            sw.Write("<Gender>"); sw.Write(city.ProblemGender); sw.WriteLine("</Gender>");
                        }
                        if (city.ProblemHunger != City.DefaultProblemWeight)
                        {
                            sw.Write("        ");
                            sw.Write("<Hunger>"); sw.Write(city.ProblemHunger); sw.WriteLine("</Hunger>");
                        }
                        if (city.ProblemMaternal != City.DefaultProblemWeight)
                        {
                            sw.Write("        ");
                            sw.Write("<Maternal>"); sw.Write(city.ProblemMaternal); sw.WriteLine("</Maternal>");
                        }
                        if (city.StartUp != -1)
                        {
                            sw.Write("        ");
                            sw.Write("<StartUp>"); sw.Write(city.StartUp); sw.WriteLine("</StartUp>");
                        }
                        if (city.FarmLandCount != -1)
                        {
                            sw.Write("        ");
                            sw.Write("<Farm>"); sw.Write(city.FarmLandCount); sw.WriteLine("</Farm>");
                        }



                        string[] linkable = city.LinkableCityName;
                        if (linkable != null && linkable.Length > 0)
                        {
                            sw.Write("        "); sw.Write("<Linkable>");
                            for (int j = 0; j < linkable.Length; j++)
                            {
                                sw.Write(linkable[i]);
                            }
                        }
                        sw.WriteLine("</Linkable>");

                        sw.Write("    </"); sw.Write(obj.SectionName); sw.WriteLine(@">");
                    }
                }
                sw.WriteLine("</cities>");
                sw.Close();
                #endregion

                #region resource
                sw = new StreamWriter(
                    File.Open(Path.Combine(folderBrowserDialog1.SelectedPath, "resources.xml"), FileMode.OpenOrCreate),
                    Encoding.UTF8);
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                sw.WriteLine("<Resources>");
                for (int i = 0; i < objectList.Count; i++)
                {
                    MapObject obj = objectList[i];
                    if (obj.Type == ObjectType.ResOil || obj.Type == ObjectType.ResWood)
                    {
                        NaturalResource res = (NaturalResource)obj.Tag;

                        sw.Write("    <"); sw.Write(obj.SectionName); sw.WriteLine(@">");

                        sw.Write("        ");
                        sw.Write("<Longitude>"); sw.Write(res.Longitude); sw.WriteLine("</Longitude>");

                        sw.Write("        ");
                        sw.Write("<Latitude>"); sw.Write(res.Latitude); sw.WriteLine("</Latitude>");

                        sw.Write("        ");
                        sw.Write("<Type>"); sw.Write(res.Type.ToString()); sw.WriteLine("</Latitude>");

                        sw.Write("        ");
                        sw.Write("<Amount>"); sw.Write(res.CurrentAmount); sw.WriteLine("</Amount>");


                        if (res.Type == NaturalResourceType.Wood)
                        {
                            Forest fore = (Forest)res;
                            sw.Write("        ");
                            sw.Write("<Radius>"); sw.Write(fore.Radius); sw.WriteLine("</Radius>");
                        }
                        sw.Write("    </"); sw.Write(obj.SectionName); sw.WriteLine(@">");
                    }
                }

                sw.WriteLine("</Resources>");
                sw.Close();

                #endregion

                #region
                sw = new StreamWriter(
                    File.Open(Path.Combine(folderBrowserDialog1.SelectedPath, "sceneObjects.xml"), FileMode.OpenOrCreate),
                    Encoding.UTF8);

                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                sw.WriteLine("<Scenery>");
                for (int i = 0; i < objectList.Count; i++)
                {
                    MapObject obj = objectList[i];
                    if (obj.Type == ObjectType.Scene)
                    {
                        sw.Write("    <"); sw.Write(obj.SectionName); sw.WriteLine(@">");

                        sw.Write("        ");
                        sw.Write("<Longitude>"); sw.Write(obj.Longitude); sw.WriteLine("</Longitude>");

                        sw.Write("        ");
                        sw.Write("<Latitude>"); sw.Write(obj.Latitude); sw.WriteLine("</Latitude>");
                      
                        sw.Write("    </"); sw.Write(obj.SectionName); sw.WriteLine(@">");

                    }
                }
                sw.WriteLine("</Scenery>");
                sw.Close();
                #endregion

                #region
                sw = new StreamWriter(
                    File.Open(Path.Combine(folderBrowserDialog1.SelectedPath, "soundObject.xml"), FileMode.OpenOrCreate),
                    Encoding.UTF8);

                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                sw.WriteLine("<SoundObjects>");
                for (int i = 0; i < objectList.Count; i++)
                {
                    MapObject obj = objectList[i];
                    if (obj.Type == ObjectType.Sound)
                    {
                        sw.Write("    <"); sw.Write(obj.SectionName); sw.WriteLine(@">");

                        sw.Write("        ");
                        sw.Write("<Longitude>"); sw.Write(obj.Longitude); sw.WriteLine("</Longitude>");

                        sw.Write("        ");
                        sw.Write("<Latitude>"); sw.Write(obj.Latitude); sw.WriteLine("</Latitude>");

                        sw.Write("    </"); sw.Write(obj.SectionName); sw.WriteLine(@">");
                    }
                }
                sw.WriteLine("</SoundObjects>");
                sw.Close();
                #endregion
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
                        g.DrawImage(cityImage, new Point(objectList[i].X, objectList[i].Y));
                        break;
                    case ObjectType.ResWood:
                        g.DrawImage(resWoodImage, new Point(objectList[i].X, objectList[i].Y));
                        break;
                    case ObjectType.ResOil:
                        g.DrawImage(resOilImage, new Point(objectList[i].X, objectList[i].Y));
                        break;
                    case ObjectType.Sound:
                        g.DrawImage(soundImage, new Point(objectList[i].X, objectList[i].Y));
                        break;
                    case ObjectType.Scene:
                        g.DrawImage(sceneImage, new Point(objectList[i].X, objectList[i].Y));
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

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < objectList.Count; i++)
            {
                MapObject obj = objectList[i];
                if (obj.Intersects(e.X, e.Y))
                {
                    SelectedObject = obj;
                    return;
                }
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            if (SelectedObject != null)
            {
                objectList.Remove(SelectedObject);
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            objectList.Add(new MapObject());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

    }
}