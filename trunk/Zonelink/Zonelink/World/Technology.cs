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

        public CityType GetCityType(CityTypeID tid) { return cityType[(int)tid]; }
        public RBallType GetRBallType(RBallTypeID tid) { return rballType[(int)tid]; }

        public City CreateCity(CityTypeID fid, Player player) { throw new NotImplementedException(); }
        public RBall CreateRBall(RBallTypeID tid, Player player, City city) { throw new NotImplementedException(); }
    
    }
}
