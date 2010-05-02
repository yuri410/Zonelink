using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Code2015.BalanceSystem;
using Code2015.World.Screen;

namespace Code2015.World
{
    public class CityGoalSite : IRenderable
    {
        public const int SiteCount = 4;

        struct GoalSite
        {
            public bool HasPiece;           
            public MdgType Type;

            public bool IsTyped;
            public MdgType Desired;

            public bool IsCapturePiece;
            public City TargetCity;

            public bool IsHighlight;
        }

        RenderSystem renderSys;

        CityObject parent;
        CityStyle style;

        GoalSite[] sites = new GoalSite[SiteCount];

        FastList<RenderOperation> opBuffer = new FastList<RenderOperation>();

        public CityGoalSite(RenderSystem rs, CityObject obj, CityStyle style)
        {
            this.renderSys = rs;
            this.parent = obj;
            this.style = style;
        }

        #region IRenderable 成员

        public RenderOperation[] GetRenderOperation()
        {
            opBuffer.FastClear();

            if (parent.IsCaptured)
            {
                for (int i = 0; i < SiteCount; i++)
                {
                    RenderOperation[] ops = null;

                    if (sites[i].IsTyped)
                    {
                        ops = style.MdgSiteEmpty[(int)sites[i].Desired].GetRenderOperation();
                    }
                    else
                    {
                        ops = style.MdgSiteInactive.GetRenderOperation();
                    }
                    if (ops != null)
                    {
                        for (int j = 0; j < ops.Length; j++)
                        {
                            ops[j].Transformation *= CityStyleTable.SiteTransform[i];
                        }
                        opBuffer.Add(ops);
                    }

                    if (sites[i].HasPiece)
                    {
                        ops = style.MdgSiteFull[(int)sites[i].Type].GetRenderOperation();

                        if (ops != null)
                        {
                            for (int j = 0; j < ops.Length; j++)
                            {
                                ops[j].Transformation *= CityStyleTable.SiteTransform[i];
                            }
                            opBuffer.Add(ops);
                        }

                        if (sites[i].IsHighlight)
                        {
                            ops = style.MdgGoalIconHL[(int)sites[i].Type].GetRenderOperation();
                        }
                        else 
                        {
                            ops = style.MdgGoalIcon[(int)sites[i].Type].GetRenderOperation();
                        }
                        if (ops != null)
                        {
                            for (int j = 0; j < ops.Length; j++)
                            {
                                ops[j].Transformation *= CityStyleTable.SiteTransform[i];
                            }
                            opBuffer.Add(ops);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < SiteCount; i++)
                {
                    RenderOperation[] ops = style.MdgSiteInactive.GetRenderOperation();
                    if (ops != null)
                    {
                        for (int j = 0; j < ops.Length; j++)
                        {
                            ops[j].Transformation *= CityStyleTable.SiteTransform[i];
                        }
                        opBuffer.Add(ops);
                    }

                    ops = style.MdgGoalIconGray[(int)parent.MajorProblem].GetRenderOperation();
                    if (ops != null)
                    {
                        for (int j = 0; j < ops.Length; j++)
                        {
                            ops[j].Transformation *= CityStyleTable.SiteTransform[i];
                        }
                        opBuffer.Add(ops);
                    }
                }

            }
            opBuffer.TrimClear();
            return opBuffer.Elements;
        }

        public RenderOperation[] GetRenderOperation(int level)
        {
            return GetRenderOperation();
        }

        #endregion


        public void SetHighlight(int i, bool v)
        {
            sites[i].IsHighlight = v;
        }

        public void Clear()
        {
            for (int i = 0; i < SiteCount; i++)
            {
                sites[i].HasPiece = false;
                if (sites[i].IsCapturePiece)
                {
                    sites[i].TargetCity.CancelCapture(parent.City);
                }
                sites[i].IsCapturePiece = false;
                sites[i].TargetCity = null;
            }
        }
        public void ClearDesired(int i)
        {
            sites[i].IsTyped = false;
        }
        public void SetDesired(int i, MdgType type) 
        {
            sites[i].IsTyped = true;
            sites[i].Desired = type;
        }

        public bool HasPiece(int i)
        {
            return sites[i].HasPiece;
        }
        public MdgType GetPieceType(int i)
        {
            return sites[i].Type;
        }
        public static bool CompareCategory(MdgType a, MdgType b) 
        {
            switch (a) 
            {
                case MdgType.Hunger:
                    return b == MdgType.Hunger;
                case MdgType.ChildMortality:
                case MdgType.Diseases:
                case MdgType.MaternalHealth:
                    return b == MdgType.ChildMortality || b == MdgType.Diseases || b == MdgType.MaternalHealth;
                case MdgType.Environment:
                    return b == MdgType.Environment;
                case MdgType.GenderEquality:
                case MdgType.Education:
                    return b == MdgType.GenderEquality || b == MdgType.Education;
            }
            return false;
        }
        public static MdgType GetDesired(CityPluginTypeId pltype) 
        {
            switch (pltype)
            {
                case CityPluginTypeId.EducationOrg:
                    return MdgType.Education;
                case CityPluginTypeId.Hospital:
                    return MdgType.Diseases;
                case CityPluginTypeId.BiofuelFactory:
                case CityPluginTypeId.OilRefinary:
                case CityPluginTypeId.WoodFactory:
                    return MdgType.Environment;
            }
            return MdgType.Hunger;
        }
        public bool Match(int i, CityPluginTypeId cpltype)
        {
            if (!sites[i].HasPiece)
                return false;
            if (sites[i].IsCapturePiece)
                return false;

            MdgType type = sites[i].Type;
            switch (cpltype)
            {
                case CityPluginTypeId.EducationOrg:
                    switch (type)
                    {
                        case MdgType.GenderEquality:
                        case MdgType.Education:
                            return true;
                    }
                    return false;
                case CityPluginTypeId.Hospital:
                    switch (type)
                    {
                        case MdgType.MaternalHealth:
                        case MdgType.ChildMortality:
                        case MdgType.Diseases:
                            return true;
                    }
                    return false;
                case CityPluginTypeId.BiofuelFactory:
                case CityPluginTypeId.OilRefinary:
                case CityPluginTypeId.WoodFactory:
                    return type == MdgType.Environment;
            }
            return false;
        }
        public bool MatchPiece(int i, MdgType type)
        {
            if (sites[i].HasPiece)
                return false;

            //switch (sites[i].plugin.TypeId)
            //{
            //    case CityPluginTypeId.EducationOrg:
            //        switch (type)
            //        {
            //            case MdgType.GenderEquality:
            //            case MdgType.Education:
            //                return true;
            //        }
            //        return false;
            //    case CityPluginTypeId.Hospital:
            //        switch (type)
            //        {
            //            case MdgType.ChildMortality:
            //            case MdgType.Diseases:
            //                return true;
            //        }
            //        return false;
            //    case CityPluginTypeId.BiofuelFactory:
            //    case CityPluginTypeId.OilRefinary:
            //    case CityPluginTypeId.WoodFactory:
            //        return type == MdgType.Environment;
            //}
            return true;
        }

        public void ClearCapturePiece(City city)
        {
            for (int i = 0; i < SiteCount; i++)
            {
                if (sites[i].IsCapturePiece && sites[i].TargetCity == city) 
                {
                    sites[i].HasPiece = false;
                    sites[i].IsCapturePiece = false;
                    sites[i].TargetCity = null;                    
                }
            }
        }
        public void SetPiece(int i, MdgType res)
        {
            sites[i].HasPiece = true;
            sites[i].Type = res;
            sites[i].TargetCity = null;

            bool passed = false;
            City target;
            if (parent.TryLink(i, res, out target))
            {
                if (parent.IsCaptured && target.CanCapture(parent.Owner))
                {
                    target.SetCapture(parent.City);


                    sites[i].IsCapturePiece = true;
                    sites[i].TargetCity = target;
                    passed = true;
                }
            }
            if (!passed)
            {
                parent.TryUpgrade();
            }
        }
        public void ClearAt(int i) 
        {
            sites[i].HasPiece = false;
            if (sites[i].IsCapturePiece)
            {
                sites[i].TargetCity.CancelCapture(parent.City);

                sites[i].IsCapturePiece = false;
                sites[i].TargetCity = null;
            }
        }

        public MdgType? GetPiece(int i)
        {
            if (!sites[i].HasPiece)
                return null;
            return sites[i].Type;
        }
    }
}
