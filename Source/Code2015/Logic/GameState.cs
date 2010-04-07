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
    public struct ScoreEntry
    {
        public Player Player;

        public float Total;
        public float Development;
        public float CO2;
    }

    /// <summary>
    ///  随机构建场景
    /// </summary>
    public class GameStateBuilder
    {
        const int MaxCities = 120;
        //const int MinCities = 85;

        public CityPluginFactory PluginFactory
        {
            get;
            private set;
        }

        public SimulationWorld SLGWorld
        {
            get;
            private set;
        }

        public GameStateBuilder()
        {
            SLGWorld = new SimulationWorld();

            PluginFactory = new CityPluginFactory();

            FileLocation fl = FileSystem.Instance.Locate("resources.xml", GameFileLocs.Config);

            GameConfiguration resCon = new GameConfiguration(fl);
            GameConfiguration.ValueCollection resVals = resCon.Values;

            FastList<NaturalResource> resources = new FastList<NaturalResource>(MaxCities);

            foreach (GameConfigurationSection sect in resVals)
            {
                string type = sect.GetString("Type", string.Empty).ToLowerInvariant();
                //switch (type) 
                //{
                //    case "wood":
                if (type == "wood")
                {
                    Forest forest = new Forest(SLGWorld);
                    forest.Parse(sect);
                    resources.Add(forest);

                }
                else if (type == "petro")
                {
                    OilField fld = new OilField(SLGWorld);
                    fld.Parse(sect);
                    resources.Add(fld);
                }
                else if (type == "farm")
                {
                    FarmLand frm = new FarmLand(SLGWorld);
                    frm.Parse(sect);
                    resources.Add(frm);
                }

                //}
                //break;
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
    public class GameState
    {
        [SLGValue]
        const float TotalTime = 30;
        SimulationWorld slgSystem;

        float remainingTime;
        GameTime newTime;
        PlayerArea[] localPlayerArea;
        Player[] localPlayers;

        public float RemainingTime
        {
            get { return remainingTime; }
        }
        public CityPluginFactory PluginFactory
        {
            get;
            private set;
        }

        public Player LocalHumanPlayer
        {
            get;
            private set;
        }

        public bool IsTimeUp
        {
            get { return remainingTime < 0; }
        }

        public GameState(GameStateBuilder srcState, Player[] localPlayer)
        {
            this.slgSystem = srcState.SLGWorld;
            PluginFactory = srcState.PluginFactory;

            this.localPlayerArea = new PlayerArea[localPlayer.Length];
            this.localPlayers = localPlayer;

            for (int i = 0; i < localPlayerArea.Length; i++)
            {
                localPlayerArea[i] = new PlayerArea(slgSystem, localPlayer[i]);

                if (localPlayer[i].Type == PlayerType.LocalHuman)
                    LocalHumanPlayer = localPlayer[i];

                localPlayer[i].SetArea(localPlayerArea[i]);
                localPlayer[i].SetParent(this);

                bool finished = false;
                while (!finished)
                {
                    int idx = Randomizer.GetRandomInt(slgSystem.CityCount);

                    City cc = slgSystem.GetCity(idx);
                    if (!cc.IsCaptured && cc.Size == UrbanSize.Large)
                    {
                        cc.ChangeOwner(localPlayer[i]);
                        finished = true;
                    }
                }
            }

            newTime = new GameTime();

            remainingTime = TotalTime;
        }


        public void Update(GameTime time)
        {
            for (int i = 0; i < localPlayers.Length; i++)
            {
                localPlayers[i].Update(time);
            }

            remainingTime -= time.ElapsedGameTimeSeconds;

            /////// 接受playerOperation，由127.0.0.1
            newTime.SetElapsedGameTime(TimeSpan.FromSeconds(time.ElapsedGameTimeSeconds * 3600));
            newTime.SetElapsedRealTime(time.ElapsedRealTime * 3600);
            newTime.SetFramesPerSecond(time.FramesPerSecond);
            newTime.SetIsRunningSlowly(time.IsRunningSlowly);
            newTime.SetTotalGameTime(TimeSpan.FromSeconds(time.TotalGameTime.TotalSeconds * 3600));
            newTime.SetTotalRealTime(time.TotalRealTime * 3600);


            slgSystem.Update(newTime);
        }

        int Comparision(ScoreEntry a, ScoreEntry b)
        {
            return -a.Total.CompareTo(b.Total);
        }

        public ScoreEntry[] GetScores()
        {
            Dictionary<Player, float> co2s = SLGWorld.EnergyStatus.GetCarbonWeights();

            ScoreEntry[] result = new ScoreEntry[localPlayers.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i].Player = localPlayers[i];
                result[i].Development = localPlayerArea[i].GetTotalDevelopment();

                co2s.TryGetValue(localPlayers[i], out result[i].CO2);

                result[i].Total += result[i].Development - 0.2f * result[i].CO2;
            }

            Array.Sort<ScoreEntry>(result, Comparision);
            return result;
        }

        public SimulationWorld SLGWorld
        {
            get { return slgSystem; }
        }
    }
}
