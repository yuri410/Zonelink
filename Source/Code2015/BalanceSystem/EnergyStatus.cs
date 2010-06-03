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
using Apoc3D.Config;
using Apoc3D.MathLib;
using Code2015.Logic;
using Code2015.Network;
using Apoc3D.Vfs;

namespace Code2015.BalanceSystem
{
    public delegate void DisasterArrivedHandler(Disaster disaster);

    public class EnergyStatus : IConfigurable, IUpdatable, ILogicStateObject
    {
        [SLGValue()]
        public const float HPLowThreshold = 300;
        [SLGValue()]
        public const float LPLowThreshold = 300;
        [SLGValue()]
        public const float FoodLowThreshold = 300;

        #region 灾难参数
        [SLGValue]
        public const float SafeRatio = 0.2f;

        [SLGValue]
        public const float SafeAmount = 10000;

        [SLGValue]
        public const float SafeTime = 15;
        [SLGValue]
        public const float RefAmount = 50000;
        [SLGValue]
        public const float DisasterPRatio = 0.001f;
        [SLGValue]
        public const float DisasterCountDown = 10;
        [SLGValue]
        public const float MaxDuration = 60;
        [SLGValue]
        public const float MaxDamage = 1000;
        [SLGValue]
        public const float MaxRadius = 30;
        #endregion

        bool changed;

        public SimulationWorld Region
        {
            get;
            private set;
        }
        /// <summary>
        ///  当前已储备的能量
        /// </summary>
        public float CurrentHR
        {
            get;
            private set;
        }
        /// <summary>
        ///  当前已储备的能源
        /// </summary>
        public float CurrentLR
        {
            get;
            private set;
        }
        /// <summary>
        ///  当前已储备的食物
        /// </summary>
        public float CurrentFood
        {
            get;
            private set;
        }

        /// <summary>
        ///  获取一个布尔值，表示当前已储备的高能资源量是否过低
        /// </summary>
        public bool IsHPLow
        {
            get { return CurrentHR < HPLowThreshold; }
        }
        public bool IsLPLow
        {
            get { return CurrentLR < LPLowThreshold; }
        }
        public bool IsFoodLow
        {
            get { return CurrentFood < FoodLowThreshold; }
        }

        /// <summary>
        ///  自然环境中，剩余未开发不可再生资源可转化为的能量
        /// </summary>
        public float RemainingHR
        {
            get;
            private set;
        }
        public float RemainingLR
        {
            get;
            private set;
        }

        public event DisasterArrivedHandler DisasterArrived;
        public event DisasterOverHandler DisasterOver;

        Dictionary<Player, float> carbonRatio = new Dictionary<Player, float>();
        Dictionary<Player, float> carbonWeight = new Dictionary<Player, float>();
        FastList<IncomingDisaster> incoming = new FastList<IncomingDisaster>();

        public Dictionary<Player, float> GetCarbonRatios()
        {
            return carbonRatio;
        }
        public Dictionary<Player, float> GetCarbonWeights()
        {
            return carbonWeight;
        }

        public EnergyStatus(SimulationWorld region)
        {
            Region = region;
        }

        public void AddDisaster(Disaster d)
        {
            Region.Add(d);
            d.Over += Disaster_Over;

            if (DisasterArrived != null)
            {
                DisasterArrived(d);
            }
        }

        public void Update(GameTime time)
        {
            float hours = (float)time.ElapsedGameTime.TotalHours;

            #region 统计

            float hr = 0;
            float lr = 0;
            float food = 0;
            SimulationWorld region = Region;
            // 已经采集的资源
            for (int i = 0; i < region.CityCount; i++)
            {
                City city = region.GetCity(i);
                hr += city.LocalHR.Current;
                lr += city.LocalLR.Current;
                food += city.LocalFood.Current;


                Player pl = city.Owner;
                if (pl != null)
                {
                    if (!carbonWeight.ContainsKey(pl))
                    {
                        carbonWeight.Add(pl, 0);
                    }
                    float val = carbonWeight[pl] + city.CarbonProduceSpeed * hours;
                    if (val < 0)
                        val = 0;
                    carbonWeight[pl] = val;
                }
            }
            CurrentFood = food;
            CurrentHR = hr;
            CurrentLR = lr;


            hr = 0;
            lr = 0;
            food = 0;
            // 自然中未开发的
            for (int i = 0; i < region.ResourceCount; i++)
            {
                NaturalResource res = region.GetResource(i);
                if (res.Type == NaturalResourceType.Petro)
                {
                    hr += res.CurrentAmount;
                }
                else if (res.Type == NaturalResourceType.Wood)
                {
                    lr += res.CurrentAmount;
                }
            }
            RemainingHR = hr;
            RemainingLR = lr;
            #endregion

            #region 计算灾害

            float total = 10000;
            Dictionary<Player, float>.ValueCollection vals = carbonWeight.Values;
            foreach (float e in vals)
            {
                total += e;
            }

            foreach (KeyValuePair<Player, float> e in carbonWeight)
            {
                if (!carbonRatio.ContainsKey(e.Key))
                {
                    carbonRatio.Add(e.Key, 0);
                }

                float p = e.Value / total;
                carbonRatio[e.Key] = p;

                if (p < SafeRatio)
                    continue;

                p *= DisasterPRatio;

                if (Randomizer.GetRandomSingle() < p && e.Value > SafeAmount)
                {
                    // 发生灾难

                    IncomingDisaster disaster;
                    disaster.CountDown = DisasterCountDown;

                    City badCity = null;
                    float mostC = float.MinValue;

                    PlayerArea area = e.Key.Area;
                    for (int i = 0; i < area.CityCount; i++)
                    {
                        City cc = area.GetCity(i);
                        float c = cc.RecentCarbonAmount;
                        if (c > mostC)
                        {
                            mostC = c;
                            badCity = cc;
                        }
                    }

                    if (badCity != null)
                    {
                        disaster.Latitude = badCity.Latitude;
                        disaster.Longitude = badCity.Longitude;

                        float adj = MathEx.Saturate((float)Math.Log(e.Value, RefAmount));
                        float pp = e.Value / total;
                        disaster.Duration = pp * MaxDuration * adj;

                        if (disaster.Duration > SafeTime)
                        {
                            disaster.Damage = pp * MaxDamage * adj;
                            disaster.Radius = pp * MaxRadius * adj;
                            disaster.BadCity = badCity;
                            if (area.Owner.Type == PlayerType.LocalHuman)
                            {
                                EventLogger.Instance.Log(EventType.Strike, badCity);
                            }
                            incoming.Add(ref disaster);
                            changed = true;
                        }
                    }
                }

            }

            #endregion

            for (int i = incoming.Count - 1; i >= 0; i--)
            {
                if (incoming[i].CountDown > 0)
                {
                    incoming.Elements[i].CountDown -= hours;

                    if (incoming.Elements[i].CountDown < 0)
                    {
                        // 发生了
                        Disaster d = new Disaster(region, incoming[i].BadCity.Owner,
                            incoming[i].Longitude, incoming[i].Latitude,
                            incoming[i].Radius,
                            incoming[i].Duration, incoming[i].Damage);

                        AddDisaster(d);

                        incoming.RemoveAt(i);
                        changed = true;
                    }
                }
            }

        }

        void Disaster_Over(Disaster d)
        {
            if (DisasterOver != null)
            {
                DisasterOver(d);
            }
            Region.Remove(d);
        }

        #region IConfigurable 成员

        public void Parse(ConfigurationSection sect)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region ILogicStateObject 成员

        public void Serialize(StateDataBuffer data)
        {
            ContentBinaryWriter bw = data.Writer;

            bw.Write(carbonRatio.Count);
            foreach (KeyValuePair<Player, float> e in carbonRatio)
            {
                bw.Write(e.Key.ID);
                bw.Write(e.Value);
            }
            bw.Write(carbonWeight.Count);
            foreach (KeyValuePair<Player, float> e in carbonWeight)
            {
                bw.Write(e.Key.ID);
                bw.Write(e.Value);
            }

            bw.Write(incoming.Count);

            for (int i = 0; i < incoming.Count; i++)
            {
                bw.Write(incoming[i].CountDown);
                bw.Write(incoming[i].Damage);
                bw.Write(incoming[i].Duration);
                bw.Write(incoming[i].Latitude);
                bw.Write(incoming[i].Longitude);
                bw.Write(incoming[i].Radius);
            }

            data.EndWrite();

            changed = false;
        }

        public void Deserialize(StateDataBuffer data)
        {
            ContentBinaryReader br = data.Reader;

            int count = br.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                int id = br.ReadInt32();

                Player pl = null;
                foreach (KeyValuePair<Player, float> e in carbonRatio)
                {
                    if (e.Key.ID == id)
                    {
                        pl = e.Key;
                    }
                }

                if (pl != null)
                {
                    carbonRatio[pl] = br.ReadSingle();
                }
                else br.ReadSingle();
            }

            count = br.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                int id = br.ReadInt32();

                Player pl = null;
                foreach (KeyValuePair<Player, float> e in carbonWeight)
                {
                    if (e.Key.ID == id)
                    {
                        pl = e.Key;
                    }
                }

                if (pl != null)
                {
                    carbonWeight[pl] = br.ReadSingle();
                }
                else br.ReadSingle();
            }

            count = br.ReadInt32();

            incoming.FastClear();
            incoming.Resize(count);
            for (int i = 0; i < count; i++) 
            {
                incoming.Elements[i].CountDown = br.ReadSingle();
                incoming.Elements[i].Damage = br.ReadSingle();
                incoming.Elements[i].Duration = br.ReadSingle();
                incoming.Elements[i].Latitude = br.ReadSingle();
                incoming.Elements[i].Longitude = br.ReadSingle();
                incoming.Elements[i].Radius = br.ReadSingle();
            }


        }

        public string StateName
        {
            get { return "Disasters"; }
        }

        public bool Changed
        {
            get { return changed; }
        }

        #endregion
    }
}
