using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Zonelink.Graphics
{
    static class EffectParameters
    {

        public static Vector3 LightDir = new Vector3(-1, 0, 0);
        public static Vector4 LightAmbient = new Vector4(1, .5f, .5f, .5f);
        public static Vector4 LightDiffuse = new Vector4(1f, .8f, .8f, .8f);
        public static Vector4 LightSpecular = new Vector4(1f, 1f, 1f, 1f);

        public static Matrix DepthViewProj;

        public static Matrix InvView;
    }
}
