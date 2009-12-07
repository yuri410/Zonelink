using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Code2015.balanceSystem
{
    class OilFiled:EnergyProducer
    {
        //这里我加了两个属性，分别是土壤的板结程度和肥沃程度
        private int Hardenth;
        private int Enrichment;
        public int setHardenth
        {
            get { return Hardenth; }
            set { Hardenth = value; }
        }
        public int setEnrichment
        {
            get { return Enrichment; }
            set { Enrichment = value; }
        }
        public bool IsSealed()
        {
            return true;
        }
    }
}
