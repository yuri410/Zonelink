using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.MathLib;
using Code2015.BalanceSystem;
using Code2015.Network;

namespace Code2015.Logic
{
    public delegate void DisasterOverHandler(Disaster d);

    public class Disaster : SimulationObject
    {
        float radius;

        float duration;

        float damage;
        SimulationWorld world;

        City[] cities;
        NaturalResource[] resources;
        float[] cityWeights;
        float[] resWeights;

        bool overInvoked;
        Player cause;
        public float Radius
        {
            get { return radius; }
        }
        public float Damage
        {
            get { return damage; }
        }
        public float Duration
        {
            get { return duration; }
        }

        
        public bool IsOver
        {
            get { return duration < float.Epsilon; }
        }


        public event DisasterOverHandler Over;

        public Disaster(SimulationWorld world, Player cause, float lng, float lat, float radius, float duration, float damage)
            : base(world)
        {
            this.cause = cause;
            this.Longitude = lng;
            this.Latitude = lat;
            this.duration = duration;
            this.damage = damage;
            this.world = world;
            this.radius = radius;

            FastList<City> cities = new FastList<City>();
            FastList<NaturalResource> resources = new FastList<NaturalResource>();


            float r2 = radius * radius;
            for (int i = 0; i < world.CityCount; i++)
            {
                City cc = world.GetCity(i);
                Vector2 d = new Vector2(lng - cc.Longitude, lat - cc.Latitude);

                if (d.LengthSquared() < r2)
                {
                    cities.Add(cc);
                }
            }

            for (int i = 0; i < world.ResourceCount; i++)
            {
                NaturalResource nres = world.GetResource(i);
                Vector2 d = new Vector2(lng - nres.Longitude, lat - nres.Latitude);
                if (d.LengthSquared() < r2)
                {
                    resources.Add(nres);
                }
            }

            cities.Trim();
            resources.Trim();

            cityWeights = new float[cities.Count];
            resWeights = new float[resources.Count];

            for (int i = 0; i < cities.Count; i++)
            {
                City cc = cities[i];
                Vector2 d = new Vector2(Longitude - cc.Longitude, Latitude - cc.Latitude);

                cityWeights[i] = 1 - MathEx.Saturate(MathEx.Sqr((d.Length() / radius)));
            }

            for (int i = 0; i < resources.Count; i++)
            {
                NaturalResource cc = resources[i];
                Vector2 d = new Vector2(Longitude - cc.Longitude, Latitude - cc.Latitude);

                resWeights[i] = 1 - MathEx.Saturate(MathEx.Sqr((d.Length() / radius)));
            }

            this.cities = cities.Elements;
            this.resources = resources.Elements;
        }

        public override void Update(GameTime time)
        {
            CarbonProduceSpeed = 0;

            float hours = (float)time.ElapsedGameTime.TotalHours;

            if (duration > 0)
            {
                for (int i = 0; i < cities.Length; i++)
                {
                    if (cities[i].IsCaptured)
                    {
                        float pop = cityWeights[i] * damage * 0.05f;
                        float dev = cityWeights[i] * damage;
                        cities[i].Damage(pop * hours, dev * hours);
                    }
                }
            }
            //for (int i = 0; i < resources.Length; i++) 
            //{
            //    //float pop = resWeights[i] * damage * 0.5f;
                
            //}

            duration -= hours;

            if (duration < 0 && !overInvoked)
            {
                if (Over != null)
                    Over(this);

                if (cause != null)
                {
                    Dictionary<Player, float> weights = world.EnergyStatus.GetCarbonWeights();
                    float co2;

                    if (weights.TryGetValue(cause, out co2))
                    {
                        co2 *= 0.75f;
                        if (co2 < 0) co2 = 0;
                        weights[cause] = co2;
                    }

                }
                overInvoked = true;
            }
        }


        public override bool Changed
        {
            get { return false; }
        }
        public override string StateName
        {
            get
            {
                return base.StateName;
            }
        }
        public override void Deserialize(StateDataBuffer data)
        {
            throw new NotSupportedException();
        }
        public override void Serialize(StateDataBuffer data)
        {
            throw new NotSupportedException();
        }
    }
}
