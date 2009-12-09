using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Code2015.balanceSystem
{
    

    class SocietyStatus:IUpdatable
    {
        public float Development
        {
            get;
            set;
        }
        public float PracticalEnvProtection
        {
            get;
            set;
        }
        public float EnvProductive
        {
            get;
            set;
        }
        public float Population
        {
            get;
            set;
        }
        Apoc3D.Collections.FastList<City> city = new Apoc3D.Collections.FastList<City>();
        
        public void Update(float time)
        { }
    }
}
