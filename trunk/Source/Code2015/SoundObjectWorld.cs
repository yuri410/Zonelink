using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Config;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.World;

namespace Code2015
{

    class SoundEmitterObject : IConfigurable
    {
        Vector3 position;
        float radius;

        SoundObject sound;
        //float probability;

        public float Radius
        {
            get { return radius; }
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }



        #region IConfigurable 成员

        public void Parse(ConfigurationSection sect)
        {
            float lng = sect.GetSingle("Longitude");
            float lat = sect.GetSingle("Latitude");
            lng = MathEx.Degree2Radian(lng);
            lat = MathEx.Degree2Radian(lat);

            float alt = TerrainData.Instance.QueryHeight(lng, lat);

            position = PlanetEarth.GetPosition(lng, lat);

            radius = sect.GetSingle("Radius");

            radius = PlanetEarth.GetTileHeight(MathEx.Degree2Radian(radius));

            string sfxName = sect["SFX"];

            sound = SoundManager.Instance.MakeSoundObjcet(sfxName, null, radius);
            sound.Position = position;

            //probability = sect.GetSingle("Probability", 1);
        }

        #endregion

        public void Update(GameTime time) 
        {
            sound.Update(time);
        }
    }

    class SoundObjectWorld
    {
        FastList<SoundEmitterObject> objectList = new FastList<SoundEmitterObject>();


        public SoundObjectWorld()
        {
            FileLocation fl = FileSystem.Instance.Locate("soundObjects.xml", GameFileLocs.Config);
            Configuration config = ConfigurationManager.Instance.CreateInstance(fl);

            foreach (KeyValuePair<string, ConfigurationSection> e in config)
            {
                SoundEmitterObject obj = new SoundEmitterObject();
                obj.Parse(e.Value);

                objectList.Add(obj);
            }
        }


        public void Update(GameTime time)
        {
            for (int i = 0; i < objectList.Count; i++)
            {
                objectList[i].Update(time);
            }
        }
    }
}
