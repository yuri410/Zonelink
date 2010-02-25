using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;

namespace Code2015.BalanceSystem
{
    /// <summary>
    ///  表示一个区域
    /// </summary>
    public class SimulationRegion : IUpdatable
    {

        FastList<SimulateObject> simulateObject = new FastList<SimulateObject>();
        EnergyStatus energyStatus;
        SocietyStatus societyStatus;

        public EnergyStatus EnergyStatus
        {
            get { return energyStatus; }
        }

        public int Count
        {
            get { return simulateObject.Count; }
        }
        public SimulateObject this[int i]
        {
            get { return simulateObject[i]; }
        }

        public void Add(SimulateObject obj)
        {
            simulateObject.Add(obj);
        }

       
        public void Remove(SimulateObject obj)
        {
            simulateObject.Remove(obj);
        }
       
        public SimulationRegion()
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
