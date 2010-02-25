using System;
using System.Collections.Generic;
using System.Text;
using XFGS = Microsoft.Xna.Framework.GamerServices;

namespace Code2015
{
    public class Player
    {
        XFGS.Gamer gamer;

        public Player(XFGS.Gamer gm)
        {
            this.gamer = gm;
            
        }

        public string Name
        {
            get;
            set;

        }
    }
}
