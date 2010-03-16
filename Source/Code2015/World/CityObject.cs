using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Animation;
using Apoc3D.Graphics.Effects;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.BalanceSystem;
using Code2015.EngineEx;
using Code2015.Logic;

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

        public ResourceHandle<ModelData>[] Ring;
        public ResourceHandle<ModelData>[] SelRing;
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

        public Model[] Ring;
        public Model[] SelRing;

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


            Ring = new Model[data.Ring.Length];
            SelRing = new Model[data.SelRing.Length];

            for (int i = 0; i < Ring.Length; i++)
                Ring[i] = new Model(data.Ring[i]);

            for (int i = 0; i < SelRing.Length; i++)
                SelRing[i] = new Model(data.SelRing[i]);



            PluginTranslate = new float[3];

            PluginTranslate[(int)UrbanSize.Large] = Game.ObjectScale * 68;
            PluginTranslate[(int)UrbanSize.Medium] = Game.ObjectScale * 47;
            PluginTranslate[(int)UrbanSize.Small] = Game.ObjectScale * 40;
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
        //public const float Scaler = 1.5384615384615384615384615384615f;

        public CultureId ID;

        public Matrix[] Urban;
        public Matrix[] Base;

        public Matrix OilRefinary;
        public Matrix WoodFactory;
        public Matrix EducationOrgan;
        public Matrix Hospital;
        public Matrix Biofuel;
        public Matrix Cow;
        public Matrix[] Ring;
        public Matrix[] SelRing;

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

            Ring = new Matrix[src.Base.Length];
            Array.Copy(src.Ring, Ring, src.Ring.Length);

            SelRing = new Matrix[src.SelRing.Length];
            Array.Copy(src.SelRing, SelRing, src.SelRing.Length);

            ID = src.ID;
        }
    }

    public class CityStyleTable
    {
        const float RingRadius = 50;

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
        static readonly string Ring_Inv = "cityring.mesh";
        static readonly string SelRing_Inv = "citysel.mesh";


        public const float SmallCityRadius = Game.ObjectScale * 48;
        public const float SmallCityRadiusRing = SmallCityRadius + Game.ObjectScale * 8;
        public const float MediumCityRadius = Game.ObjectScale * 78;
        public const float MediumCityRadiusRing = MediumCityRadius + Game.ObjectScale * 15;
        public const float LargeCityRadius = Game.ObjectScale * 100;
        public const float LargeCityRadiusRing = LargeCityRadius + Game.ObjectScale * 15;

        public const float CitySelRingScale = 1.25f;

        public static readonly float[] CityRadius = new float[] { SmallCityRadius, MediumCityRadius, LargeCityRadius };

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
            styles[0].Ring = new ResourceHandle<ModelData>[3];
            styles[0].SelRing = new ResourceHandle<ModelData>[3];

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

            fl = FileSystem.Instance.Locate(Ring_Inv, GameFileLocs.Model);
            styles[0].Ring[0] = ModelManager.Instance.CreateInstance(rs, fl);
            styles[0].Ring[1] = ModelManager.Instance.CreateInstance(rs, fl);
            styles[0].Ring[2] = ModelManager.Instance.CreateInstance(rs, fl);

            fl = FileSystem.Instance.Locate(SelRing_Inv, GameFileLocs.Model);
            styles[0].SelRing[0] = ModelManager.Instance.CreateInstance(rs, fl);
            styles[0].SelRing[1] = ModelManager.Instance.CreateInstance(rs, fl);
            styles[0].SelRing[2] = ModelManager.Instance.CreateInstance(rs, fl);

            #endregion

            #region 初始化变换调整
            adjusts = new CityObjectTRAdjust[(int)CultureId.Count];

            adjusts[0].Base = new Matrix[3];
            adjusts[0].Urban = new Matrix[3];
            adjusts[0].SelRing = new Matrix[3];
            adjusts[0].Ring = new Matrix[3];


            Matrix scale = Matrix.Scaling(Game.ObjectScale, Game.ObjectScale, Game.ObjectScale);
            for (int i = 0; i < adjusts[0].Base.Length; i++)
                adjusts[0].Base[i] = scale;
            for (int i = 0; i < adjusts[0].Urban.Length; i++)
                adjusts[0].Urban[i] = scale;

            adjusts[0].Urban[(int)UrbanSize.Large] = Matrix.Translation(-20, 33, 0) * Matrix.Scaling(Game.ObjectScale / 1.5f, Game.ObjectScale / 1.5f, Game.ObjectScale / 1.5f);
            adjusts[0].Urban[(int)UrbanSize.Medium] = Matrix.Translation(0, 11, 0) * scale;
            adjusts[0].Urban[(int)UrbanSize.Small] = Matrix.Translation(-8, 3.7f, -2.5f) * scale;

            adjusts[0].WoodFactory = Matrix.Translation(0, 6.25f, 0) * scale;
            adjusts[0].EducationOrgan = Matrix.Translation(0, 4, 0) * scale;
            adjusts[0].Cow = Matrix.RotationY(-MathEx.PiOver2) * scale;// Matrix.Scaling(0, 0, -1);
            adjusts[0].Hospital = Matrix.Translation(0, 2, 0) * scale;
            adjusts[0].OilRefinary = Matrix.Translation(0, 11f, 0) * scale;
            adjusts[0].Biofuel = Matrix.Translation(0, 4.5f, 0) * scale;

            {
                float s = (SmallCityRadiusRing) / RingRadius;
                adjusts[0].Ring[(int)UrbanSize.Small] = Matrix.Translation(22f, 0, 0) * Matrix.Scaling(s, 1, s);
                s = CitySelRingScale * (SmallCityRadiusRing) / RingRadius;
                adjusts[0].SelRing[(int)UrbanSize.Small] = Matrix.Translation(22f, 0, 0) * Matrix.Scaling(s, 1, s);

                s = (MediumCityRadiusRing) / RingRadius;
                adjusts[0].Ring[(int)UrbanSize.Medium] = Matrix.Scaling(s, 1, s);
                s = CitySelRingScale * (MediumCityRadiusRing) / RingRadius;
                adjusts[0].SelRing[(int)UrbanSize.Medium] = Matrix.Scaling(s, 1, s);

                s = (LargeCityRadiusRing) / RingRadius;
                adjusts[0].Ring[(int)UrbanSize.Large] = Matrix.Scaling(s, 1, s);
                s = CitySelRingScale * (LargeCityRadiusRing) / RingRadius;
                adjusts[0].SelRing[(int)UrbanSize.Large] = Matrix.Scaling(s, 1, s);

            }

            #endregion
            //for (CultureId i = CultureId.Asia; i < CultureId.Count; i++)
            //{

            //}
        }

        public CityObjectTRAdjust CreateTRAdjust(CultureId culture)
        {
            return new CityObjectTRAdjust(ref adjusts[(int)culture]);
        }
        float RandomAngle
        {
            get
            {
                return MathEx.PIf * 2 * Randomizer.GetRandomSingle();
            }
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

            style.BiofuelFactory.CurrentAnimation = new NoAnimation(Matrix.RotationY(RandomAngle) * adjusts[(int)culture].Biofuel);
            style.OilRefinary.CurrentAnimation = new NoAnimation(Matrix.RotationY(RandomAngle) * adjusts[(int)culture].OilRefinary);
            style.WoodFactory.CurrentAnimation = new NoAnimation(Matrix.RotationY(RandomAngle) * adjusts[(int)culture].WoodFactory);
            style.EducationOrgan.CurrentAnimation = new NoAnimation(Matrix.RotationY(RandomAngle) * adjusts[(int)culture].EducationOrgan);
            style.Hospital.CurrentAnimation = new NoAnimation(Matrix.RotationY(RandomAngle) * adjusts[(int)culture].Hospital);
            style.Cow.CurrentAnimation = new NoAnimation(adjusts[(int)culture].Cow);

            for (int i = 0; i < style.Ring.Length; i++)
            {
                style.Ring[i].CurrentAnimation = new NoAnimation(adjusts[(int)culture].Ring[i]);
            }
            for (int i = 0; i < style.SelRing.Length; i++)
            {
                style.SelRing[i].CurrentAnimation = new NoAnimation(adjusts[(int)culture].SelRing[i]);
            }
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

    public delegate void CityVisibleHander(CityObject obj);

    public class CityObject : SceneObject, ISelectableObject
    {
        struct PluginEntry
        {
            public CityPlugin plugin;
            public PluginPositionFlag position;
            public Matrix transform;
        }

        City city;
        CityStyle style;
        CityOwnerRing sideRing;

        RenderSystem renderSys;
        SceneManagerBase sceMgr;
        Map map;

        FastList<Harvester> harvesters = new FastList<Harvester>();
        FastList<PluginEntry> plugins;

        PluginPositionFlag pluginFlags;

        FastList<RenderOperation> opBuffer = new FastList<RenderOperation>();

        Vector3 position;


        bool isSelected;

        public City City 
        {
            get { return city; }
        }
        public string Name
        {
            get { return city.Name; }
        }
        public Vector3 Position
        {
            get { return position; }
        }

        public float Longitude
        {
            get { return city.Longitude; }
        }
        public float Latitude
        {
            get { return city.Latitude; }
        }
        public UrbanSize Size
        {
            get { return city.Size; }
        }
        public void Flash(int duration)
        {
            sideRing.Flash(duration);
        }
        public bool IsPlayerCapturing(Player pl) 
        {
            return city.Capture.IsPlayerCapturing(pl);
        }
        public bool CanCapture(Player pl)
        {
            if (Owner != null) 
            {
                return false;
            }
            return city.Capture.CanCapture(pl) && pl.Area.CanCapture(city);
        }
        public bool IsCapturing 
        {
            get { return city.Capture.IsCapturing; }
        }
        public bool IsCaptured 
        {
            get { return city.IsCaptured; }
        }
        public CaptureState Capture 
        {
            get { return city.Capture; }
        }
        public Player Owner
        {
            get { return city.Owner; }
        }

        public int HarvesterCount
        {
            get { return harvesters.Count; }
        }
        public Harvester GetHarvester(int idx) 
        {
            return harvesters[idx];
        }

        bool ISelectableObject.IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; }
        }

        public event CityVisibleHander CityVisible;

        public CityObject(RenderSystem rs, Map map, SceneManagerBase sceMgr, City city, CityStyleTable styleSet)
            : base(false)
        {
            this.city = city;
            this.city.Parent = this;
            this.sceMgr = sceMgr;
            this.map = map;

            this.plugins = new FastList<PluginEntry>();
            this.style = styleSet.CreateStyle(city.Culture);
            this.renderSys = rs;

            city.PluginAdded += City_PluginAdded;
            city.PluginRemoved += City_PluginRemoved;
            city.NearbyCityAdded += City_Linked;
            city.CityOwnerChanged += City_OwnerChanged;

            float radLong = MathEx.Degree2Radian(city.Longitude);
            float radLat = MathEx.Degree2Radian(city.Latitude);

            float altitude = TerrainData.Instance.QueryHeight(radLong, radLat);
            Vector3 pos = PlanetEarth.GetPosition(radLong, radLat, PlanetEarth.PlanetRadius + TerrainMeshManager.PostHeightScale * altitude + 5);

            Transformation = PlanetEarth.GetOrientation(radLong, radLat);

            Transformation.TranslationValue = pos;
            BoundingSphere.Radius = CityStyleTable.CityRadius[(int)city.Size];
            BoundingSphere.Center = pos;
            position = pos;

                if (city.Owner != null)
                    City_OwnerChanged(city.Owner);
            
            sideRing = new CityOwnerRing(this, style);
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

            if (IsCaptured && (plugin.TypeId == CityPluginTypeId.OilRefinary ||
                plugin.TypeId == CityPluginTypeId.WoodFactory))
            {
                Harvester harv = new Harvester(renderSys, map, style.Cow);
                harv.Latitude = MathEx.Degree2Radian(Latitude - 2);
                harv.Longtitude = MathEx.Degree2Radian(Longitude);
                harvesters.Add(harv);
                sceMgr.AddObjectToScene(harv);

                NaturalResource res = plugin.CurrentResource;
                if (res != null)
                {
                    harv.SetAuto(
                        MathEx.Degree2Radian(res.Longitude), MathEx.Degree2Radian(res.Latitude),
                        MathEx.Degree2Radian(Longitude), MathEx.Degree2Radian(Latitude));
                }
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
        void City_OwnerChanged(Player owner)
        {
            if (owner != null)
            {
                //Color4F color = new Color4F(owner.SideColor);
                //ringMaterial.Ambient *= color;
                //ringMaterial.Diffuse *= color;

            }
        }

        public override RenderOperation[] GetRenderOperation()
        {
            opBuffer.FastClear();
            if (CityVisible != null)
            {
                CityVisible(this);
            }

            RenderOperation[] ops = style.Base[(int)city.Size].GetRenderOperation();
            if (ops != null)
                opBuffer.Add(ops);

            ops = style.Urban[(int)city.Size].GetRenderOperation();
            if (ops != null)
                opBuffer.Add(ops);


            for (int i = 0; i < plugins.Count; i++)
            {
                ops = null;
                switch (plugins[i].plugin.TypeId)
                {
                    case CityPluginTypeId.BiofuelFactory:
                        ops = style.BiofuelFactory.GetRenderOperation();
                        break;
                    case CityPluginTypeId.EducationOrg:
                        ops = style.EducationOrgan.GetRenderOperation();
                        break;
                    case CityPluginTypeId.Hospital:
                        ops = style.Hospital.GetRenderOperation();
                        break;
                    case CityPluginTypeId.OilRefinary:
                        ops = style.OilRefinary.GetRenderOperation();
                        break;
                    case CityPluginTypeId.WoodFactory:
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

            if (isSelected)
            {
                ops = style.SelRing[(int)city.Size].GetRenderOperation();
                if (ops != null)
                {
                    opBuffer.Add(ops);
                }
            }

            ops = sideRing.GetRenderOperation();
            if (ops != null)
            {
                opBuffer.Add(ops);
            }

            opBuffer.Trim();
            return opBuffer.Elements;
        }

        public override void Update(GameTime dt)
        {
            BoundingSphere.Radius = CityStyleTable.CityRadius[(int)city.Size];

            sideRing.Update(dt);
        }

        //public override void OnAddedToScene(object sender, SceneManagerBase sceneMgr)
        //{
        //    base.OnAddedToScene(sender, sceneMgr);
        //}

        public override bool IsSerializable
        {
            get { return false; }
        }
    }
}
