using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zonelink
{
    /// <summary>
    ///  表示玩家的信息
    /// </summary>
    struct PlayerInfo { }

    /// <summary>
    ///  （关卡中的）城市信息
    /// </summary>
    struct CityInfo { }



    /// <summary>
    ///  （关卡中的）资源点信息
    /// </summary>
    struct ResourceInfo
    {

    }
    /// <summary>
    ///  记录关卡地图的数据
    /// </summary>
    class Level
    {
        /// <summary>
        ///  开始关卡时的玩家信息
        /// </summary>
        PlayerInfo[] startingPlayers;

        CityInfo[] cities;
        ResourceInfo[] resources;
    }
}
