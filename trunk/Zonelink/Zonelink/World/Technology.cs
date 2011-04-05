using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zonelink.World
{
    

    /// <summary>
    ///  记录，维护表示所有科技类型的所有对象
    ///  并创建相应科技的“部队”
    /// </summary>
    class Technology
    {
        CityType[] cityType;
        RBallType[] rballType;

        //单例
        public static readonly Technology Instance = new Technology(); 

        public RBallType GetRBallType(RBallType tid) { return rballType[(int)tid]; }

        //根据城市类型产生相应的资源球
        public RBall CreateRBall(City city)  
        {
            RBallType type;

             switch (city.CityType)
            {
                 case CityType.Disease:
                    type = RBallType.Disease;
                    break;

                 case CityType.Education:
                    {
                        type = RBallType.Education;
                        break;
                    }
                 case CityType.Green:
                    {
                        type = RBallType.Green;
                        break;
                    }

                 case CityType.Health:
                    {
                        type = RBallType.Health ;
                        break;
                    }   
                 case CityType.Volience:
                    {
                        type = RBallType.Volience;
                        break;
                    }
                 case CityType.Oil:
                    {
                        type = RBallType.Oil;
                        break;
                    }
                 default:
                    {
                         //Error Handle
                        type = RBallType.Disease;
                        break; 
                    }             
             }

             return new RBall(type, city.Owner, city);
        }
    
    }
}
