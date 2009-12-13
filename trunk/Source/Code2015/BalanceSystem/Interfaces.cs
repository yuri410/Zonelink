using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;

namespace Code2015.BalanceSystem
{
    interface IUpdatable
    {
        void Update(GameTime time);
    }

    /// <summary>
    ///  表示能够产生/吸收碳的对象
    /// </summary>
    interface ICarbon
    {
        /// <summary>
        ///  获取碳变化
        /// </summary>
        /// <returns></returns>
        float CarbonChange { get; }
    }
}
