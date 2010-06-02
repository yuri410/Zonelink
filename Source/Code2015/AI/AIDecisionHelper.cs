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
using Apoc3D.Collections;
using Apoc3D.MathLib;
using Code2015.BalanceSystem;
using Code2015.Logic;
using Code2015.World;

namespace Code2015.AI
{
    class AIDecisionHelper
    {
        struct CityData
        {
            public City city;

            public float HRCount;
            public float LRCount;
            public float FoodCount;

            public FastList<City> NearbyCity;
        }


        Dictionary<City, CityData> cityDataTable;

        public AIDecisionHelper(SimulationWorld world)
        {
            float r = CityGrade.GetGatherRadius(UrbanSize.Large);

            cityDataTable = new Dictionary<City, CityData>(world.CityCount);

            for (int i = 0; i < world.CityCount; i++)
            {
                City cc = world.GetCity(i);
                Vector2 myPos = new Vector2(cc.Latitude, cc.Longitude);

                CityData data = new CityData();
                data.city = cc;
                data.FoodCount = cc.FarmLandCount;

                for (int j = 0; j < world.ResourceCount; j++)
                {
                    NaturalResource res = world.GetResource(j);

                    Vector2 pos = new Vector2(res.Latitude, res.Longitude);
                    float dist = Vector2.Distance(pos, myPos);
                    if (dist < r)
                    {
                        switch (res.Type)
                        {
                            case NaturalResourceType.Petro:
                                data.HRCount++;
                                break;
                            case NaturalResourceType.Wood:
                                data.LRCount++;
                                break;
                        }
                    }
                }

                data.NearbyCity = new FastList<City>();
                for (int j = 0; j < world.CityCount; j++)
                {
                    City cc2 = world.GetCity(j);
                    if (cc != cc2)
                    {
                        Vector2 pos = new Vector2(cc2.Latitude, cc2.Longitude);
                        float dist = Vector2.Distance(pos, myPos);

                        if (dist < PlayerArea.CaptureDistanceThreshold)
                        {
                            data.NearbyCity.Add(cc2);
                        }
                    }
                }
                cityDataTable.Add(cc, data);
            }
        }

        public float GetCityMark(City cc, float a, float b, float c) 
        {
            if (cc.IsCaptured)
                return float.MinValue;
            CityData data;
            if (cityDataTable.TryGetValue(cc, out data))
            {
                return a * data.HRCount + b * data.LRCount + c * data.FoodCount;
            }
            return float.MinValue;
        }
    }
}
