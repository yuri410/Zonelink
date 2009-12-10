using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;

namespace Code2015.BalanceSystem
{
    class City : IUpdatable
    {
        private string name;
        private float population, development, food, disease;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public float Development
        {
            get { return development; }
            set { development = value; }
        }
        public float Population
        {
            get { return population; }
            set { population = value; }
        }
        public float Food
        {
            get { return food; }
            set { food = value; }
        }
        public float Disease
        {
            get { return disease; }
            set { disease = value; }
        }
        public void Update(GameTime time)
        { }

    }
}
