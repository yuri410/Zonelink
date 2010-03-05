using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Scene;

namespace Code2015.World
{
    class OilFieldObject : StaticModelObject, ISelectableObject
    {
        
        public override bool IsSerializable
        {
            get { return false ; }
        }

        #region ISelectableObject 成员

        bool ISelectableObject.IsSelected
        {
            get;
            set;
        }

        #endregion
    }
}
