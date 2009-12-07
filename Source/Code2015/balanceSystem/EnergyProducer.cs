using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Code2015.balanceSystem
{
    class EnergyProducer:IUpdatable
    {
        private float ProductSpeed;
        private float TotalAmount;
        private string ProductionType;

        public float setProductSpeed
        {
            get { return ProductSpeed; }
            set { ProductSpeed = value; }
        }
        public float setTotalAmount
        {
            get { return TotalAmount; }
            set { TotalAmount = value; }
        }
        public string setProductType
        {
            get { return ProductionType; }
            set { ProductionType = value; }
        }
        public EnergyProducer()
        { }

        public void Update(float time)
        { }
    }
   
}
