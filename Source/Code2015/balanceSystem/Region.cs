using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Code2015.balanceSystem
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

        LocalEcoSystem[] lacalecoSystem=null;
        EnergyStatus energyStatus=null;
        SocietyStatus societyStatus=null;
        public void Update(float time)
        { }
    }
}
