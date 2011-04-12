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
http://www.gnu.org/copyleft/gpl.txt.

-----------------------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.Logic;

namespace Code2015.World
{
    struct ScoreEntry
    {
        public Player Player;

        public float Total;
        public float Development;
        //public float CO2;
    }

    ///// <summary>
    /////  随机构建场景
    ///// </summary>
    //class GameStateBuilder
    //{
    //    const int MaxCities = 120;
        


    //    public BattleField Field
    //    {
    //        get;
    //        private set;
    //    }

    //    public GameStateBuilder()
    //    {
    //        Field = new BattleField();


    //        FileLocation fl = FileSystem.Instance.Locate("resources.xml", GameFileLocs.Config);

    //        GameConfiguration resCon = new GameConfiguration(fl);
    //        GameConfiguration.ValueCollection resVals = resCon.Values;

    //        FastList<NaturalResource> resources = new FastList<NaturalResource>(MaxCities);

    //        foreach (GameConfigurationSection sect in resVals)
    //        {
    //            string type = sect.GetString("Type", string.Empty).ToLowerInvariant();
               
    //            if (type == "wood")
    //            {
    //                ForestObject forest = new ForestObject();
    //                forest.Parse(sect);
    //                resources.Add(forest);

    //            }
    //            else if (type == "petro")
    //            {
    //                OilFieldObject fld = new OilFieldObject();
    //                fld.Parse(sect);
    //                resources.Add(fld);
    //            }
    //        }
    //        for (int i = 0; i < resources.Count; i++)
    //        {
    //            Field.Add(resources[i]);
    //        }
            

    //        fl = FileSystem.Instance.Locate("cities.xml", GameFileLocs.Config);

    //        resCon = new GameConfiguration(fl);
    //        resVals = resCon.Values;

    //        FastList<City> cities = new FastList<City>(MaxCities);
    //        Dictionary<string, City> resolveTable = new Dictionary<string, City>(MaxCities);

    //        foreach (GameConfigurationSection sect in resVals)
    //        {
    //            City city = new City(Field, null);
    //            city.Parse(sect);
    //            cities.Add(city);

    //            resolveTable.Add(sect.Name, city);
    //        }


    //        for (int i = 0; i < cities.Count; i++)
    //        {
    //            cities[i].ResolveCities(resolveTable);
    //            Field.Add(cities[i]);
    //        }



    //    }
    //}

    /// <summary>
    ///  表示游戏逻辑状态。
    /// </summary>
    class GameState
    {
        BattleField battleField;

        PlayerArea[] localPlayerArea;
        Player[] localPlayers;

        public Player LocalHumanPlayer
        {
            get;
            private set;
        }
        public bool CheckGameOver()
        {
            for (int i = 0; i < localPlayers.Length; i++)
            {
                if (localPlayers[i].Win) { return true; }
            }
            return LocalHumanPlayer.Area.CityCount == 0;
        }
        public void InitialStandards()
        {
            for (int i = 0; i < localPlayerArea.Length; i++)
            {
                //CityPluginFactory fac = new CityPluginFactory();


                //CityPlugin woodFac = fac.MakeWoodFactory();
                //woodFac.Upgrade(0.4f);

                //localPlayerArea[i].RootCity.Add(woodFac);
            }
        }

        public GameState(Player[] localPlayer)
        {
            this.battleField = new BattleField();// srcState.Field;
            //PluginFactory = srcState.PluginFactory;

            this.localPlayerArea = new PlayerArea[localPlayer.Length];
            this.localPlayers = localPlayer;

            Dictionary<int, FastList<City>> startAreas = new Dictionary<int, FastList<City>>();
            for (int i = 0; i < battleField.CityCount; i++)
            {
                City cc = battleField.Cities[i];
                if (cc.StartUp != -1)
                {
                    FastList<City> list;
                    if (!startAreas.TryGetValue(cc.StartUp, out list))
                    {
                        list = new FastList<City>();
                        startAreas.Add(cc.StartUp, list);
                    }

                    list.Add(cc);
                }
            }

            #region 确定玩家起始城市
            ExistTable<int> startAreaTable = new ExistTable<int>(startAreas.Count);

            for (int i = 0; i < localPlayerArea.Length; i++)
            {
                localPlayerArea[i] = new PlayerArea(battleField, localPlayer[i]);

                if (localPlayer[i].Type == PlayerType.LocalHuman)
                    LocalHumanPlayer = localPlayer[i];

                localPlayer[i].SetArea(localPlayerArea[i]);
                localPlayer[i].SetParent(this);



                bool finished = false;
                while (!finished)
                {
                    int area = Randomizer.GetRandomInt(startAreas.Count) + 1;

                    if (!startAreaTable.Exists(area))
                    {
                        startAreaTable.Add(area);
                        FastList<City> list = startAreas[area];

                        int cidx = Randomizer.GetRandomInt(list.Count);
                        list[cidx].IsHomeCity = true;
                        list[cidx].ChangeOwner(localPlayer[i]);
                        finished = true;
                    }
                }
            }
            #endregion

            //newTime = new GameTime();

            //remainingTime = TotalTime;
        }


        public void Update(GameTime time)
        {
            for (int i = 0; i < localPlayers.Length; i++)
            {
                localPlayers[i].Update(time);
            }

            //remainingTime -= time.ElapsedGameTimeSeconds;

            ///////// 接受playerOperation，由127.0.0.1
            //newTime.SetElapsedGameTime(TimeSpan.FromSeconds(time.ElapsedGameTimeSeconds * 3600));
            //newTime.SetElapsedRealTime(time.ElapsedRealTime * 3600);
            //newTime.SetFramesPerSecond(time.FramesPerSecond);
            //newTime.SetIsRunningSlowly(time.IsRunningSlowly);
            //newTime.SetTotalGameTime(TimeSpan.FromSeconds(time.TotalGameTime.TotalSeconds * 3600));
            //newTime.SetTotalRealTime(time.TotalRealTime * 3600);


            battleField.Update(time);
        }

        int Comparision(ScoreEntry a, ScoreEntry b)
        {
            return -a.Total.CompareTo(b.Total);
        }

        public int LocalPlayerCount
        {
            get { return localPlayers.Length; }
        }
        public Player GetLocalPlayer(int i)
        {
            return localPlayers[i];
        }
        public ScoreEntry[] GetScores()
        {
            //Dictionary<Player, float> co2s = SLGWorld.EnergyStatus.GetCarbonWeights();

            ScoreEntry[] result = new ScoreEntry[localPlayers.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i].Player = localPlayers[i];
                result[i].Development = localPlayerArea[i].GetTotalDevelopment();

                //co2s.TryGetValue(localPlayers[i], out result[i].CO2);

                result[i].Total += result[i].Development;// -0.2f * result[i].CO2;
            }

            Array.Sort<ScoreEntry>(result, Comparision);
            return result;
        }

        public BattleField Field
        {
            get { return battleField; }
        }
    }
}
