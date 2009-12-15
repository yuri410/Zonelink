using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.Vfs;

namespace Code2015.EngineEx
{
    class TerrainMesh : Resource, IRenderable
    {
        FileLocation resLoc;

        Model terrain;

        int level;

        RenderSystem renderSystem;

        static string GetHashString(int x, int y, int lod)
        {
            return "TM" + x.ToString() + y.ToString() + lod.ToString();
        }

        public TerrainMesh(RenderSystem rs, int x, int y, int lod)
            : base(TerrainMeshManager.Instance, GetHashString(x, y, lod))
        {
            resLoc = FileSystem.Instance.TryLocate("tile_" + x.ToString() + "_" + y.ToString() + "_" + lod.ToString(), FileLocateRules.Model);
            level = lod;
            renderSystem = rs;
        }

        public override int GetSize()
        {
            return resLoc.Size;
        }

        protected override void load()
        {
            TDMPIO data = new TDMPIO();
            data.Load(resLoc);

            MeshData meshData = new MeshData(renderSystem);


            #region 顶点数据

            

            #endregion

            ResourceInterlock.EnterAtomicOp();
            GameMesh mesh = new GameMesh(renderSystem, meshData);

            terrain = new Model(renderSystem, new GameMesh[] { mesh });

            ResourceInterlock.ExitAtomicOp();
        }

        protected override void unload()
        {
            terrain.Dispose();
            terrain = null;
        }

        #region IRenderable 成员

        public RenderOperation[] GetRenderOperation()
        {
            if (terrain != null)
            {
                return terrain.GetRenderOperation();
            }
            return null;
        }

        public RenderOperation[] GetRenderOperation(int level)
        {
            return GetRenderOperation();
        }

        #endregion
    }
}
