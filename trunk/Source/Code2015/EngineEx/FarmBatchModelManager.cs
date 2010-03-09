using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Core;
using Apoc3D.Graphics;

namespace Code2015.EngineEx
{
    class FarmBatchModelManager : ResourceManager
    {
        static volatile FarmBatchModelManager singleton;
        static volatile object syncHelper = new object();

        public static FarmBatchModelManager Instance
        {
            get
            {
                if (singleton == null)
                {
                    lock (syncHelper)
                    {
                        if (singleton == null)
                        {
                            singleton = new FarmBatchModelManager(1048576 * 40);
                        }
                    }
                }
                return singleton;
            }
        }

        private FarmBatchModelManager(int cacheSize)
            : base(cacheSize)
        {
        }

        public ResourceHandle<TreeBatchModel> CreateInstance(RenderSystem rs, ForestInfo info)
        {
            //Resource retrived = base.Exists(TreeBatchModel.GetHashString(x, y, lod));
            //if (retrived == null)
            //{
            TreeBatchModel mdl = new TreeBatchModel(rs, info);
            base.NotifyResourceNew(mdl);
            //}
            //else
            //{
            //    retrived.Use();
            //}
            return new ResourceHandle<TreeBatchModel>(mdl);
        }
    }
}
