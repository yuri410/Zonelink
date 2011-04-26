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
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Animation;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.Logic;

namespace Code2015.World
{
    class SelectionMarker : SceneObject
    {
        public const float LinkWidthScale = 3.300f;
        public const float LinkHeightScale = 1.0f;
        public const float LinkBaseLength = 450;

        public const float HarvestRingRadius = 175;
        public const float ResourceRingRadius = 360;

        public const float CitySelScale = 1.9f;

        const float RingRadius = 100;

        BattleField battleField;
        Player player;
        City mouseHoverCity;
        City selectedCity;

        NaturalResource selectedResource;
        NaturalResource mouseHoverResource;

        Harvester selectedHarv;
        Harvester mouseHoverHarv;

        City[] nodes;
        City[] targets;

        Model inner_marker;
        Model outter_marker;

        Model[] linkArrow;
        Model[] nodeLinks;

        FastList<RenderOperation> opBuffer = new FastList<RenderOperation>();

        float ringRotation;

        ISelectableObject selectedObject;
        ISelectableObject mouseHoverObject;


        public int PathNodeLength
        {
            get 
            {
                if (nodes != null)
                {
                    return nodes.Length;
                }
                return 0;
            }
        }
        public bool HasPath
        {
            get { return nodes != null; }
        }

        public ISelectableObject MouseHoverObject
        {
            get { return mouseHoverObject; }
            set
            {
                if (mouseHoverObject != value)
                {
                    mouseHoverObject = value;

                    if (mouseHoverObject != null)
                    {
                        mouseHoverCity = mouseHoverObject as City;
                        mouseHoverResource = mouseHoverObject as NaturalResource;
                        mouseHoverHarv = mouseHoverObject as Harvester;

                    }
                    else
                    {
                        mouseHoverCity = null;
                        mouseHoverResource = null;
                        mouseHoverResource = null;
                    }

                    if (mouseHoverCity != null && selectedCity !=null && mouseHoverCity != selectedCity)
                    {

                        battleField.BallPathFinder.Reset();
                        BallPathFinderResult result = battleField.BallPathFinder.FindPath(selectedCity, mouseHoverCity);
                        if (result != null)
                        {
                            City[] nds = new City[result.NodeCount + 1];
                            nds[0] = selectedCity;
                            for (int i = 0; i < result.NodeCount; i++)
                            {
                                nds[i + 1] = result[i];
                            }
                            SetNodes(nds);
                        }
                        else 
                        {
                            SetNodes(null);
                        }
                    }
                    else
                    {
                        SetNodes(null);
                    }
                    //if (mouseHoverCity == null)
                    //{
                    //    selectionMarker.MouseHoverCity = null;
                    //}
                }


            }
        }

        public ISelectableObject SelectedObject
        {
            get { return selectedObject; }
            set
            {
                if (selectedObject != value)
                {
                    selectedObject = value;

                    if (selectedObject != null)
                    {
                        selectedCity = selectedObject as City;
                        selectedHarv = selectedObject as Harvester;
                        selectedResource = selectedObject as NaturalResource;

                        if (selectedCity != null)
                        {
                            //Vector3 ppos = renderSys.Viewport.Project(selectedCity.Position, camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);

                            //selectedProjPos.X = (int)ppos.X;
                            //selectedProjPos.Y = (int)ppos.Y;

                            City cc = selectedCity;
                            City[] nearby = new City[cc.LinkableCityCount];

                            for (int i = 0; i < cc.LinkableCityCount; i++)
                            {
                                nearby[i] = cc.GetLinkableCity(i);
                            }

                            SetCity(selectedCity, nearby);
                        }
                        else 
                        {
                            nodes = null;
                        }

                        if (selectedResource != null)
                        {
                            float s = ResourceRingRadius / RingRadius;
                            Matrix scale = Matrix.Scaling(s, 1, s);

                            inner_marker.CurrentAnimation.Clear();


                            if (selectedResource.Type == NaturalResourceType.Wood)
                            {
                                ForestObject forest = selectedResource as ForestObject;
                                inner_marker.CurrentAnimation.Add(new NoAnimaionPlayer(Matrix.Translation(0, 5, 0) * scale * forest.ForestTransform));
                            }
                            else
                            {
                                inner_marker.CurrentAnimation.Add(new NoAnimaionPlayer(Matrix.Translation(0, 5, 0) * scale * selectedResource.Transformation));
                            }
                        }
                    }
                    else
                    {
                        selectedCity = null;
                        selectedHarv = null;
                        selectedResource = null;
                        nodes = null;
                    }

                    //if (selectedCity == null)
                    //{
                    //    SetCity(null, null);
                    //}
                }
            }
        }


        Matrix GetCityLinkTransform(City start, City end) 
        {
            //City end = targets[i];

            Vector3 dir = end.Position - start.Position;

            dir.Normalize();

            Vector3 pa = start.Position + dir * (City.CityOutterRadius);
            Vector3 pb = end.Position - dir * (City.CityOutterRadius);


            Vector3 center = 0.5f * (pa + pb);
            Vector3 normal = start.Transformation.Up + end.Transformation.Up;
            normal *= 0.5f;

            float dist = Vector3.Distance(pa, pb);

            float scale = dist;
            Matrix ori = Matrix.Identity;
            ori.Right = Vector3.Normalize(pa - pb);
            ori.Up = normal;
            ori.Forward = Vector3.Normalize(Vector3.Cross(ori.Up, ori.Right));
            ori.TranslationValue = center + normal * 150;

            return Matrix.RotationX(-MathEx.PiOver2) * Matrix.RotationY(-MathEx.PiOver2) *
                    Matrix.Scaling(dist / LinkBaseLength, 1 + LinkHeightScale, LinkWidthScale) * ori;
        }

        private void SetNodes(City[] nodes)
        {
            this.nodes = nodes;

            if (nodes != null)
            {
                for (int i = 0; i < nodes.Length - 1; i++)
                {
                    City a = nodes[i];
                    City b = nodes[i + 1];

                    Matrix ori = GetCityLinkTransform(a, b);

                    nodeLinks[i].CurrentAnimation.Clear();
                    nodeLinks[i].CurrentAnimation.Add(new NoAnimaionPlayer(ori));
                }
            }

        }
        private void SetCity(City start, City[] targets)
        {
            this.selectedCity = start;
            this.targets = targets;
            this.nodes = null;

            for (int i = 0; i < targets.Length; i++)
            {
                Matrix ori = GetCityLinkTransform(start, targets[i]);

                linkArrow[i].CurrentAnimation.Clear();
                linkArrow[i].CurrentAnimation.Add(new NoAnimaionPlayer(ori));
            }

          

            {
                float s = 0.8f * CitySelScale * City.CityOutterRadius / RingRadius;
                Matrix scale = Matrix.Scaling(s, 1, s);

                inner_marker.CurrentAnimation.Clear();
                inner_marker.CurrentAnimation.Add(new NoAnimaionPlayer(scale * start.Transformation));
            }
        }

        public SelectionMarker(RenderSystem rs, BattleField btfld, Player player)
            : base(false)
        {
            this.battleField = btfld;
            this.player = player;
            this.linkArrow = new Model[4];
            this.nodeLinks = new Model[BattleField.MaxCities];

            string fileName = "linkarrow.mesh";
            {
                FileLocation fl2 = FileSystem.Instance.Locate(fileName, GameFileLocs.Model);
                ResourceHandle<ModelData> mdlData = ModelManager.Instance.CreateInstance(rs, fl2);
                for (int i = 0; i < linkArrow.Length; i++)
                {
                    linkArrow[i] = new Model(mdlData);                    
                }
                for (int i = 0; i < nodeLinks.Length; i++)
                {
                    nodeLinks[i] = new Model(mdlData);                    
                }
            }
            FileLocation fl = FileSystem.Instance.Locate("citysel_inner.mesh", GameFileLocs.Model);
            inner_marker = new Model(ModelManager.Instance.CreateInstance(rs, fl));

            fl = FileSystem.Instance.Locate("citysel_outter.mesh", GameFileLocs.Model);
            outter_marker = new Model(ModelManager.Instance.CreateInstance(rs, fl));

            BoundingSphere.Radius = float.MaxValue;
            Transformation = Matrix.Identity;
        }

        #region IRenderable 成员

        public override RenderOperation[] GetRenderOperation()
        {
            opBuffer.FastClear();

            if (selectedCity != null)
            {
                if (nodes == null)
                {
                    if (selectedCity.IsCaptured && selectedCity.Owner == player)
                    {
                        for (int i = 0; i < targets.Length; i++)
                        {

                            RenderOperation[] ops = linkArrow[i].GetRenderOperation();
                            if (ops != null)
                            {
                                opBuffer.Add(ops);
                            }

                        }
                    }
                }
                else if (nodes != null)
                {
                    if (selectedCity.IsCaptured && selectedCity.Owner == player)
                    {
                        for (int i = 0; i < nodes.Length - 1; i++)
                        {
                            RenderOperation[] ops = nodeLinks[i].GetRenderOperation();
                            if (ops != null)
                            {
                                opBuffer.Add(ops);
                            }
                        }
                    }
                }
            }
            if (selectedCity != null || selectedResource != null || selectedHarv != null)
            {
                RenderOperation[] ops = inner_marker.GetRenderOperation();
                if (ops != null)
                {
                    opBuffer.Add(ops);
                }

            }
            if (mouseHoverCity != null || mouseHoverHarv != null || mouseHoverResource != null)
            {
                RenderOperation[] ops = outter_marker.GetRenderOperation();
                if (ops != null)
                {
                    opBuffer.Add(ops);
                }
            }
            opBuffer.Trim();
            return opBuffer.Elements;
        }

        public override RenderOperation[] GetRenderOperation(int level)
        {
            return GetRenderOperation();
        }

        #endregion

        public override void Update(GameTime time)
        {
            float ddt = time.ElapsedGameTimeSeconds;
            ringRotation += ddt / 4.5f;
            if (ringRotation > 2 * MathEx.PIf)
                ringRotation -= 2 * MathEx.PIf;

            if (mouseHoverCity != null)
            {
                float s = CitySelScale * City.CityOutterRadius / RingRadius;
                Matrix scale = Matrix.Scaling(s, 1, s);

                outter_marker.CurrentAnimation.Clear();
                outter_marker.CurrentAnimation.Add(new NoAnimaionPlayer(scale * Matrix.RotationY(-ringRotation) * mouseHoverCity.Transformation));
            }

            if (mouseHoverResource != null)
            {
                float s = ResourceRingRadius / RingRadius;
                Matrix scale = Matrix.Scaling(s, 1, s);

                outter_marker.CurrentAnimation.Clear();

                if (mouseHoverResource.Type == NaturalResourceType.Wood)
                {
                    ForestObject forest = mouseHoverResource as ForestObject;
                    outter_marker.CurrentAnimation.Add(new NoAnimaionPlayer(Matrix.Translation(0, 5, 0) * scale * Matrix.RotationY(-ringRotation) * forest.ForestTransform));
                }
                else 
                {
                    outter_marker.CurrentAnimation.Add(new NoAnimaionPlayer(Matrix.Translation(0, 5, 0) * scale * Matrix.RotationY(-ringRotation) * mouseHoverResource.Transformation));
                }
            }

            if (mouseHoverHarv != null)
            {
                float s = HarvestRingRadius / RingRadius;
                Matrix scale = Matrix.Scaling(s, 1, s);

                outter_marker.CurrentAnimation.Clear();
                outter_marker.CurrentAnimation.Add(new NoAnimaionPlayer(Matrix.Translation(0, 5, 0) * scale * Matrix.RotationY(-ringRotation) * mouseHoverHarv.Transformation));
            }


            if (selectedHarv != null) 
            {
                float s = HarvestRingRadius / RingRadius;
                Matrix scale = Matrix.Scaling(s, 1, s);

                inner_marker.CurrentAnimation.Clear();
                inner_marker.CurrentAnimation.Add(new NoAnimaionPlayer(Matrix.Translation(0, 5, 0) * scale * selectedHarv.Transformation));
            }
        }

        public override bool IsSerializable
        {
            get { return false; }
        }
    }
}
