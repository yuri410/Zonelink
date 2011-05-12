/*
-----------------------------------------------------------------------------
This source file is part of Zonelink

Copyright (c) 2009+ Tao Games

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  if not, write to the Free Software Foundation, 
Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA, or go to
http://www.gnu.org/copyleft/gpl.txt.

-----------------------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
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

            Vector3 normal = viewPos;
            normal.Normalize();

            Plane plane = new Plane(normal * (PlanetEarth.PlanetRadius + TerrainMeshManager.PostZeroLevel), normal);
            Matrix reflection = Matrix.Reflection(plane);

            position = Vector3.TransformSimple(viewPos, reflection);

            front = Vector3.TransformNormal(srcCamera.Front, reflection);
            top = Vector3.TransformNormal(srcCamera.Top, reflection);
            right = Vector3.TransformNormal(srcCamera.Right, reflection);


            viewMatrix = Matrix.LookAtRH(position, position + front, top);

            orientation = Quaternion.RotationMatrix(viewMatrix);

            frustum.View = viewMatrix;
            frustum.Projection = srcCamera.ProjectionMatrix;
            frustum.Update();

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
            get { return front; }
        }

        public Vector3 Top
        {
            get { return top; }
        }

        public Vector3 Right
        {
            get {  return right; }
        }

        public RenderTarget RenderTarget
        {
            get;
            set;
        }

        #endregion


        public RenderMode Mode
        {
            get { return RenderMode.Simple; }
        }


        #region ICamera 成员


        public Matrix GetSMTrans()
        {
            return Matrix.Identity;
        }
        
        #endregion

        #region ICamera 成员


        public float GetSMScale()
        {
            return 1;
        }

        #endregion
    }
}
