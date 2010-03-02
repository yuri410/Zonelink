using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Animation;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.BalanceSystem;
using Code2015.EngineEx;

namespace Code2015.World
{
    public struct CityStyleData
    {
        public CultureId ID;
        
        public ResourceHandle<ModelData>[] Base;
        public ResourceHandle<ModelData>[] Urban;

        public ResourceHandle<ModelData> OilRefinary;
        public ResourceHandle<ModelData> WoodFactory;
        public ResourceHandle<ModelData> BiofuelFactory;

        public ResourceHandle<ModelData> EducationOrgan;

        public ResourceHandle<ModelData> Hospital;

        public ResourceHandle<ModelData> Cow;
    }
    public struct CityStyle
    {
        public CultureId ID;

        public Model[] Base;
        public Model[] Urban;

        public Model OilRefinary;
        public Model WoodFactory;
        public Model BiofuelFactory;
        public Model EducationOrgan;

        public Model Hospital;

        public Model Cow;

        public float[] PluginTranslate;

        public CityStyle(ref CityStyleData data)
        {
            ID = data.ID;
            Urban = new Model[data.Urban.Length];
            Base = new Model[data.Base.Length];

            Cow = new Model(data.Cow);

            for (int i = 0; i < Base.Length; i++)
                Base[i] = new Model(data.Base[i]);

            for (int i = 0; i < Urban.Length; i++)
                Urban[i] = new Model(data.Urban[i]);

            OilRefinary = new Model(data.OilRefinary);
            WoodFactory = new Model(data.WoodFactory);
            BiofuelFactory = new Model(data.BiofuelFactory);

            Hospital = new Model(data.Hospital);
            EducationOrgan = new Model(data.EducationOrgan);


            PluginTranslate = new float[3];

            PluginTranslate[(int)UrbanSize.Large] = CityObjectTRAdjust.Scaler * 68;
            PluginTranslate[(int)UrbanSize.Medium] = CityObjectTRAdjust.Scaler * 47;
            PluginTranslate[(int)UrbanSize.Small] = CityObjectTRAdjust.Scaler * 40;
        }

        public Vector3 GetPluginTranslation(PluginPositionFlag p, UrbanSize size)
        {
            switch (p)
            {
                case PluginPositionFlag.P1:
                    switch (size)
                    {
                        case UrbanSize.Medium:
                            return new Vector3(PluginTranslate[(int)size], 0, 0);
                        case UrbanSize.Small:
                            return new Vector3(PluginTranslate[(int)size], 0, 0);
                        case UrbanSize.Large:
                            return new Vector3(-PluginTranslate[(int)size], 0, 0);
                        default:
                            throw new ArgumentException();
                    }
                case PluginPositionFlag.P2:
                    switch (size)
                    {
                        case UrbanSize.Medium:
                            return new Vector3(-0.5f * PluginTranslate[(int)size], 0, MathEx.Root3 * 0.5f * PluginTranslate[(int)size]);
                        case UrbanSize.Large:
                            return new Vector3(0, 0, -PluginTranslate[(int)size]);
                        default:
                            throw new ArgumentException();
                    }
                case PluginPositionFlag.P3:
                    switch (size)
                    {
                        case UrbanSize.Medium:
                            return new Vector3(-0.5f * PluginTranslate[(int)size], 0, -MathEx.Root3 * 0.5f * PluginTranslate[(int)size]);
                        case UrbanSize.Large:
                            return new Vector3(PluginTranslate[(int)size], 0, 0);
                        default:
                            throw new ArgumentException();
                    }
                case PluginPositionFlag.P4:
                    switch (size)
                    {
                        case UrbanSize.Large:
                            return new Vector3(0, 0, PluginTranslate[(int)size]);
                        default:
                            throw new ArgumentException();
                    }
            }
            return Vector3.Zero;
        }

        
    }

    public struct CityObjectTRAdjust
    {
        public const float Scaler = 1.5384615384615384615384615384615f;

        public CultureId ID;

        public Matrix[] Urban;
        public Matrix[] Base;

        public Matrix OilRefinary;
        public Matrix WoodFactory;
        public Matrix EducationOrgan;
        public Matrix Hospital;
        public Matrix Biofuel;
        public Matrix Cow;

        public CityObjectTRAdjust(ref CityObjectTRAdjust src)
        {
            OilRefinary = src.OilRefinary;
            WoodFactory = src.WoodFactory;
            EducationOrgan = src.EducationOrgan;
            Hospital = src.Hospital;
            Biofuel = src.Biofuel;
            Cow = src.Cow;

            Urban = new Matrix[src.Urban.Length];
            Array.Copy(src.Urban, Urban, src.Urban.Length);

            Base = new Matrix[src.Base.Length];
            Array.Copy(src.Base, Base, src.Base.Length);

            ID = src.ID;
        }
    }

    public class CityStyleTable
    {
        static readonly string SmallCityCenter_Inv = "small.mesh";
        static readonly string MediumCityCenter_Inv = "medium.mesh";
        static readonly string LargeCityCenter_Inv = "large.mesh";
        static readonly string SmallBase_Inv = "basesmall_l1.mesh";
        static readonly string MediumBase_Inv = "basemedium_l1.mesh";
        static readonly string LargeBase_Inv = "baselarge_l1.mesh";


        static readonly string OilRefinary_Inv = "oilref.mesh";
        static readonly string WoodFactory_Inv = "woodfac.mesh";
        static readonly string BioFuelFactory_Inv = "biofuel.mesh";
        static readonly string EducationOrgan_Inv = "eduorg.mesh";
        static readonly string Hospital_Inv = "hospital.mesh";
        static readonly string Cow_Inv = "cow.mesh";

        CityStyleData[] styles;
        CityObjectTRAdjust[] adjusts;

        public CityStyleTable(RenderSystem rs)
        {
            styles = new CityStyleData[(int)CultureId.Count];

            // initialize all

            #region 初始化默认样式
            styles[0].ID = CultureId.Asia;
            styles[0].Urban = new ResourceHandle<ModelData>[3];
            styles[0].Base = new ResourceHandle<ModelData>[3];

            FileLocation fl = FileSystem.Instance.Locate(SmallCityCenter_Inv, GameFileLocs.Model);
            styles[0].Urban[0] = ModelManager.Instance.CreateInstance(rs, fl);
            fl = FileSystem.Instance.Locate(MediumCityCenter_Inv, GameFileLocs.Model);
            styles[0].Urban[1] = ModelManager.Instance.CreateInstance(rs, fl);
            fl = FileSystem.Instance.Locate(LargeCityCenter_Inv, GameFileLocs.Model);
            styles[0].Urban[2] = ModelManager.Instance.CreateInstance(rs, fl);

            fl = FileSystem.Instance.Locate(SmallBase_Inv, GameFileLocs.Model);
            styles[0].Base[0] = ModelManager.Instance.CreateInstance(rs, fl);
            fl = FileSystem.Instance.Locate(MediumBase_Inv, GameFileLocs.Model);
            styles[0].Base[1] = ModelManager.Instance.CreateInstance(rs, fl);
            fl = FileSystem.Instance.Locate(LargeBase_Inv, GameFileLocs.Model);
            styles[0].Base[2] = ModelManager.Instance.CreateInstance(rs, fl);


            fl = FileSystem.Instance.Locate(OilRefinary_Inv, GameFileLocs.Model);
            styles[0].OilRefinary = ModelManager.Instance.CreateInstance(rs, fl);

            fl = FileSystem.Instance.Locate(WoodFactory_Inv, GameFileLocs.Model);
            styles[0].WoodFactory = ModelManager.Instance.CreateInstance(rs, fl);

            fl = FileSystem.Instance.Locate(BioFuelFactory_Inv, GameFileLocs.Model);
            styles[0].BiofuelFactory = ModelManager.Instance.CreateInstance(rs, fl);

            fl = FileSystem.Instance.Locate(EducationOrgan_Inv, GameFileLocs.Model);
            styles[0].EducationOrgan = ModelManager.Instance.CreateInstance(rs, fl);

            fl = FileSystem.Instance.Locate(Hospital_Inv, GameFileLocs.Model);
            styles[0].Hospital = ModelManager.Instance.CreateInstance(rs, fl);

            fl = FileSystem.Instance.Locate(Cow_Inv, GameFileLocs.Model);
            styles[0].Cow = ModelManager.Instance.CreateInstance(rs, fl);

            #endregion

            #region 初始化变换调整
            adjusts = new CityObjectTRAdjust[(int)CultureId.Count];

            adjusts[0].Base = new Matrix[3];
            adjusts[0].Urban = new Matrix[3];


            Matrix scale = Matrix.Scaling(CityObjectTRAdjust.Scaler, CityObjectTRAdjust.Scaler, CityObjectTRAdjust.Scaler);
            for (int i = 0; i < adjusts[0].Base.Length; i++)
                adjusts[0].Base[i] = scale;
            for (int i = 0; i < adjusts[0].Urban.Length; i++)
                adjusts[0].Urban[i] = scale;

            adjusts[0].Urban[(int)UrbanSize.Large] = Matrix.Translation(-20, 33, 0);
            adjusts[0].Urban[(int)UrbanSize.Medium] = Matrix.Translation(0, 11, 0) * scale;
            adjusts[0].Urban[(int)UrbanSize.Small] = Matrix.Translation(-8, 3.7f, -2.5f) * scale;

            adjusts[0].WoodFactory = Matrix.Translation(0, 6.25f, 0) * scale;
            adjusts[0].EducationOrgan = Matrix.Translation(0, 4, 0) * scale;
            adjusts[0].Cow = Matrix.Scaling(0, 0, -1);
            adjusts[0].Hospital = Matrix.Translation(0, 2, 0) * scale;
            adjusts[0].OilRefinary = Matrix.Translation(0, 11f, 0) * scale;
            adjusts[0].Biofuel = Matrix.Translation(0, 4.5f, 0) * scale;
            #endregion
            //for (CultureId i = CultureId.Asia; i < CultureId.Count; i++)
            //{

            //}
        }

        public CityObjectTRAdjust CreateTRAdjust(CultureId culture) 
        {
            return new CityObjectTRAdjust(ref adjusts[(int)culture]);
        }
        public CityStyle CreateStyle(CultureId culture)
        {
            CityStyleData data = styles[(int)culture];
            
            CityStyle style = new CityStyle(ref data);

            for (int i = 0; i < style.Base.Length; i++) 
            {
                style.Base[i].CurrentAnimation = new NoAnimation(adjusts[(int)culture].Base[i]);
            }
            for (int i = 0; i < style.Urban.Length; i++)
            {
                style.Urban[i].CurrentAnimation = new NoAnimation(adjusts[(int)culture].Urban[i]);
            }

            style.BiofuelFactory.CurrentAnimation = new NoAnimation(adjusts[(int)culture].Biofuel);
            style.OilRefinary.CurrentAnimation = new NoAnimation(adjusts[(int)culture].OilRefinary);
            style.WoodFactory.CurrentAnimation = new NoAnimation(adjusts[(int)culture].WoodFactory);
            style.EducationOrgan.CurrentAnimation = new NoAnimation(adjusts[(int)culture].EducationOrgan);
            style.Hospital.CurrentAnimation = new NoAnimation(adjusts[(int)culture].Hospital);
            style.Cow.CurrentAnimation = new NoAnimation(adjusts[(int)culture].Cow);
            
            return style;
        }
    }

    public enum PluginPositionFlag
    {
        None = 0,
        P1 = 1,
        P2 = 1 << 1,
        P3 = 1 << 2,
        P4 = 1 << 3
    }

    public class CityObject : SceneObject
    {
        struct PluginEntry
        {
            public CityPlugin plugin;
            public PluginPositionFlag position;
            public Matrix transform;
        }

        City city;
        CityStyle style;

        RenderSystem renderSys;
        SceneManagerBase sceMgr;

        FastList<PluginEntry> plugins;

        PluginPositionFlag pluginFlags;

        FastList<RenderOperation> opBuffer = new FastList<RenderOperation>();

        Vector3 position;
        public Vector3 Position
        {
            get { return position; }
        }

        public CityObject(RenderSystem rs, City city, CityStyleTable styleSet)
            : base(false)
        {
            this.city = city;
            city.Parent = this;
            this.plugins = new FastList<PluginEntry>();
            this.style = styleSet.CreateStyle(city.Culture);
            this.renderSys = rs;

            city.PluginAdded += City_PluginAdded;
            city.PluginRemoved += City_PluginRemoved;

            float radLong = MathEx.Degree2Radian(city.Longitude);
            float radLat = MathEx.Degree2Radian(city.Latitude);

            Vector3 pos = PlanetEarth.GetPosition(radLong, radLat, PlanetEarth.PlanetRadius + 150);
            
            Transformation = Matrix.Identity;

            Transformation.Up = PlanetEarth.GetNormal(radLong, radLat);
            Transformation.Right = PlanetEarth.GetTangentX(radLong, radLat);
            Transformation.Forward = -PlanetEarth.GetTangentY(radLong, radLat);

            Transformation.TranslationValue = pos;//Matrix.RotationZ(-radLat) * Matrix.RotationX(-radLong) * 

            BoundingSphere.Radius = 200;
            BoundingSphere.Center = pos;
            position = pos;

            switch (city.Size)
            {
                case UrbanSize.Small:
                    city.Add(new CityPlugin(new CityPluginType("A")));

                    break;
                case UrbanSize.Medium:
                    city.Add(new CityPlugin(new CityPluginType("B")));
                    city.Add(new CityPlugin(new CityPluginType("C")));
                    city.Add(new CityPlugin(new CityPluginType("D")));
                    break;
                case UrbanSize.Large:
                    city.Add(new CityPlugin(new CityPluginType("E")));
                    city.Add(new CityPlugin(new CityPluginType("B")));
                    city.Add(new CityPlugin(new CityPluginType("C")));
                    city.Add(new CityPlugin(new CityPluginType("D")));
                    break;
            }
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

        void City_Linked(City a, City b) 
        {
            if (b != null)
            {
                CityLink link = new CityLink(renderSys, a.Parent, b.Parent);

                sceMgr.AddObjectToScene(link);
            }
        }
        void City_PluginAdded(City city, CityPlugin plugin)
        {
            PluginEntry ent = new PluginEntry();

            if ((pluginFlags & PluginPositionFlag.P1) == 0)
            {
                pluginFlags |= PluginPositionFlag.P1;
                ent.position = PluginPositionFlag.P1;
            }
            else if ((pluginFlags & PluginPositionFlag.P2) == 0)
            {
                pluginFlags |= PluginPositionFlag.P2;
                ent.position = PluginPositionFlag.P2;
            }
            else if ((pluginFlags & PluginPositionFlag.P3) == 0)
            {
                pluginFlags |= PluginPositionFlag.P3;
                ent.position = PluginPositionFlag.P3;
            }
            else if ((pluginFlags & PluginPositionFlag.P4) == 0)
            {
                pluginFlags |= PluginPositionFlag.P4;
                ent.position = PluginPositionFlag.P4;
            }
            ent.plugin = plugin;

            ent.transform = Matrix.Translation(style.GetPluginTranslation(ent.position, city.Size));
            plugins.Add(ent);
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
            opBuffer.FastClear();

            RenderOperation[] ops = style.Base[(int)city.Size].GetRenderOperation();
            if (ops != null)
                opBuffer.Add(ops);

            ops = style.Urban[(int)city.Size].GetRenderOperation();
            if (ops != null)
                opBuffer.Add(ops);


            for (int i = 0; i < plugins.Count; i++)
            {
                ops = null;
                switch (plugins[i].plugin.Type.TypeName)
                {
                    case "A":
                        ops = style.BiofuelFactory.GetRenderOperation();
                        break;
                    case "B":
                        ops = style.EducationOrgan.GetRenderOperation();
                        break;
                    case "C":
                        ops = style.Hospital.GetRenderOperation();
                        break;
                    case "D":
                        ops = style.OilRefinary.GetRenderOperation();
                        break;
                    case "E":
                        ops = style.WoodFactory.GetRenderOperation();
                        break;
                }
                if (ops != null)
                {
                    for (int j = 0; j < ops.Length; j++)
                    {
                        ops[j].Transformation *= plugins[i].transform;
                    }
                    opBuffer.Add(ops);
                }
            }

            opBuffer.Trim();
            return opBuffer.Elements;
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
