using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Apoc3D.Vfs;
using Apoc3D.Config;
using Code2015.EngineEx;
using Microsoft.Xna.Framework.Audio;

namespace Code2015
{
    public enum SoundKind
    { 
        city,farm,forest,lightning,oil,rain,river,sea,truck,wind
    }
   
    public class SoundManager
    {

        private static volatile SoundManager instance;
        private static object synRoot = new object();
        private SoundManager(IServiceProvider service,string root)
        {
            Service = service;
            RootDirectory = root;
        }
        public static SoundManager GetInstance(IServiceProvider service,string root)
        {
                if(instance==null)
                    lock (synRoot)
                    {
                        if (instance == null)
                            instance = new SoundManager(service,root);
                    }
                return instance;   
        }

        public IServiceProvider Service
        {
            get;
            set;
        }
        public string RootDirectory
        {
            get;
            set;
        }

        ContentManager contentManager;
        public ContentManager MakeContentManager( )
        {
            contentManager = new ContentManager(this.Service, this.RootDirectory);
            return contentManager;
        }

        public void ReadXml(string xmlUrl)
        {
            FileLocation fl = FileSystem.Instance.Locate(xmlUrl, GameFileLocs.Config);
            Configuration conf = ConfigurationManager.Instance.CreateInstance(fl);
            foreach (KeyValuePair<string, ConfigurationSection> c in conf)
            {
                ConfigurationSection sect = c.Value;
               
                string type = sect.GetString("Type", string.Empty).ToLowerInvariant();//normal
                if (type == "normal")
                {
                    //
                }
                string control = sect.GetString("Control", string.Empty).ToLowerInvariant();//none,loop,random
                string[] temp = control.Split(new char[] { ',' }, StringSplitOptions.None);
                for (int i = 0; i < temp.Length; i++)
                {
                    if (temp[i] == "none")
                    {
                        //
                    }
                    else if (temp[i] == "loop"||temp[i]=="random")
                    { 
                        //
                    }
                }

            }
        }

    }

}

