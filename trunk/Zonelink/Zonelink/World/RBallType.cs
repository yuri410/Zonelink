using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zonelink.World
{
    /// <summary>
    /// 资源球类型的标识
    /// </summary>
    enum RBallTypeID
    {
        Oil,
        Disease,
        Volience,
        Green,
        Health,
        Education
    }
    /// <summary>
    ///  表示资源球的类型
    ///  不同类型的资源球不用再从这里继承，通过参数体现
    /// </summary>
    class RBallType
    {
        RBallTypeID typeId;
    }
}
