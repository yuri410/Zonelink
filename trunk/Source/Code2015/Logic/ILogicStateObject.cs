using System;
using System.Collections.Generic;
using System.Text;
using Code2015.Network;

namespace Code2015.Logic
{
    /// <summary>
    ///  表示游戏逻辑中的状态对象，可以从数据更新状态
    /// </summary>
    interface ILogicStateObject
    {
        void Serialize(StateDataBuffer data);
        void Deserialize(StateDataBuffer data);

        string StateName { get; }
        bool Changed { get; }
    }
}
