using System;
using System.Collections.Generic;
using System.Text;

namespace Code2015.Logic
{
    class GameGoal
    {
        public float Development
        {
            get;
            private set;
        }

        public float DevelopmentPercentage
        {
            get;
            private set;
        }

        public GameGoal(float dev)
        {
            Development = dev;
        }

        public void Check(Player player)
        {
            DevelopmentPercentage = player.Area.GetTotalDevelopment();
        }
    }
}
