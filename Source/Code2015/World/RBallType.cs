using System;
using System.Collections.Generic;
using System.Text;

namespace Code2015.World
{
    /// <summary>
    /// 资源球类型的标识
    /// </summary>
    enum RBallType
    {
        Oil,
        Disease,
        Volience,
        Green,
        Health,
        Education
    }

    ///// <summary>
    /////  表示资源球的类型
    /////  不同类型的资源球不用再从这里继承，通过参数体现
    ///// </summary>
    //class RBallType : EntityType
    //{
    //    public RBallTypeID TypeID { get; set; }


    //    public override void Parse()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
