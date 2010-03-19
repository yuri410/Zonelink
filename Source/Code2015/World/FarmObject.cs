using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.Scene;
using Code2015.BalanceSystem;

namespace Code2015.World
{
    class FarmObject : SceneObject, ISelectableObject
    {
        FarmLand farm;
        RenderSystem renderSys;


        Model[] tiles;
        Model house;

        public FarmObject(RenderSystem rs, FarmLand farm)
            : base(false)
        {
            this.farm = farm;
            this.renderSys = rs;
        }

        public override RenderOperation[] GetRenderOperation()
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime dt)
        {
            
        }

        public override bool IsSerializable
        {
            get { return false; }
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
