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

        EnergyStatus energyStatus;
        SocietyStatus societyStatus;

        public EnergyStatus EnergyStatus
        {
            get { return energyStatus; }
        }


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
       
        public SimulationWorld()
        {
            energyStatus = new EnergyStatus(this);
            societyStatus = new SocietyStatus(this);
        }

        public void Update(GameTime time)
        {
            for (int i = 0; i < simulateObject.Count; i++)
            {
                simulateObject[i].Update(time);
            }

            societyStatus.Update(time);
            energyStatus.Update(time);


        }

    }
}
