using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.Vfs;
using DX = SlimDX.Direct3D9;
using Apoc3D.MathLib;

namespace meshconv
{
    public class XText2ModelConverter
    {
        static DX.Direct3D d3d;
        static DX.Device device;

        public static void Initialize()
        {
            d3d = new DX.Direct3D();

            DX.PresentParameters pm = new DX.PresentParameters();
            pm.Windowed = true;
            device = new DX.Device(d3d, 0, DX.DeviceType.Reference, IntPtr.Zero, DX.CreateFlags.None, pm);
        }

        public static void Convert(string source, string dest)
        {
            FileStream fs = File.Open(source, FileMode.Open, FileAccess.Read);
            DX.Mesh mesh = DX.Mesh.FromStream(device, fs, DX.MeshFlags.Managed);

            DX.ExtendedMaterial[] mats = mesh.GetMaterials();
            Material[][] outMats = new Material[mats.Length][];
            for (int i = 0; i < mats.Length; i++)
            {
                outMats[i] = new Material[1];
                outMats[i][0] = new Material(null);
                
                DX.Material mat = mats[i].MaterialD3D;

                SlimDX.Color4 clr = mat.Ambient;

                outMats[i][0].Ambient = new Color4F(clr.Alpha, clr.Red, clr.Green, clr.Blue);
                
                clr = mat.Diffuse; 
                outMats[i][0].Diffuse = new Color4F(clr.Alpha, clr.Red, clr.Green, clr.Blue);
                
                clr = mat.Specular;
                outMats[i][0].Specular = new Color4F(clr.Alpha, clr.Red, clr.Green, clr.Blue);

                outMats[i][0].SetTextureFile(0, mats[i].TextureFileName);
            }


            string name = Path.GetFileNameWithoutExtension(source);

            MeshData outMesh = new MeshData((RenderSystem)null);
            outMesh.Materials = outMats;

            //EditableMesh outMesh = new EditableMesh(name, mesh, outMats);



            mesh.Dispose();

            outMdl.Entities = new EditableMesh[] { outMesh };

            //TransformAnimation transAnim = new TransformAnimation(1);
            //transAnim.Nodes[0].Transforms = new Matrix[1] { Matrix.RotationX(-MathEx.PIf / 2) };

            //outMdl.SetTransformAnimInst(new TransformAnimationInstance(transAnim));

            EditableModel.ToFile(outMdl, dest);


            outMdl.Dispose();
        }
    }
}