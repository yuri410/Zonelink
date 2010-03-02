using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Collections;
using Apoc3D;
using Apoc3D.MathLib;

namespace Code2015.BalanceSystem
{
    /// <summary>
    ///  表示玩家当前所有的城市
    /// </summary>
    public class PlayerArea : IUpdatable
    {
        FastList<City> cities = new FastList<City>();

        City rootCity;
        SimulationRegion simulator;

        public PlayerArea(SimulationRegion region)
        {
            simulator = region;
        }

        public City RootCity
        {
            get { return rootCity; }
        }

        /// <summary>
        ///  计算一个城市是否可以被占据
        ///  只有离玩家最近的城市才能占领，太远的，中间隔有更近的城市的无法占领
        /// </summary>
        /// <returns></returns>
        public bool CanCapture(City city)
        {
            if (cities.Count == 0)
                return false;

            float dist = float.MaxValue;
            City minCity = null;
            for (int i = 0; i < cities.Count; i++)
            {
                if (!cities[i].IsDead)
                {
                    float cdist = new Vector2(cities[i].Longitude - city.Longitude, cities[i].Latitude - city.Latitude).Length();

                    if (cdist < dist)
                    {
                        dist = cdist;
                        minCity = city;
                    }
                }
            }

            if (minCity != null)
            {
                // 检查是否在两个城市之间还有城市
                City midCity = null;
                float minDist = float.MaxValue;

                for (int i = 0; i < simulator.CityCount; i++)
                {
                    City cc = simulator.GetCity(i);
                    float cdist = new Vector2(cc.Longitude - minCity.Longitude, cc.Latitude - minCity.Latitude).Length();

                    if (cdist < minDist)
                    {
                        minDist = cdist;
                        midCity = cc;
                    }
                }

                return midCity == null;
            }
            return false;
        }


        /// <summary>
        ///  告知玩家控制了一个新的城市 
        /// </summary>
        /// <param name="city"></param>
        public void NotifyNewCity(City city)
        {
            if (rootCity == null)
            {
                rootCity = city;
            }

            
        }

        public void Update(GameTime time)
        {

        }
    }
}
