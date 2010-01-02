using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;

namespace Code2015.BalanceSystem
{
    /// <summary>
    ///  表示一个区域
    /// </summary>
    public class SimulateRegion : IUpdatable
    {
        FastList<LocalEcoSystem> localEcoSystem = new FastList<LocalEcoSystem>();
        FastList<SimulateObject> simulateObject = new FastList<SimulateObject>();
        EnergyStatus energyStatus;
        SocietyStatus societyStatus;

        /// <summary>
        ///  模拟区域的面积
        /// </summary>
        public float Area
        {
            get;
            private set;
        }
        /// <summary>
        /// 模拟区域的边界
        /// </summary>
        public Object Border
        {
            get;
            private set;
        }
        /// <summary>
        /// 模拟区域所属玩家
        /// </summary>
        public Player Owner
        {
            get;
            private set;
        }

        public SimulateRegion(Player owner, object border)
        {
            this.Owner = owner;
            this.Border = border;
        }

        public void Update(GameTime time)
        { }


    }
}
