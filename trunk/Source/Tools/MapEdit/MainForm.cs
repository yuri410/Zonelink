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
        #region joujia
        Image currentImage;

        List<MapObject> objectList = new List<MapObject>();

        MapObject selectedObject;

        ObjectType filter;
        bool drawString;
        #endregion

        bool isDraging;

        List<Image> bgImages = new List<Image>();

        Graphics g = null;
        Image cityImage, resWoodImage, resOilImage, soundImage, sceneImage;
        Pen pen;
        Brush brush;
        Font font;
       
        MapObject SelectedObject
        {
            get { return selectedObject; }
            set
            {
                if (selectedObject != value)
                {
                    panel1.Visible = false;
                    panel2.Visible = false;
                    panel3.Visible = false;
                    panel4.Visible = false;

                    selectedObject = value;

                    if (selectedObject != null)
                    {
                        switch (selectedObject.Type)
                        {
                            case ObjectType.City:

                                MapCity city = (MapCity)selectedObject.Tag;
                                textBox1.Text = city.Name;
                                numericUpDown2.Value = city.FarmCount;

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
                            case ObjectType.ResWood:
                            case ObjectType.ResOil:
                                MapResource oil = (MapResource)selectedObject.Tag;

                                numericUpDown9.Value = (decimal)oil.Amount;
                                numericUpDown10.Value = (decimal)oil.Radius;
                                switch (oil.Type)
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
                                MapSceneObject so = (MapSceneObject)selectedObject.Tag;

                                checkBox1.Checked = so.IsForest;

                                numericUpDown12.Value = (decimal)so.Amount;
                                numericUpDown13.Value = (decimal)so.Radius;

                                textBox3.Text = so.Model;

                                panel4.Dock = DockStyle.Fill;
                                panel4.Visible = true;
                                break;
                            case ObjectType.Sound:
                                MapSoundObject sndObj = (MapSoundObject)selectedObject.Tag;

                                textBox2.Text = sndObj.SFXName;
                                numericUpDown11.Value = (decimal)sndObj.Radius;

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
            g = pictureBox1.CreateGraphics();
            ConfigurationManager.Initialize();

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            

          
            cityImage = Image.FromFile("City");
            resWoodImage = Image.FromFile("ResWood");
            resOilImage = Image.FromFile("ResOil");
            soundImage = Image.FromFile("Sound");
            sceneImage = Image.FromFile("Scene");
          
            pen = new Pen(Color.Black);
            brush = pen.Brush;
            font = new Font("Arial", 7, FontStyle.Regular);
            
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
                        MapSceneObject sceObj = (MapSceneObject)obj.Tag;

                        sw.Write("    <"); sw.Write(obj.SectionName); sw.WriteLine(@">");

                        sw.Write("        ");
                        sw.Write("<Longitude>"); sw.Write(obj.Longitude); sw.WriteLine("</Longitude>");

                        sw.Write("        ");
                        sw.Write("<Latitude>"); sw.Write(obj.Latitude); sw.WriteLine("</Latitude>");

                        sw.Write("        ");
                        sw.Write("<IsForest>"); sw.Write(sceObj.IsForest); sw.WriteLine("</IsForest>");

                        sw.Write("        ");
                        sw.Write("<Radius>"); sw.Write(sceObj.Radius); sw.WriteLine("</Radius>");

                        sw.Write("        ");
                        sw.Write("<Amount>"); sw.Write(sceObj.Amount); sw.WriteLine("</Amount>");

                        sw.Write("        ");
                        sw.Write("<Model>"); sw.Write(sceObj.Model); sw.WriteLine("</Model>");

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
                        MapSoundObject sndObj = (MapSoundObject)obj.Tag;

                        sw.Write("    <"); sw.Write(obj.SectionName); sw.WriteLine(@">");

                        sw.Write("        ");
                        sw.Write("<Longitude>"); sw.Write(obj.Longitude); sw.WriteLine("</Longitude>");

                        sw.Write("        ");
                        sw.Write("<Latitude>"); sw.Write(obj.Latitude); sw.WriteLine("</Latitude>");

                        sw.Write("        ");
                        sw.Write("<SFX>"); sw.Write(sndObj.SFXName); sw.WriteLine("</SFX>");

                        sw.Write("        ");
                        sw.Write("<Radius>"); sw.Write(sndObj.Radius); sw.WriteLine("</Radius>");



                        sw.Write("    </"); sw.Write(obj.SectionName); sw.WriteLine(@">");
                    }
                }
                sw.WriteLine("</SoundObjects>");
                sw.Close();
                #endregion
            }

        }

        private void DrawAll()
        {
            if (currentImage != null)
            {
                g.DrawImage(currentImage, new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height));
            }
            for (int i = 0; i < objectList.Count; i++)
            {
                MapObject m = objectList[i];

                switch (m.Type)
                {
                    case ObjectType.City:
                        if ((filter & ObjectType.City) == ObjectType.City)
                        {
                            g.DrawImage(cityImage, m.X, m.Y);
                        }
                        if (drawString && !string.IsNullOrEmpty(m.StringDisplay))
                        {
                            g.DrawString(m.StringDisplay, font, brush, m.X, m.Y);
                        }
                        break;
                    case ObjectType.ResWood:
                        if ((filter & ObjectType.ResWood) == ObjectType.ResWood)
                        {
                            g.DrawImage(resWoodImage, m.X, m.Y);
                        }
                        if (drawString && !string.IsNullOrEmpty(m.StringDisplay))
                        {
                            g.DrawString(m.StringDisplay, font, brush, m.X, m.Y);
                        }
                        break;
                    case ObjectType.ResOil:
                        if ((filter & ObjectType.ResOil) == ObjectType.ResOil)
                        {
                            g.DrawImage(resOilImage, m.X, m.Y);
                        }
                        if (drawString && !string.IsNullOrEmpty(m.StringDisplay))
                        {
                            g.DrawString(m.StringDisplay, font, brush, m.X, m.Y);
                        }
                        break;
                    case ObjectType.Sound:
                        if ((filter & ObjectType.Sound) == ObjectType.Sound)
                        {
                            g.DrawImage(soundImage, m.X, m.Y);
                        }
                        if (drawString && !string.IsNullOrEmpty(m.StringDisplay))
                        {
                            g.DrawString(m.StringDisplay, font, brush, m.X, m.Y);
                        }
                        break;
                    case ObjectType.Scene:
                        if ((filter & ObjectType.Scene) == ObjectType.Scene)
                        {
                            g.DrawImage(sceneImage, m.X, m.Y);
                        }
                        if (drawString && !string.IsNullOrEmpty(m.StringDisplay))
                        {
                            g.DrawString(m.StringDisplay, font, brush, m.X, m.Y);
                        }
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
                    MapCity city = new MapCity(sim, sect);

                    MapObject obj = new MapObject();
                    obj.Longitude = city.Longitude;
                    obj.Latitude = city.Latitude;
                    obj.Tag = city;
                    obj.Type = ObjectType.City;
                    obj.StringDisplay = city.Name;
                    objectList.Add(obj);
                }

                config = ConfigurationManager.Instance.CreateInstance(new FileLocation(Path.Combine(dir, "sceneObjects.xml")));
                foreach (KeyValuePair<string, ConfigurationSection> s in config)
                {
                    ConfigurationSection sect = s.Value;

                    MapSceneObject sceObj = new MapSceneObject(sect);
                    MapObject obj = new MapObject();
                    obj.Longitude = sect.GetSingle("Longitude");
                    obj.Latitude = sect.GetSingle("Latitude");
                    obj.Type = ObjectType.Scene;
                    obj.Tag = sceObj;
                    obj.StringDisplay = sceObj.Model;
                    objectList.Add(obj);
                }

                config = ConfigurationManager.Instance.CreateInstance(new FileLocation(Path.Combine(dir, "resources.xml")));

                foreach (KeyValuePair<string, ConfigurationSection> s in config)
                {
                    ConfigurationSection sect = s.Value;

                    MapResource res = new MapResource(sim, sect);

                    MapObject obj = new MapObject();

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
                    obj.StringDisplay = obj.Type == ObjectType.ResOil ? "O" : "W" + res.Amount.ToString();
                    objectList.Add(obj);
                }

                config = ConfigurationManager.Instance.CreateInstance(new FileLocation(Path.Combine(dir, "soundObjects.xml")));
                foreach (KeyValuePair<string, ConfigurationSection> s in config)
                {
                    ConfigurationSection sect = s.Value;

                    MapSoundObject sndObj = new MapSoundObject(sect);

                    MapObject obj = new MapObject();
                    obj.Longitude = sect.GetSingle("Longitude");
                    obj.Latitude = sect.GetSingle("Latitude");

                    obj.Type = ObjectType.Sound;

                    obj.Tag = sndObj;
                    obj.StringDisplay = sndObj.SFXName;
                    objectList.Add(obj);
                  
                }
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Image img = Image.FromFile(openFileDialog1.FileName);
                checkedListBox1.Items.Add(img);
                bgImages.Add(img);
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentImage = (Image)checkedListBox1.SelectedItem;
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
            if (selectedObject != null)
            {
                switch (selectedObject.Type)
                {
                    case ObjectType.City:
                        MapCity city = (MapCity)selectedObject.Tag;

                        city.Name = textBox1.Text;
                        int numFarms = (int)numericUpDown2.Value;


                        city.ProblemHunger = (float)numericUpDown1.Value;
                        city.ProblemEducation = (float)numericUpDown3.Value;
                        city.ProblemGender = (float)numericUpDown4.Value;
                        city.ProblemChild = (float)numericUpDown5.Value;
                        city.ProblemMaternal = (float)numericUpDown5.Value;
                        city.ProblemDisease = (float)numericUpDown6.Value;
                        city.ProblemEnvironment = (float)numericUpDown7.Value;

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
                        if (string.IsNullOrEmpty(selectedObject.SectionName))
                        {
                            selectedObject.SectionName = "City" + Guid.NewGuid().ToString("N");
                        }
                        selectedObject.StringDisplay = city.Name;
                        break;
                    case ObjectType.ResOil:
                    case ObjectType.ResWood:
                        MapResource oil = (MapResource)selectedObject.Tag;

                        oil.Amount = (float)numericUpDown9.Value;
                        oil.Radius = (float)numericUpDown10.Value;

                        if (radioButton5.Checked)
                        {
                            oil.Type = NaturalResourceType.Wood;
                        }
                        else if (radioButton6.Checked)
                        {
                            oil.Type = NaturalResourceType.Petro;
                        }
                        if (string.IsNullOrEmpty(selectedObject.SectionName))
                        {
                            selectedObject.SectionName = "Resource" + Guid.NewGuid().ToString("N");
                        }
                        selectedObject.StringDisplay = selectedObject.Type == ObjectType.ResOil ? "O" : "W" + oil.Amount.ToString();
                        break;
                    case ObjectType.Scene:
                        MapSceneObject so = (MapSceneObject)selectedObject.Tag;

                        so.IsForest = checkBox1.Checked;

                        so.Amount = (float)numericUpDown12.Value;
                        so.Radius = (float)numericUpDown13.Value;
                        so.Model = textBox3.Text;
                        
                        if (string.IsNullOrEmpty(selectedObject.SectionName))
                        {
                            selectedObject.SectionName = "Scene" + Guid.NewGuid().ToString("N");
                        }
                        selectedObject.StringDisplay = so.Model;
                        break;
                    case ObjectType.Sound:
                        MapSoundObject sndObj = (MapSoundObject)selectedObject.Tag;

                        sndObj.Radius = (float)numericUpDown11.Value;
                        sndObj.SFXName = textBox2.Text;

                        if (string.IsNullOrEmpty(selectedObject.SectionName))
                        {
                            selectedObject.SectionName = "Sound" + Guid.NewGuid().ToString("N");
                        }
                        selectedObject.StringDisplay = sndObj.SFXName;
                        break;
                }

            }
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            drawString = toolStripButton11.Checked;
        }

        #region filter
        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            if (toolStripButton10.Checked)
            {
                filter |= ObjectType.City;
            }
            else
            {
                filter ^= ObjectType.City;
            }
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            if (toolStripButton9.Checked)
            {
                filter |= ObjectType.ResWood;
            }
            else
            {
                filter ^= ObjectType.ResWood;
            }
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            if (toolStripButton8.Checked)
            {
                filter |= ObjectType.ResOil;
            }
            else
            {
                filter ^= ObjectType.ResOil;
            }
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            if (toolStripButton7.Checked)
            {
                filter |= ObjectType.Scene;
            }
            else 
            {
                filter ^= ObjectType.Scene;
            }
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            if (toolStripButton6.Checked)
            {
                filter |= ObjectType.Sound;
            }
            else
            {
                filter ^= ObjectType.Sound;
            }
        }
        #endregion

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDraging && selectedObject != null) 
            {

            }   
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < objectList.Count; i++)
            {
                MapObject obj = objectList[i];
                if (obj.Intersects(e.X, e.Y))
                {
                    SelectedObject = obj;
                    isDraging = true;
                    return;
                }
            }
            SelectedObject = null;
            isDraging = false;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isDraging = false;
        }
    }
}
