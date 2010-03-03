using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.Vfs;
using Code2015.BalanceSystem;
using Code2015.EngineEx;
using Code2015.Logic;

namespace Code2015.World
{
    /// <summary>
    ///  随机构建场景
    /// </summary>
    class GameStateBuilder
    {
        const int MaxCities = 100;
        const int MinCities = 85;

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

    /// <summary>
    ///  表示游戏逻辑状态。
    /// </summary>
    class GameState
    {
        SimulationRegion slgSystem;
       
        PlayerArea[] localPlayerArea;

        public GameState(GameStateBuilder srcState, Player[] localPlayer)
        {
            slgSystem = srcState.SLGWorld;

            localPlayerArea = new PlayerArea[localPlayer.Length];

            System.Diagnostics.Debug.Assert(localPlayer.Length == 1, "测试阶段仅支持一个本地玩家");
            for (int i = 0; i < localPlayerArea.Length; i++)
            {
                localPlayerArea[i] = new PlayerArea(slgSystem, localPlayer[i]);

                localPlayer[i].SetArea(localPlayerArea[i]);

                // 测试
                slgSystem.GetCity(0).ChangeOwner(localPlayer[i]);
            }
        }

        int test;
        public void Update(GameTime time)
        {
            /////// 接受playerOperation，由127.0.0.1

            if (test++ == 1000)
            {
                slgSystem.GetCity(1).ChangeOwner(localPlayerArea[0].Owner);
            }
            //slgSystem.Update(time);
        }


        public SimulationRegion SLGWorld
        {
            get { return slgSystem; }
        }
    }
}
