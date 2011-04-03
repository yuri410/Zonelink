using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zonelink.World
{
    /// <summary>
    ///  表示资源球
    /// </summary>
    class RBall : Entity
    {
        RBallType type;


        public RBall(RBallType type, Player owner)
            : base(type, owner)
        {
            this.type = type;
        }
    }
}
