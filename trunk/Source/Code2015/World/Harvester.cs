using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Scene;
using Apoc3D.Graphics;
using Apoc3D.Vfs;
using Code2015.EngineEx;

namespace Code2015.World
{ 
    class Harvester : DynamicObject
    {

        
        public Harvester(RenderSystem rs)
        {
            FileLocation fl = FileSystem.Instance.Locate("cow.mesh", GameFileLocs.Model);

            ModelL0 = new Model(ModelManager.Instance.CreateInstance(rs, fl));



        }




        public override bool IsSerializable
        {
            get { return false; ; }
        }
    }
}
