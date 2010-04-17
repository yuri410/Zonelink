using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.MathLib;
using Code2015.BalanceSystem;
using Code2015.World;

namespace Code2015.Logic
{
    public enum PlayerType
    {
        LocalAI,
        LocalHuman,
        Remote
    }

    public class Player
    {
        public GameGoal Goal
        {
            get;
            private set;
        }

        public Player(string name, GameGoal goal)
        {
            this.Goal = goal;
            this.Name = name;
            this.Type = PlayerType.LocalHuman;
        }

        public string Name
        {
            get;
            set;

        }

        public PlayerType Type
        {
            get;
            protected set;
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

        public virtual void SetParent(GameState state) { }
        public void SetArea(PlayerArea area)
        {
            if (Area == null) 
            {
                Area = area;
            }
        }

        public bool Win
        {
            get;
            private set;
        }


        public virtual void Update(GameTime time)
        {
            if (Goal != null)
            {
                Goal.Check(this);

                Win |= Goal.DevelopmentPercentage >= 1;
            }
        }
    }
}
