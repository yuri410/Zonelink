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
        const int MaxCities = 120;
        //const int MinCities = 85;

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

            FastList<NaturalResource> resources = new FastList<NaturalResource>(MaxCities);

            foreach (GameConfigurationSection sect in resVals) 
            {
                string type = sect.GetString("Type", string.Empty);
                switch (type) 
                {
                    case "Wood":
                        Forest forest = new Forest(SLGWorld);
                        forest.Parse(sect);
                        resources.Add(forest);
                        break;
                }
                break;
            }
            for (int i = 0; i < resources.Count; i++)
            {
                SLGWorld.Add(resources[i]);
            }


            fl = FileSystem.Instance.Locate("cities.xml", GameFileLocs.Config);

            resCon = new GameConfiguration(fl);
            resVals = resCon.Values;

            FastList<City> cities = new FastList<City>(MaxCities);

            foreach (GameConfigurationSection sect in resVals)
            {
                City city = new City(SLGWorld);
                city.Parse(sect);
                cities.Add(city);
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


        GameTime newTime;
        PlayerArea[] localPlayerArea;


        public Player LocalHumanPlayer
        {
            get;
            private set;
        }

        public GameState(GameStateBuilder srcState, Player[] localPlayer)
        {
            slgSystem = srcState.SLGWorld;

            localPlayerArea = new PlayerArea[localPlayer.Length];

            System.Diagnostics.Debug.Assert(localPlayer.Length == 1, "测试阶段仅支持一个本地玩家");
            for (int i = 0; i < localPlayerArea.Length; i++)
            {
                localPlayerArea[i] = new PlayerArea(slgSystem, localPlayer[i]);

                if (localPlayer[i].Type == PlayerType.LocalHuman)
                    LocalHumanPlayer = localPlayer[i];

                localPlayer[i].SetArea(localPlayerArea[i]);

                // 测试
                slgSystem.GetCity(0).ChangeOwner(localPlayer[i]);
            }

            newTime = new GameTime();
        }

        //int test;
        public void Update(GameTime time)
        {
            /////// 接受playerOperation，由127.0.0.1
            newTime.SetElapsedGameTime(TimeSpan.FromSeconds(time.ElapsedGameTimeSeconds * 3600));
            newTime.SetElapsedRealTime(time.ElapsedRealTime * 3600);
            newTime.SetFramesPerSecond(time.FramesPerSecond);
            newTime.SetIsRunningSlowly(time.IsRunningSlowly);
            newTime.SetTotalGameTime(TimeSpan.FromSeconds(time.TotalGameTime.TotalSeconds * 3600));
            newTime.SetTotalRealTime(time.TotalRealTime * 3600);

            //if (test++ == 1000)
            //{
            //    slgSystem.GetCity(1).ChangeOwner(localPlayerArea[0].Owner);
            //}
            slgSystem.Update(newTime);
        }


        public SimulationRegion SLGWorld
        {
            get { return slgSystem; }
        }
    }
}
