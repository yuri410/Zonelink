using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Scene;
using Code2015.EngineEx;
using Code2015.Logic;
using Apoc3D.MathLib;

namespace Zonelink.World
{
    /// <summary>
    ///  表示游戏世界中实体
    /// </summary>
    abstract class WorldObject : StaticModelObject
    {
        /// <summary>
        ///  拥有此物体的玩家
        /// </summary>
        public Player Owner { get; protected set; }


        //是否被选中
        public bool IsSelected { get; set; }
       
        //经纬度
        public float Longitude { get; protected set; }
        public float Latitude { get; protected set; }
        public float Radius { get; protected set; }

        //Entey中心位置 
        public Vector3 Position { get; set; }

        protected WorldObject(Player owner) 
        {
            this.Owner = owner;
            //fsmMachine = new FSMMachine(this);
            this.IsSelected = false;
        }

        protected WorldObject() 
        {
            //fsmMachine = new FSMMachine(this);
        }

        ////状态转换，事件处理
        //public bool HandleMessage(Message msg)
        //{
        //    return fsmMachine.HandleMessage(msg);
        //}

        ////更新状态
        //public override void Update(GameTime dt)
        //{
        //    this.fsmMachine.Update(dt);
        //}


        /// <summary>
        ///  配置文件解析
        /// </summary>
        /// <param name="sect"></param>
        public virtual void Parse(GameConfigurationSection sect)
        {
            Longitude = sect.GetSingle("Longitude");
            Latitude = sect.GetSingle("Latitude");
        }

        public override void Render()
        {
            throw new NotSupportedException();
        }
    }
}
