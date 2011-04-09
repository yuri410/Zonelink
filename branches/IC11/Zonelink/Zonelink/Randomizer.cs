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

namespace Apoc3D
{
    public static class Randomizer
    {
        static Random random = new Random();

        public static bool GetRandomBool()
        {
            return GetRandomInt(int.MaxValue) % 2 == 0;
        }
        public static int GetRandomInt(int max)
        {
            return random.Next(max);
        }

        public static float GetRandomSingle()
        {            
            return (float)random.NextDouble();
        }

        public static float NextFloat() { return (float)random.NextDouble(); }

        public unsafe static int Random(float* p, int count)
        {
            float total = 0;
            for (int i = 0; i < count; i++)
            {
                total += p[i];
            }

            float rnd = GetRandomSingle() * total;

            float cmp = 0;
            for (int i = 0; i < count; i++)
            {
                cmp += p[i];
                if (rnd < cmp)
                {
                    return i;
                }
            }
            return 0;
        }

        public static int Random(float[] p)
        {
            float total = 0;
            for (int i = 0; i < p.Length; i++)
            {
                total += p[i];
            }

            float rnd = GetRandomSingle() * total;

            float cmp = 0;
            for (int i = 0; i < p.Length; i++)
            {
                cmp += p[i];
                if (rnd < cmp)
                {
                    return i;
                }                
            }
            return 0;
        }
    }
}
