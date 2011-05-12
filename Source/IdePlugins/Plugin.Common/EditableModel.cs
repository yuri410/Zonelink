using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.Vfs;

namespace Plugin.Common
{
    class EditableModel : ModelBase<MeshData>, IDisposable
    {
        public EditableModel() { }

        protected override MeshData LoadMesh(BinaryDataReader data)
        {
            throw new NotSupportedException();
        }

        protected override BinaryDataWriter SaveMesh(MeshData mesh)
        {
            return mesh.Save();
        }

        protected override void unload()
        {
            
        }

    }
}
