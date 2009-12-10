using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;

namespace Code2015.BalanceSystem
{
    class Region : IUpdatable
    {
        private float Area;
        private Object Border;
        private Object Owner;

        public float setArea
        {
            get { return Area; }
            set { Area = value; }
        }
        public Object setBorder
        {
            get { return Border; }
            set { Border = value; }
        }
        public Object setOwner
        {
            get { return Owner; }
            set { Owner = value; }
        }

        LocalEcoSystem[] lacalecoSystem;
        EnergyStatus energyStatus;
        SocietyStatus societyStatus;

        public void Update(GameTime time)
        { }
    }
}
