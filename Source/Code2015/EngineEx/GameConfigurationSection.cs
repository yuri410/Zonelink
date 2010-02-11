using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Config;
using Apoc3D.MathLib;

namespace Code2015.EngineEx
{
    class GameConfigurationSection : ConfigurationSection
    {
        public GameConfigurationSection(string name)
            : base(name)
        {

        }

        public override ConfigurationSection GetSubSection(string key)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetSubSection(string key, out ConfigurationSection sect)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetPaths(string key, out string[] res)
        {
            throw new NotImplementedException();
        }

        public override string[] GetPaths(string key)
        {
            throw new NotImplementedException();
        }

        public override void GetRectangle(string key, out Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetColorRGBA(string key, out ColorValue clr)
        {
            throw new NotImplementedException();
        }

        public override ColorValue GetColorRGBA(string key, ColorValue def)
        {
            throw new NotImplementedException();
        }

        public override ColorValue GetColorRGBA(string key)
        {
            throw new NotImplementedException();
        }

        public override int GetColorRGBInt(string key)
        {
            throw new NotImplementedException();
        }

        public override int GetColorRGBInt(string key, int def)
        {
            throw new NotImplementedException();
        }

        public override bool TryGetBool(string key, out bool res)
        {
            throw new NotImplementedException();
        }

        public override bool GetBool(string key)
        {
            throw new NotImplementedException();
        }

        public override bool GetBool(string key, bool def)
        {
            throw new NotImplementedException();
        }

        public override float GetSingle(string key)
        {
            throw new NotImplementedException();
        }

        public override float GetSingle(string key, float def)
        {
            throw new NotImplementedException();
        }

        public override float[] GetSingleArray(string key)
        {
            throw new NotImplementedException();
        }

        public override float[] GetSingleArray(string key, float[] def)
        {
            throw new NotImplementedException();
        }

        public override string GetString(string key, string def)
        {
            throw new NotImplementedException();
        }

        public override string[] GetStringArray(string key)
        {
            throw new NotImplementedException();
        }

        public override string[] GetStringArray(string key, string[] def)
        {
            throw new NotImplementedException();
        }

        public override int GetInt(string key)
        {
            throw new NotImplementedException();
        }

        public override int GetInt(string key, int def)
        {
            throw new NotImplementedException();
        }

        public override int[] GetIntArray(string key)
        {
            throw new NotImplementedException();
        }

        public override int[] GetIntArray(string key, int[] def)
        {
            throw new NotImplementedException();
        }

        public override Size GetSize(string key)
        {
            throw new NotImplementedException();
        }

        public override Size GetSize(string key, Size def)
        {
            throw new NotImplementedException();
        }

        public override Point GetPoint(string key)
        {
            throw new NotImplementedException();
        }

        public override Point GetPoint(string key, Point def)
        {
            throw new NotImplementedException();
        }

        public override float GetPercentage(string key)
        {
            throw new NotImplementedException();
        }

        public override float GetPercentage(string key, float def)
        {
            throw new NotImplementedException();
        }

        public override float[] GetPercetageArray(string key)
        {
            throw new NotImplementedException();
        }

        public override string GetUIString(string key)
        {
            throw new NotImplementedException();
        }

        public override string GetUIString(string key, string def)
        {
            throw new NotImplementedException();
        }
    }
}
