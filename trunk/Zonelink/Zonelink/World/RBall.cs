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
        public RBallType TypeId { get; private set; }

        //属于哪个城市
        City belongToCity;

        //是否被消灭
        public bool IsDied { get; private set; }

        //血条
        public float Health { get; private set; }

        //旋转速度
        private float speed; 

        public RBall(RBallType type, Player owner, City city)
            : base(owner)
        {
            this.TypeId = type;
            this.belongToCity = city;
        }

        public override void Render()
        {
            throw new NotImplementedException();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime dt)
        {
            throw new NotImplementedException();
        }
    }
}
