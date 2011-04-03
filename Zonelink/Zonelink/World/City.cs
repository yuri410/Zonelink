using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zonelink.World
{
    /// <summary>
    ///  表示游戏世界中的城市
    /// </summary>
    class City : Entity
    {
        CityType type;

        public City(CityType type, Player owner)
            : base(type, owner)
        {
            this.type = type;
        }
    }
}
