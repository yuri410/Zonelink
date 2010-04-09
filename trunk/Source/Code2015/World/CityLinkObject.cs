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
        const float LinkBaseLength = 100;
        const float LinkWidthScale = 0.002f;
        const float LinkHeightScale = 4 * 1f / LinkBaseLength;

        SceneManagerBase sceneMgr;

        CityObject start;
        CityObject end;
        CityLink alink;
        CityLink blink;

        Model link_e;

        TransferEffect atobEff;
        TransferEffect btoaEff;
        TransferEmitter emittera;
        TransferEmitter emitterb;

        bool isVisible;

        public CityLinkObject(RenderSystem renderSys, CityObject a, CityObject b)
            : base(false)
        {
            start = a;
            end = b;

            //FileLocation fl = FileSystem.Instance.Locate("track.mesh", GameFileLocs.Model);
            //ModelL0 = road;// new Model(ModelManager.Instance.CreateInstance(renderSys, fl));

            //ModelL0.CurrentAnimation = new NoAnimation(Matrix.RotationY(MathEx.PiOver2) *
            //    Matrix.Scaling(dist / LinkBaseLength, 1 + LinkHeightScale, 1 + LinkWidthScale * dist));



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




            FileLocation fl = FileSystem.Instance.Locate("link_e.mesh", GameFileLocs.Model);
            link_e = new Model(ModelManager.Instance.CreateInstance(renderSys, fl));
            link_e.CurrentAnimation =
                new NoAnimation(Matrix.Scaling(dist / LinkBaseLength, 1 + LinkHeightScale, 1 + LinkWidthScale * dist) * ori);

            orientation = Matrix.Identity;
            position = Vector3.Zero;// 0.5f * (pa + pb);

            BoundingSphere.Center = 0.5f * (pa + pb);
            BoundingSphere.Radius = dist * 0.5f;

            ModelL0 = link_e;



            atobEff = new TransferEffect(renderSys);
            btoaEff = new TransferEffect(renderSys);

            Vector3 startPos = a.Position;
            Vector3 endPos = b.Position;

            emittera = new TransferEmitter(startPos, endPos, ori.Forward);
            emitterb = new TransferEmitter(endPos, startPos, -ori.Forward);

            atobEff.Emitter = emittera;
            atobEff.Modifier = new TransferModifier();

            btoaEff.Emitter = emitterb;
            btoaEff.Modifier = new TransferModifier();
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

            //return base.GetRenderOperation();
            RenderOperation[] ops = ModelL0.GetRenderOperation();
            if (ops != null)
            {
                opBuffer.Add(ops);
            }
          
            emittera.IsVisible = alink != null ? alink.IsTransportingLR : true;
            ops = atobEff.GetRenderOperation();
            if (ops != null)
            {
                opBuffer.Add(ops);
            }

            emitterb.IsVisible = blink != null ? blink.IsTransportingLR : true;
            ops = btoaEff.GetRenderOperation();
            if (ops != null)
            {
                opBuffer.Add(ops);
            }

            opBuffer.Trim();
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

            if (alink != null && blink != null && sceneMgr!=null )
            {
                if (alink.Disabled && blink.Disabled)
                {
                    sceneMgr.RemoveObjectFromScene(this);
                }
            }
            
            if (isVisible)
            {
                atobEff.Update(dt);
                btoaEff.Update(dt);
                isVisible = false;
            }
        }
    }
}
