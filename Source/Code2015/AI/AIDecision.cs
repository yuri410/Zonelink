using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.MathLib;
using Code2015.BalanceSystem;
using Code2015.Logic;

namespace Code2015.AI
{
    class AIDecision
    {
        AIPlayer player;
        PlayerArea area;

        float decisionTime;

        List<City> cityBuffer = new List<City>();

        SimulationWorld world;
        CityPluginFactory pluginFactory;

        [SLGValue]
        const float AIDecisionDelay = 8;
        [SLGValue]
        const float DecisionRandom = 4;

        public AIDecision(SimulationWorld world, AIPlayer player)
        {
            this.world = world;
            this.player = player;
            this.area = player.Area;

            pluginFactory = new CityPluginFactory();
            
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
                const float P = .25f;

                float ran = Randomizer.GetRandomSingle();

                if (ran < P)
                {
                    //for (int i = 0; i < world.CityCount; i++)
                    //{
                    //    City cc = world.GetCity(i);
                    //    if (!cc.IsCaptured && cc.Owner != player)
                    //    {
                    //        if (area.CanCapture(cc))
                    //        {
                    //            cc.Capture.SetCapture (player , );
                    //        }
                    //    }
                    //}
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
                        }
                        else
                        {
                            cc.Parent.Upgrade();
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
