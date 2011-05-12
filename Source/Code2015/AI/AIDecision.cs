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
using Code2015.Logic;
using Code2015.World;

namespace Code2015.AI
{
    class AIDecision
    {
        const int AIHelpMiniumBallCount = 10;
        const int AIAttackMiniumBallCount = 5;

        const int AIMaxCities = 10;
        AIPlayer player;
        PlayerArea area;
        AIDecisionHelper helper;
        float decisionTime;

        List<City> cityBuffer = new List<City>();

        BattleField world;


        [SLGValue]
        const float AIDecisionDelay = 5;
        [SLGValue]
        const float DecisionRandom = 4;

        public AIDecision(BattleField world, AIPlayer player)
        {
            this.world = world;
            this.player = player;
            this.area = player.Area;


            helper = new AIDecisionHelper(world);
        }

        void Attack()
        {
            float maxAttack = 0;

            City bestAttackCity = null;
            City bestAttackCityParent = null;


            for (int i = 0; i < area.CityCount; i++)
            {
                City cc = area.GetCity(i);

                for (int j = 0; j < cc.LinkableCityCount; j++)
                {
                    City cc2 = cc.GetLinkableCity(j);
                    if (cc2.Owner != player)
                    {
                        float m = helper.GetCityMark(cc2, 2.5f, 1, -0.005f) + cc.NearbyOwnedBallCount * 0.5f;
                        if (m > maxAttack)
                        {
                            maxAttack = m;
                            bestAttackCity = cc2;
                            bestAttackCityParent = cc;
                        }
                    }
                }
            }

            if (bestAttackCity != null && bestAttackCityParent.CanHandleCommand())
            {
                int abc = bestAttackCityParent.GetOwnedAttackBallCount();
                if (abc >= AIAttackMiniumBallCount && abc >= bestAttackCity.GetOwnedAttackBallCount())
                {
                    bestAttackCityParent.Throw(bestAttackCity);
                }
            }
        }

        int GetCityDanger(City a)
        {
            int result = 0;
            for (int j = 0; j < a.LinkableCityCount; j++)
            {
                City cc = a.GetLinkableCity(j);
                if (cc.Owner == null)
                {
                    result++;

                }
                else if (cc.Owner != player)
                {
                    result += 5;
                }
            }
            return result;
        }
        public void Update(GameTime time)
        {
            if (area.CityCount == 0)
                return;

            if (decisionTime < 0)
            {
                //if (Randomizer.GetRandomSingle() < 0.2f)
                //{
                    if (area.CityCount < AIMaxCities)
                    {
                        Attack();
                    }
                   
                //}
                //else 
                //{

                //    City bestHelpCity = null;
                //    City bestHelpCitySource = null;
                //    float score = 0;

                //    for (int i = 0; i < area.CityCount; i++)
                //    {
                //        City a = area.GetCity(i);
                //        int ad = GetCityDanger(a);

                //        if (ad < 2)
                //        {
                //            for (int j = i + 1; j < area.CityCount; j++)
                //            {
                //                City b = area.GetCity(j);
                //                int bd = GetCityDanger(b);
                //                if (bd > 10)
                //                {
                //                    float s = Vector3.Distance(a.Position, b.Position);
                //                    s = 7000 - s;
                //                    s += b.NearbyOwnedBallCount * 100;
                //                    if (s > score)
                //                    {
                //                        bestHelpCity = a;
                //                        bestHelpCitySource = b;
                //                    }
                //                }

                //            }
                //        }
                //    }

                //    if (bestHelpCity != null && bestHelpCitySource.GetOwnedAttackBallCount() > AIHelpMiniumBallCount)
                //    {
                //        if (!bestHelpCitySource.Throw(bestHelpCity))
                //        {
                //            Attack();
                //        }
                //    }
                //    else
                //    {
                //        Attack();
                //    }
                //}
                    decisionTime = AIDecisionDelay + Randomizer.GetRandomSingle() * DecisionRandom;
            }
            else
            {
                decisionTime -= time.ElapsedGameTimeSeconds;
            }

        }
    }
}
