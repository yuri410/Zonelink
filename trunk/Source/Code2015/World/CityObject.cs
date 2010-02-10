using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.Scene;
using Code2015.BalanceSystem;
using Apoc3D.Collections;

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

        public CityStyle GetStyleSet(CultureId culture)
        {
            return styles[(int)culture];
        }
    }


    enum PluginPositionFlag
    {
        P1 = 1,
        P2 = 1 << 1,
        P3 = 1 << 2,
        P4 = 1 << 3
    }

    class CityObject : SceneObject
    {
        struct PluginEntry
        {
            public CityPlugin plugin;
            public PluginPositionFlag position;
        }

        City city;
        CityStyle style;

        SceneManagerBase sceMgr;

        FastList<PluginEntry> plugins;

        PluginPositionFlag pluginFlags;

        public CityObject(City city,CityStyleTable styleSet)
            : base(false)
        {
            this.city = city;
            this.plugins = new FastList<PluginEntry>();
            this.style = styleSet.GetStyleSet(city.Culture);

            city.PluginAdded += City_PluginAdded;
            city.PluginRemoved += City_PluginRemoved;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                city.PluginAdded -= City_PluginAdded;
                city.PluginRemoved -= City_PluginRemoved;
            }
        }
        

        void City_PluginAdded(City city, CityPlugin plugin)
        {

        }
        void City_PluginRemoved(City city, CityPlugin plugin)
        {

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
