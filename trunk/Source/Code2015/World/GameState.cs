using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.Vfs;
using Code2015.BalanceSystem;
using Code2015.EngineEx;

namespace Code2015.World
{
    /// <summary>
    ///  随机构建场景
    /// </summary>
    class GameStateBuilder
    {
        const int MaxCities = 2;
        const int MinCities = 1;

        public SimulationRegion SLGWorld
        {
            get;
            private set;
        }

        public GameStateBuilder()
        {
            SLGWorld = new SimulationRegion();

            FileLocation fl = FileSystem.Instance.Locate("cities.xml", GameFileLocs.Config);

            GameConfiguration resCon = new GameConfiguration(fl);
            GameConfiguration.ValueCollection resVals = resCon.Values;

            ExistTable<string> cityTable = new ExistTable<string>(MaxCities);
            FastList<City> cities = new FastList<City>(MaxCities);

            // 随机选取City
            while (cities.Count < MinCities && cities.Count < MaxCities)
            {
                foreach (GameConfigurationSection sect in resVals)
                {
                    if (!cityTable.Exists(sect.Name))
                    {
                        bool flag = Randomizer.GetRandomBool();

                        //if (flag)
                        {
                            City city = new City(SLGWorld);
                            city.Parse(sect);
                            cities.Add(city);
                            cityTable.Add(sect.Name);
                        }
                        break;
                    }
                }
            }

            for (int i = 0; i < cities.Count; i++) 
            {
                SLGWorld.Add(cities[i]);
            }

        }
    }

    class GameState
    {
        SimulationRegion slgSystem;


        public GameState(GameStateBuilder srcState)
        {
            slgSystem = srcState.SLGWorld;
        }

        public void Update(GameTime time)
        {
            slgSystem.Update(time);
        }


        public SimulationRegion SLGWorld
        {
            get { return slgSystem; }
        }
    }
}
