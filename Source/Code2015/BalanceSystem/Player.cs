﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Code2015.BalanceSystem
{
    public class Player:IUpdatable
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
        /// <summary>
        /// 玩家控制的区域
        /// </summary>
        public object OwnedRegion
        {
            get;
            set;
        }
        /// <summary>
        /// 玩家的积分
        /// </summary>
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

        #region IUpdatable 成员

        public void Update(Apoc3D.GameTime time)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
