using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;

namespace Code2015.BalanceSystem
{
    /// <summary>
    ///  表示一个区域
    /// </summary>
    class Region : IUpdatable
    {

        LocalEcoSystem[] lacalecoSystem;
        EnergyStatus energyStatus;
        SocietyStatus societyStatus;

        /// <summary>
        ///  获取该区域的面积
        /// </summary>
        public float Area
        {
            get;
            private set;
        }

        public Object Border
        {
            get;
            private set;
        }
        public Player Owner
        {
            get;
            private set;
        }

        public Region(Player owner, object border) 
        {
            this.Owner = owner;
            this.Border = border;
        }

        public void Update(GameTime time)
        { }
    }
}
