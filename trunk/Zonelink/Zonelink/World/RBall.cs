using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zonelink.State;

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


        //状态
        public bool IsIdle { get; set; }

        
        public RBall( Player owner, City city)
            : base(owner)
        {
            this.belongToCity = city;

            switch (city.Type)
            {
                case CityType.Disease:
                    {
                        this.TypeId = RBallType.Disease;
                        break;
                    }
                case CityType.Education:
                    {
                        this.TypeId = RBallType.Education;
                        break;
                    }    
                case CityType.Volience:
                    {
                        this.TypeId = RBallType.Volience;
                        break;
                    }
                case CityType.Health:
                    {
                        this.TypeId = RBallType.Health;
                        break;
                    }

                //默认进入待命状态，在城市上空旋转
                case CityType.Green:
                    {
                        this.TypeId = RBallType.Green;
                        this.fsmMachine.CurrentState = new RStayState();
                        break;
                    }
                case CityType.Oil:
                    {
                        this.TypeId = RBallType.Oil;
                        this.fsmMachine.CurrentState = new RStayState();
                        break;
                    }

            }


            
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
