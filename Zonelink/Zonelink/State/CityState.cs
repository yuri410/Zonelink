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
