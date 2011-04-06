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
            ((City)entity).AnimationType = CityAnimationType.Stopped;
        }

        public void ExecState(Entity entity, GameTime gameTime)
        {                           
            City city = entity as City;
            //更新城市状态Health, Development
            if (city.Development < RulesTable.CityMaxDevelopment)
            {
                city.Development += RulesTable.CityDevHealthRate * city.HealthValue;
            }

            System.Console.WriteLine(city.Development);
            //产生资源球
            if (city.Development > 2000)
            {
                city.fsmMachine.ChangeState(new ProduceRBall());
            }
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
            ((City)entity).AnimationType = CityAnimationType.SendBall;
        }

        public void ExecState(Entity entity, GameTime gameTime)
        {
            City city = entity as City;

            //转换为Send状态
            city.AnimationType = CityAnimationType.SendBall;

            //产生完毕，切换状态
            if (city.animationPlayOver == true)
            {
                city.fsmMachine.ChangeState(new CityDevelopmentState());
            }
        }

        public void ExitState(Entity entity)
        {
            throw new NotImplementedException();
        }

        public bool OnHandleMessage(Entity entity, Message msg)
        {
            throw new NotImplementedException();
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
