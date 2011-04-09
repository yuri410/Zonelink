/*
-----------------------------------------------------------------------------
This source file is part of Apoc3D Engine

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

namespace Apoc3D.Ide
{
    public static class PresetedPlatform
    {
        public const int VirtualBike = 1;
        public const int YurisRevenge = 1 << 1;
        public const int Ra2Reload = 1 << 2;
      
        public const int All = -1;
    

        public static string VirtualBikeName
        {
            get { return DevStringTable.Instance["GUI:VB"]; }
        }
        public static string YurisRevengeName
        {
            get { return DevStringTable.Instance["GUI:RA2YR"]; }
        }

        public static string Ra2ReloadName
        {
            get { return DevStringTable.Instance["GUI:RA2RL"]; }
        }
    }

    public class PlatformManager
    {
        static PlatformManager singleton;

        public static PlatformManager Instance
        {
            get
            {
                if (singleton == null)
                {
                    singleton = new PlatformManager();
                }
                return singleton; 
            }
        }

        Dictionary<int, string> platforms;

        private PlatformManager()
        {
            platforms = new Dictionary<int, string>(10);
        }

        public void RegisterPlatform(int fieldCode, string platformName)
        {
            platforms.Add(fieldCode, platformName);
        }

        public void UnregisterPlatform(int fieldCode)
        {
            platforms.Remove(fieldCode);
        }


        public bool Exists(string platformName)
        {
            return platforms.ContainsValue(platformName);
        }
        public bool Exists(int fieldCode)
        {
            return platforms.ContainsKey(fieldCode);
        }

        public string this[int fieldCode]
        {
            get
            {
                return platforms[fieldCode];
            }
        }
        public int Count
        {
            get { return platforms.Count; }
        }
        public KeyValuePair<int, string>[] GetPlatforms()
        {
            KeyValuePair<int, string>[] res = new KeyValuePair<int, string>[platforms.Count];
            int index = 0;
            foreach (KeyValuePair<int, string> e in platforms)
            {
                res[index++] = e;
            }
            return res;
        }
        
    }
}
