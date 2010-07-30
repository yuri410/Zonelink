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
    //public enum FarmHouseSize 
    //{
    //    Small,
    //    Medium,
    //    Large
    //}
    public class FarmLand : SimulationObject
    {
        [SLGValue]
        const float FoodProduceSpeed = 50;
        [SLGValue]
        const float AbsorbCarbonSpeed = 40;//FoodProduceSpeed * 1.11f;

        //const float MaxAmount = 10000;
        //[SLGValueAttribute()]
        //const float INITFoodAmount = 100000;
        //[SLGValue]
        //const float ABSORBCarbonSpeed = 1000;
        //[SLGValue]
        //const float SOURCEProduceSpeed = 500;

        //FastList<PlantSpecies> foodPlants;

        City parent;
        public FarmLand(SimulationWorld region, City parent)
            : base(region)
        {
            //foodPlants = new FastList<PlantSpecies>();
            this.parent = parent;
        }
      
        //public float AbsorbCarbonSpeed
        //{
        //    get;
        //    private set;
        //}

        //public int BaseHeight
        //{
        //    get;
        //    private set;
        //}

        //public int BaseWidth
        //{
        //    get;
        //    private set;
        //}

        //public FarmHouseSize HouseSize
        //{
        //    get;
        //    private set;
        //}
        //public bool Add(PlantSpecies foodplant)
        //{
        //    foodPlants.Add(foodplant);

        //    return true;
        //}

        //public void Remove(PlantSpecies foodplant)
        //{
        //    foodPlants.Remove(foodplant);
        //}
        //public override void Parse(ConfigurationSection sect)
        //{
        //    base.Parse(sect);

        //    string fund = sect["BasePos"];

        //    string[] v = fund.Split('X');

        //    if (v.Length == 2)
        //    {
        //        BaseHeight = int.Parse(v[0]);
        //        BaseWidth = int.Parse(v[1]);
        //    }
        //    else 
        //    {
        //        BaseHeight = 1;
        //        BaseWidth = 1;
        //    }

        //    HouseSize = (FarmHouseSize)Enum.Parse(typeof(FarmHouseSize), sect.GetString("Size", string.Empty));
        //}
        
        public override void Update(GameTime time)
        {
            float hours = (float)time.ElapsedGameTime.TotalHours;

            CarbonProduceSpeed = -AbsorbCarbonSpeed;

            if (parent != null)
            {
                parent.LocalFood.Commit(FoodProduceSpeed * hours);
            }
        }

        public override bool Changed
        {
            get { return false; }
        }
        public override string StateName
        {
            get { return string.Empty; }
        }
        public override void Deserialize(StateDataBuffer data)
        {
            throw new NotSupportedException();
        }
        public override void Serialize(StateDataBuffer data)
        {
            throw new NotSupportedException();
        }

    }
}
