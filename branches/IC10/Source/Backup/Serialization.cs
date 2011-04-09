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
using System.IO;
using System.Xml.Serialization;

namespace Apoc3D.Ide
{
    public static class Serialization
    {
        public static T XmlDeserialize<T>(string file) where T : class
        {
            StreamReader sr = new StreamReader(file, Encoding.Unicode);

            T obj = (T)(new XmlSerializer(typeof(T)).Deserialize(sr));

            sr.Close();
            return obj;
        }

        public static void XmlSerialize<T>(T obj, string file) where T : class
        {
            FileStream fs = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
            fs.SetLength(0);
            StreamWriter sw = new StreamWriter(fs, Encoding.Unicode);

            new XmlSerializer(typeof(T)).Serialize(sw, obj);

            sw.Close();
        }
    }
}
