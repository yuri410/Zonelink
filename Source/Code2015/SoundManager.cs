using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Apoc3D.Vfs;
using Apoc3D.Config;
using Code2015.EngineEx;
using Microsoft.Xna.Framework.Audio;
using Apoc3D.MathLib;

namespace Code2015
{

    public class SoundManager
    {
        public static readonly int TotalSounds = 10;//环境音效的种类

      
        private static volatile SoundManager instance;
        private static object synRoot = new object();
        /// <summary>
        /// 需要提供一个service,以及音效所在文件夹的根目录
        /// </summary>
        /// <param name="service"></param>
        /// <param name="root"></param>
        private SoundManager(IServiceProvider service, string root,string xmlurl)
        {
            Service = service;
            RootDirectory = root;
            XMLUrl = xmlurl;
        }
        public static SoundManager GetInstance(IServiceProvider service, string root,string Xmlurl)
        {
            if (instance == null)
                lock (synRoot)
                {
                    if (instance == null)
                        instance = new SoundManager(service, root,Xmlurl);
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
        public string XMLUrl
        {
            get;
            set;
        }
        /// <summary>
        /// ContentManager用于加载声音素材
        /// </summary>
        ContentManager contentManager;
        /// <summary>
        /// service 与 rootDirectory都与soundManager的一样
        /// </summary>
        /// <returns></returns>
        public ContentManager MakeContentManager()
        {
            contentManager = new ContentManager(this.Service, this.RootDirectory);
            return contentManager;

        }
        /// <summary>
        /// 用于存储所有声音的名称
        /// </summary>
       

        bool isloop = false;
        public bool IsLoop
        {
            get { return isloop; }
            set { isloop = value; }
        }
      
        public string[,] SoundsName = new string[10,10];
        public string[] ObjectName = new string[10];
        public SoundObject SoundObject
        {
            get;
            set;
        }      
        public void Initialize()
        {
           
            FileLocation fl = FileSystem.Instance.Locate(this.XMLUrl, GameFileLocs.Config);
            Configuration conf = ConfigurationManager.Instance.CreateInstance(fl);
            foreach (KeyValuePair<string, ConfigurationSection> c in conf)
            {
                ConfigurationSection sect = c.Value;

                //获得所有环境声音的名称
                int i = 0;
                ObjectName[i] = c.Key.ToLowerInvariant();
                

                string control = sect.GetString("Control", string.Empty).ToLowerInvariant();

                if (control.Contains("loop") || control.Contains("random"))
                {
                    string fadein = sect.GetString("Fadein", string.Empty).ToLowerInvariant();
                    SoundsName[i, 0] = fadein;
                    string[] mediumstring = sect.GetStringArray("Medium");
                    for (int index = 0; index < mediumstring.Length; index++)
                    {
                        SoundsName[i, index + 1] = mediumstring[index];
                    }
                    string fadeout = sect.GetString("Fadeout", string.Empty).ToLowerInvariant();
                    SoundsName[i, mediumstring.Length + 2] = fadeout;

               
                    
                        this.isloop = true;
                }
                else if (control.Contains("none"))
                {
                    string[] mediumstring = sect.GetStringArray("Medium");
                   
                    for (int index = 0; index < mediumstring.Length; index++)
                    {
                        SoundsName[i, index] = mediumstring[index];
                    }
                    this.isloop = false;
                }

                i++;

            }
            
        }
        public SoundObject MakeSoundObjcet(string name,Vector3 position)
        {
            string objectname = "";
            for (int i = 0; i < ObjectName.Length; i++)
            { 
                if(ObjectName[i].Contains(name))
                    objectname=name;
            }
            return new SoundObject(this, name, position);                     
        }

        //public SoundEffectGame[] LoadSounds(string name)
        //{
        //    SoundEffectGame[] soundsEffect = new SoundEffectGame[MaxEffect];


        //    for (int i = 0; i < MaxEffect; i++)
        //    {
        //        try
        //        {
        //            soundsEffect[i] = contentManager.Load<SoundEffectGame>(name);
        //        }
        //        catch (Exception e)
        //        {
        //            Console.WriteLine(e.Message + "加载失败");
        //        }
        //    }
        //    return soundsEffect;
        //}
        /// <summary>
        /// 将各个音效连接起来
        /// </summary>
        /// <param name="start">切入的音乐</param>
        /// <param name="medium">中间循环的音乐</param>
        /// <param name="end">淡出的音乐</param>
        /// <param name="distance">摄像机和发声物体之间的距离</param>
        /*
          public void ConnectEffect(SoundEffectGame start, SoundEffectGame[] medium, SoundEffectGame end, float distance)
          {
              //离得远时的音效
              if (distance > MaxDistance)
              {
                  SoundEffectInstance startInstance = start.Play();
                  startInstance.Pause();
                  startInstance.Volume = 0.3f;
                  startInstance.Play();
                  SoundEffectInstance mediumInstance;
                  if (startInstance.State == SoundState.Stopped)
                  {
                      int i = 0;
                      if (i < medium.Length)
                      {
                          mediumInstance = medium[i].Play();
                          mediumInstance.Pause();
                          mediumInstance.Volume = 0.3f;
                          mediumInstance.Play();
                          if (mediumInstance.State == SoundState.Stopped)
                          {
                              i++;
                              mediumInstance = medium[i].Play();
                              mediumInstance.Pause();
                              mediumInstance.Volume = 0.3f;
                              mediumInstance.Play();
                          }
                      }
                  }

              }
              //靠近发声物体时的音效
              else if (distance <= MaxDistance && distance >= MinDistance)
              {
                  SoundEffectInstance startInstance = start.Play();
                  startInstance.Pause();
                  startInstance.Volume = 0.8f;
                  startInstance.Play();
                  SoundEffectInstance mediumInstance;
                  if (startInstance.State == SoundState.Stopped)
                  {
                      int i = 0;
                      if (i < medium.Length)
                      {
                          mediumInstance = medium[i].Play();
                          mediumInstance.Pause();
                          mediumInstance.Volume = 0.8f;
                          mediumInstance.Play();
                          if (mediumInstance.State == SoundState.Stopped)
                          {
                              i++;
                              mediumInstance = medium[i].Play();
                              mediumInstance.Pause();
                              mediumInstance.Volume = 0.8f;
                              mediumInstance.Play();
                          }
                      }
                  }
              }
              else if (distance < MinDistance)
              {
                  SoundEffectInstance startInstance = start.Play();
                  startInstance.Pause();
                  startInstance.Volume = 1.0f;
                  startInstance.Play();
                  SoundEffectInstance mediumInstance;
                  if (startInstance.State == SoundState.Stopped)
                  {
                      int i = 0;
                      if (i < medium.Length)
                      {
                          mediumInstance = medium[i].Play();
                          mediumInstance.Pause();
                          mediumInstance.Volume = 1.0f;
                          mediumInstance.Play();
                          if (mediumInstance.State == SoundState.Stopped)
                          {
                              i++;
                              mediumInstance = medium[i].Play();
                              mediumInstance.Pause();
                              mediumInstance.Volume = 1.0f;
                              mediumInstance.Play();
                          }
                      }
                  }
              }






          }*/
    }

}

