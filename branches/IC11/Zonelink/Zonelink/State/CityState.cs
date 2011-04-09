using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zonelink.World;
using Microsoft.Xna.Framework;

namespace Zonelink.State
{
    //城市发展状态
    class CityDevelopmentState : FSMState
    {
        public void EnterState(Entity entity)
        {
            City city = entity as City;
            if (city != null)
            {
                city.AnimationType = CityAnimationState.Stopped;
            }
        }

        public void ExecState(Entity entity, GameTime gameTime)
        {  
         
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            City city = entity as City;

            //if (city.Owner != null)
            //{
                city.Develop(dt);
                city.UpdateResource(gameTime);
                if (city.CanProduceRBall())
                {
                    city.fsmMachine.ChangeState(new ProduceRBall());
                }
            //}         
           
        }

        public void ExitState(Entity entity)
        {

        }

        public bool OnHandleMessage(Entity entity, Message msg)
        {
            return true;
        }
    }

    //产生资源球转台，控制动画
    class ProduceRBall : FSMState
    {
        #region FSMState Members

        public void EnterState(Entity entity)
        {
            ((City)entity).AnimationType = CityAnimationState.SendBall;
            ((City)entity).animationPlayOver = false;
        }

        public void ExecState(Entity entity, GameTime gameTime)
        {
            City city = entity as City;

            city.ProduceBall();

            //产生完毕，切换状态
            if (city.animationPlayOver == true)
            {
                city.fsmMachine.ChangeState(new CityDevelopmentState());
            }
        }

        public void ExitState(Entity entity)
        {
           
        }

        public bool OnHandleMessage(Entity entity, Message msg)
        {
            return true;
        }

        #endregion
    }

 



    class CityBeingAttackedState : FSMState
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

    class CityAttackedState : FSMState
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

    class CityOccupiedState : FSMState
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
