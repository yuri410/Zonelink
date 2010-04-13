using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.MathLib;
using Code2015.BalanceSystem;
using Code2015.World;

namespace Code2015.Logic
{
    /// <summary>
    ///  表示玩家当前所有的城市
    /// </summary>
    public class PlayerArea : IUpdatable
    {
        public const float CaptureDistanceThreshold = 12;

        FastList<City> cities = new FastList<City>();

        City rootCity;
        SimulationWorld simulator;

        Player owner;

        public int CityCount
        {
            get { return cities.Count; }
        }

        public City GetCity(int i) 
        {
            return cities[i];
        }


        public float GetTotalDevelopment()
        {
            float result = 0;
            for (int i = 0; i < cities.Count; i++)
            {
                result += cities[i].Development;
            }
            return result;
        }

        public PlayerArea(SimulationWorld region, Player player)
        {
            this.simulator = region;
            this.owner = player;
        }

        public Player Owner
        {
            get { return owner; }
        }

        public City RootCity
        {
            get { return rootCity; }
        }



        /// <summary>
        ///  获取此玩家所有的城市中离目标城市最近的一个
        /// </summary>
        /// <param name="city">目标城市</param>
        /// <returns></returns>
        public City GetNearestCity(City city)
        {
            float dist = float.MaxValue;
            City minCity = null;
            for (int i = 0; i < cities.Count; i++)
            {
                if (!cities[i].IsRecovering && city != cities[i])
                {
                    float cdist = new Vector2(cities[i].Longitude - city.Longitude, cities[i].Latitude - city.Latitude).Length();

                    if (cdist < dist)
                    {
                        dist = cdist;
                        minCity = cities[i];
                    }
                }
            }
            return minCity;
        }

        

        /// <summary>
        ///  计算一个城市是否可以被占据
        ///  只有离玩家最近的城市才能占领，太远的，中间隔有更近的城市的无法占领
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public bool CanCapture(City city)
        {
            if (cities.Count == 0)
                return false;

            City minCity = GetNearestCity(city);

            if (minCity != null)
            {
               
                // 检查是否在两个城市之间还有城市
                City midCity = null;
                float minDist = float.MaxValue;

                for (int i = 0; i < simulator.CityCount; i++)
                {
                    City cc = simulator.GetCity(i);

                    bool flag1 = !object.ReferenceEquals(cc, minCity);
                    bool flag2 = !object.ReferenceEquals(cc.Owner, minCity.Owner);
                    if (flag1 && flag2)
                    {
                        float cdist = new Vector2(cc.Longitude - minCity.Longitude, cc.Latitude - minCity.Latitude).Length();

                        if (cdist < minDist)
                        {
                            minDist = cdist;
                            midCity = cc;
                        }
                    }
                }

                Vector2 d = new Vector2(city.Longitude - minCity.Longitude, city.Latitude - minCity.Latitude);


                return (object.ReferenceEquals(midCity, null) || object.ReferenceEquals(city, midCity)) || d.Length() < CaptureDistanceThreshold;
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


            if (cities.Count > 0)
            {
                // 加入城市网络
                City minCty = GetNearestCity(city);
                if (minCty != null)
                {
                    city.AddNearbyCity(minCty);
                    minCty.AddNearbyCity(city);
                }
            }
            cities.Add(city);
        }
        public void NotifyLostCity(City city) 
        {
            if (object.ReferenceEquals(rootCity, city)) 
            {
                rootCity = null;
            }

            cities.Remove(city);
        }

        public void Update(GameTime time)
        {

        }
    }
}
