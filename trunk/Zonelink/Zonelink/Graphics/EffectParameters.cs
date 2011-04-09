using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Apoc3D;
using Microsoft.Xna.Framework.Graphics;

namespace Zonelink.Graphics
{
    static class EffectParameters
    {
        public static Vector4 TerrainAmbient = new Vector4(0.5f, 0.5f, 0.5f, 1);
        public static Vector4 TerrainDiffuse = new Vector4(1f, 1f, 1f, 1f);

        public static ICamera CurrentCamera
        {
            get;
            set;
        }

        public static Vector3 LightDir = new Vector3(-1, 0, 0);
        public static Vector4 LightAmbient = new Vector4( .5f, .5f, .5f,1);
        public static Vector4 LightDiffuse = new Vector4( .8f, .8f, .8f,1);
        public static Vector4 LightSpecular = new Vector4(1f, 1f, 1f, 1f);

        public static Matrix DepthViewProj;

        public static Matrix InvView;
        public static Texture2D ShadowMap;
    }
}
