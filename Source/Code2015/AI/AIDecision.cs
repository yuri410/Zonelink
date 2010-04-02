using System;
using System.Collections.Generic;
using System.Text;
using Code2015.Logic;

namespace Code2015.AI
{
    class AIDecision
    {
        AIPlayer player;
        PlayerArea area;

        public AIDecision(AIPlayer player)
        {
            this.player = player;
            this.area = player.Area;


        }
    }
}
