using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Scene;
using Code2015.EngineEx;
using Code2015.Logic;
using Apoc3D.MathLib;
using Apoc3D.Graphics;

namespace Code2015.World
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


        /// <summary>
        ///  经度
        /// </summary>
        public float Longitude { get; protected set; }
        /// <summary>
        ///  纬度
        /// </summary>
        public float Latitude { get; protected set; }
        //public float Radius { get; protected set; }


        protected WorldObject(Player owner) 
        {
            this.Owner = owner;
            //fsmMachine = new FSMMachine(this);
            //this.IsSelected = false;
        }

        protected WorldObject() 
        {
            //fsmMachine = new FSMMachine(this);
        }

        public abstract void InitalizeGraphics(RenderSystem rs);

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


    }

    abstract class WorldDynamicObject : DynamicObject
    { 
        /// <summary>
        ///  拥有此物体的玩家
        /// </summary>
        public Player Owner { get; protected set; }

        protected WorldDynamicObject(Player owner) 
        {
            this.Owner = owner;
            //fsmMachine = new FSMMachine(this);
            //this.IsSelected = false;
        }

        protected WorldDynamicObject() 
        {
            //fsmMachine = new FSMMachine(this);
        }
    }

}
