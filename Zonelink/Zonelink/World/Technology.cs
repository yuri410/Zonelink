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
        public RBall CreateRBall(City city, RBallType type)  
        {
            return new RBall(type, city.Owner, city);
        }
    
    }
}
