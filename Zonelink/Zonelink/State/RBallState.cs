using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zonelink.World;
using Microsoft.Xna.Framework;

namespace Zonelink.State
{
    //待命
    class RStayState : FSMState
    {
        public  void EnterState(Entity entity)
        {

        }

        public  void ExecState(Entity entity, GameTime gameTime)
        {
            //Update, 在城市上空旋转

        }

        public void ExitState(Entity entity)
        {

        }


        public bool OnHandleMessage(Entity entity, Message msg)
        {
            switch (msg.Type)
            {
                case EventType.CityBeingAttacked:
                    {
                        entity.fsmMachine.ChangeState(new RBallAttackState());
                        
                    }
                    break;
                default:
                    //错误事件转换 
                    break;
                    
            }
            return true;
        }
    }

//---------------------------------------------------------------------------------

    //进攻状态
    class RBallAttackState : FSMState
    {
        public void EnterState(Entity entity)
        {

        }

        public void ExecState(Entity entity, GameTime gameTime)
        {
            RBall ball = (RBall)entity;
            if (ball.Health < 0f)
            {
                ball.fsmMachine.ChangeState(new RBallDiedState());
            } 
            else
            {
                //Update,去攻打别的城市
            }
        }

        public void ExitState(Entity entity)
        {
            
        }


        public bool OnHandleMessage(Entity entity, Message msg)
        {
            switch (msg.Type)
            {
                case EventType.CityBeingAttacked:
                    {
                        entity.fsmMachine.ChangeState(new RBallAttackState());
                    }
                    break;
                default:
                    //错误事件转换 
                    break;

            }
            return true;
        }
    }

//---------------------------------------------------------------------------------
    //死亡
    class RBallDiedState : FSMState
    {
        public void EnterState(Entity entity)
        {

        }

        public void ExecState(Entity entity, GameTime gameTime)
        {

        }

        public void ExitState(Entity entity)
        {

        }


        public bool OnHandleMessage(Entity entity, Message msg)
        {
            return true;
        }
    }

}
