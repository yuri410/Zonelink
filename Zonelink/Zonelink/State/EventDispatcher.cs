using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zonelink.World;
using System.Collections;

namespace Zonelink.State
{
     class EventDispatcher
    {
        public static readonly EventDispatcher Instance = new EventDispatcher();  

        private EventDispatcher() { }

        private void Dispatch(Entity receiver, Message msg)
        {
            receiver.HandleMessage(msg);
        }

        public void DispatchEvent(double delayTime, Entity sender, Entity receiver,
                                    EventType type)
        {
            Dispatch( receiver, new Message(0, sender, receiver, type) );
        }
       
    }
}
