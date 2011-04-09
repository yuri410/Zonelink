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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Ide.Converters;
using Apoc3D;

namespace Apoc3D.Ide
{
    public class ConverterManager
    {
        static ConverterManager singleton;
        public static ConverterManager Instance
        {
            get
            {
                if (singleton == null)
                    singleton = new ConverterManager();
                return singleton; 
            }
        }


        List<ConverterBase> converters;

        private ConverterManager()
        {
            converters = new List<ConverterBase>();
        }
        public void Register(ConverterBase fac)
        {
            if (fac == null) 
            {
                throw new ArgumentNullException("fac");
            }
            converters.Add(fac);
        }
        public void Unregister(ConverterBase fac)
        {
            if (fac == null)
            {
                throw new ArgumentNullException("fac");
            }
            converters.Remove(fac);
        }

        public ConverterBase[] GetAllConverters()
        {
            return converters.ToArray();
        }
        public ConverterBase[] GetConvertersDest(string dstExt)
        {
            List<ConverterBase> res = new List<ConverterBase>(converters.Count);

            for (int i = 0; i < converters.Count; i++)
            {
                string[] dest = converters[i].DestExt;
                for (int j = 0; j < dest.Length; j++)
                {
                    if (CaseInsensitiveStringComparer.Compare(dstExt, dest[j]))
                    {
                        res.Add(converters[i]);
                        break;
                    }
                }
            }

            return res.ToArray();
        }
        public ConverterBase[] GetConvertersSrc(string srcExt)
        {
            List<ConverterBase> res = new List<ConverterBase>(converters.Count);

            for (int i = 0; i < converters.Count; i++)
            {
                string[] source = converters[i].SourceExt;
                for (int j = 0; j < source.Length; j++)
                {
                    if (CaseInsensitiveStringComparer.Compare(srcExt, source[j]))
                    {
                        res.Add(converters[i]);
                        break;
                    }
                }
            }

            return res.ToArray();
        }
        public ConverterBase[] GetConverters(string srcExt, string dstExt)
        {
            List<ConverterBase> res = new List<ConverterBase>(converters.Count);

            for (int i = 0; i < converters.Count; i++)
            {
                string[] source = converters[i].SourceExt;
                string[] dest = converters[i].DestExt;
                bool p1 = false;
                bool p2 = false;
                for (int j = 0; j < source.Length; j++)
                {
                    if (CaseInsensitiveStringComparer.Compare(srcExt, source[j]))
                    {
                        p1 = true;
                        break;
                    }
                }
                
                for (int j = 0; j < dest.Length; j++)
                {
                    if (CaseInsensitiveStringComparer.Compare(dstExt, dest[j]))
                    {
                        p2 = true;
                        break;
                    }
                }

                if (p1 & p2)
                {
                    res.Add(converters[i]);
                }
            }

            return res.ToArray();
        }
    }
}
