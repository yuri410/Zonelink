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
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Animation;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.ParticleSystem;

namespace Code2015.World
{
    /// <summary>
    ///  标记可占领城市
    /// </summary>
    class CityLinkObject : Entity
    {
        public const int MaxLevel = 3;

        public const float LinkBaseLength = 100;
        public const float LinkWidthScale = 0.006f;
        public const float LinkHeightScale = 4 * 1f / LinkBaseLength;

        public const float LRThreshold = 0.01f;
        public const float HRThreshold = 0.01f;
        public const float FoodThreshold = 0.01f;

        public const float LRUnit = 0.33f;
        public const float HRUnit = 0.33f;
        public const float FoodUnit = 0.33f;

        int updataCounter;
        SceneManagerBase sceneMgr;

        City start;
        City end;
        CityLink alink;
        CityLink blink;

        Model link_e;

        //TransferEffect[] atobGreen = new TransferEffect[MaxLevel];
        //TransferEffect[] atobRed = new TransferEffect[MaxLevel];
        //TransferEffect[] atobYellow = new TransferEffect[MaxLevel];

        //TransferEmitter[] atobGreenE = new TransferEmitter[MaxLevel];
        //TransferEmitter[] atobRedE = new TransferEmitter[MaxLevel];
        //TransferEmitter[] atobYellowE = new TransferEmitter[MaxLevel];

        //TransferEffect[] btoaGreen = new TransferEffect[MaxLevel];
        //TransferEffect[] btoaRed = new TransferEffect[MaxLevel];
        //TransferEffect[] btoaYellow = new TransferEffect[MaxLevel];

        //TransferEmitter[] btoaGreenE = new TransferEmitter[MaxLevel];
        //TransferEmitter[] btoaRedE = new TransferEmitter[MaxLevel];
        //TransferEmitter[] btoaYellowE = new TransferEmitter[MaxLevel];



        //int ABGreenLevel;
        //int ABRedLevel;
        //int ABYellowLevel;

        //int BAGreenLevel;
        //int BARedLevel;
        //int BAYellowLevel;


        bool isVisible;

        public CityLinkObject(RenderSystem renderSys, City a, City b)
            : base(false)
        {
            start = a;
            end = b;


            Vector3 dir = end.Position - start.Position;

            dir.Normalize();

            Vector3 pa = start.Position + dir * CityStyleTable.CityRadius;
            Vector3 pb = end.Position - dir * CityStyleTable.CityRadius;


            float dist = Vector3.Distance(pa, pb);

            float longitude = MathEx.Degree2Radian(0.5f * (start.Longitude + end.Longitude));
            float latitude = MathEx.Degree2Radian(0.5f * (start.Latitude + end.Latitude));

            Matrix ori = Matrix.Identity;
            ori.Right = Vector3.Normalize(pa - pb);
            ori.Up = PlanetEarth.GetNormal(longitude, latitude);
            ori.Forward = Vector3.Normalize(Vector3.Cross(ori.Up, ori.Right));
            ori.TranslationValue = 0.5f * (pa + pb);



            string fileName = "link_e.mesh";

            if (a.IsCaptured)
            {
                ColorValue color = a.Owner.SideColor;

                if (color == ColorValue.Red)
                {
                    fileName = "link_e_red.mesh";
                }
                else if (color == ColorValue.Green)
                {
                    fileName = "link_e_green.mesh";
                }
                else if (color == ColorValue.Blue)
                {
                    fileName = "link_e_blue.mesh";
                }
                else
                {
                    fileName = "link_e_yellow.mesh";
                }
            }
            else if (b.IsCaptured)
            {
                ColorValue color = b.Owner.SideColor;

                if (color == ColorValue.Red)
                {
                    fileName = "link_e_red.mesh";
                }
                else if (color == ColorValue.Green)
                {
                    fileName = "link_e_green.mesh";
                }
                else if (color == ColorValue.Blue)
                {
                    fileName = "link_e_blue.mesh";
                }
                else
                {
                    fileName = "link_e_yellow.mesh";
                }
            }

            FileLocation fl = FileSystem.Instance.Locate(fileName, GameFileLocs.Model);
            link_e = new Model(ModelManager.Instance.CreateInstance(renderSys, fl));
            link_e.CurrentAnimation.Clear();
            link_e.CurrentAnimation.Add(new NoAnimaionPlayer(Matrix.Scaling(dist / LinkBaseLength, 1 + LinkHeightScale, 1 + LinkWidthScale * dist) * ori));

            orientation = Matrix.Identity;
            position = Vector3.Zero;// 0.5f * (pa + pb);

            BoundingSphere.Center = 0.5f * (pa + pb);
            BoundingSphere.Radius = dist * 0.5f;

            ModelL0 = link_e;



            //Vector3 startPos = a.Position;
            //Vector3 endPos = b.Position;

            //for (int i = 0; i < MaxLevel; i++)
            //{
            //    TransferEffect abg = new TransferEffect(renderSys, TransferType.Wood);
            //    TransferEmitter abgE = new TransferEmitter(startPos, endPos, ori.Forward);

            //    abgE.IsVisible = false;
            //    abgE.IsShutDown = true;
            //    atobGreen[i] = abg;
            //    atobGreenE[i] = abgE;

            //    abg.Modifier = new TransferModifier();
            //    abg.Emitter = abgE;

            //    TransferEffect bag = new TransferEffect(renderSys, TransferType.Wood);
            //    TransferEmitter bagE = new TransferEmitter(endPos, startPos, -ori.Forward);

            //    bagE.IsVisible = false;
            //    bagE.IsShutDown = true;
            //    btoaGreen[i] = bag;
            //    btoaGreenE[i] = bagE;

            //    bag.Modifier = new TransferModifier();
            //    bag.Emitter = bagE;

            //    TransferEffect abr = new TransferEffect(renderSys, TransferType.Oil);
            //    TransferEmitter abrE = new TransferEmitter(startPos, endPos, ori.Forward);

            //    abrE.IsVisible = false;
            //    abrE.IsShutDown = true;
            //    atobRed[i] = abr;
            //    atobRedE[i] = abrE;

            //    abr.Modifier = new TransferModifier();
            //    abr.Emitter = abrE;

            //    TransferEffect bar = new TransferEffect(renderSys, TransferType.Oil);
            //    TransferEmitter barE = new TransferEmitter(endPos, startPos, -ori.Forward);

            //    barE.IsVisible = false;
            //    barE.IsShutDown = true;
            //    btoaRed[i] = bar;
            //    btoaRedE[i] = barE;

            //    bar.Modifier = new TransferModifier();
            //    bar.Emitter = barE;


            //    TransferEffect aby = new TransferEffect(renderSys, TransferType.Food);
            //    TransferEmitter abyE = new TransferEmitter(startPos, endPos, ori.Forward);

            //    abyE.IsVisible = false;
            //    abyE.IsShutDown = true;
            //    atobYellow[i] = aby;
            //    atobYellowE[i] = abyE;

            //    aby.Modifier = new TransferModifier();
            //    aby.Emitter = abyE;


            //    TransferEffect bay = new TransferEffect(renderSys, TransferType.Food);
            //    TransferEmitter bayE = new TransferEmitter(endPos, startPos, -ori.Forward);

            //    bayE.IsVisible = false;
            //    bayE.IsShutDown = true;
            //    btoaYellow[i] = bay;
            //    btoaYellowE[i] = bayE;

            //    bay.Modifier = new TransferModifier();
            //    bay.Emitter = bayE;
            //}

        }

        public override bool IsSerializable
        {
            get { return false; }
        }

        FastList<RenderOperation> opBuffer = new FastList<RenderOperation>();

        public override RenderOperation[] GetRenderOperation()
        {
            isVisible = true;
            opBuffer.FastClear();

            RenderOperation[] ops = ModelL0.GetRenderOperation();
            if (ops != null)
            {
                opBuffer.Add(ops);
            }

            //for (int i = 0; i < MaxLevel; i++)
            //{
            //    if (!atobGreenE[i].IsShutDown)
            //    {
            //        ops = atobGreen[i].GetRenderOperation();
            //        if (ops != null)
            //        {
            //            opBuffer.Add(ops);
            //        }
            //    }
            //    if (!atobRedE[i].IsShutDown)
            //    {
            //        ops = atobRed[i].GetRenderOperation();
            //        if (ops != null)
            //        {
            //            opBuffer.Add(ops);
            //        }
            //    }
            //    if (!atobYellowE[i].IsShutDown)
            //    {
            //        ops = atobYellow[i].GetRenderOperation();
            //        if (ops != null)
            //        {
            //            opBuffer.Add(ops);
            //        }
            //    }
            //    if (!btoaGreenE[i].IsShutDown)
            //    {
            //        ops = btoaGreen[i].GetRenderOperation();
            //        if (ops != null)
            //        {
            //            opBuffer.Add(ops);
            //        }
            //    }
            //    if (!btoaRedE[i].IsShutDown)
            //    {
            //        ops = btoaRed[i].GetRenderOperation();
            //        if (ops != null)
            //        {
            //            opBuffer.Add(ops);
            //        }
            //    }
            //    if (!btoaYellowE[i].IsShutDown)
            //    {
            //        ops = btoaYellow[i].GetRenderOperation();
            //        if (ops != null)
            //        {
            //            opBuffer.Add(ops);
            //        }
            //    }
            //}

            opBuffer.TrimClear();
            return opBuffer.Elements;

        }
        public override RenderOperation[] GetRenderOperation(int level)
        {
            return GetRenderOperation();
        }

        public override void OnAddedToScene(object sender, SceneManagerBase sceneMgr)
        {
            this.sceneMgr = sceneMgr;
        }


        public override void Update(GameTime dt)
        {
            base.Update(dt);

            if ((alink == null || blink == null) && (start.IsCaptured && end.IsCaptured))
            {
                blink = start.City.GetLink(end.City);
                alink = end.City.GetLink(start.City);
            }


            if (isVisible)
            {
                //if (updataCounter++ > 10)
                //{
                //    if (alink != null && blink != null)
                //    {
                //        for (int i = 0; i < MaxLevel; i++)
                //        {
                //            atobGreenE[i].IsVisible = false;
                //            btoaGreenE[i].IsVisible = false;
                //            atobRedE[i].IsVisible = false;
                //            btoaRedE[i].IsVisible = false;

                //            atobYellowE[i].IsVisible = false;
                //            btoaYellowE[i].IsVisible = false;
                //        }

                //        #region LR

                //        float abs = alink.LR - blink.LR;

                //        if (abs > LRThreshold)
                //        {
                //            ABGreenLevel = 1 + (int)(alink.LR / LRUnit);
                //            if (ABGreenLevel > MaxLevel) ABGreenLevel = MaxLevel - 1;

                //            for (int i = 0; i < ABGreenLevel; i++)
                //                atobGreenE[i].IsVisible = true;
                //        }
                //        else if (abs < -LRThreshold)
                //        {
                //            BAGreenLevel = 1 + (int)(blink.LR / LRUnit);

                //            if (BAGreenLevel > MaxLevel) BAGreenLevel = MaxLevel - 1;

                //            for (int i = 0; i < BAGreenLevel; i++)
                //                btoaGreenE[i].IsVisible = true;
                //        }


                //        #endregion
                //        #region HR
                //        abs = alink.HR - blink.HR;

                //        if (abs > HRThreshold)
                //        {
                //            ABRedLevel = 1 + (int)(alink.HR / HRUnit);

                //            if (ABRedLevel > MaxLevel) ABRedLevel = MaxLevel - 1;

                //            for (int i = 0; i < ABRedLevel; i++)
                //                atobRedE[i].IsVisible = true;
                //        }
                //        else if (abs < -HRThreshold)
                //        {
                //            BARedLevel = 1 + (int)(blink.HR / HRUnit);

                //            if (BARedLevel > MaxLevel) BARedLevel = MaxLevel - 1;

                //            for (int i = 0; i < BARedLevel; i++)
                //                btoaRedE[i].IsVisible = true;
                //        }


                //        #endregion
                //        #region Food

                //        abs = alink.Food - blink.Food;

                //        if (abs > FoodThreshold)
                //        {
                //            ABYellowLevel = 1 + (int)(alink.Food / FoodUnit);

                //            if (ABYellowLevel > MaxLevel) ABYellowLevel = MaxLevel - 1;

                //            for (int i = 0; i < ABYellowLevel; i++)
                //                atobYellowE[i].IsVisible = true;
                //        }
                //        else if (abs < -FoodThreshold)
                //        {
                //            BAYellowLevel = 1 + (int)(blink.Food / FoodUnit);

                //            if (BAYellowLevel > MaxLevel) BAYellowLevel = MaxLevel - 1;

                //            for (int i = 0; i < BAYellowLevel; i++)
                //                btoaYellowE[i].IsVisible = true;
                //        }

                //        #endregion
                //    }
                //    else
                //    {
                //        City city;
                //        City src;
                //        bool inversed = false;
                //        if (!start.IsCaptured)
                //        {
                //            city = start.City;
                //            src = end.City;
                //            inversed = true;
                //        }
                //        else
                //        {
                //            city = end.City;
                //            src = start.City;
                //        }

                //        if (city != null)
                //        {

                //            if (!inversed)
                //            {
                //                ABGreenLevel = MaxLevel;
                //                ABRedLevel = MaxLevel;

                //                for (int i = 0; i < MaxLevel; i++)
                //                    atobGreenE[i].IsVisible = true;

                //                for (int i = 0; i < MaxLevel; i++)
                //                    atobRedE[i].IsVisible = true;
                //            }
                //            else
                //            {
                //                BAGreenLevel = MaxLevel;
                //                BARedLevel = MaxLevel;

                //                for (int i = 0; i < MaxLevel; i++)
                //                    btoaGreenE[i].IsVisible = true;

                //                for (int i = 0; i < MaxLevel; i++)
                //                    btoaRedE[i].IsVisible = true;
                //            }
                //        }
                //    }
                //    updataCounter = 0;
                //}


                //for (int i = 0; i < MaxLevel; i++)
                //{
                //    atobGreen[i].Update(dt);
                //    atobRed[i].Update(dt);
                //    atobYellow[i].Update(dt);
                //    btoaGreen[i].Update(dt);
                //    btoaRed[i].Update(dt);
                //    btoaYellow[i].Update(dt);
                //}

                isVisible = false;
            }
        }
    }
}
