﻿/*
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
http://www.gnu.org/copyleft/lesser.txt.

-----------------------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Config;
using Apoc3D.MathLib;
using Apoc3D.Vfs;

namespace Code2015.EngineEx
{
    class GameConfigurationSection : ConfigurationSection
    {
        const char ValueSeprater = ',';
        const char PathSeprater = '|';
        const char Percentage = '%';

        static readonly char[] ValueSepArray = new char[] { ValueSeprater };

        public GameConfigurationSection(string name)
            : base(name)
        {

        }

        public GameConfigurationSection(GameConfiguration parent, string name, int capacity)
            : base(parent, name, capacity)
        { }

        #region Parser

        public override bool TryGetSubSection(string key, out ConfigurationSection sect)
        {
            if (ParentConfig == null)
            {
                throw new InvalidOperationException();
            }

            string res;
            if (TryGetValue(key, out res))
            {
                sect = ParentConfig[res];
                return true;
            }
            sect = null;
            return false;
        }
        public override ConfigurationSection GetSubSection(string key)
        {
            if (ParentConfig == null)
            {
                throw new InvalidOperationException();
            }
            return ParentConfig[this[key]];
        }
        public override bool TryGetPaths(string key, out string[] res)
        {
            string v;
            if (TryGetValue(key, out v))
            {
                string[] pams = v.Split(PathSeprater);

                for (int i = 0; i < pams.Length; i++)
                {
                    pams[i] = FileNameTokens.Instance.Filter(pams[i]);
                }
                res = pams;
                return true;
            }
            res = null;
            return false;
        }

        public override string[] GetPaths(string key)
        {
            string[] pams = this[key].Split(PathSeprater);
            for (int i = 0; i < pams.Length; i++)
            {
                pams[i] = FileNameTokens.Instance.Filter(pams[i]);
            }
            return pams;
        }
        public override void GetRectangle(string key, out Rectangle rect)
        {
            string v = this[key];
            string[] pams = v.Split(ValueSeprater);
            int len = pams.Length;

            rect = new Rectangle(int.Parse(pams[0]),
                                 int.Parse(pams[1]),
                                 int.Parse(pams[2]),
                                 int.Parse(pams[3]));

        }

        public override bool TryGetColorRGBA(string key, out ColorValue clr)
        {
            string v;
            if (this.TryGetValue(key, out v))
            {
                string[] val = v.Split(ValueSeprater);
                clr = new ColorValue(byte.Parse(val[3]), byte.Parse(val[0]), byte.Parse(val[1]), byte.Parse(val[2]));
                return true;
            }

            clr = default(ColorValue);
            return false;
        }
        public override ColorValue GetColorRGBA(string key, ColorValue def)
        {
            string v;
            if (this.TryGetValue(key, out v))
            {
                string[] val = v.Split(ValueSeprater);

                return new ColorValue(byte.Parse(val[3]),
                    byte.Parse(val[0]),
                    byte.Parse(val[1]),
                    byte.Parse(val[2]));
            }

            return def;
        }
        public override ColorValue GetColorRGBA(string key)
        {
            string v = this[key];
            string[] val = v.Split(ValueSeprater);

            return new ColorValue(byte.Parse(val[3]),
                byte.Parse(val[0]),
                byte.Parse(val[1]),
                byte.Parse(val[2]));
        }

        public override int GetColorRGBInt(string key)
        {
            string v = this[key];
            string[] val = v.Split(ValueSeprater);

            unchecked
            {
                return ((int)0xff000000 | ((int.Parse(val[0]) & 0xff) << 16) | ((int.Parse(val[1]) & 0xff) << 8) | (int.Parse(val[2]) & 0xff));
            }
        }
        public override int GetColorRGBInt(string key, int def)
        {
            string v;

            if (TryGetValue(key, out v))
            {
                string[] val = v.Split(ValueSeprater);

                unchecked
                {
                    return ((int)0xff000000 | ((int.Parse(val[0]) & 0xff) << 16) | ((int.Parse(val[1]) & 0xff) << 8) | (int.Parse(val[2]) & 0xff));
                }
            }
            return def;
        }

        public override bool TryGetBool(string key, out bool res)
        {
            string v;
            if (this.TryGetValue(key, out v))
            {
                v = v.ToUpper();
                if (v == "YES")
                    res = true;
                else if (v == "NO")
                    res = false;
                else if (v == "TRUE")
                    res = true;
                else if (v == "FALSE")
                    res = false;
                else
                    res = int.Parse(v) != 0;
                return true;
            }

            res = default(bool);
            return false;
        }
        public override bool GetBool(string key)
        {
            string v = this[key];
            v = v.ToUpper();
            if (v == "YES")
                return true;
            else if (v == "NO")
                return false;
            else if (v == "TRUE")
                return true;
            else if (v == "FALSE")
                return false;
            else
                return int.Parse(v) != 0;
        }
        public override bool GetBool(string key, bool def)
        {
            string v;
            if (this.TryGetValue(key, out v))
            {
                v = v.ToUpper();
                if (v == "YES")
                    return true;
                else if (v == "NO")
                    return false;
                else if (v == "TRUE")
                    return true;
                else if (v == "FALSE")
                    return false;
                else
                    return int.Parse(v) != 0;
            }

            return def;
        }

        public override float GetSingle(string key)
        {
            return float.Parse(this[key]);
        }
        public override float GetSingle(string key, float def)
        {
            string v;
            if (this.TryGetValue(key, out v))
                return float.Parse(v);
            else
                return def;
        }

        public override float[] GetSingleArray(string key)
        {
            string v = this[key];
            string[] pams = v.Split(ValueSeprater);
            int len = pams.Length;

            float[] res = new float[len];

            for (int i = 0; i < len; i++)
                res[i] = float.Parse(pams[i]);
            return res;
        }
        public override float[] GetSingleArray(string key, float[] def)
        {
            string v;
            if (this.TryGetValue(key, out v))
            {
                string[] pams = v.Split(ValueSeprater);
                int len = pams.Length;

                float[] res = new float[len];

                for (int i = 0; i < len; i++)
                    res[i] = float.Parse(pams[i]);
                return res;
            }
            else
                return def;
        }

        public override string[] GetStringArray(string key, string[] def)
        {
            string res;
            if (TryGetValue(key, out res))
            {
#if XBOX
                string[] arr = res.Split(ValueSepArray);

                List<string> result = new List<string>(arr.Length);

                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = arr[i].Trim();
                    if (arr[i].Length > 0)
                    {
                        result.Add(arr[i]);
                    }
                }
                return result.ToArray();
#else
                string[] arr = res.Split(ValueSepArray, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = arr[i].Trim();
                }
                return arr;
#endif

            }
            return def;
        }
        public override string GetString(string key, string def)
        {
            string res;
            if (TryGetValue(key, out res))
            {
                return res.Trim();
            }
            return def;
        }
        public override string[] GetStringArray(string key)
        {
            string res = this[key];
            string[] arr = res.Split(ValueSeprater);
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = arr[i].Trim();
            }
            return arr;
        }

        public override int GetInt(string key)
        {
            return int.Parse(this[key]);
        }
        public override int GetInt(string key, int def)
        {
            string v;
            if (this.TryGetValue(key, out v))
                return int.Parse(v);
            else
                return def;
        }

        public override int[] GetIntArray(string key)
        {
            string v = this[key];

            string[] pams = v.Split(ValueSeprater);
            int len = pams.Length;

            int[] res = new int[len];

            for (int i = 0; i < len; i++)
                res[i] = int.Parse(pams[i]);
            return res;
        }
        public override int[] GetIntArray(string key, int[] def)
        {
            string v;
            if (this.TryGetValue(key, out v))
            {
                string[] pams = v.Split(ValueSeprater);
                int len = pams.Length;

                int[] res = new int[len];

                for (int i = 0; i < len; i++)
                    res[i] = int.Parse(pams[i]);
                return res;
            }
            else
                return def;

        }

        public override Size GetSize(string key)
        {
            string v = this[key];

            string[] pams = v.Split(ValueSeprater);
            int len = pams.Length;

            if (len == 2)
            {
                int[] res = new int[len];

                for (int i = 0; i < len; i++)
                    res[i] = int.Parse(pams[i]);

                return new Size(res[0], res[1]);
            }
            throw new FormatException();
        }
        public override Size GetSize(string key, Size def)
        {
            string v;
            if (this.TryGetValue(key, out v))
            {
                string[] pams = v.Split(ValueSeprater);
                int len = pams.Length;

                if (len == 2)
                {
                    int[] res = new int[len];

                    for (int i = 0; i < len; i++)
                        res[i] = int.Parse(pams[i]);

                    return new Size(res[0], res[1]);
                }
                throw new FormatException();
            }
            else
                return def;
        }

        public override Point GetPoint(string key)
        {
            string v = this[key];

            string[] pams = v.Split(ValueSeprater);
            int len = pams.Length;

            if (len == 2)
            {
                int[] res = new int[len];

                for (int i = 0; i < len; i++)
                    res[i] = int.Parse(pams[i]);

                return new Point(res[0], res[1]);
            }
            throw new FormatException();
        }
        public override Point GetPoint(string key, Point def)
        {
            string v;
            if (this.TryGetValue(key, out v))
            {
                string[] pams = v.Split(ValueSeprater);
                int len = pams.Length;

                if (len == 2)
                {
                    int[] res = new int[len];

                    for (int i = 0; i < len; i++)
                        res[i] = int.Parse(pams[i]);

                    return new Point(res[0], res[1]);
                }
                throw new FormatException();
            }
            else
                return def;
        }


        public override float GetPercentage(string key)
        {
            string val = this[key];
            int pos = val.IndexOf(Percentage);
            return float.Parse(val.Substring(0, val.Length - pos - 1));
        }
        public override float GetPercentage(string key, float def)
        {
            string val;
            if (TryGetValue(key, out val))
            {
                int pos = val.IndexOf(Percentage);
                return float.Parse(val.Substring(0, val.Length - pos)) * 0.01f;
            }
            else
            {
                return def;
            }
        }

        public override float[] GetPercetageArray(string key)
        {
            string[] v = this[key].Split(ValueSeprater);
            float[] res = new float[v.Length];

            for (int i = 0; i < v.Length; i++)
            {
                int pos = v[i].IndexOf(Percentage);
                res[i] = float.Parse(v[i].Substring(0, v[i].Length - pos)) * 0.01f;

            }
            return res;
        }


        public override string GetUIString(string key)
        {
            string v = this[key];
            string[] pams = v.Split(ValueSeprater);
            int len = pams.Length;

            StringBuilder sb = new StringBuilder(len);
            for (int i = 0; i < len; i++)
                sb.Append(StringTableManager.StringTable[pams[i].Trim()]);
            return sb.ToString();
        }
        public override string GetUIString(string key, string def)
        {
            string v;
            if (!this.TryGetValue(key, out v))
                v = def;

            string[] pams = v.Split(ValueSeprater);
            int len = pams.Length;

            StringBuilder sb = new StringBuilder(len);
            for (int i = 0; i < len; i++)
                sb.Append(StringTableManager.StringTable[pams[i].Trim()]);
            return sb.ToString();
        }


        #endregion
    }
}
