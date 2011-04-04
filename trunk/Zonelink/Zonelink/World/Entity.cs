using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zonelink.State;
using Microsoft.Xna.Framework;

namespace Zonelink.World
{
    /// <summary>
    ///  表示游戏世界中实体
    /// </summary>
    class Entity
    {
        EntityType type;     
        protected Player owner;
        
        public FSMMachine fsmMachine { get; protected set; }

        //经纬度
        public float Longitude { get; protected set; }
        public float Latitude { get; protected set; }
        public float Radius { get; protected set; }

        protected Entity(EntityType type, Player owner) 
        { 
            this.type = type; 
            this.owner = owner;
            fsmMachine = new FSMMachine(this); 
        }

        protected Entity(EntityType type) 
        {
            this.type = type;
            fsmMachine = new FSMMachine(this);
        }

        //Entey Position 
        public Vector3 GetCenterPosition()
        {
            return PlanetEarth.GetPosition(Longitude, Latitude, Radius);
        }

        //状态转换，事件处理
        public bool HandleMessage(Message msg)
        {
            return fsmMachine.HandleMessage(msg);
        }      
    
    }
}
