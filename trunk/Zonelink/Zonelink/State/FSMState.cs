using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zonelink.World;
using Zonelink.State;
using Microsoft.Xna.Framework;

namespace Zonelink
{
     interface FSMState
    {
        void EnterState(Entity entity);

        void ExecState(Entity entity, GameTime gameTime);

        void ExitState(Entity entity);

        bool OnHandleMessage(Entity entity, Message msg);
    }
}
