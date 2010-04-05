using System;
using System.Collections.Generic;
using System.Text;
using Code2015.Logic;

namespace Code2015.AI
{
    class AIPlayer : Player
    {
        public AIPlayer()
            : base("Computer")
        {
            Type = PlayerType.LocalAI;
        }


    }
}
