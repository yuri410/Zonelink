using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Collections;
using Apoc3D;

namespace Code2015.BalanceSystem
{
  
    /// <summary>
    /// 农作物的类
    /// </summary>
    public enum KindofPlant { Wheat,Rice,Corn,Bean,Peanut}
    public class FarmLand : NaturalResource
    {
        [SLGValueAttribute()]
        const float HarvestNeedTime = 5;
        
        public string Name
        {
            get;
            set;
        }
        public FarmLand(string name)
        {
          
            this.Name = name;
        }
        
        /// <summary>
        /// 农场种植的农作物
        /// </summary>
        public FastList<PlantSpecies> PlantsOfFarm
        {
            get;
            set;
        }
        /// <summary>
        /// 从用户获得的要种植的农作物
        /// </summary>
        /// <param name="plantsoffarm"></param>
        public void GetPlantsOfFarm(FastList<PlantSpecies> plantsoffarm)
        {
            this.PlantsOfFarm = plantsoffarm;
        }


        public float GetHarvestTime()
        {
            return HarvestNeedTime;
        }

        public string SetPlantOfFarm(string name)
        {
            FarmLand farm = new FarmLand(name);
            return farm.Name;
        }

       
        public FastList<PlantSpecies> InitFarm()
        {
            FastList<PlantSpecies> farms = new FastList<PlantSpecies>();
            farms.Add(new PlantSpecies("Wheat"));//小麦
            farms.Add(new PlantSpecies("Rice"));//水稻
            return farms;
        }


       
      
      

     
    }
}
