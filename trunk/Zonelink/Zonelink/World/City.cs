using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zonelink.State;

namespace Zonelink.World
{

    /// <summary>
    ///  表示游戏世界中的城市
    /// </summary>
    class City : Entity
    {  
        public float HealthValue { get; private set; }
        public float Development { get; private set; }
        public CityType Type { get; private set; }

        //城市创造出来的资源球 
        private List<RBall> visibleBallList = new List<RBall>();

        public City(CityType type, Player owner)
            : base(type, owner)
        {
            this.Type = type;
        }

        //发展到一定程度时产生资源
        public void ProduceResourceBall()
        {
            visibleBallList.Add( Technology.Instance.CreateRBall(GetResourceType(), this.owner, this) ) ;
        }

        private RBallTypeID GetResourceType()
        {
            switch (this.Type.TypeId)
            {
                case CityTypeID.Disease:
                    return RBallTypeID.Disease;
                
                case CityTypeID.Education:
                    return RBallTypeID.Education;

                case CityTypeID.Green:
                    return RBallTypeID.Green;

                case CityTypeID.Health:
                    return RBallTypeID.Health;

                case CityTypeID.Oil:
                    return RBallTypeID.Oil;

                case CityTypeID.Volience:
                    return RBallTypeID.Volience;
                
                default:
                    //Error
                    return RBallTypeID.Disease;
            }
        }

    }
}
