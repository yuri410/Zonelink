using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Code2015.BalanceSystem;
using Apoc3D.Config;
using Apoc3D.Vfs;

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
        Dictionary<PlantCategory, TreeModelData> categoryModels = new Dictionary<PlantCategory, TreeModelData>();
        Dictionary<PlantType, TreeModelData> typeModels = new Dictionary<PlantType, TreeModelData>();


        private TreeModelLibrary(RenderSystem rs)
        {
            renderSys = rs;

            FileLocation fl = FileSystem.Instance.Locate("trees.xml", GameFileLocs.Config);
            Configuration conf = ConfigurationManager.Instance.CreateInstance(fl);

            foreach (KeyValuePair<string, ConfigurationSection> s in conf)
            {
                ConfigurationSection sect = s.Value;
                TreeModelData mdl;

                mdl.Category = (PlantCategory)Enum.Parse(typeof(PlantCategory), sect.GetString("Category", string.Empty));
                mdl.Type = (PlantType)Enum.Parse(typeof(PlantType), sect.GetString("Type", string.Empty));

                string fileName = sect.GetString("File", string.Empty);
                FileLocation fl2 = new FileLocation(fileName);

                ModelMemoryData mdlData = new ModelMemoryData(rs, fl2);

                




            }
        }




        protected override void dispose()
        {
            categoryModels.Clear();
            typeModels.Clear();
        }
    }
}
