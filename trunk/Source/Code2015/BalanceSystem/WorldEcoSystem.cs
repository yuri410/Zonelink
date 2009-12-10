using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;

namespace Code2015.BalanceSystem
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

        public void Update(GameTime time)
        { }
    }
}
