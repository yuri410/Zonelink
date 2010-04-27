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
using Code2015.BalanceSystem;
using Code2015.EngineEx;
using Code2015.ParticleSystem;

namespace Code2015.World
{
    class CityLinkObject : Entity
    {
        public const int MaxLevel = 2;

        public const float LinkBaseLength = 100;
        public const float LinkWidthScale = 0.002f;
        public const float LinkHeightScale = 4 * 1f / LinkBaseLength;

        public const float LRThreshold = 0.1f;
        public const float HRThreshold = 0.1f;
        public const float FoodThreshold = 0.1f;

        public const float LRUnit = 0.2f;
        public const float HRUnit = 0.2f;
        public const float FoodUnit = 0.2f;

        int updataCounter;
        SceneManagerBase sceneMgr;

        CityObject start;
        CityObject end;
        CityLink alink;
        CityLink blink;

        Model link_e;

        TransferEffect[] atobGreen = new TransferEffect[MaxLevel];
        TransferEffect[] atobRed = new TransferEffect[MaxLevel];
        TransferEffect[] atobYellow = new TransferEffect[MaxLevel];

        TransferEmitter[] atobGreenE = new TransferEmitter[MaxLevel];
        TransferEmitter[] atobRedE = new TransferEmitter[MaxLevel];
        TransferEmitter[] atobYellowE = new TransferEmitter[MaxLevel];

        TransferEffect[] btoaGreen = new TransferEffect[MaxLevel];
        TransferEffect[] btoaRed = new TransferEffect[MaxLevel];
        TransferEffect[] btoaYellow = new TransferEffect[MaxLevel];

        TransferEmitter[] btoaGreenE = new TransferEmitter[MaxLevel];
        TransferEmitter[] btoaRedE = new TransferEmitter[MaxLevel];
        TransferEmitter[] btoaYellowE = new TransferEmitter[MaxLevel];


        TransferEffect atobEff;
        TransferEffect btoaEff;
        TransferEmitter emittera;
        TransferEmitter emitterb;

        int ABGreenLevel;
        int ABRedLevel;
        int ABYellowLevel;

        int BAGreenLevel;
        int BARedLevel;
        int BAYellowLevel;



        bool isVisible;

        public CityLinkObject(RenderSystem renderSys, CityObject a, CityObject b)
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
            link_e.CurrentAnimation =
                new NoAnimation(Matrix.Scaling(dist / LinkBaseLength, 1 + LinkHeightScale, 1 + LinkWidthScale * dist) * ori);

            orientation = Matrix.Identity;
            position = Vector3.Zero;// 0.5f * (pa + pb);

            BoundingSphere.Center = 0.5f * (pa + pb);
            BoundingSphere.Radius = dist * 0.5f;

            ModelL0 = link_e;

            Vector3 startPos = a.Position;
            Vector3 endPos = b.Position;

            emittera = new TransferEmitter(startPos, endPos, ori.Forward);
            emitterb = new TransferEmitter(endPos, startPos, -ori.Forward);
            atobEff = new TransferEffect(renderSys, TransferType.Default);
            btoaEff = new TransferEffect(renderSys, TransferType.Default);
            atobEff.Emitter = emittera;
            atobEff.Modifier = new ParticleModifier();

            btoaEff.Emitter = emitterb;
            btoaEff.Modifier = new ParticleModifier();

            for (int i = 0; i < MaxLevel; i++)
            {
                TransferEffect abg = new TransferEffect(renderSys, TransferType.Wood);
                TransferEmitter abgE = new TransferEmitter(startPos, endPos, ori.Forward);

                abgE.IsVisible = false;
                abgE.IsShutDown = true;
                atobGreen[i] = abg;
                atobGreenE[i] = abgE;

                abg.Modifier = new TransferModifier();
                abg.Emitter = abgE;

                TransferEffect bag = new TransferEffect(renderSys, TransferType.Wood);
                TransferEmitter bagE = new TransferEmitter(endPos, startPos, -ori.Forward);

                bagE.IsVisible = false;
                bagE.IsShutDown = true; 
                btoaGreen[i] = bag;
                btoaGreenE[i] = bagE;

                bag.Modifier = new TransferModifier();
                bag.Emitter = bagE;

                TransferEffect abr = new TransferEffect(renderSys, TransferType.Oil);
                TransferEmitter abrE = new TransferEmitter(startPos, endPos, ori.Forward);

                abrE.IsVisible = false;
                abrE.IsShutDown = true; 
                atobRed[i] = abr;
                atobRedE[i] = abrE;

                abr.Modifier = new TransferModifier();
                abr.Emitter = abrE;

                TransferEffect bar = new TransferEffect(renderSys, TransferType.Oil);
                TransferEmitter barE = new TransferEmitter(endPos, startPos, -ori.Forward);

                barE.IsVisible = false;
                barE.IsShutDown = true; 
                btoaRed[i] = bar;
                btoaRedE[i] = barE;

                bar.Modifier = new TransferModifier();
                bar.Emitter = barE;


                TransferEffect aby = new TransferEffect(renderSys, TransferType.Food);
                TransferEmitter abyE = new TransferEmitter(startPos, endPos, ori.Forward);

                abyE.IsVisible = false;
                abyE.IsShutDown = true; 
                atobYellow[i] = aby;
                atobYellowE[i] = abyE;

                aby.Modifier = new TransferModifier();
                aby.Emitter = abyE;


                TransferEffect bay = new TransferEffect(renderSys, TransferType.Food);
                TransferEmitter bayE = new TransferEmitter(endPos, startPos, -ori.Forward);

                bayE.IsVisible = false;
                bayE.IsShutDown = true; 
                btoaYellow[i] = bay;
                btoaYellowE[i] = bayE;

                bay.Modifier = new TransferModifier();
                bay.Emitter = bayE;
            }

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

            for (int i = 0; i < MaxLevel; i++)
            {
                if (!atobGreenE[i].IsShutDown)
                {
                    ops = atobGreen[i].GetRenderOperation();
                    if (ops != null)
                    {
                        opBuffer.Add(ops);
                    }
                }
                if (!atobRedE[i].IsShutDown)
                {
                    ops = atobRed[i].GetRenderOperation();
                    if (ops != null)
                    {
                        opBuffer.Add(ops);
                    }
                }
                if (!atobYellowE[i].IsShutDown)
                {
                    ops = atobYellow[i].GetRenderOperation();
                    if (ops != null)
                    {
                        opBuffer.Add(ops);
                    }
                }
                if (!btoaGreenE[i].IsShutDown)
                {
                    ops = btoaGreen[i].GetRenderOperation();
                    if (ops != null)
                    {
                        opBuffer.Add(ops);
                    }
                }
                if (!btoaRedE[i].IsShutDown)
                {
                    ops = btoaRed[i].GetRenderOperation();
                    if (ops != null)
                    {
                        opBuffer.Add(ops);
                    }
                }
                if (!btoaYellowE[i].IsShutDown)
                {
                    ops = btoaYellow[i].GetRenderOperation();
                    if (ops != null)
                    {
                        opBuffer.Add(ops);
                    }
                }
            }

            if (!emittera.IsShutDown)
            {
                ops = atobEff.GetRenderOperation();
                if (ops != null)
                {
                    opBuffer.Add(ops);
                }
            }
            if (!emitterb.IsShutDown)
            {
                ops = btoaEff.GetRenderOperation();
                if (ops != null)
                {
                    opBuffer.Add(ops);
                }
            }

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

            //if (alink != null && blink != null && sceneMgr != null)
            //{
            //    if (alink.Disabled && blink.Disabled)
            //    {
            //        sceneMgr.RemoveObjectFromScene(this);
            //    }
            //}

            if (isVisible)
            {
                if (updataCounter++ > 10)
                {
                    if (alink != null && blink != null)
                    {
                        emittera.IsVisible = false;
                        emitterb.IsVisible = false;
                        #region LR
                        bool abg = alink != null ? alink.LR > LRThreshold : true;
                        if (!abg)
                        {
                            for (int i = 0; i < MaxLevel; i++)
                                atobGreenE[i].IsVisible = false;
                            ABGreenLevel = 0;

                        }
                        else
                        {
                            ABGreenLevel = (int)(alink.LR / LRUnit);

                            if (ABGreenLevel >= MaxLevel) ABGreenLevel = MaxLevel - 1;

                            for (int i = 0; i < ABGreenLevel; i++)
                                atobGreenE[i].IsVisible = true;
                            for (int i = ABGreenLevel; i < MaxLevel; i++)
                                atobGreenE[i].IsVisible = false;
                        }

                        bool bag = blink != null ? blink.LR > LRThreshold : true;
                        if (!bag)
                        {
                            for (int i = 0; i < MaxLevel; i++)
                                btoaGreenE[i].IsVisible = false;
                            BAGreenLevel = 0;

                        }
                        else
                        {
                            BAGreenLevel = (int)(blink.LR / LRUnit);

                            if (BAGreenLevel >= MaxLevel) BAGreenLevel = MaxLevel - 1;

                            for (int i = 0; i < BAGreenLevel; i++)
                                btoaGreenE[i].IsVisible = true;
                            for (int i = BAGreenLevel; i < MaxLevel; i++)
                                btoaGreenE[i].IsVisible = false;
                        }
                        #endregion
                        #region HR
                        bool abr = alink != null ? alink.HR > HRThreshold : true;
                        if (!abr)
                        {
                            for (int i = 0; i < MaxLevel; i++)
                                atobRedE[i].IsVisible = false;
                            ABRedLevel = 0;

                        }
                        else
                        {
                            ABRedLevel = (int)(alink.HR / HRUnit);

                            if (ABRedLevel >= MaxLevel) ABRedLevel = MaxLevel - 1;

                            for (int i = 0; i < ABRedLevel; i++)
                                atobRedE[i].IsVisible = true;
                            for (int i = ABRedLevel; i < MaxLevel; i++)
                                atobRedE[i].IsVisible = false;
                        }

                        bool bar = blink != null ? blink.HR > HRThreshold : true;
                        if (!bar)
                        {
                            for (int i = 0; i < MaxLevel; i++)
                                btoaRedE[i].IsVisible = false;
                            BARedLevel = 0;

                        }
                        else
                        {
                            BARedLevel = (int)(blink.HR / HRUnit);

                            if (BARedLevel >= MaxLevel) BARedLevel = MaxLevel - 1;

                            for (int i = 0; i < BARedLevel; i++)
                                btoaRedE[i].IsVisible = true;
                            for (int i = BARedLevel; i < MaxLevel; i++)
                                btoaRedE[i].IsVisible = false;
                        }
                        #endregion
                        #region Food
                        bool abry = alink != null ? alink.Food > FoodThreshold : true;
                        if (!abry)
                        {
                            for (int i = 0; i < MaxLevel; i++)
                                atobYellowE[i].IsVisible = false;
                            ABYellowLevel = 0;

                        }
                        else
                        {
                            ABYellowLevel = (int)(alink.Food / FoodUnit);

                            if (ABYellowLevel >= MaxLevel) ABYellowLevel = MaxLevel - 1;

                            for (int i = 0; i < ABYellowLevel; i++)
                                atobYellowE[i].IsVisible = true;
                            for (int i = ABYellowLevel; i < MaxLevel; i++)
                                atobYellowE[i].IsVisible = false;
                        }

                        bool bay = blink != null ? blink.Food > FoodThreshold : true;
                        if (!bay)
                        {
                            for (int i = 0; i < MaxLevel; i++)
                                btoaYellowE[i].IsVisible = false;
                            BAYellowLevel = 0;

                        }
                        else
                        {
                            BAYellowLevel = (int)(blink.Food / FoodUnit);

                            if (BAYellowLevel >= MaxLevel) BAYellowLevel = MaxLevel - 1;

                            for (int i = 0; i < BAYellowLevel; i++)
                                btoaYellowE[i].IsVisible = true;
                            for (int i = BAYellowLevel; i < MaxLevel; i++)
                                btoaYellowE[i].IsVisible = false;
                        }
                        #endregion
                    }
                    else
                    {
                        emittera.IsVisible = true;
                        emitterb.IsVisible = true;
                    }
                    updataCounter = 0;
                }

                for (int i = 0; i < MaxLevel; i++)
                {
                    atobGreen[i].Update(dt);
                    atobRed[i].Update(dt);
                    atobYellow[i].Update(dt);
                    btoaGreen[i].Update(dt);
                    btoaRed[i].Update(dt);
                    btoaYellow[i].Update(dt);
                }
                atobEff.Update(dt);
                btoaEff.Update(dt);
                isVisible = false;
            }
        }
    }
}
