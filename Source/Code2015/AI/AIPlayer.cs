using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Code2015.BalanceSystem;
using Code2015.Logic;

namespace Code2015.AI
{
    class AIPlayer : Player
    {
        AIDecision decision;

        public AIPlayer(SimulationWorld world)
            : base("Computer")
        {
            Type = PlayerType.LocalAI;
            decision = new AIDecision(world, this);
        }

        public override void Update(GameTime time)
        {
            decision.Update(time);
        }

    }
}
