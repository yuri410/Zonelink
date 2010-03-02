using System;
using System.Collections.Generic;
using System.Text;
//using XFGS = Microsoft.Xna.Framework.GamerServices;
using Apoc3D.MathLib;
using Code2015.BalanceSystem;

namespace Code2015.Logic
{
    public class Player
    {
        //XFGS.Gamer gamer;

        public Player(string name)
        {
            this.Name = name;
            
        }

        public string Name
        {
            get;
            set;

        }

        public bool IsLocal
        {
            get { return true; }
        }

        public ColorValue SideColor
        {
            get;
            set;
        }

        /// <summary>
        ///  表示玩家所控制的所有城市
        /// </summary>
        public PlayerArea Area
        {
            get;
            private set;
        }

        public void SetArea(PlayerArea area)
        {
            if (Area == null) 
            {
                Area = area;
            }
        }

    }
}
