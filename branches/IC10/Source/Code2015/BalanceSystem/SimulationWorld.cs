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
using Code2015.Logic;

namespace Code2015.BalanceSystem
{
    /// <summary>
    ///  表示一个区域
    /// </summary>
    public class SimulationWorld : IUpdatable
    {
        FastList<SimulationObject> simulateObject = new FastList<SimulationObject>();
        FastList<City> cities = new FastList<City>();
        FastList<NaturalResource> resources = new FastList<NaturalResource>();

        //EnergyStatus energyStatus;
        //SocietyStatus societyStatus;

        //public EnergyStatus EnergyStatus
        //{
        //    get { return energyStatus; }
        //}


        public int CityCount
        {
            get { return cities.Count; }
        }
        public int ResourceCount
        {
            get { return resources.Count; }
        }

        public City GetCity(int i)
        {
            return cities[i];
        }
        public NaturalResource GetResource(int i)
        {
            return resources[i];
        }

        public NaturalResource FindResource(string stateName)
        {
            for (int i = 0; i < resources.Count; i++)
            {
                if (resources[i].StateName == stateName) 
                {
                    return resources[i];
                }
            }
            return null;
        }

        public int Count
        {
            get { return simulateObject.Count; }
        }
        public SimulationObject this[int i]
        {
            get { return simulateObject[i]; }
        }

        public void Add(SimulationObject obj)
        {
            simulateObject.Add(obj);

            City city = obj as City;
            if (city != null)
            {
                cities.Add(city);
            }

            NaturalResource res = obj as NaturalResource;
            if (res != null)
            {
                resources.Add(res);
            }
        }
       
        public void Remove(SimulationObject obj)
        {
            simulateObject.Remove(obj);

            City city = obj as City;
            if (city != null)
            {
                cities.Remove(city);
            }

            NaturalResource res = obj as NaturalResource;
            if (res != null)
            {
                resources.Remove(res);
            }
        }
       
        //public SimulationWorld()
        //{
        //    energyStatus = new EnergyStatus(this);
        //    societyStatus = new SocietyStatus(this);
        //}

        public void Update(GameTime time)
        {
            for (int i = 0; i < simulateObject.Count; i++)
            {
                simulateObject[i].Update(time);
            }

            //societyStatus.Update(time);
            //energyStatus.Update(time);


        }

    }
}
