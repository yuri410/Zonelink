/*
-----------------------------------------------------------------------------
This source file is part of Zonelink

Copyright (c) 2009+ Tao Games

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  if not, write to the Free Software Foundation, 
Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA, or go to
http://www.gnu.org/copyleft/gpl.txt.

-----------------------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Config;
using Code2015.EngineEx;
using Code2015.World;
using Microsoft.Xna.Framework;
using Zonelink.World;
using System.IO;
using Zonelink;

namespace Code2015
{

    class SoundEmitterObject
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
            lng = MathHelper.ToRadians(lng);
            lat = MathHelper.ToRadians(lat);

            float alt = TerrainData.Instance.QueryHeight(lng, lat);

            position = PlanetEarth.GetPosition(lng, lat);

            radius = sect.GetSingle("Radius");

            radius = PlanetEarth.GetTileHeight(MathHelper.ToRadians(radius));

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
            GameConfiguration config = new GameConfiguration(Path.Combine(GameFileLocs.Configs, "soundObjects.xml"));
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