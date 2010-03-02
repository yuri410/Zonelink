using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Effects;
using Apoc3D.Graphics.Geometry;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Code2015.EngineEx;
using Code2015.Effects;

namespace Code2015.World
{
    class Atmosphere : StaticModelObject
    {
        const float AtmosphereThickness = 2000;

        Sphere atmoSphere;
        RenderSystem renderSys;

        public Atmosphere(RenderSystem rs)
            : base(false)
        {
            renderSys = rs;

            Material[][] mats = new Material[1][];
            mats[0] = new Material[1];
            mats[0][0] = new Material(renderSys);
            mats[0][0].AlphaRef = -1;
            mats[0][0].ZEnabled = true;
            mats[0][0].ZWriteEnabled = true;
            mats[0][0].IsTransparent = false;
            mats[0][0].CullMode = CullMode.None;
            
            mats[0][0].Ambient = new Color4F(1, 1, 1, 1);
            mats[0][0].Diffuse = new Color4F(1, 1, 1, 1);

            
            mats[0][0].SetEffect(EffectManager.Instance.GetModelEffect(AtmosphereEffectFactory.Name));
            atmoSphere = new Sphere(rs, PlanetEarth.PlanetRadius + AtmosphereThickness,
               PlanetEarth.ColTileCount, PlanetEarth.LatTileCount, mats);

            base.ModelL0 = atmoSphere;

            Transformation = Matrix.Identity;
            BoundingSphere.Radius = PlanetEarth.PlanetRadius;// +AtmosphereThickness;
        }

        public override RenderOperation[] GetRenderOperation()
        {
            RenderOperation[] ops = base.GetRenderOperation();
            for (int i = 0; i < ops.Length; i++)
            {
                ops[i].Priority = RenderPriority.First;
            }
            return ops;
        }

        public override bool IsSerializable
        {
            get { return false; }
        }
    }
}
