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
using Apoc3D.MathLib;
using Code2015.BalanceSystem;
using Code2015.Logic;
using Code2015.World;

namespace Code2015.AI
{
    class AIDecision
    {
        const int AIMaxCities = 8;
        AIPlayer player;
        PlayerArea area;
        AIDecisionHelper helper;
        float decisionTime;

        List<City> cityBuffer = new List<City>();

        SimulationWorld world;
        CityPluginFactory pluginFactory;

        [SLGValue]
        const float AIDecisionDelay = 20;
        [SLGValue]
        const float DecisionRandom = 4;

        public AIDecision(SimulationWorld world, AIPlayer player)
        {
            this.world = world;
            this.player = player;
            this.area = player.Area;

            pluginFactory = new CityPluginFactory();

            helper = new AIDecisionHelper(world);
        }


        public void Update(GameTime time)
        {
            // 电脑单一决策。
            // 每隔一段时间

            // .25的概率
            // 寻找附近权值较大城市
            // 占领。

            // .75的概率
            // 选择一个城市。根据附近情况1对1建立工厂附加物。
            // 其余随机填充

            if (area.CityCount == 0)
                return;

            if (decisionTime < 0)
            {
                const float P = .2f;

                float ran = Randomizer.GetRandomSingle();

                if (ran < P && area.CityCount < AIMaxCities)
                {
                    float max = float.MinValue;
                    City bestCity = null;
                    City bestCityParent = null;
                    for (int i = 0; i < area.CityCount; i++)
                    {
                        City cc = area.GetCity(i);

                        for (int j = 0; j < cc.LinkableCityCount; j++)
                        {
                            City cc2 = cc.GetLinkableCity(j);
                            if (cc2.CanCapture(player))
                            {
                                float m = helper.GetCityMark(cc2, 1, 1, 1);
                                if (m > max)
                                {
                                    max = m;
                                    bestCity = cc2;
                                    bestCityParent = cc;
                                }
                            }
                        }
                    }

                    if (bestCity != null)
                    {
                        bestCity.SetCapture(bestCityParent);
                    }

                }
                else
                {
                    bool finished = false;
                    while (!finished)
                    {
                        int i = Randomizer.GetRandomInt(area.CityCount);

                        City cc = area.GetCity(i);
                        float r = CityGrade.GetGatherRadius(cc.Size);

                        if (cc.CanAddPlugins)
                        {
                            Vector2 myPos = new Vector2(cc.Latitude, cc.Longitude);

                            cc.LocalLR.Commit(400);
                            for (int j = 0; j < world.ResourceCount && cc.CanAddPlugins; j++)
                            {
                                NaturalResource res = world.GetResource(j);
                                if (!res.IsLow)
                                {
                                    Vector2 pos = new Vector2(res.Latitude, res.Longitude);
                                    float dist = Vector2.Distance(pos, myPos);

                                    if (dist <= r)
                                    {
                                        switch (res.Type)
                                        {
                                            case NaturalResourceType.Petro:
                                                CityPlugin plugin = pluginFactory.MakeOilRefinary();
                                                cc.Add(plugin);
                                                break;
                                            case NaturalResourceType.Wood:
                                                plugin = pluginFactory.MakeWoodFactory();
                                                cc.Add(plugin);
                                                break;
                                        }
                                    }
                                }
                            }
                            if (cc.CanAddPlugins)
                            {
                                int rem = CityGrade.GetMaxPlugins(cc.Size) - cc.PluginCount;

                                for (int j = 0; j < rem; j++)
                                {
                                    CityPlugin plugin;

                                    bool k = Randomizer.GetRandomBool();

                                    if (k)
                                    {
                                        plugin = pluginFactory.MakeHospital();
                                    }
                                    else
                                    {
                                        plugin = pluginFactory.MakeEducationAgent();
                                    }
                                    cc.Add(plugin);
                                }
                            }
                            finished = true;

                            //cc.Parent.UpgradeAI();
                            cc.Parent.UpgradeAI();
                        }
                        else
                        {
                            cc.LocalLR.Commit(200);
                            cc.Parent.UpgradeAI();
                            cc.Parent.UpgradeAI();
                            finished = true;
                        }
                    }
                }

                decisionTime = AIDecisionDelay + Randomizer.GetRandomSingle() * DecisionRandom;
            }
            else
            {
                decisionTime -= time.ElapsedGameTimeSeconds;
            }

        }
    }
}
