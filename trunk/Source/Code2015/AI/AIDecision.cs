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
        class CityWeights
        {
            public float OilBallRequirements;
            public float BulletRequirements;
            public float VirusRequirements;

            public float BaseBias;

            public CityWeights()
            {
                BaseBias = Randomizer.GetRandomSingle() * 0.01f;
            }
        }

        /** AI Decision making is based on a weighted system
         *  
         *  The sending of balls with AI's own bound is based on:
         *   OilBall = Type, Adjacency
         *   Bullet = Type, Adjacency, Limit, Level
         *   Virus = Health, Type
         *   
         *  Details:
         *   OilBall: CityDangerIndex * (1+ Type is neutral city)
         *   Bullet: (Type is not Violence * 0.5 + 0.5) * InvCityDangerIndex * (Level!=10) * (Type is not Violence) ? (5-NumOfBullet) : 1 
         *   Virus:  if all cities are full health then back to Producer, else 1-HealthRate * (Type is not VCity)
         */
        const int AIMoveMiniumAttackBallCount = 10;
        const int AIMoveMiniumBuffBallCount = 2;

        const int AIMaxCityCallCount = 50;
        const int AIAttackMiniumBallCount = 5;

        const int AIMaxCities = 25;
        AIPlayer player;
        PlayerArea area;

        /// <summary>
        ///  This decision helper is for attack/expanding decision only
        /// </summary>
        AIDecisionHelper helper;
        float decisionTime;

        //List<City> cityBuffer = new List<City>();
        Dictionary<City, CityWeights> cityWeights = new Dictionary<City, CityWeights>(25);

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

            area.NewCity += PlayerArea_NewCity;
            area.LostCity += PlayerArea_RemoveCity;

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

            if (bestAttackCity != null)// && bestAttackCityParent.CanHandleCommand()
            {

                int targetAttackCount = bestAttackCity.GetOwnedAttackBallCount();

                int accumBallCount = 0;
                int tries = 0;
                for (int i = Randomizer.GetRandomInt(area.CityCount); 
                    tries < area.CityCount && accumBallCount <= targetAttackCount; 
                    i = Randomizer.GetRandomInt(area.CityCount))
                {
                    City cc = area.GetCity(i);

                    int toAdd = cc.NearbyOwnedBallCount;

                    if (toAdd >= AIAttackMiniumBallCount && cc.Throw(bestAttackCity))
                    {
                        accumBallCount += toAdd;
                    }
                    tries++;
                }
                //int abc = bestAttackCityParent.GetOwnedAttackBallCount();
                //if (abc >= AIAttackMiniumBallCount && abc >= bestAttackCity.GetOwnedAttackBallCount())
                //{
                //    bestAttackCityParent.Throw(bestAttackCity);
                //}
            }
        }

        int GetCityDangerIndex(City a)
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
                if (Randomizer.GetRandomSingle() < 0.2f)
                {
                    if (area.CityCount < AIMaxCities)
                    {
                        Attack();
                    }

                }
                else
                {
                    for (int k = 0; k < 2 && area.CityCount > 1; k++)
                    {
                        //bool succeed = false;
                        int i = Randomizer.GetRandomInt(area.CityCount);
                        int j = Randomizer.GetRandomInt(area.CityCount);
                        while (i == j)
                        {
                            j = Randomizer.GetRandomInt(area.CityCount);
                        }

                        City ca = area.GetCity(i);
                        City cb = area.GetCity(j);

                        CityWeights caw;
                        CityWeights cbw;

                        cityWeights.TryGetValue(ca, out caw);
                        cityWeights.TryGetValue(cb, out cbw);

                        if (caw != null && cbw != null)
                        {
                            world.BallPathFinder.Reset();
                            BallPathFinderResult result = world.BallPathFinder.FindPath(ca, cb);
                            if (result != null)
                            {
                                if (caw.BulletRequirements < cbw.BulletRequirements)
                                {
                                    if (ca.NearbyOwnedBallCount > AIMoveMiniumBuffBallCount && 
                                        cb.NearbyOwnedBallCount < AIMaxCityCallCount)
                                        ca.Throw(cb, RBallType.Volience);
                                }
                                else
                                {
                                    if (cb.NearbyOwnedBallCount > AIMoveMiniumBuffBallCount &&
                                        ca.NearbyOwnedBallCount < AIMaxCityCallCount)
                                        cb.Throw(ca, RBallType.Volience);
                                }

                                if (caw.OilBallRequirements < cbw.OilBallRequirements)
                                {
                                    if (ca.NearbyOwnedBallCount > AIMoveMiniumAttackBallCount &&
                                        cb.NearbyOwnedBallCount < AIMaxCityCallCount)
                                        ca.Throw(cb, RBallType.Oil);
                                }
                                else
                                {
                                    if (cb.NearbyOwnedBallCount > AIMoveMiniumAttackBallCount &&
                                        ca.NearbyOwnedBallCount < AIMaxCityCallCount)
                                        cb.Throw(ca, RBallType.Oil);
                                }

                                if (caw.VirusRequirements < cbw.VirusRequirements)
                                {
                                    if (ca.NearbyOwnedBallCount > AIMoveMiniumBuffBallCount &&
                                        cb.NearbyOwnedBallCount < AIMaxCityCallCount)
                                        ca.Throw(cb, RBallType.Disease);
                                }
                                else
                                {
                                    if (cb.NearbyOwnedBallCount > AIMoveMiniumBuffBallCount &&
                                        ca.NearbyOwnedBallCount < AIMaxCityCallCount)
                                        cb.Throw(ca, RBallType.Disease);
                                }

                            }
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

        void PlayerArea_NewCity(City cc)
        {
            UpdateCityWeights();
        }
        void PlayerArea_RemoveCity(City cc)
        {
            UpdateCityWeights();
        }

        void UpdateCityWeights()
        {
            bool allFullHealth = true;
            for (int i = 0; i < area.CityCount; i++)
            {
                City cc = area.GetCity(i);

                if (cc.HPRate < 1 - 0.01f)
                {
                    allFullHealth = false;
                    break;
                }
            }

            for (int i = 0; i < area.CityCount; i++)
            {
                City cc = area.GetCity(i);
                CityWeights weight;

                if (!cityWeights.TryGetValue(cc, out weight))
                {
                    weight = new CityWeights();
                    cityWeights.Add(cc, weight);
                }

                int dindex = GetCityDangerIndex(cc);
                weight.OilBallRequirements = dindex * (1 + cc.Type == CityType.Neutral ? 1 : 0);
                weight.OilBallRequirements += weight.BaseBias;

                int isNotViolence = (cc.Type != CityType.Volience ? 1 : 0);
                weight.BulletRequirements = (1 - (isNotViolence * cc.Level) / 10.0f) * (isNotViolence * 0.5f + 0.5f) / dindex;
                weight.BulletRequirements += weight.BaseBias;

                weight.VirusRequirements = allFullHealth ? (cc.Type == CityType.Disease ? 1 : 0) : (1 - cc.HPRate);
                weight.VirusRequirements += weight.BaseBias;
            }
        }
    }
}
