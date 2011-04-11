/*
-----------------------------------------------------------------------------
This source file is part of Zonelink

Copyright (c) 2009+ Tao Games

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  if not, write to the Free Software Foundation, 
Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA, or go to
http://www.gnu.org/copyleft/gpl.txt.

-----------------------------------------------------------------------------
*/
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


        public ResourceHandle<ModelData> FarmLand;

        public ResourceHandle<ModelData> EducationOrgan;

        public ResourceHandle<ModelData> Hospital;

        public ResourceHandle<ModelData>[] Cow;

        public ResourceHandle<ModelData> Ring;
        public ResourceHandle<ModelData> SelRing;

        public ResourceHandle<ModelData> MdgSiteInactive;
        public ResourceHandle<ModelData>[] MdgSiteEmpty;
        public ResourceHandle<ModelData>[] MdgSiteFull;


        public ResourceHandle<ModelData>[] MdgGoalIconHL;
        public ResourceHandle<ModelData>[] MdgGoalIcon;
        public ResourceHandle<ModelData>[] MdgGoalIconGray;
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

        public Model[] Cow;

        public Model Ring;
        public Model SelRing;

        public Model MdgSiteInactive;
        public Model[] MdgSiteEmpty;
        public Model[] MdgSiteFull;
        public Model[] MdgGoalIcon;
        public Model[] MdgGoalIconHL;
        public Model[] MdgGoalIconGray;

        public float PluginTranslate;

        public const float BracketTranslate = 88;

        public CityStyle(ref CityStyleData data)
        {
            ID = data.ID;
            Urban = new Model[data.Urban.Length];
            Base = new Model[data.Base.Length];

            Cow = new Model[data.Cow.Length];
            for (int i = 0; i < Cow.Length; i++)
                Cow[i] = new Model(data.Cow[i]);

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

            Matrix siteScale2 = Matrix.Scaling(0.6f, 0.6f, 0.6f);
            Matrix siteScale = Matrix.Scaling(0.6f, 0.6f, 0.6f);
            MdgSiteFull = new Model[data.MdgSiteFull.Length];
            for (int i = 0; i < MdgSiteFull.Length; i++)
            {
                MdgSiteFull[i] = new Model(data.MdgSiteFull[i]);
                MdgSiteFull[i].CurrentAnimation.Clear();
                MdgSiteFull[i].CurrentAnimation.Add(new NoAnimaionPlayer(siteScale));
            }

            MdgSiteEmpty = new Model[data.MdgSiteEmpty.Length];
            for (int i = 0; i < MdgSiteEmpty.Length; i++)
            {
                MdgSiteEmpty[i] = new Model(data.MdgSiteEmpty[i]);
                MdgSiteEmpty[i].CurrentAnimation.Clear();
                MdgSiteEmpty[i].CurrentAnimation.Add(new NoAnimaionPlayer(siteScale));
            }

            MdgGoalIcon = new Model[data.MdgGoalIcon.Length];
            for (int i = 0; i < MdgGoalIcon.Length; i++)
                MdgGoalIcon[i] = new Model(data.MdgGoalIcon[i]);
            

            MdgGoalIconHL = new Model[data.MdgGoalIconHL.Length];
            for (int i = 0; i < MdgGoalIconHL.Length; i++)
                MdgGoalIconHL[i] = new Model(data.MdgGoalIconHL[i]);

            MdgGoalIconGray = new Model[data.MdgGoalIconGray.Length];
            for (int i = 0; i < MdgGoalIconGray.Length; i++)
            {
                MdgGoalIconGray[i] = new Model(data.MdgGoalIconGray[i]);
                MdgGoalIconGray[i].CurrentAnimation.Clear();
                MdgGoalIconGray[i].CurrentAnimation.Add(new NoAnimaionPlayer(siteScale));
            }

            MdgSiteInactive = new Model(data.MdgSiteInactive);
            MdgSiteInactive.CurrentAnimation.Clear();
            MdgSiteInactive.CurrentAnimation.Add(new NoAnimaionPlayer(siteScale));

            PluginTranslate = Game.ObjectScale * 68;
        }

        public Vector3 GetPluginTranslation(int p)
        {
            switch (p)
            {
                case 0:
                    return new Vector3(
                       PluginTranslate * MathEx.Root2 * 0.5f,
                       0,
                       -PluginTranslate * MathEx.Root2 * 0.5f);
                case 1:
                    return new Vector3(
                       -PluginTranslate * MathEx.Root2 * 0.5f,
                       0,
                       -PluginTranslate * MathEx.Root2 * 0.5f);
                case 2:
                    return new Vector3(
                        -PluginTranslate * MathEx.Root2 * 0.5f,
                        0,
                        PluginTranslate * MathEx.Root2 * 0.5f);
                case 3:
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
        const int CowFrameCount = 30;

        public static Matrix[] FarmTransform;
        public static Matrix[] SiteTransform;

        static CityStyleTable()
        {
            const float FarmRadius = Game.ObjectScale * 85;
            FarmTransform = new Matrix[City.MaxFarmLand];

            for (int i = 0; i < City.MaxFarmLand; i++)
            {
                FarmTransform[i] = Matrix.Translation(0, 5, FarmRadius) * Matrix.RotationY(i * MathEx.PiOver2);
            }

            SiteTransform = new Matrix[CityGoalSite.SiteCount];
            for (int i = 0; i < CityGoalSite.SiteCount; i++)
            {
                Matrix a = Matrix.Translation(CityRadiusRing, 1, 0) * Matrix.RotationY(i * MathEx.PiOver2 + MathEx.PiOver4);

                SiteTransform[i] = Matrix.Scaling(Game.ObjectScale, Game.ObjectScale, Game.ObjectScale) * Matrix.Translation(a.TranslationValue);
            }
        }

        const float RingRadius = 100;

        static readonly string SmallCityCenter_Am = "mz_small.mesh";
        static readonly string MediumCityCenter_Am = "mz_medium.mesh";
        static readonly string LargeCityCenter_Am = "mz_large.mesh";

        static readonly string SmallCityCenter_Af = "fz_small.mesh";
        static readonly string MediumCityCenter_Af = "fz_medium.mesh";
        static readonly string LargeCityCenter_Af = "fz_large.mesh";
        static readonly string SmallCityCenter_Er = "oz_small.mesh";
        static readonly string MediumCityCenter_Er = "oz_medium.mesh";
        static readonly string LargeCityCenter_Er = "oz_large.mesh";

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
        static readonly string Cow_Inv = "cow";
        static readonly string Ring_Inv = "cityring.mesh";
        static readonly string SelRing_Inv = "citysel.mesh";
        static readonly string SiteBase_Inv = "sitebase.mesh";

        static readonly string[] GoalSiteIconHL = new string[] 
        {
            "goal1hl.mesh", "goal2hl.mesh", "goal3hl.mesh", 
            "goal4hl.mesh", "goal5hl.mesh", "goal6hl.mesh", "goal7hl.mesh"
        };
        static readonly string[] GoalSiteIconGray = new string[] 
        {
            "goal1gray.mesh", "goal2gray.mesh", "goal3gray.mesh", 
            "goal4gray.mesh", "goal5gray.mesh", "goal6gray.mesh", "goal7gray.mesh"
        };
        static readonly string[] GoalSiteIcon = new string[] 
        {
            "goal1.mesh", "goal2.mesh", "goal3.mesh", 
            "goal4.mesh", "goal5.mesh", "goal6.mesh", "goal7.mesh"
        };
        static readonly string[] GoalSitesEmptyTyped = new string[] 
        {
            "goal1site0.mesh", "goal2site0.mesh", "goal3site0.mesh", 
            "goal4site0.mesh", "goal5site0.mesh", "goal6site0.mesh", "goal7site0.mesh"
        };
        static readonly string[] GoalSitesFullTyped = new string[] 
        {
            "goal1site1.mesh", "goal2site1.mesh", "goal3site1.mesh", 
            "goal4site1.mesh", "goal5site1.mesh", "goal6site1.mesh", "goal7site1.mesh"
        };


        public const float CityRadiusDeg = 3.5f;
        public const float CityRadius = Game.ObjectScale * 100;
        public const float CityRadiusRing = CityRadius + Game.ObjectScale * 15;

        public const float CitySelRingScale = 2.6f;


        CityStyleData[] styles;
        CityObjectTRAdjust[] adjusts;

        void BuildCommon(RenderSystem rs, ref CityStyleData style)
        {

            FileLocation fl = FileSystem.Instance.Locate(FarmLand_Inv, GameFileLocs.Model);
            style.FarmLand = ModelManager.Instance.CreateInstance(rs, fl);

            fl = FileSystem.Instance.Locate(OilRefinary_Inv, GameFileLocs.Model);
            style.OilRefinary = ModelManager.Instance.CreateInstance(rs, fl);

            fl = FileSystem.Instance.Locate(WoodFactory_Inv, GameFileLocs.Model);
            style.WoodFactory = ModelManager.Instance.CreateInstance(rs, fl);

            fl = FileSystem.Instance.Locate(BioFuelFactory_Inv, GameFileLocs.Model);
            style.BiofuelFactory = ModelManager.Instance.CreateInstance(rs, fl);

            fl = FileSystem.Instance.Locate(EducationOrgan_Inv, GameFileLocs.Model);
            style.EducationOrgan = ModelManager.Instance.CreateInstance(rs, fl);

            fl = FileSystem.Instance.Locate(Hospital_Inv, GameFileLocs.Model);
            style.Hospital = ModelManager.Instance.CreateInstance(rs, fl);

            style.Cow = new ResourceHandle<ModelData>[CowFrameCount];
            for (int i = 0; i < CowFrameCount; i++)
            {
                fl = FileSystem.Instance.Locate(Cow_Inv + i.ToString("D2") + ".mesh", GameFileLocs.Model);
                style.Cow[i] = ModelManager.Instance.CreateInstance(rs, fl);
            }

            fl = FileSystem.Instance.Locate(Ring_Inv, GameFileLocs.Model);
            style.Ring = ModelManager.Instance.CreateInstance(rs, fl);

            fl = FileSystem.Instance.Locate(SelRing_Inv, GameFileLocs.Model);
            style.SelRing = ModelManager.Instance.CreateInstance(rs, fl);

            fl = FileSystem.Instance.Locate(SiteBase_Inv, GameFileLocs.Model);
            style.MdgSiteInactive = ModelManager.Instance.CreateInstance(rs, fl);

            int count = (int)MdgType.Count -1;
            style.MdgSiteFull = new ResourceHandle<ModelData>[count];
            style.MdgSiteEmpty = new ResourceHandle<ModelData>[count];
            style.MdgGoalIcon = new ResourceHandle<ModelData>[count];
            style.MdgGoalIconGray = new ResourceHandle<ModelData>[count];
            style.MdgGoalIconHL = new ResourceHandle<ModelData>[count];

            
            for (int i = 0; i < 7; i++)
            {
                fl = FileSystem.Instance.Locate(GoalSitesFullTyped[i], GameFileLocs.Model);
                style.MdgSiteFull[i] = ModelManager.Instance.CreateInstance(rs, fl);
                
                fl = FileSystem.Instance.Locate(GoalSitesEmptyTyped[i], GameFileLocs.Model);
                style.MdgSiteEmpty[i] = ModelManager.Instance.CreateInstance(rs, fl);

                fl = FileSystem.Instance.Locate(GoalSiteIcon[i], GameFileLocs.Model);
                style.MdgGoalIcon[i] = ModelManager.Instance.CreateInstance(rs, fl);

                fl = FileSystem.Instance.Locate(GoalSiteIconGray[i], GameFileLocs.Model);
                style.MdgGoalIconGray[i] = ModelManager.Instance.CreateInstance(rs, fl);

                fl = FileSystem.Instance.Locate(GoalSiteIconHL[i], GameFileLocs.Model);
                style.MdgGoalIconHL[i] = ModelManager.Instance.CreateInstance(rs, fl);
            }
        }
        void BuildAsia(RenderSystem rs)
        {
            int idx = (int)CultureId.Asia;

            #region 初始化默认样式
            styles[idx].ID = CultureId.Asia;
            styles[idx].Urban = new ResourceHandle<ModelData>[3];
            styles[idx].Base = new ResourceHandle<ModelData>[3];

            FileLocation fl = FileSystem.Instance.Locate(SmallCityCenter_Inv, GameFileLocs.Model);
            styles[idx].Urban[0] = ModelManager.Instance.CreateInstance(rs, fl);
            fl = FileSystem.Instance.Locate(MediumCityCenter_Inv, GameFileLocs.Model);
            styles[idx].Urban[1] = ModelManager.Instance.CreateInstance(rs, fl);
            fl = FileSystem.Instance.Locate(LargeCityCenter_Inv, GameFileLocs.Model);
            styles[idx].Urban[2] = ModelManager.Instance.CreateInstance(rs, fl);

            fl = FileSystem.Instance.Locate(SmallBase_Inv, GameFileLocs.Model);
            styles[idx].Base[0] = ModelManager.Instance.CreateInstance(rs, fl);
            fl = FileSystem.Instance.Locate(MediumBase_Inv, GameFileLocs.Model);
            styles[idx].Base[1] = ModelManager.Instance.CreateInstance(rs, fl);
            fl = FileSystem.Instance.Locate(LargeBase_Inv, GameFileLocs.Model);
            styles[idx].Base[2] = ModelManager.Instance.CreateInstance(rs, fl);

            BuildCommon(rs, ref styles[idx]);

            #endregion
        }
        void BuildAfrica(RenderSystem rs)
        {
            int idx = (int)CultureId.Africa;

            #region 初始化默认样式
            styles[idx].ID = CultureId.Africa;
            styles[idx].Urban = new ResourceHandle<ModelData>[3];
            styles[idx].Base = new ResourceHandle<ModelData>[3];

            FileLocation fl = FileSystem.Instance.Locate(SmallCityCenter_Af, GameFileLocs.Model);
            styles[idx].Urban[0] = ModelManager.Instance.CreateInstance(rs, fl);
            fl = FileSystem.Instance.Locate(MediumCityCenter_Af, GameFileLocs.Model);
            styles[idx].Urban[1] = ModelManager.Instance.CreateInstance(rs, fl);
            fl = FileSystem.Instance.Locate(LargeCityCenter_Af, GameFileLocs.Model);
            styles[idx].Urban[2] = ModelManager.Instance.CreateInstance(rs, fl);

            fl = FileSystem.Instance.Locate(SmallBase_Inv, GameFileLocs.Model);
            styles[idx].Base[0] = ModelManager.Instance.CreateInstance(rs, fl);
            fl = FileSystem.Instance.Locate(MediumBase_Inv, GameFileLocs.Model);
            styles[idx].Base[1] = ModelManager.Instance.CreateInstance(rs, fl);
            fl = FileSystem.Instance.Locate(LargeBase_Inv, GameFileLocs.Model);
            styles[idx].Base[2] = ModelManager.Instance.CreateInstance(rs, fl);


            BuildCommon(rs, ref styles[idx]);


            #endregion
        }
        void BuildEr(RenderSystem rs)
        {
            int idx = (int)CultureId.Europe;
            #region 初始化默认样式
            styles[idx].ID = CultureId.Europe;
            styles[idx].Urban = new ResourceHandle<ModelData>[3];
            styles[idx].Base = new ResourceHandle<ModelData>[3];

            FileLocation fl = FileSystem.Instance.Locate(SmallCityCenter_Er, GameFileLocs.Model);
            styles[idx].Urban[0] = ModelManager.Instance.CreateInstance(rs, fl);
            fl = FileSystem.Instance.Locate(MediumCityCenter_Er, GameFileLocs.Model);
            styles[idx].Urban[1] = ModelManager.Instance.CreateInstance(rs, fl);
            fl = FileSystem.Instance.Locate(LargeCityCenter_Er, GameFileLocs.Model);
            styles[idx].Urban[2] = ModelManager.Instance.CreateInstance(rs, fl);

            fl = FileSystem.Instance.Locate(SmallBase_Inv, GameFileLocs.Model);
            styles[idx].Base[0] = ModelManager.Instance.CreateInstance(rs, fl);
            fl = FileSystem.Instance.Locate(MediumBase_Inv, GameFileLocs.Model);
            styles[idx].Base[1] = ModelManager.Instance.CreateInstance(rs, fl);
            fl = FileSystem.Instance.Locate(LargeBase_Inv, GameFileLocs.Model);
            styles[idx].Base[2] = ModelManager.Instance.CreateInstance(rs, fl);

            BuildCommon(rs, ref styles[idx]);

            #endregion
        }
        void BuildAm(RenderSystem rs)
        {
            int idx = (int)CultureId.American;
            #region 初始化默认样式
            styles[idx].ID = CultureId.Europe;
            styles[idx].Urban = new ResourceHandle<ModelData>[3];
            styles[idx].Base = new ResourceHandle<ModelData>[3];

            FileLocation fl = FileSystem.Instance.Locate(SmallCityCenter_Am, GameFileLocs.Model);
            styles[idx].Urban[0] = ModelManager.Instance.CreateInstance(rs, fl);
            fl = FileSystem.Instance.Locate(MediumCityCenter_Am, GameFileLocs.Model);
            styles[idx].Urban[1] = ModelManager.Instance.CreateInstance(rs, fl);
            fl = FileSystem.Instance.Locate(LargeCityCenter_Am, GameFileLocs.Model);
            styles[idx].Urban[2] = ModelManager.Instance.CreateInstance(rs, fl);

            fl = FileSystem.Instance.Locate(SmallBase_Inv, GameFileLocs.Model);
            styles[idx].Base[0] = ModelManager.Instance.CreateInstance(rs, fl);
            fl = FileSystem.Instance.Locate(MediumBase_Inv, GameFileLocs.Model);
            styles[idx].Base[1] = ModelManager.Instance.CreateInstance(rs, fl);
            fl = FileSystem.Instance.Locate(LargeBase_Inv, GameFileLocs.Model);
            styles[idx].Base[2] = ModelManager.Instance.CreateInstance(rs, fl);

            BuildCommon(rs, ref styles[idx]);

            #endregion
        }

        public CityStyleTable(RenderSystem rs)
        {
            styles = new CityStyleData[(int)CultureId.Count];

            // initialize all
            BuildAsia(rs);
            BuildAfrica(rs);
            BuildEr(rs);
            BuildAm(rs);

            #region 初始化变换调整
            adjusts = new CityObjectTRAdjust[(int)CultureId.Count];

            adjusts[0].Base = new Matrix[3];
            adjusts[0].Urban = new Matrix[3];


            for (int i = 0; i < adjusts[0].Base.Length; i++)
                adjusts[0].Base[i] = Matrix.Scaling(Game.ObjectScale * 1.75f, Game.ObjectScale * 1.75f, Game.ObjectScale * 1.75f);


            Matrix scale = Matrix.Scaling(Game.ObjectScale, Game.ObjectScale, Game.ObjectScale);

            adjusts[0].Urban[(int)UrbanSize.Large] = 
                Matrix.Translation(32.5f, 1, 1) * Matrix.Scaling(Game.ObjectScale / 0.92f, Game.ObjectScale / 0.92f, Game.ObjectScale / 0.92f);
            adjusts[0].Urban[(int)UrbanSize.Medium] = Matrix.Scaling(Game.ObjectScale / 3.05f, Game.ObjectScale / 3.05f, Game.ObjectScale / 3.05f) * Matrix.Translation(-36, 3, 25);
            adjusts[0].Urban[(int)UrbanSize.Small] = Matrix.Scaling(Game.ObjectScale * 2.36f, Game.ObjectScale * 2.36f, Game.ObjectScale * 2.36f) * Matrix.Translation(9, 1, -18);

            adjusts[0].Farm = Matrix.Scaling(Game.ObjectScale, Game.ObjectScale, Game.ObjectScale) * Matrix.RotationY(MathEx.PiOver2);

            adjusts[0].Cow = Matrix.RotationY(MathEx.PIf) *
                Matrix.Scaling(Game.ObjectScale * 0.67f, Game.ObjectScale * 0.67f, Game.ObjectScale * 0.67f); // Matrix.Scaling(0, 0, -1);


            adjusts[0].Hospital = Matrix.Translation(0, 1, -1.8f) *
                Matrix.Scaling(Game.ObjectScale * 0.65f, Game.ObjectScale * 0.65f, Game.ObjectScale * 0.65f);
            adjusts[0].OilRefinary = Matrix.Translation(-2.5f, 1, 0) *
                Matrix.Scaling(Game.ObjectScale * 0.92f, Game.ObjectScale * 0.92f, Game.ObjectScale * 0.92f);
            adjusts[0].Biofuel = Matrix.Translation(-1, 1, 14) *
                Matrix.Scaling(Game.ObjectScale * 0.73f, Game.ObjectScale * 0.73f, Game.ObjectScale * 0.73f);
            adjusts[0].WoodFactory = Matrix.Translation(3.6f, 1, 0) *
                Matrix.Scaling(Game.ObjectScale * 1.1f, Game.ObjectScale * 1.1f, Game.ObjectScale * 1.1f);
            adjusts[0].EducationOrgan = Matrix.Translation(-27f, 1, 11) *
                Matrix.Scaling(Game.ObjectScale * 0.46f, Game.ObjectScale * 0.46f, Game.ObjectScale * 0.46f);

            {
                float s = (CityRadiusRing) / RingRadius;
                adjusts[0].Ring = Matrix.Scaling(s, 1, s);
                s = CitySelRingScale * (CityRadiusRing) / RingRadius;
                adjusts[0].SelRing = Matrix.Scaling(s, 1, s);
            }

            #endregion

            adjusts[(int)CultureId.Africa] = adjusts[0];
            adjusts[(int)CultureId.Africa].Urban = new Matrix[3];
            adjusts[(int)CultureId.Africa].Urban[(int)UrbanSize.Large] = 
                Matrix.Translation(-1f, 1, 24) * 
                Matrix.Scaling(Game.ObjectScale / 1.47f, Game.ObjectScale / 1.47f, Game.ObjectScale / 1.47f);
            adjusts[(int)CultureId.Africa].Urban[(int)UrbanSize.Medium] = 
                Matrix.Scaling(Game.ObjectScale / 1.25f, Game.ObjectScale / 1.25f, Game.ObjectScale / 1.25f) 
                * Matrix.Translation(-10, 3, -22);
            adjusts[(int)CultureId.Africa].Urban[(int)UrbanSize.Small] = 
                Matrix.Scaling(Game.ObjectScale / 1.35f, Game.ObjectScale / 1.35f, Game.ObjectScale / 1.35f) * 
                Matrix.Translation(-3, 1, -3);


            adjusts[(int)CultureId.Europe] = adjusts[0];
            adjusts[(int)CultureId.Europe].Urban = new Matrix[3];
            adjusts[(int)CultureId.Europe].Urban[(int)UrbanSize.Large] = 
                Matrix.Translation(10, 1, 7) * Matrix.Scaling(Game.ObjectScale / 1.45f, Game.ObjectScale / 1.45f, Game.ObjectScale / 1.45f);
            adjusts[(int)CultureId.Europe].Urban[(int)UrbanSize.Medium] =
                Matrix.Scaling(Game.ObjectScale / 2.0f, Game.ObjectScale / 2f, Game.ObjectScale / 2f)
                * Matrix.Translation(-27, 3,-13);
            adjusts[(int)CultureId.Europe].Urban[(int)UrbanSize.Small] =
                Matrix.Scaling(Game.ObjectScale / 1.2f, Game.ObjectScale / 1.2f, Game.ObjectScale / 1.2f)
                * Matrix.Translation(30, 1, -13.5f);


            adjusts[(int)CultureId.American] = adjusts[0];
            adjusts[(int)CultureId.American].Urban = new Matrix[3];

            adjusts[(int)CultureId.American].Urban[(int)UrbanSize.Large] =
                Matrix.Translation(6, 1, 7) * Matrix.Scaling(Game.ObjectScale / 1.68f, Game.ObjectScale / 1.68f, Game.ObjectScale /1.68f);
            adjusts[(int)CultureId.American].Urban[(int)UrbanSize.Medium] =
                Matrix.Scaling(Game.ObjectScale / 1.65f, Game.ObjectScale / 1.65f, Game.ObjectScale / 1.65f)
                * Matrix.Translation(-67, 3, -13);
            adjusts[(int)CultureId.American].Urban[(int)UrbanSize.Small] =
                Matrix.Scaling(Game.ObjectScale / 0.42f, Game.ObjectScale / 0.42f, Game.ObjectScale / 0.42f)
                * Matrix.Translation(8, 1, -17f);



        }

        public CityObjectTRAdjust CreateTRAdjust(CultureId culture)
        {
            return new CityObjectTRAdjust(ref adjusts[(int)culture]);
        }
        float RandomAngle
        {
            get
            {
                return 0;// MathEx.PIf * 2 * Randomizer.GetRandomSingle();
            }
        }

        public CityStyle CreateStyle(CultureId culture)
        {
            CityStyleData data = styles[(int)culture];

            CityStyle style = new CityStyle(ref data);

            for (int i = 0; i < style.Base.Length; i++)
            {
                style.Base[i].CurrentAnimation.Clear();
                style.Base[i].CurrentAnimation .Add(new NoAnimaionPlayer(adjusts[(int)culture].Base[i]));
            }
            for (int i = 0; i < style.Urban.Length; i++)
            {
                style.Urban[i].CurrentAnimation.Clear();
                style.Urban[i].CurrentAnimation .Add(new NoAnimaionPlayer(adjusts[(int)culture].Urban[i]));
            }

            style.BiofuelFactory.CurrentAnimation.Clear();
            style.BiofuelFactory.CurrentAnimation .Add(new NoAnimaionPlayer(adjusts[(int)culture].Biofuel * Matrix.RotationY(RandomAngle)));
            style.OilRefinary.CurrentAnimation.Clear();
            style.OilRefinary.CurrentAnimation .Add(new NoAnimaionPlayer(adjusts[(int)culture].OilRefinary * Matrix.RotationY(RandomAngle)));
            style.WoodFactory.CurrentAnimation.Clear();
            style.WoodFactory.CurrentAnimation .Add(new NoAnimaionPlayer(adjusts[(int)culture].WoodFactory * Matrix.RotationY(RandomAngle)));
            style.EducationOrgan.CurrentAnimation.Clear();
            style.EducationOrgan.CurrentAnimation .Add(new NoAnimaionPlayer(adjusts[(int)culture].EducationOrgan * Matrix.RotationY(RandomAngle)));
            style.Hospital.CurrentAnimation.Clear();
            style.Hospital.CurrentAnimation .Add(new NoAnimaionPlayer(adjusts[(int)culture].Hospital * Matrix.RotationY(RandomAngle)));

            for (int i = 0; i < style.Cow.Length; i++)
            {
                style.Cow[i].CurrentAnimation.Clear();
                style.Cow[i].CurrentAnimation .Add(new NoAnimaionPlayer(adjusts[(int)culture].Cow));
            }
            style.FarmLand.CurrentAnimation.Clear();
            style.FarmLand.CurrentAnimation .Add(new NoAnimaionPlayer(adjusts[(int)culture].Farm));

            style.Ring.CurrentAnimation.Clear();
            style.Ring.CurrentAnimation .Add(new NoAnimaionPlayer(adjusts[(int)culture].Ring));


            style.SelRing.CurrentAnimation.Clear();
            style.SelRing.CurrentAnimation.Add(new NoAnimaionPlayer(adjusts[(int)culture].SelRing));

            return style;
        }

        public void Dispose()
        {
            //for (int i = 0; i < styles.Length; i++)
            //{
            //    CityStyleData style = styles[i];
            //    for (int j = 0; j < style.MdgGoalIcon.Length; j++)
            //    {
            //        style.MdgGoalIcon[j].Dispose();
            //    }
            //    for (int j = 0; j < style.MdgGoalIconGray.Length; j++)
            //    {
            //        style.MdgGoalIconGray[j].Dispose();
            //    }
            //    for (int j = 0; j < style.MdgGoalIconHL.Length; j++)
            //    {
            //        style.MdgGoalIconHL[j].Dispose();
            //    }
            //    for (int j = 0; j < style.MdgSiteEmpty.Length; j++)
            //    {
            //        style.MdgSiteEmpty[j].Dispose();
            //    }
            //    for (int j = 0; j < style.MdgSiteFull.Length; j++)
            //    {
            //        style.MdgSiteFull[j].Dispose();
            //    }
            //    style.MdgSiteInactive.Dispose();
            //    style.OilRefinary.Dispose();
            //    style.Ring.Dispose();
            //    style.SelRing.Dispose();
            //    for (int j = 0; j < style.Urban.Length; j++)
            //    {
            //        style.Urban[j].Dispose();
            //    }
            //    style.WoodFactory.Dispose();
            //    for (int j = 0; j < style.Base.Length; j++)
            //    {
            //        style.Base[j].Dispose();
            //    }
            //    style.BiofuelFactory.Dispose();
            //    for (int j=0;j<style
            //}
        }
    }
}
