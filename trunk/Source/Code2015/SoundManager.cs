using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Config;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using XF = Microsoft.Xna.Framework;

namespace Code2015
{
    [Flags]
    public enum SoundControl
    {
        None = 0,
        Loop = 1,
        Random = 1 << 1,
        Ambient = 1 << 2
    }
    public enum SoundType
    {
        Normal = 0,
        Player,
        World,
        Global
    }

    public class SoundManager
    {
        private static volatile SoundManager instance;
        private static object synRoot = new object();

        public static SoundManager Instance
        {
            get
            {
                return instance;
            }
        }
        public static void Initialize(IServiceProvider service)
        {
            instance = new SoundManager(service, "Sounds", "soundEffect.xml");

        }

        AudioListener listener;
        Dictionary<string, SoundEffectGame> sfxTable;

        /// <summary>
        /// 需要提供一个service,以及音效所在文件夹的根目录
        /// </summary>
        /// <param name="service"></param>
        /// <param name="root"></param>
        private SoundManager(IServiceProvider service, string root, string xmlurl)
        {
            Service = service;
            RootDirectory = root;
            XMLUrl = xmlurl;
            listener = new AudioListener();

            contentManager = new ContentManager(service);
            FileLocation fl = FileSystem.Instance.Locate(this.XMLUrl, GameFileLocs.Config);
            Configuration conf = ConfigurationManager.Instance.CreateInstance(fl);

            sfxTable = new Dictionary<string, SoundEffectGame>(conf.Count);
            foreach (KeyValuePair<string, ConfigurationSection> c in conf)
            {
                ConfigurationSection sect = c.Value;
                SoundEffectGame sfx = new SoundEffectGame(listener, this, sect);

                sfxTable.Add(sfx.Name, sfx);
            }
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

        public ContentManager Content
        {
            get { return contentManager; }
        }


        public SoundObject MakeSoundObjcet(string name, ISoundObjectParent parent,  float r, float min, float max)
        {
            SoundEffectGame sfx = sfxTable[name];
            switch (sfx.Type)
            {
                case SoundType.Global:
                case SoundType.World:
                    if ((sfx.Control & SoundControl.Loop) == SoundControl.Loop)
                    {
                        return new AmbientLoopSoundObject(this, parent, sfx, r, min, max);
                    }
                    else if ((sfx.Control & SoundControl.Ambient) == SoundControl.Ambient)
                    {
                        return new AmbientSoundObject(this, parent, sfx, r, min, max);
                    }
                    return new Normal3DSoundObject(this, parent, sfx, r, min, max);                  
                case SoundType.Normal:
                case SoundType.Player:
                    return new NormalSoundObject(this, parent, sfx);
            }
            throw new NotSupportedException();
        }
    }

}

