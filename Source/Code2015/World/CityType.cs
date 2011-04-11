using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zonelink.World
{
    /// <summary>
    /// 城市的类型的标识
    /// </summary>
    enum CityTypeID 
    {
        /// <summary>
        /// 中性城市，无功能
        /// </summary>
        Neutral,

        Oil,
        Disease,
        Volience,
        Green,
        Health,
        Education
    }

    /// <summary>
    ///  表示城市的类型
    ///  不同类型的城市不用再从这里继承，通过参数体现
    /// </summary>
    class CityType : EntityType
    {
        public CityTypeID TypeId { get; protected set; }

        public override void Parse()
        {
            throw new NotImplementedException();
        }
    }
}
