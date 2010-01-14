using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Code2015.World;

namespace Code2015.EngineEx
{
    class ReflectionCamera : ICamera
    {
        Camera srcCamera;
        Frustum frustum;

        Matrix viewMatrix;
        Vector3 position;
        Quaternion orientation;


        Vector3 front;
        Vector3 top;
        Vector3 right;

        public ReflectionCamera(Camera srcCamera)
        {
            this.srcCamera = srcCamera;
            frustum = new Frustum();
        }

        #region ICamera 成员

        public void Update(GameTime time)
        {
            Matrix view = srcCamera.ViewMatrix;
            Vector3 viewPos = srcCamera.Position;
            //float len = viewPos.Length();

            Vector3 normal = viewPos;
            normal.Normalize();

            Plane plane = new Plane(normal * (PlanetEarth.PlanetRadius + TerrainMeshManager.PostZeroLevel), normal);
            Matrix reflection = Matrix.Reflection(plane);


            viewMatrix = view * reflection;


            position = Vector3.TransformSimple(viewPos, reflection); //normal * (2 * PlanetEarth.PlanetRadius - len);
            orientation = Quaternion.RotationMatrix(view * reflection);

            frustum.View = viewMatrix;
            frustum.Projection = srcCamera.ProjectionMatrix;
            frustum.Update();



            front = MathEx.GetMatrixFront(ref viewMatrix);
            top = MathEx.GetMatrixUp(ref viewMatrix);
            right = MathEx.GetMatrixRight(ref viewMatrix);

        }

        public Frustum Frustum
        {
            get { return frustum; }
        }

        public Matrix ProjectionMatrix
        {
            get { return frustum.Projection; }
        }

        public Matrix ViewMatrix
        {
            get { return viewMatrix; }
        }

        public Vector3 Position
        {
            get { return position; }
        }

        public Quaternion Orientation
        {
            get { return orientation; }
        }

        public float FarPlane
        {
            get { return srcCamera.FarPlane; }
        }

        public float NearPlane
        {
            get { return  srcCamera.NearPlane; }
        }

        public float AspectRatio
        {
            get { return srcCamera.AspectRatio; }
        }

        public float FieldOfView
        {
            get { return srcCamera.FieldOfView; }
        }

        public Vector3 Front
        {
            get { throw new NotImplementedException(); }
        }

        public Vector3 Top
        {
            get { throw new NotImplementedException(); }
        }

        public Vector3 Right
        {
            get { throw new NotImplementedException(); }
        }

        public RenderTarget RenderTarget
        {
            get;
            set;
        }

        #endregion
    }
}
