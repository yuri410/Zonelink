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
using Code2015.Network;

namespace Code2015.BalanceSystem
{
    /// <summary>
    ///  表示自然资源的类型
    /// </summary>
    public enum NaturalResourceType
    {
        None,
        Wood,
        Petro
    }


    public abstract class NaturalResource : SimulationObject
    {
        int countDown;
        bool isExploiting;

        protected NaturalResource(SimulationWorld region, NaturalResourceType type)
            : base(region)
        {
            Type = type;
        }
        

        /// <summary>
        ///  获取自然资源的类型
        /// </summary>
        public NaturalResourceType Type
        {
            get;
            private set;
        }

        public bool IsLow
        {
            get { return CurrentAmount < 1000; }
        }


        /// <summary>
        ///  获取当前自然资源
        /// </summary>
        public float CurrentAmount
        {
            get;
            protected set;
        }

        public object OutputTarget
        {
            get;
            set;
        }

        /// <summary>
        ///  开采一定数量的自然资源
        /// </summary>
        /// <param name="amount">申请值</param>
        /// <returns>实际得到的</returns>
        public float Exploit(float amount)
        {
            if (amount < CurrentAmount)
            {
                CurrentAmount -= amount;
                return amount;
            }

            isExploiting = true;

            float r = CurrentAmount;
            CurrentAmount = 0;
            return r;

        }

        public override bool Changed
        {
            get { return isExploiting; }
        }
       

        public override void Update(GameTime time)
        {
            if (countDown-- < 0)
            {
                isExploiting = false;
                countDown = 300;
            }
        }
        public override void Deserialize(StateDataBuffer data)
        {
            CurrentAmount = data.Reader.ReadSingle();
        }
        public override void Serialize(StateDataBuffer data)
        {
            data.Writer.Write(CurrentAmount);
            data.EndWrite();
        }
        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);

            CurrentAmount = sect.GetSingle("Amount", 0);
        }


    }
}
