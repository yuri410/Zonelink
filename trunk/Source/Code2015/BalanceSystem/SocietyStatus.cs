using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Collections;
using Apoc3D;

namespace Code2015.BalanceSystem
{
    public class SocietyStatus : IUpdatable
    {


        public float Development
        {
            get;
            set;
        }


        public float Population
        {
            get;
            set;
        }

        public void Update(GameTime time)
        { }
    }
}
