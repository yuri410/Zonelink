using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Core;
using Apoc3D.Graphics;

namespace Code2015.EngineEx
{
    class TreeBatchModelManager : ResourceManager
    {
        static volatile TreeBatchModelManager singleton;
        static volatile object syncHelper = new object();

        public static TreeBatchModelManager Instance
        {
            get
            {
                if (singleton == null)
                {
                    lock (syncHelper)
                    {
                        if (singleton == null)
                        {
                            singleton = new TreeBatchModelManager(1048576 * 40);
                        }
                    }
                }
                return singleton;
            }
        }

        private TreeBatchModelManager(int cacheSize)
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
