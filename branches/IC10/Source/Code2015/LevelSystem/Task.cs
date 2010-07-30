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
using XF = Microsoft.Xna.Framework;

namespace Code2015.LevelSystem
{
    class TaskManager
    {

    }

    enum TaskType
    {
        FloodDisaster

    }

    class Task
    {
        Dictionary<XF.PlayerIndex, float> points;

        public Task()
        {
            points = new Dictionary<XF.PlayerIndex, float>(5);

            points.Add(XF.PlayerIndex.One, 0);
            points.Add(XF.PlayerIndex.Two, 0);
            points.Add(XF.PlayerIndex.Three, 0);
            points.Add(XF.PlayerIndex.Four, 0);

        }

        public float GetContribution(XF.PlayerIndex player)
        {
            return points[player];
        }

        public void Contribute(XF.PlayerIndex player, float score)
        {
            points[player] += score;
        }




        public string Name
        {
            get;
            private set;
        }

        public TaskType Type
        {
            get;
            private set;
        }

    }
}
