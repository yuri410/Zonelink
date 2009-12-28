using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Code2015.BalanceSystem
{
    public class Player
    {
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// 玩家拥有的钱
        /// </summary>
        public float OwnedMoney
        {
            get;
            set;
        }
        public object OwnedRegion
        {
            get;
            set;
        }

        public int OwnedGrades
        {
            get;
            set;
        }
        public string SetPlayerName(string name)
        {
            return this.Name = name;
        }

        SimulateRegion simulateRegion;
    }
}
