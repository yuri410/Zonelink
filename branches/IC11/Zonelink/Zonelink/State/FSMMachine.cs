using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zonelink.World;
using Microsoft.Xna.Framework;
using Zonelink.State;

namespace Zonelink
{
    class FSMMachine
    {    
        public FSMState CurrentState  { get; set; }
        public FSMState PreviousState { get; set; }
        public Entity Owner { get; set; }
        

        public FSMMachine(Entity entity)
        {
            Owner = entity;
        }


        public void Update(GameTime gameTime)
        {
            if (CurrentState != null)
                CurrentState.ExecState(Owner, gameTime);
        }

        public void ChangeState(FSMState newState)
        {
            //Keep previous state
            PreviousState = CurrentState;
            CurrentState.ExitState(Owner);

            //Change to new state
            CurrentState = newState;
            CurrentState.EnterState(Owner);
        }
  
        public void RevertToPreviousState()
        {
            ChangeState(PreviousState);
        }

        public bool IsInState(FSMState state) 
        {
            return (CurrentState == state);
        }


        public bool HandleMessage(Message msg)
        {
            if( CurrentState != null )
            {
                return CurrentState.OnHandleMessage(Owner, msg);
            }
            return false;
        }
     }
}
