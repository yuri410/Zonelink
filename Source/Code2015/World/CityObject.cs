using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.Scene;
using Code2015.BalanceSystem;

namespace Code2015.World
{
    struct CityStyle
    {
        public CultureId ID;

        public Model[] Urban;
        
        public Model[] OilRefinary;
        public Model[] WoodFactory;

        public Model[] EducationOrgan;

        public Model[] Hospital;
    }

    class CityStyleTable
    {
        CityStyle[] styles;

        public CityStyleTable()
        {
            styles = new CityStyle[(int)CultureId.Count];

            // initialize all

            //for (CultureId i = CultureId.Asia; i < CultureId.Count; i++)
            //{

            //}
        }

        public CityStyle GetModel(CultureId culture)
        {
            return styles[(int)culture];
        }
    }

    class CityObject : SceneObject
    {
        City city;

        SceneManagerBase sceMgr;

        public CityObject(City city)
            : base(false)
        {
            this.city = city;
        }

        public override RenderOperation[] GetRenderOperation()
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime dt)
        {

        }

        public override void OnAddedToScene(object sender, SceneManagerBase sceneMgr)
        {
            base.OnAddedToScene(sender, sceneMgr);
            this.sceMgr = sceneMgr;
        }

        public override bool IsSerializable
        {
            get { return false; }
        }
    }
}
