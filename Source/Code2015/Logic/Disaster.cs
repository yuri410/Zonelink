using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.MathLib;
using Code2015.BalanceSystem;

namespace Code2015.Logic
{
    class Disaster
    {
        float longitude;
        float latitude;

        float radius;

        float duration;

        float damage;
        SimulationRegion world;

        City[] cities;
        NaturalResource[] resources;
        float[] cityWeights;
        float[] resWeights;

        public float Longitude
        {
            get { return longitude; }
        }
        public float Latitude 
        {
            get { return latitude; }
        }

        public float Duration
        {
            get { return duration; }
        }


        public bool IsOver
        {
            get { return duration < float.Epsilon; }
        }

        public Disaster(SimulationRegion world, float lng, float lat, float duration, float damage)
        {
            this.longitude = lng;
            this.latitude = lat;
            this.duration = duration;
            this.damage = damage;
            this.world = world;


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
                Vector2 d = new Vector2(longitude - cc.Longitude, latitude - cc.Latitude);

                cityWeights[i] = MathEx.Sqr((d.Length() / radius));
            }

            for (int i = 0; i < resources.Count; i++)
            {
                NaturalResource cc = resources[i];
                Vector2 d = new Vector2(longitude - cc.Longitude, latitude - cc.Latitude);

                resWeights[i] = MathEx.Sqr((d.Length() / radius));
            }

            this.cities = cities.Elements;
            this.resources = resources.Elements;
        }

        public void Update(GameTime time)
        {
            float hours = (float)time.ElapsedGameTime.TotalHours;

            for (int i = 0; i < cities.Length; i++)
            {
                float pop = cityWeights[i] * damage * 0.5f;
                float dev = cityWeights[i] * damage;
                cities[i].Damage(pop, dev);
            }
            //for (int i = 0; i < resources.Length; i++) 
            //{
            //    //float pop = resWeights[i] * damage * 0.5f;
                
            //}

            duration -= hours;

            //if (duration < 0)
            //{

            //}
        }
    }
}
