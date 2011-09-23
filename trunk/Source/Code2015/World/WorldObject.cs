using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Scene;
using Code2015.EngineEx;
using Code2015.Logic;
using Apoc3D.MathLib;
using Apoc3D.Graphics;
using Apoc3D;

namespace Code2015.World
{
    /// <summary>
    ///  表示游戏世界中实体
    /// </summary>
    abstract class WorldObject : StaticModelObject
    {
        protected BattleField battleField;
        /// <summary>
        ///  拥有此物体的玩家
        /// </summary>
        public Player Owner { get; protected set; }

        public bool IsInVisibleRange
        {
            get { return Visiblity > 0.3f; }
        }
        public float Visiblity
        {
            get;
            private set;
        }
        /// <summary>
        ///  经度
        /// </summary>
        public float Longitude { get; protected set; }
        /// <summary>
        ///  纬度
        /// </summary>
        public float Latitude { get; protected set; }


        protected WorldObject(Player owner, BattleField battfield) 
        {
            this.Owner = owner;
            this.battleField = battfield;
            //fsmMachine = new FSMMachine(this);
            //this.IsSelected = false;
        }
        protected WorldObject(BattleField battfield)
        {
            this.battleField = battfield;
            //fsmMachine = new FSMMachine(this);
        }



        public abstract void InitalizeGraphics(RenderSystem rs);

        ////状态转换，事件处理
        //public bool HandleMessage(Message msg)
        //{
        //    return fsmMachine.HandleMessage(msg);
        //}

        //更新状态
        public override void Update(GameTime dt)
        {
            Visiblity = battleField.Fog.GetVisibility(MathEx.Degree2Radian(Longitude),
                   MathEx.Degree2Radian(Latitude));
        }

        public override bool IntersectsSelectionRay(ref Ray ray)
        {
            bool d = Vector3.DistanceSquared(ref ray.Position, ref position) < 6000 * 6000;
            if (d)
            {
                return base.IntersectsSelectionRay(ref ray);
            }
            return false;
        }

        /// <summary>
        ///  配置文件解析
        /// </summary>
        /// <param name="sect"></param>
        public virtual void Parse(GameConfigurationSection sect)
        {
            Longitude = sect.GetSingle("Longitude");
            Latitude = sect.GetSingle("Latitude");
        }


        public override bool IsSerializable
        {
            get { return false; }
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

        public override bool IsSerializable
        {
            get { return false; }
        }

        public override bool IntersectsSelectionRay(ref Ray ray)
        {
            bool d = Vector3.DistanceSquared(ref ray.Position, ref position) < 6000 * 6000;
            if (d)
            {
                return base.IntersectsSelectionRay(ref ray);
            }
            return false;
        }
    }

}
