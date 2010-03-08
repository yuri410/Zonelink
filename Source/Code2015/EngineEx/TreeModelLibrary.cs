using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;

namespace Code2015.EngineEx
{
    class TreeModelLibrary : Singleton
    {
        static TreeModelLibrary singleton;

        public static TreeModelLibrary Instance
        {
            get { return singleton; }
        }

        public static void Initialize(RenderSystem rs)
        {
            singleton = new TreeModelLibrary(rs);
        }

        RenderSystem renderSys;

        private TreeModelLibrary(RenderSystem rs)
        {
            renderSys = rs;
        }




        protected override void dispose()
        {
            throw new NotImplementedException();
        }
    }
}
