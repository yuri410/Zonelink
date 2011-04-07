using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Apoc3D;

namespace Zonelink.World
{
    /// <summary>
    ///  表示资源球
    /// </summary>
    class RBall : Entity
    {
        const float MinRadius = 25;
        const float MaxRadius = 50;
        const float GoRoundVel = 0.7f;
        const float MaxSpeed = 2;
        const float MinSpeed = 1;
        const float MaxLinSpeed = MaxRadius * MaxSpeed;

        public RBallType Type { get; private set; }

        //属于哪个城市
        City belongToCity;
        public City Parent { get { return this.belongToCity; } }

        //攻击目标
        City target;

        /// <summary>
        /// 在城=城市间移动的速度
        /// </summary>
        private Vector3 velocity;

        private float levelChange;

        /// <summary>
        /// 在城市上方旋转半径
        /// </summary>
        private float currentRadius;

        private float targetRadius;

        /// <summary>
        /// 旋转角度
        /// </summary>
        private float roundAngle;

        /// <summary>
        /// 旋转速度
        /// </summary>
        private float roundSpeed;


        //是否被消灭
        public bool IsDied { get; private set; }

        //血条
        public float Health { get; private set; }


        //状态
        public bool IsIdle { get; set; }


        //资源球的旋转半径
        float NextRadius()
        {
            //return Randomizer.NextFloat() * (MaxRadius - MinRadius) + MinRadius;
            return 0;
        }


        public RBall(Player owner, City city, RBallType type)
            : base(owner)
        {
            this.Owner = owner;
            this.belongToCity = city;
            this.Type = type;
            //设置血量
            this.Health = city.Development * 0.15f;

            //IsDied = false;

            currentRadius = NextRadius();

            roundSpeed = Randomizer.NextFloat() * (MaxSpeed - MinSpeed) + MinSpeed;
            targetRadius = currentRadius;

        }

        public void Damage(float v)
        {
            Health -= v;
            if (Health < 0)
            {
                this.Owner = null;
                this.IsDied = false;
            }
        }


        public void SetTarget(City target)
        {
            this.target = target;
        }


        public override void Render()
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            levelChange -= dt;
            roundAngle += dt * GoRoundVel * roundSpeed;

            //没有攻击别的City，待命
            if (this.Parent != null && this.target == null)
            {
                Vector3 dir = new Vector3((float)Math.Cos(roundAngle), (float)Math.Sin(roundAngle), 0);
                this.Position = this.Parent.Position + dir * currentRadius;

                velocity = Vector3.Zero;
            }
            else if (this.target != null)
            {
                ////攻击城市
                //Vector3 dir = target.Position - Position;

                //if (this.Parent != null)
                //{

                //}

            }
        }
    }
}
