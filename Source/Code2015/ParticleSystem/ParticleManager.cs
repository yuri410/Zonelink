using System;
using System.Collections.Generic;
using System.Text;

namespace Code2015.ParticleSystem
{
    class ParticleManager
    {
        static ParticleManager singleton;

        public static ParticleManager Instance
        {
            get
            {
                if (singleton == null)
                {
                    singleton = new ParticleManager();
                }
                return singleton;
            }
        }



        Queue<Particle> pool;

        private ParticleManager()
        {
            pool = new Queue<Particle>();
        }

        public Particle CreateParticle()
        {
            if (pool.Count > 0)
            {
                return pool.Dequeue();
            }
            return new Particle();
        }

        public void Retire(Particle p)
        {
            pool.Enqueue(p);
        }


    }
}
