using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Collections;
using Code2015.BalanceSystem;
using Code2015.World;
using Apoc3D.MathLib;

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

        }

        
        GameState gameLogic;

        Dictionary<City, CityData> cityDataTable;

        public AIDecisionHelper(GameState state)
        {
            this.gameLogic = state;
            
            float r = CityGrade.GetGatherRadius(UrbanSize.Large);

            SimulationWorld world = state.SLGWorld;

            cityDataTable = new Dictionary<City, CityData>(world.CityCount);

            for (int i = 0; i < world.CityCount; i++)
            {
                City cc = world.GetCity(i);
                Vector2 myPos = new Vector2(cc.Latitude, cc.Longitude);

                CityData data = new CityData();
                data.city = cc;

                for (int j = 0; j < world.ResourceCount; j++)
                {
                    NaturalResource res = world.GetResource(j);

                    Vector2 pos = new Vector2(res.Latitude, res.Longitude);
                    float dist = Vector2.Distance(pos, myPos);
                    if (dist < r)
                    {
                        switch (res.Type)
                        {
                            case NaturalResourceType.Food:
                                data.FoodCount++;
                                break;
                            case NaturalResourceType.Petro:
                                data.HRCount++;
                                break;
                            case NaturalResourceType.Wood:
                                data.LRCount++;
                                break;
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
