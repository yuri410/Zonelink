using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zonelink.World;

namespace Zonelink.State
{
    class Message
    {
        public Entity Sender { get; set; }
        public Entity Receiver { get; set; }
        public EventType Type { get; set; }

        public float DispatchTime { get; set; }
        //Addition Information

        public Message(float time, Entity sender, Entity receiver, EventType type)
        {
            Sender = sender;
            DispatchTime = time;
            Receiver = receiver;
        }
    }
}
