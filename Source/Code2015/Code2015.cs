using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using X = Microsoft.Xna.Framework;
using XGS = Microsoft.Xna.Framework.GamerServices;
using XN = Microsoft.Xna.Framework.Net;

namespace Code2015
{
    class Code2015 : IRenderWindowHandler
    {
        RenderSystem renderSys;

        public Code2015(RenderSystem rs) 
        {
            this.renderSys = rs;
        }

        #region IRenderWindowHandler 成员

        public void Initialize()
        {
        }

        public void Load()
        {
        }

        public void Unload()
        {
        }

        public void Update(GameTime time)
        {
            
        }

        public void Draw()
        {
            renderSys.Clear(ClearFlags.DepthBuffer | ClearFlags.Target, ColorValue.CadetBlue, 1, 0);
            renderSys.BeginFrame();
            renderSys.EndFrame();
        }

        #endregion
    }
}
