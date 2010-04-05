using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Code2015.BalanceSystem;
using Code2015.Logic;
using Code2015.World;

namespace Code2015.AI
{
    class AIPlayer : Player
    {
        AIDecision decision;

        public AIPlayer()
            : base("Computer")
        {
            Type = PlayerType.LocalAI;
        }

        public override void Update(GameTime time)
        {
            if (decision !=null )
            decision.Update(time);
        }

        public override void SetParent(GameState state)
        {
            decision = new AIDecision(state.SLGWorld, this);
        }
    }
}
