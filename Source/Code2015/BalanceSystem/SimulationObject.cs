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
http://www.gnu.org/copyleft/lesser.txt.

-----------------------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Config;
using Code2015.Logic;
using Code2015.Network;

namespace Code2015.BalanceSystem
{
    public abstract class SimulationObject : IConfigurable, IUpdatable, ILogicStateObject
    {


        /// <summary>
        /// 经度
        /// </summary>
        public float Longitude
        {
            get;
            protected set;
        }
        /// <summary>
        /// 纬度
        /// </summary>
        public float Latitude
        {
            get;
            protected set;
        }

        /// <summary>
        ///  碳元素的排放速度
        /// </summary>
        public float CarbonProduceSpeed
        {
            get;
            protected set;
        }

        public SimulationWorld Region
        {
            get;
            private set;
        }

        protected SimulationObject(SimulationWorld region)
        {
            Region = region;
        }

        /// <summary>
        ///  派生类先更新
        /// </summary>
        /// <param name="time"></param>
        public virtual void Update(GameTime time)
        {

        }

        #region IConfigurable 成员


        public virtual void Parse(ConfigurationSection sect)
        {
            Longitude = sect.GetSingle("Longitude");
            Latitude = sect.GetSingle("Latitude");

            stateName = sect.Name;

        }
        #endregion

        #region ILogicStateObject 成员

        public abstract void Serialize(StateDataBuffer data);
        public abstract void Deserialize(StateDataBuffer data);

        string stateName;
        public virtual string StateName
        {
            get { return stateName; }
        }


        public abstract bool Changed
        {
            get;
        }

        #endregion
    }
}
