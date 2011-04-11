﻿/*
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
using Code2015.ParticleSystem;

namespace Code2015.World
{
    //public enum PluginPositionFlag
    //{
    //    None = 0,
    //    P1 = 1,
    //    P2 = 1 << 1,
    //    P3 = 1 << 2,
    //    P4 = 1 << 3
    //}

    public delegate void CityVisibleHander(CityObject obj);    

    public class CityObject : SceneObject, ISelectableObject
    {
        //[SLGValue]
       // public const int MaxPlugin = CityGrade.LargePluginCount;

        struct PluginEntry
        {
            //public CityPlugin plugin;
            //public PluginPositionFlag position;

            /// <summary>
            ///  附加物的变换矩阵
            /// </summary>
            public Matrix transform;
            //public MdgResource CurrentPiece;
        }

        class SmokeEffectBuffer
        {
            SmokeEffect[] smokes = new SmokeEffect[CityGrade.LargePluginCount];
            bool[] activeState = new bool[CityGrade.LargePluginCount];
            RenderSystem renderSys;
            CityObject city;

            bool isVisible;

            public SmokeEffectBuffer(RenderSystem rs, CityObject city)
            {
                this.city = city;
                this.renderSys = rs;
                for (int i = 0; i < smokes.Length; i++)
                {
                    smokes[i] = new SmokeEffect(rs);
                    smokes[i].Modifier = new SmokeModifier();

                    SmokeEmitter se = new SmokeEmitter();
                    se.Up = city.Transformation.Up;
                    se.Right = city.Transformation.Right;
                    se.Front = city.Transformation.Forward;

                    smokes[i].Emitter = se;
                }

            }

            public void RenderNotify()
            {
                for (int i = 0; i < activeState.Length; i++)
                    activeState[i] = false;
            }

            public RenderOperation[] GetRenderOperation(int idx)
            {
                isVisible = true;

                ((SmokeEmitter)smokes[idx].Emitter).Position = Vector3.TransformSimple(city.GetPluginPosition(idx) + Vector3.UnitY * 100, city.Transformation);

                activeState[idx] = true;
                return smokes[idx].GetRenderOperation();
            }

            public void Update(GameTime time) 
            {
                if (isVisible)
                {
                    for (int i = 0; i < activeState.Length; i++)
                    {
                        if (activeState[i])
                        {
                            smokes[i].Update(time);
                        }
                    }
                }
                isVisible = false;
            }
        }


        City city;
        CityStyle style;
        CityOwnerRing sideRing;


        //CityGoalSite goalSite;

        RenderSystem renderSys;
        Map map;
        SmokeEffectBuffer smoke;

        //Dictionary<CityPlugin, Harvester> harvTable = new Dictionary<CityPlugin, Harvester>();

        //FastList<Harvester> harvesters = new FastList<Harvester>();
        //PluginEntry[] plugins = new PluginEntry[CityGrade.LargePluginCount];


        //PluginPositionFlag pluginFlags;

        FastList<RenderOperation> opBuffer = new FastList<RenderOperation>();

        //Dictionary<CityObject, CityLinkObject> linkObjects = new Dictionary<CityObject, CityLinkObject>();

        Matrix invTrans;
        Vector3 position;
        CityLinkManager linkMgr;
        bool isSelected;
        //bool isVisible;
        #region 属性

        public bool IsScaleIncreased
        {
            get;
            set;
        }
        public bool IsUpgraded
        {
            get;
            set;
        }
        public bool IsLinked
        {
            get;
            set;
        }
        //public bool IsUnlinked
        //{
        //    get;
        //    set;
        //}

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

        //public float Satisfaction
        //{
        //    get { return city.Satisfaction; }
        //}

        //public UrbanSize Size
        //{
        //    get { return city.Size; }
        //}
        public void Flash(int duration)
        {
            sideRing.Flash(duration);
        }
        //public bool IsPlayerCapturing(Player pl)
        //{
        //    return city.Capture.IsPlayerCapturing(pl);
        //}
        public bool CanCapture(Player pl)
        {
            return city.CanCapture(pl);
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


        #endregion

        bool ISelectableObject.IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
            }
        }

        public event CityVisibleHander CityVisible;

        public CityObject(RenderSystem rs, Map map, SceneManagerBase sceMgr, CityLinkManager linkMgr, City city, CityStyleTable styleSet)
            : base(false)
        {
            this.city = city;
            this.city.Parent = this;
            base.SceneManager = sceMgr;
            this.map = map;
            this.linkMgr = linkMgr;

            this.style = styleSet.CreateStyle(city.Culture);
            this.renderSys = rs;

            //city.PluginAdded += City_PluginAdded;
            //city.PluginRemoved += City_PluginRemoved;
            city.NearbyCityAdded += City_Linked;
            city.NearbyCityRemoved += City_UnLinked;
            city.CaptureSet += City_CaptureSet;
            city.CaptureCancel += City_CaptureCancel;
            city.CityGrow += City_Grow;
            city.CityOwnerChanged += City_OwnerChanged;

            float radLong = MathEx.Degree2Radian(city.Longitude);
            float radLat = MathEx.Degree2Radian(city.Latitude);

            float altitude = TerrainData.Instance.QueryHeight(radLong, radLat);
            Vector3 pos = PlanetEarth.GetPosition(radLong, radLat, PlanetEarth.PlanetRadius + TerrainMeshManager.PostHeightScale * altitude + 5);

            Transformation = PlanetEarth.GetOrientation(radLong, radLat);
            invTrans = Matrix.Invert(Transformation);

            Transformation.TranslationValue = pos;
            BoundingSphere.Radius = CityStyleTable.CityRadius;
            BoundingSphere.Center = pos;
            position = pos;

            if (city.Owner != null)
                City_OwnerChanged(city.Owner);

            sideRing = new CityOwnerRing(rs, this, style);
            //goalSite = new CityGoalSite(rs, this, style);

            smoke = new SmokeEffectBuffer(rs, this);
            sound = SoundManager.Instance.MakeSoundObjcet("city", null, CityStyleTable.CityRadius * 2);
            sound.Position = pos;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            //if (disposing)
            //{
            //    city.PluginAdded -= City_PluginAdded;
            //    city.PluginRemoved -= City_PluginRemoved;
            //}
        }


        void City_CaptureCancel(City a, City b)
        {
            if (b != null)
            {
                linkMgr.Unlink(SceneManager, a.Parent, b.Parent);
            }

        }
        void City_CaptureSet(City a, City b)
        {
            if (b != null)
            {
                linkMgr.Link(renderSys, SceneManager, a.Parent, b.Parent);
            }

        }
        void City_UnLinked(City a, City b)
        {
            if (b != null)
            {
                linkMgr.Unlink(SceneManager, a.Parent, b.Parent);
                //IsUnlinked = true;
            }

        }
        void City_Grow()
        {
            IsScaleIncreased = true;
        }
        void City_Linked(City a, City b)
        {
            if (b != null && a != null)
            {
                IsLinked = true;

                //goalSite.ClearCapturePiece(a);

                //goalSite.ClearCapturePiece(b);


            }

        }
        //void City_PluginAdded(City city, CityPlugin plugin)
        //{
        //    PluginEntry ent = new PluginEntry();

        //    //if ((pluginFlags & PluginPositionFlag.P1) == 0)
        //    //{
        //    //    pluginFlags |= PluginPositionFlag.P1;
        //    //    ent.position = PluginPositionFlag.P1;
        //    //}
        //    //else if ((pluginFlags & PluginPositionFlag.P2) == 0)
        //    //{
        //    //    pluginFlags |= PluginPositionFlag.P2;
        //    //    ent.position = PluginPositionFlag.P2;
        //    //}
        //    //else if ((pluginFlags & PluginPositionFlag.P3) == 0)
        //    //{
        //    //    pluginFlags |= PluginPositionFlag.P3;
        //    //    ent.position = PluginPositionFlag.P3;
        //    //}
        //    //else if ((pluginFlags & PluginPositionFlag.P4) == 0)
        //    //{
        //    //    pluginFlags |= PluginPositionFlag.P4;
        //    //    ent.position = PluginPositionFlag.P4;
        //    //}
        //    ent.plugin = plugin;

        //    int pos = -1;
        //    for (int i = 0; i < MaxPlugin; i++)
        //    {
        //        if (plugins[i].plugin == null)
        //        {
        //            pos = i;
        //            break;
        //        }
        //    }

        //    ent.transform = Matrix.Translation(style.GetPluginTranslation(pos));

        //    plugins[pos] = ent;

        //    //goalSite.SetDesired(pos, CityGoalSite.GetDesired(plugin.TypeId));

        //}

        
        public void AddHarv(CityPlugin plugin)
        {
            if (IsCaptured && (plugin.TypeId == CityPluginTypeId.OilRefinary ||
                plugin.TypeId == CityPluginTypeId.WoodFactory))
            {
                Harvester harv = new Harvester(renderSys, map, style.Cow);
                harv.Latitude = MathEx.Degree2Radian(Latitude - 2);
                harv.Longtitude = MathEx.Degree2Radian(Longitude);
                harvTable.Add(plugin, harv);
                base.SceneManager.AddObjectToScene(harv);

                NaturalResource res = plugin.CurrentResource;
                if (res != null)
                {
                    harv.SetAuto(
                        MathEx.Degree2Radian(res.Longitude), MathEx.Degree2Radian(res.Latitude),
                        MathEx.Degree2Radian(Longitude), MathEx.Degree2Radian(Latitude));
                }
            }
        }
        public void RemoveHarv(CityPlugin plugin)
        {
            if (harvTable.ContainsKey(plugin))
            {
                Harvester harv = harvTable[plugin];

                SceneManager.RemoveObjectFromScene(harv);

                harvTable.Remove(plugin);
            }
        }
        //void City_PluginRemoved(City city, CityPlugin plugin)
        //{
        //    RemoveHarv(plugin);

        //    int idx = -1;
        //    for (int i = 0; i < MaxPlugin; i++)
        //    {
        //        if (plugins[i].plugin == plugin)
        //        {
        //            idx = i;
        //            break;
        //        }
        //    }
        //    if (idx != -1)
        //    {
        //        goalSite.ClearDesired(idx);
        //        plugins[idx].plugin = null;
        //    }
        //}
        void City_OwnerChanged(Player owner)
        {
            if (IsCaptured)
            {
                //Color4F color = new Color4F(owner.SideColor);
                //ringMaterial.Ambient *= color;
                //ringMaterial.Diffuse *= color;

                foreach (KeyValuePair<CityPlugin, Harvester> e in harvTable)
                {
                    CityPlugin cp = e.Key;
                    NaturalResource res = cp.CurrentResource;
                    e.Value.SetAuto(
                        MathEx.Degree2Radian(res.Longitude), MathEx.Degree2Radian(res.Latitude),
                        MathEx.Degree2Radian(Longitude), MathEx.Degree2Radian(Latitude));
                }

            }
            else
            {
                foreach (KeyValuePair<CityPlugin, Harvester> e in harvTable)
                {
                    CityPlugin cp = e.Key;
                    NaturalResource res = cp.CurrentResource;
                    e.Value.Move(MathEx.Degree2Radian(Longitude), MathEx.Degree2Radian(Latitude));
                }
               
            }
        }

        public override RenderOperation[] GetRenderOperation()
        {
            //isVisible = true;
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

            smoke.RenderNotify();
            for (int i = 0; i < MaxPlugin; i++)
            {
                CityPlugin cp = plugins[i].plugin;
                if (cp == null)
                    continue;

                ops = null;
                switch (cp.TypeId)
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
                        if (!cp.IsSelling && !cp.IsBuilding)
                        {
                            RenderOperation[] ops2 = smoke.GetRenderOperation(i);
                            if (ops2 != null)
                            {
                                opBuffer.Add(ops2);
                            }
                        }
                        break;
                    case CityPluginTypeId.WoodFactory:
                        ops = style.WoodFactory.GetRenderOperation();
                        break;
                }
                if (ops != null)
                {
                    for (int j = 0; j < ops.Length; j++)
                    {
                        if (cp.IsBuilding || cp.IsSelling) 
                        {
                            float bp = plugins[i].plugin.BuildProgress;
                            ops[j].Transformation *= Matrix.Translation(0, -75 + bp * bp * bp * 75, 0);
                        }
                        ops[j].Transformation *= plugins[i].transform;
                    }
                    opBuffer.Add(ops);
                }
            }

            for (int i = 0; i < city.FarmLandCount; i++)
            {
                ops = style.FarmLand.GetRenderOperation();
                if (ops != null)
                {
                    for (int j = 0; j < ops.Length; j++)
                    {
                        ops[j].Transformation *= CityStyleTable.FarmTransform[i];
                    }

                    opBuffer.Add(ops);
                }
            }


            if (isSelected)
            {
                ops = style.SelRing.GetRenderOperation();
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
            ops = goalSite.GetRenderOperation();
            if (ops != null)
            {
                opBuffer.Add(ops);
            }

            ops = sideRing.GetRenderOperation2();
            if (ops != null)
            {
                opBuffer.Add(ops);
                for (int j = 0; j < ops.Length; j++)
                {
                    ops[j].Transformation *= invTrans;
                }
            }

            opBuffer.Trim();
            return opBuffer.Elements;
        }

        public void UpgradeAI()
        {
            for (int i = 0; i < MaxPlugin; i++)
            {
                CityPlugin cp = plugins[i].plugin;
                if (cp != null)
                {
                    cp.Upgrade(CityPlugin.UpgradeAmount);
                }
            }
        }
        void Upgrade()
        {
            bool passed = false;

            // 升级
            for (int i = 0; i < MaxPlugin; i++)
            {
                CityPlugin cp = plugins[i].plugin;
                if (cp == null)
                    continue;
                if (!cp.IsSelling && !cp.IsBuilding && goalSite.Match(i, cp.TypeId))
                {
                    plugins[i].plugin.Upgrade(CityPlugin.UpgradeAmount);
                 
                    goalSite.ClearAt(i);
                    passed = true;
                }
            }
            IsUpgraded = passed;
        }
        public bool TryLink(int goalIdx, MdgType type, out City target)
        {
            Vector3 t = CityStyleTable.SiteTransform[goalIdx].TranslationValue;
            t = Vector3.TransformNormal(t, Transformation);
            t.Normalize();

            for (int i = 0; i < city.LinkableCityCount; i++)
            {
                Vector3 dir = city.GetLinkableCity(i).Parent.position - position;
                dir.Normalize();

                float dot = Vector3.Dot(ref t, ref dir);
                if (dot > 0.5f)
                {
                    target = city.GetLinkableCity(i);
                    if (CityGoalSite.CompareCategory(target.MajorProblem, type))
                    {
                        return true;
                    }
                }
            }
            target = null;
            return false;
        }

        //void TryFarm()
        //{
        //    for (int i = 0; i < CityGoalSite.SiteCount; i++)
        //    {
        //        if (goalSite.GetPieceType(i) != MdgType.Hunger || !goalSite.HasPiece(i))
        //        {
        //            return;
        //        }
        //    }

        //    goalSite.Clear();
        //    city.AddFarm();
        //}

        //public bool TryUpgrade()
        //{
        //    TryFarm();
        //    if (MatchSite())
        //    {
        //        Upgrade();
        //        return true;
        //    }
        //    return false;
        //}

        //public bool MatchSite()
        //{
        //    //if (plugins.Count == 0)
        //    //    return false;
        //    bool passed = false;
        //    for (int i = 0; i < MaxPlugin; i++)
        //    {
        //        CityPlugin cp = plugins[i].plugin;
        //        if (cp == null)
        //            continue;

        //        if (!cp.IsBuilding && !cp.IsSelling)
        //        {
        //            bool result = goalSite.Match(i, cp.TypeId);
        //            passed = true;

        //            if (!result)
        //                return false;
        //        }
        //        else return false;
        //    }
        //    return passed;
        //}

        public override void Update(GameTime dt)
        {
            if (IsCaptured && Owner.Type == PlayerType.LocalHuman)
               // TryUpgrade();

            BoundingSphere.Radius = CityStyleTable.CityRadius;

            sideRing.Update(dt);
            //smoke.RenderNotify();
            smoke.Update(dt);
            sound.Update(dt);
        }


        public override bool IsSerializable
        {
            get { return false; }
        }
    }
}
