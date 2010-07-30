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
using Apoc3D.Collections;
using Apoc3D.Config;
using Code2015.Network;

namespace Code2015.BalanceSystem
{
    public class Forest : NaturalResource
    {
        /// <summary>
        ///  
        /// </summary>
        [SLGValueAttribute()]
        const float AbsorbCarbonRate = 1000;


        [SLGValue]
        const float RecoverRate = 0.015f;
        [SLGValue]
        const float RecoverBias = 20f;

        [SLGValue]
        const float TimesMaxAmount = 2;


        public float MaxAmount
        {
            get;
            private set;
        }


        public float Radius
        {
            get;
            private set;
        }


        public Forest(SimulationWorld region)
            : base(region, NaturalResourceType.Wood)
        {

        }



        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);

            Radius = sect.GetSingle("Radius");

            MaxAmount = CurrentAmount * TimesMaxAmount;
        }


        public override void Update(GameTime time)
        {
            float hours = (float)time.ElapsedGameTime.TotalHours;

            if (CurrentAmount < MaxAmount)
            {
                CurrentAmount += (CurrentAmount * RecoverRate + RecoverBias) * hours;
            }
        }


    }
}
