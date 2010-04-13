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
    public struct CityStyleData
    {
        public CultureId ID;

        public ResourceHandle<ModelData>[] Base;
        public ResourceHandle<ModelData>[] Urban;

        public ResourceHandle<ModelData> OilRefinary;
        public ResourceHandle<ModelData> WoodFactory;
        public ResourceHandle<ModelData> BiofuelFactory;


        public ResourceHandle<ModelData> FarmLand;

        public ResourceHandle<ModelData> EducationOrgan;

        public ResourceHandle<ModelData> Hospital;

        public ResourceHandle<ModelData> Cow;

        public ResourceHandle<ModelData> Ring;
        public ResourceHandle<ModelData> SelRing;

        public ResourceHandle<ModelData> MdgSiteInactive;
        public ResourceHandle<ModelData>[] MdgSiteEmpty;
        public ResourceHandle<ModelData>[] MdgSite;
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

        public Model FarmLand;

        public Model Hospital;

        public Model Cow;

        public Model Ring;
        public Model SelRing;

        public Model MdgSiteInactive;
        public Model[] MdgSiteEmpty;
        public Model[] MdgSite;

        public float PluginTranslate;

        public const float BracketTranslate = 88;

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

            FarmLand = new Model(data.FarmLand);
            Ring = new Model(data.Ring);
            SelRing = new Model(data.SelRing);

            MdgSite = new Model[data.MdgSite.Length];
            for (int i = 0; i < MdgSite.Length; i++)
                MdgSite[i] = new Model(data.MdgSite[i]);

            MdgSiteEmpty = new Model[data.MdgSiteEmpty.Length];
            for (int i = 0; i < MdgSiteEmpty.Length; i++)
                MdgSiteEmpty[i] = new Model(data.MdgSiteEmpty[i]);

            MdgSiteInactive = new Model(data.MdgSiteInactive);
            //PluginTranslate = new float[3];

            PluginTranslate = Game.ObjectScale * 68;
        }

        public Vector3 GetPluginTranslation(PluginPositionFlag p, UrbanSize size)
        {
            switch (p)
            {
                case PluginPositionFlag.P1:
                    return new Vector3(
                       PluginTranslate * MathEx.Root2 * 0.5f,
                       0,
                       -PluginTranslate * MathEx.Root2 * 0.5f);
                case PluginPositionFlag.P2:
                    return new Vector3(
                       -PluginTranslate * MathEx.Root2 * 0.5f,
                       0,
                       -PluginTranslate * MathEx.Root2 * 0.5f);
                case PluginPositionFlag.P3:
                    return new Vector3(
                        -PluginTranslate * MathEx.Root2 * 0.5f,
                        0,
                        PluginTranslate * MathEx.Root2 * 0.5f);
                case PluginPositionFlag.P4:
                    return new Vector3(
                        PluginTranslate * MathEx.Root2 * 0.5f,
                        0,
                        PluginTranslate * MathEx.Root2 * 0.5f);
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

        public Matrix Farm;
        public Matrix OilRefinary;
        public Matrix WoodFactory;
        public Matrix EducationOrgan;
        public Matrix Hospital;
        public Matrix Biofuel;
        public Matrix Cow;
        public Matrix Ring;
        public Matrix SelRing;

        public CityObjectTRAdjust(ref CityObjectTRAdjust src)
        {
            OilRefinary = src.OilRefinary;
            WoodFactory = src.WoodFactory;
            EducationOrgan = src.EducationOrgan;
            Hospital = src.Hospital;
            Biofuel = src.Biofuel;
            Cow = src.Cow;
            Farm = src.Farm;

            Urban = new Matrix[src.Urban.Length];
            Array.Copy(src.Urban, Urban, src.Urban.Length);

            Base = new Matrix[src.Base.Length];
            Array.Copy(src.Base, Base, src.Base.Length);

            Ring = src.Ring;
            SelRing = src.SelRing;
            //Ring = new Matrix[src.Base.Length];
            //Array.Copy(src.Ring, Ring, src.Ring.Length);

            //SelRing = new Matrix[src.SelRing.Length];
            //Array.Copy(src.SelRing, SelRing, src.SelRing.Length);

            ID = src.ID;
        }
    }

    public class CityStyleTable
    {
        public static Matrix[] FarmTransform;
        public static Matrix[] SiteTransform;

        static CityStyleTable()
        {
            const float FarmRadius = Game.ObjectScale * 75;
            FarmTransform = new Matrix[City.MaxFarmLand];

            for (int i = 0; i < City.MaxFarmLand; i++)
            {
                FarmTransform[i] = Matrix.Translation(0, 5, FarmRadius) * Matrix.RotationY(i * MathEx.PiOver2);
            }

            SiteTransform = new Matrix[CityGoalSite.SiteCount];
            for (int i = 0; i < CityGoalSite.SiteCount; i++)
            {
                SiteTransform[i] = Matrix.Scaling(Game.ObjectScale * 2, Game.ObjectScale * 2, Game.ObjectScale * 2) * 
                    Matrix.Translation(CityRadiusRing, 5, 0) * Matrix.RotationY(i * MathEx.PiOver2 + MathEx.PiOver4);
            }
        }

        const float RingRadius = 50;

        static readonly string SmallCityCenter_Inv = "small.mesh";
        static readonly string MediumCityCenter_Inv = "medium.mesh";
        static readonly string LargeCityCenter_Inv = "large.mesh";
        static readonly string SmallBase_Inv = "basesmall.mesh";
        static readonly string MediumBase_Inv = "basemedium.mesh";
        static readonly string LargeBase_Inv = "baselarge.mesh";

        static readonly string FarmLand_Inv = "farm.mesh";
        static readonly string OilRefinary_Inv = "oilref.mesh";
        static readonly string WoodFactory_Inv = "woodfac.mesh";
        static readonly string BioFuelFactory_Inv = "biofuel.mesh";
        static readonly string EducationOrgan_Inv = "eduorg.mesh";
        static readonly string Hospital_Inv = "hospital.mesh";
        static readonly string Cow_Inv = "cow.mesh";
        static readonly string Ring_Inv = "cityring.mesh";
        static readonly string SelRing_Inv = "citysel.mesh";
        static readonly string SiteBase_Inv = "sitebase.mesh";

        static readonly string[] GoalSites = new string[] 
        {
            "goal1site.mesh", "goal2site.mesh", "goal3site.mesh", 
            "goal4site.mesh", "goal5site.mesh", "goal6site.mesh", "goal7site.mesh"
        };

        //public const float SmallCityRadius = Game.ObjectScale * 48;
        //public const float SmallCityRadiusRing = SmallCityRadius + Game.ObjectScale * 8;
        //public const float MediumCityRadius = Game.ObjectScale * 78;
        //public const float MediumCityRadiusRing = MediumCityRadius + Game.ObjectScale * 15;
        public const float CityRadiusDeg = 3.5f;
        public const float CityRadius = Game.ObjectScale * 100;
        public const float CityRadiusRing = CityRadius + Game.ObjectScale * 15;

        public const float CitySelRingScale = 1.25f;

        //public static readonly float CityRadius = LargeCityRadius;// new float[] { SmallCityRadius, MediumCityRadius, LargeCityRadius };

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


            fl = FileSystem.Instance.Locate(FarmLand_Inv, GameFileLocs.Model);
            styles[0].FarmLand = ModelManager.Instance.CreateInstance(rs, fl);

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
            styles[0].Ring = ModelManager.Instance.CreateInstance(rs, fl);
            //styles[0].Ring[1] = ModelManager.Instance.CreateInstance(rs, fl);
            //styles[0].Ring[2] = ModelManager.Instance.CreateInstance(rs, fl);

            fl = FileSystem.Instance.Locate(SelRing_Inv, GameFileLocs.Model);
            styles[0].SelRing = ModelManager.Instance.CreateInstance(rs, fl);
            //styles[0].SelRing[1] = ModelManager.Instance.CreateInstance(rs, fl);
            //styles[0].SelRing[2] = ModelManager.Instance.CreateInstance(rs, fl);

            fl = FileSystem.Instance.Locate(SiteBase_Inv, GameFileLocs.Model);
            styles[0].MdgSiteInactive = ModelManager.Instance.CreateInstance(rs, fl);

            styles[0].MdgSite = new ResourceHandle<ModelData>[(int)MdgType.Count - 1];
            styles[0].MdgSiteEmpty = new ResourceHandle<ModelData>[(int)MdgType.Count - 1];
            for (int i = 0; i < 7; i++)
            {
                fl = FileSystem.Instance.Locate(GoalSites[i], GameFileLocs.Model);
                styles[0].MdgSite[i] = ModelManager.Instance.CreateInstance(rs, fl);
                styles[0].MdgSiteEmpty[i] = ModelManager.Instance.CreateInstance(rs, fl);
            }

            #endregion

            #region 初始化变换调整
            adjusts = new CityObjectTRAdjust[(int)CultureId.Count];

            adjusts[0].Base = new Matrix[3];
            adjusts[0].Urban = new Matrix[3];
            //adjusts[0].SelRing = new Matrix[3];
            //adjusts[0].Ring = new Matrix[3];


            for (int i = 0; i < adjusts[0].Base.Length; i++)
                adjusts[0].Base[i] = Matrix.Scaling(Game.ObjectScale * 1.75f, Game.ObjectScale * 1.75f, Game.ObjectScale * 1.75f);
            //for (int i = 0; i < adjusts[0].Urban.Length; i++)
            //    adjusts[0].Urban[i] = scale;
            Matrix scale = Matrix.Scaling(Game.ObjectScale, Game.ObjectScale, Game.ObjectScale);

            adjusts[0].Urban[(int)UrbanSize.Large] = Matrix.Translation(26, 1, 7) * Matrix.Scaling(Game.ObjectScale / 1.2f, Game.ObjectScale / 1.2f, Game.ObjectScale / 1.2f);
            adjusts[0].Urban[(int)UrbanSize.Medium] = Matrix.Scaling(Game.ObjectScale / 3.05f, Game.ObjectScale / 3.05f, Game.ObjectScale / 3.05f) * Matrix.Translation(-36, 3, 25);
            adjusts[0].Urban[(int)UrbanSize.Small] = Matrix.Scaling(Game.ObjectScale * 2.36f, Game.ObjectScale * 2.36f, Game.ObjectScale * 2.36f) * Matrix.Translation(9, 1, -18);

            adjusts[0].Farm = Matrix.Scaling(Game.ObjectScale, Game.ObjectScale, Game.ObjectScale) * Matrix.RotationY(MathEx.PiOver2);
            adjusts[0].WoodFactory = Matrix.Translation(0, 1, 0) * scale;
            adjusts[0].EducationOrgan = Matrix.Translation(0, 1, 0);
            adjusts[0].Cow = Matrix.RotationY(-MathEx.PiOver2) * Matrix.Translation(0, 7, 0) * scale; // Matrix.Scaling(0, 0, -1);
            adjusts[0].Hospital = Matrix.Translation(0, 1, 0);
            adjusts[0].OilRefinary = Matrix.Translation(0, 11f, 0) * scale;
            adjusts[0].Biofuel = Matrix.Translation(0, 1, 0) * scale;
            //adjusts[0].Urban[(int)UrbanSize.Large] = Matrix.Translation(-20, 33, 0) * Matrix.Scaling(Game.ObjectScale / 1.5f, Game.ObjectScale / 1.5f, Game.ObjectScale / 1.5f);
            //adjusts[0].Urban[(int)UrbanSize.Medium] = Matrix.Translation(0, 11, 0) * scale;
            //adjusts[0].Urban[(int)UrbanSize.Small] = Matrix.Translation(-8, 3.7f, -2.5f) * scale;

            //adjusts[0].WoodFactory = Matrix.Translation(0, 6.25f, 0) * scale;
            //adjusts[0].EducationOrgan = Matrix.Translation(0, 4, 0) * scale;
            //adjusts[0].Cow = Matrix.RotationY(-MathEx.PiOver2) * Matrix.Translation(0, 7, 0) * scale; // Matrix.Scaling(0, 0, -1);
            //adjusts[0].Hospital = Matrix.Translation(0, 2, 0) * scale;
            //adjusts[0].OilRefinary = Matrix.Translation(0, 11f, 0) * scale;
            //adjusts[0].Biofuel = Matrix.Translation(0, 4.5f, 0) * scale;

            {
                float s = (CityRadiusRing) / RingRadius;
                adjusts[0].Ring = Matrix.Scaling(s, 1, s);
                s = CitySelRingScale * (CityRadiusRing) / RingRadius;
                adjusts[0].SelRing = Matrix.Scaling(s, 1, s);

                //s = (LargeCityRadiusRing) / RingRadius;
                //adjusts[0].Ring[(int)UrbanSize.Medium] = Matrix.Scaling(s, 1, s);
                //s = CitySelRingScale * (LargeCityRadiusRing) / RingRadius;
                //adjusts[0].SelRing[(int)UrbanSize.Medium] = Matrix.Scaling(s, 1, s);

                //s = (LargeCityRadiusRing) / RingRadius;
                //adjusts[0].Ring[(int)UrbanSize.Large] = Matrix.Scaling(s, 1, s);
                //s = CitySelRingScale * (LargeCityRadiusRing) / RingRadius;
                //adjusts[0].SelRing[(int)UrbanSize.Large] = Matrix.Scaling(s, 1, s);

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
            style.FarmLand.CurrentAnimation = new NoAnimation(adjusts[(int)culture].Farm);

            style.Ring.CurrentAnimation = new NoAnimation(adjusts[(int)culture].Ring);
            style.SelRing.CurrentAnimation = new NoAnimation(adjusts[(int)culture].SelRing);

            return style;
        }
    }

}
