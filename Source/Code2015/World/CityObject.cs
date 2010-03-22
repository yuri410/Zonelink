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
using Code2015.World.Screen;

namespace Code2015.World
{
   
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

            /// <summary>
            ///  附加物的变换矩阵
            /// </summary>
            public Matrix transform;


            public bool HasPiece;
            public MdgType PieceType;

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

        public float Satisfaction
        {
            get { return city.Satisfaction; }
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

        public int PluginCount
        {
            get { return city.PluginCount; }
        }
        public Matrix GetPluginTransform(int i)
        {
            return plugins[i].transform;
        }
        public Vector3 GetPluginPosition(int i) 
        {

            return style.GetPluginTranslation(plugins[i].position, city.Size);
        }
        public CityPlugin GetPlugin(int i) 
        {
            return plugins[i].plugin;
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
