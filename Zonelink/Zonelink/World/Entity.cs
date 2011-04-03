using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zonelink.World
{
    /// <summary>
    ///  表示游戏世界中实体
    /// </summary>
    class Entity
    {
        EntityType type;
        
        protected Player owner;

        protected Entity(EntityType type, Player owner) { this.type = type; this.owner = owner; }
        protected Entity(EntityType type) { this.type = type; }
    }
}
