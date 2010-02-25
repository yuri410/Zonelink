﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.BalanceSystem;
using Code2015.EngineEx;
using Apoc3D.MathLib;

namespace Code2015.World
{
    struct CityStyleData
    {
        public CultureId ID;

        public ResourceHandle<ModelData>[] Urban;

        public ResourceHandle<ModelData>[] OilRefinary;
        public ResourceHandle<ModelData>[] WoodFactory;
        public ResourceHandle<ModelData>[] BiofuelFactory;

        public ResourceHandle<ModelData>[] EducationOrgan;

        public ResourceHandle<ModelData>[] Hospital;

        public ResourceHandle<ModelData> Cow;
    }
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
        static readonly string SmallCityCenter_Inv = "small.mesh";
        static readonly string MediumCityCenter_Inv = "medium.mesh";
        static readonly string LargeCityCenter_Inv = "large.mesh";

        static readonly string OilRefinary_Inv = "oilref.mesh";
        static readonly string WoodFactory_Inv = "woodfac.mesh";
        static readonly string BioFuelFactory_Inv = "biofuel.mesh";
        static readonly string EducationOrgan_Inv = "eduorg.mesh";
        static readonly string Hospital_Inv = "hospital.mesh";
        static readonly string Cow_Inv = "cow.mesh";

        CityStyleData[] styles;

        public CityStyleTable(RenderSystem rs)
        {
            styles = new CityStyleData[(int)CultureId.Count];

            // initialize all
            styles[0].ID = CultureId.Asia;
            styles[0].Urban = new ResourceHandle<ModelData>[3];

            FileLocation fl = FileSystem.Instance.Locate(SmallCityCenter_Inv, GameFileLocs.Model);
            styles[0].Urban[0] = ModelManager.Instance.CreateInstance(rs, fl);
            fl = FileSystem.Instance.Locate(MediumCityCenter_Inv, GameFileLocs.Model);
            styles[0].Urban[1] = ModelManager.Instance.CreateInstance(rs, fl);
            fl = FileSystem.Instance.Locate(LargeCityCenter_Inv, GameFileLocs.Model);
            styles[0].Urban[2] = ModelManager.Instance.CreateInstance(rs, fl);


            styles[0].OilRefinary = new ResourceHandle<ModelData>[1];
            styles[0].WoodFactory = new ResourceHandle<ModelData>[1];
            styles[0].BiofuelFactory = new ResourceHandle<ModelData>[1];
            styles[0].EducationOrgan = new ResourceHandle<ModelData>[1];
            styles[0].Hospital = new ResourceHandle<ModelData>[1];

            fl = FileSystem.Instance.Locate(OilRefinary_Inv, GameFileLocs.Model);
            styles[0].OilRefinary[0] = ModelManager.Instance.CreateInstance(rs, fl);

            fl = FileSystem.Instance.Locate(WoodFactory_Inv, GameFileLocs.Model);
            styles[0].WoodFactory[0] = ModelManager.Instance.CreateInstance(rs, fl);

            fl = FileSystem.Instance.Locate(BioFuelFactory_Inv, GameFileLocs.Model);
            styles[0].BiofuelFactory[0] = ModelManager.Instance.CreateInstance(rs, fl);

            fl = FileSystem.Instance.Locate(EducationOrgan_Inv, GameFileLocs.Model);
            styles[0].EducationOrgan[0] = ModelManager.Instance.CreateInstance(rs, fl);

            fl = FileSystem.Instance.Locate(Hospital_Inv, GameFileLocs.Model);
            styles[0].Hospital[0] = ModelManager.Instance.CreateInstance(rs, fl);

            fl = FileSystem.Instance.Locate(Cow_Inv, GameFileLocs.Model);
            styles[0].Cow = ModelManager.Instance.CreateInstance(rs, fl);
            //for (CultureId i = CultureId.Asia; i < CultureId.Count; i++)
            //{

            //}
        }

        public CityStyle CreateStyle(CultureId culture)
        {
            CityStyle result;

            result.ID = culture;

            CityStyleData data = styles[(int)culture];
            result.Urban = new Model[data.Urban.Length];
            result.OilRefinary = new Model[data.OilRefinary.Length];
            result.WoodFactory = new Model[data.WoodFactory.Length];
            result.Hospital = new Model[data.Hospital.Length];
            result.EducationOrgan = new Model[data.EducationOrgan.Length];

            for (int i = 0; i < result.Urban.Length; i++)
                result.Urban[i] = new Model(data.Urban[i]);

            for (int i = 0; i < result.OilRefinary.Length; i++)
                result.OilRefinary[i] = new Model(data.OilRefinary[i]);

            for (int i = 0; i < result.WoodFactory.Length; i++)
                result.WoodFactory[i] = new Model(data.WoodFactory[i]);

            for (int i = 0; i < result.Hospital.Length; i++)
                result.Hospital[i] = new Model(data.Hospital[i]);
            
            for (int i = 0; i < result.EducationOrgan.Length; i++)
                result.EducationOrgan[i] = new Model(data.EducationOrgan[i]);


            return result;
        }
    }


    enum PluginPositionFlag
    {
        None = 0,
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

        RenderOperation[] opBuffer;



        public CityObject(City city, CityStyleTable styleSet)
            : base(false)
        {
            this.city = city;
            this.plugins = new FastList<PluginEntry>();
            this.style = styleSet.CreateStyle(city.Culture);

            city.PluginAdded += City_PluginAdded;
            city.PluginRemoved += City_PluginRemoved;

            Vector3 pos = PlanetEarth.GetPosition(city.Longitude, city.Latitude);
            Transformation = Matrix.Translation(pos);//Matrix.RotationX(MathEx.PiOver2) * 
                //Matrix.Translation(0, 0, PlanetEarth.PlanetRadius) * Matrix.RotationX(city.Latitude) * Matrix.RotationY(city.Longitude);
            BoundingSphere.Radius = float.MaxValue;
            BoundingSphere.Center = Vector3.Zero;
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
            if ((pluginFlags & PluginPositionFlag.P1) == 0)
            {
                pluginFlags |= PluginPositionFlag.P1;
                PluginEntry ent;
                ent.plugin = plugin;
                ent.position = PluginPositionFlag.P1;
                plugins.Add(ent);
            }
            if ((pluginFlags & PluginPositionFlag.P2) == 0) 
            {
                pluginFlags |= PluginPositionFlag.P2;
                PluginEntry ent;
                ent.plugin = plugin;
                ent.position = PluginPositionFlag.P2;
                plugins.Add(ent);
            }
            if ((pluginFlags & PluginPositionFlag.P3) == 0)
            {
                pluginFlags |= PluginPositionFlag.P3;
                PluginEntry ent;
                ent.plugin = plugin;
                ent.position = PluginPositionFlag.P3;
                plugins.Add(ent);
            }
            if ((pluginFlags & PluginPositionFlag.P4) == 0)
            {
                pluginFlags |= PluginPositionFlag.P4;
                PluginEntry ent;
                ent.plugin = plugin;
                ent.position = PluginPositionFlag.P4;
                plugins.Add(ent);
            }
        }
        void City_PluginRemoved(City city, CityPlugin plugin)
        {
            for (int i = 0; i < plugins.Count; i++)
            {
                if (object.ReferenceEquals(plugin, plugins[i].plugin))
                {
                    pluginFlags ^= plugins[i].position;
                    break;
                }
            }
        }

        

        public override RenderOperation[] GetRenderOperation()
        {
            return style.Urban[2].GetRenderOperation();
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
