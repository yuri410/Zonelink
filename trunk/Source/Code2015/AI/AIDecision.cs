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

                            cc.Parent.UpgradeAI();
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
