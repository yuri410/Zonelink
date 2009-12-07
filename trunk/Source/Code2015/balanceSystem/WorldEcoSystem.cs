using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Code2015.balanceSystem
{
    class WorldEcoSystem:IUpdatable
    {
        public float ENWeight
        {
            get;
            set;
        }
        public float TemperatureShift
        {
            get;
            set;
        }
        public float SeaLevel
        {
            get;
            set;
        }
        CarbonGroup[] carbonGroup;
        //计算碳含量
        public float AccumulatedCarbon()
        {
            return 0.0f;
        }

         public void Update(float time)
        { }
    }
}
