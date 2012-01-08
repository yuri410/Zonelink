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
using Apoc3D.MathLib;
using Code2015.World;

namespace Code2015.Logic
{
    public enum PlayerType
    {
        LocalAI,
        LocalHuman,
        Remote
    }

    class Player
    {
        public int ID
        {
            get;
            private set;
        }

        public int ExchangeCount
        {
            get;
            set;
        }


        public Player(string name,  int id)
        {
            this.Name = name;
            this.Type = PlayerType.LocalHuman;
            this.ID = id;
        }

        public string Name
        {
            get;
            set;

        }

        public PlayerType Type
        {
            get;
            protected set;
        }

        public ColorValue SideColor
        {
            get;
            set;
        }

        /// <summary>
        ///  表示玩家所控制的所有城市
        /// </summary>
        public PlayerArea Area
        {
            get;
            private set;
        }

        public virtual void SetParent(GameState state) { }
        public void SetArea(PlayerArea area)
        {
            if (Area == null) 
            {
                Area = area;
            }
        }

        public bool Win
        {
            get;
            private set;
        }


        public virtual void Update(GameTime time)
        {
            //if (Goal != null)
            //{
            //Goal.Check(this);


            //}
            if (Area != null)
            {
                Area.Update(time);
                Win |= Area.CityCount >= 23;
            }
        }
    }
}
