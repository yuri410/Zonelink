using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Vfs;
using Code2015.BalanceSystem;
using Code2015.EngineEx;
using Apoc3D.Collections;
using Apoc3D.Graphics;

namespace Code2015.World
{
    /// <summary>
    ///  随机构建场景
    /// </summary>
    class GameStateBuilder
    {
        const int MaxCities = 100;
        const int MinCities = 80;

        public SimulationRegion SLGWorld
        {
            get;
            private set;
        }

        public GameStateBuilder()
        {
            SLGWorld = new SimulationRegion();

            FileLocation fl = FileSystem.Instance.Locate("resources.xml", GameFileLocs.Config);
            
            GameConfiguration resCon = new GameConfiguration(fl);
            GameConfiguration.ValueCollection resVals = resCon.Values;

            ExistTable<string> cityTable = new ExistTable<string>(MaxCities);
            FastList<City> cities = new FastList<City>(MaxCities);

            while (cities.Count < MinCities && cities.Count < MaxCities)
            {
                foreach (GameConfigurationSection sect in resVals)
                {
                    if (!cityTable.Exists(sect.Name))
                    {
                        bool flag = Randomizer.GetRandomBool();

                        if (flag)
                        {
                            City city = new City(SLGWorld.EnergyStatus);
                            city.Parse(sect);
                            cities.Add(city);
                            cityTable.Add(sect.Name);
                        }
                    }
                }
            }


            fl = FileSystem.Instance.Locate("cities.xml", GameFileLocs.Config);


        }
    }

    class GameState
    {
        SimulationRegion slgSystem;

        CityStyleTable cityStyles;

        public GameState(RenderSystem rs, GameStateBuilder srcState)
        {
            slgSystem = srcState.SLGWorld;
            cityStyles = new CityStyleTable(rs);
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
