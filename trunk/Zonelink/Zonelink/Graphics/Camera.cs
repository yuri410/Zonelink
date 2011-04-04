/*
-----------------------------------------------------------------------------
This source file is part of Apoc3D Engine

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
using Zonelink.MathLib;
using Microsoft.Xna.Framework;


namespace Apoc3D
{
    /// <summary>
    ///  定义摄像机
    /// </summary>
    public interface ICamera
    {
        Frustum Frustum { get; }

        Matrix ProjectionMatrix { get; }
        Matrix ViewMatrix { get; }

        void Update(GameTime time);

        /// <summary>
        ///  获取摄像机的位置
        /// </summary>
        Vector3 Position { get; }

        /// <summary>
        ///  获取摄像机的朝向
        /// </summary>
        Quaternion Orientation { get; }

        /// <summary>
        ///  获取远裁剪平面的位置
        /// </summary>
        float FarPlane { get; }

        /// <summary>
        ///  获取近裁剪平面的位置
        /// </summary>
        float NearPlane { get; }

        /// <summary>
        ///  获取宽高比
        /// </summary>
        float AspectRatio { get; }

        /// <summary>
        ///  获取视野范围，使用角度表示
        /// </summary>
        float FieldOfView { get; }

        Vector3 Front { get; }
        Vector3 Top { get; }
        Vector3 Right { get; }




        Matrix GetSMTrans();
        float GetSMScale();
    }


    /// <summary>
    /// 表示摄像机
    /// </summary>
    public class Camera : ICamera
    {
        #region Fields and Properties
        protected Vector3 position;

        protected Quaternion orientation;

        protected Vector3 front;

        protected Vector3 top;

        protected Vector3 right;

        float fovy;
        float near;
        float far;
        float aspect;

        Frustum frustum = new Frustum();

        protected bool isProjDirty;


        public Frustum Frustum
        {
            get { return frustum; }
        }

        /// <summary>
        /// Gets the view direction(AKA z axis in camera space)
        /// 获取摄像机的朝向（摄像机空间Z轴）
        /// </summary>
        public Vector3 Front
        {
            get { return front; }
        }

        /// <summary>
        /// 摄像机Y
        /// </summary>
        public Vector3 Top
        {
            get { return top; }
        }

        /// <summary>
        /// 摄像机X
        /// </summary>
        public Vector3 Right
        {
            get { return right; }
        }

        /// <summary>
        /// Gets or sets the position of the camera eye point.
        /// 获取或设置试点的位置
        /// </summary>
        /// <value>
        /// The position of the camera eye point.
        /// 视点的位置
        /// </value>
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        ///  获取或设置视点的朝向
        /// </summary>
        public Quaternion Orientation
        {
            get { return orientation; }
            set { orientation = value; }
        }

        /// <summary>
        ///  获取或设置一个浮点数，表示投影fovy参数
        /// </summary>
        public float FieldOfView
        {
            get { return MathHelper.ToDegrees(fovy); }
            set
            {
                fovy = MathHelper.ToRadians(value);
                isProjDirty = true;
            }
        }

        /// <summary>
        ///  获取或设置一个浮点数，表示近裁剪平面的距离
        /// </summary>
        public float NearPlane
        {
            get { return near; }
            set
            {
                near = value;
                isProjDirty = true;
            }
        }

        /// <summary>
        ///  获取或设置一个浮点数，表示远裁剪平面的距离
        /// </summary>
        public float FarPlane
        {
            get { return far; }
            set
            {
                far = value;
                isProjDirty = true;
            }
        }

        /// <summary>
        ///  获取或设置一个浮点数，表示AspectRatio
        /// </summary>
        public float AspectRatio
        {
            get { return aspect; }
            set
            {
                aspect = value;
                isProjDirty = true;
            }
        }

        /// <summary>
        ///  获取视点变换矩阵
        /// </summary>
        public Matrix ViewMatrix
        {
            get { return frustum.View; }
            protected set { frustum.View = value; }
        }

        /// <summary>
        ///  获取投影矩阵
        /// </summary>
        public Matrix ProjectionMatrix
        {
            get { return frustum.Projection; }
            protected set { frustum.Projection = value; }
        }

        /// <summary>
        ///  获取一个浮点数，表示近裁剪平面的宽度
        /// </summary>
        public float NearPlaneWidth
        {
            get;
            protected set;
        } 
        /// <summary>
        ///  获取一个浮点数，表示近裁剪平面的高度
        /// </summary>
        public float NearPlaneHeight
        {
            get;
            protected set;
        }
        #endregion

        public Camera(float aspect)
        {
            FarPlane = 200;
            FieldOfView = 45;
            NearPlane = 0.1f;
            AspectRatio = aspect;
            orientation = Quaternion.Identity;
            Update(null);
        }

        #region 方法

        public virtual void GetSubareaProjection(ref Rectangle rect, out Matrix mat)
        {
            //Matrix.PerspectiveRH(rect.Width, rect.Height, NearPlane, FarPlane, out mat);           
            Matrix.CreatePerspectiveOffCenter(rect.Left * NearPlaneWidth, rect.Right * NearPlaneWidth, rect.Bottom * NearPlaneHeight, rect.Top * NearPlaneHeight, NearPlane, FarPlane, out mat);
        }

        public virtual void UpdateProjection()
        {
            NearPlaneHeight = (float)(Math.Tan(fovy * 0.5f)) * NearPlane * 2;
            NearPlaneWidth = NearPlaneHeight * AspectRatio;


            frustum.Projection = Matrix.CreatePerspective(NearPlaneWidth, NearPlaneHeight, near, far);

            isProjDirty = false;
        }

        /// <summary>
        ///  更新相机的状态，每一帧均被引擎调用
        /// </summary>
        /// <param name="dt">帧时间间隔，以秒为单位</param>
        public virtual void Update(GameTime time)
        {
            // 如果需要更新Projection Matrix
            if (isProjDirty)
            {
                UpdateProjection();
            }

            //更新摄像机的Front,Top,Right变量
            Matrix m = Matrix.CreateFromQuaternion(orientation);

            front = m.Forward;// MathEx.GetMatrixFront(ref m);
            top = m.Up;// MathEx.GetMatrixUp(ref m);
            right = m.Right;// MathEx.GetMatrixRight(ref m);

            frustum.View = Matrix.CreateLookAt(position, position + front, top);

            frustum.Update();
        }

        /// <summary>
        ///  重置摄像机
        /// </summary>
        public virtual void ResetView() { }

        #endregion

        public virtual float GetSMScale() 
        {
            return 9.5f;
        }
        public virtual Matrix GetSMTrans()
        {
            return Matrix.Identity;
        }
    }


    public class ChaseCamera : Camera
    {
        public ChaseCamera(float acpectRatio)
            : base(acpectRatio)
        {
            top = new Vector3(0, 1, 0);
        }

        #region Chased object properties (set externally each frame)

        /// <summary>
        /// Position of object being chased.
        /// </summary>
        public Vector3 ChasePosition
        {
            get { return chasePosition; }
            set { chasePosition = value; }
        }
        private Vector3 chasePosition;

        /// <summary>
        /// Direction the chased object is facing.
        /// </summary>
        public Vector3 ChaseDirection
        {
            get { return chaseDirection; }
            set { chaseDirection = value; }
        }
        private Vector3 chaseDirection = Vector3.UnitZ;

        #endregion

        #region Desired camera positioning (set when creating camera or changing view)

        /// <summary>
        /// Desired camera position in the chased object's coordinate system.
        /// </summary>
        public Vector3 DesiredPositionOffset
        {
            get { return desiredPositionOffset; }
            set { desiredPositionOffset = value; }
        }
        private Vector3 desiredPositionOffset = new Vector3(0, 1.2f, 4);

        /// <summary>
        /// Desired camera position in world space.
        /// </summary>
        public Vector3 DesiredPosition
        {
            get
            {
                // Ensure correct value even if update has not been called this frame
                UpdateWorldPositions();

                return desiredPosition;
            }
        }
        private Vector3 desiredPosition;

        /// <summary>
        /// Look at point in the chased object's coordinate system.
        /// </summary>
        public Vector3 LookAtOffset
        {
            get { return lookAtOffset; }
            set { lookAtOffset = value; }
        }
        private Vector3 lookAtOffset = new Vector3(0, 0, 0);

        /// <summary>
        /// Look at point in world space.
        /// </summary>
        public Vector3 LookAt
        {
            get
            {
                // Ensure correct value even if update has not been called this frame
                UpdateWorldPositions();

                return lookAt;
            }
        }
        private Vector3 lookAt;

        #endregion

        #region Camera physics (typically set when creating camera)

        /// <summary>
        /// Physics coefficient which controls the influence of the camera's position
        /// over the spring force. The stiffer the spring, the closer it will stay to
        /// the chased object.
        /// </summary>
        public float Stiffness
        {
            get { return stiffness; }
            set { stiffness = value; }
        }
        private float stiffness = 1800.0f;

        /// <summary>
        /// Physics coefficient which approximates internal friction of the spring.
        /// Sufficient damping will prevent the spring from oscillating infinitely.
        /// </summary>
        public float Damping
        {
            get { return damping; }
            set { damping = value; }
        }
        private float damping = 600.0f;

        /// <summary>
        /// Mass of the camera body. Heaver objects require stiffer springs with less
        /// damping to move at the same rate as lighter objects.
        /// </summary>
        public float Mass
        {
            get { return mass; }
            set { mass = value; }
        }
        private float mass = 50.0f;

        #endregion

        #region Current camera properties (updated by camera physics)
        /// <summary>
        /// Velocity of camera.
        /// </summary>
        public Vector3 Velocity
        {
            get { return velocity; }
        }
        private Vector3 velocity;

        #endregion


        #region Methods

        /// <summary>
        /// Rebuilds object space values in world space. Invoke before publicly
        /// returning or privately accessing world space values.
        /// </summary>
        private void UpdateWorldPositions()
        {
            // Construct a matrix to transform from object space to worldspace
            Matrix transform = Matrix.Identity;
            //transform.Forward = ChaseDirection;
            //transform.Up = Up;
            right = Vector3.Cross(top, chaseDirection);
            right.Normalize();

            //Vector3 vec = chaseDirection;

            front = chaseDirection;

            transform.M31 = -chaseDirection.X;
            transform.M32 = -chaseDirection.Y;
            transform.M33 = -chaseDirection.Z;

            transform.M21 = top.X;
            transform.M22 = top.Y;
            transform.M23 = top.Z;

            transform.M11 = right.X;
            transform.M12 = right.Y;
            transform.M13 = right.Z;

            // Calculate desired camera properties in world space
            desiredPosition = ChasePosition +
                Vector3.TransformNormal(DesiredPositionOffset, transform);
            lookAt = ChasePosition +
                Vector3.TransformNormal(LookAtOffset, transform);
        }

        /// <summary>
        /// Rebuilds camera's view and projection matricies.
        /// </summary>
        private void UpdateMatrices()
        {
            Frustum.View = Matrix.CreateLookAt(this.Position, this.LookAt, this.top);
            //projection = Matrix.CreatePerspectiveFieldOfView(FieldOfView,
            //    AspectRatio, NearPlaneDistance, FarPlaneDistance);
        }

        /// <summary>
        /// Forces camera to be at desired position and to stop moving. The is useful
        /// when the chased object is first created or after it has been teleported.
        /// Failing to call this after a large change to the chased object's position
        /// will result in the camera quickly flying across the world.
        /// </summary>
        public override void ResetView()
        {
            UpdateWorldPositions();

            // Stop motion
            velocity = Vector3.Zero;

            // Force desired position
            position = desiredPosition;

            UpdateMatrices();
        }

        /// <summary>
        /// Animates the camera from its current position towards the desired offset
        /// behind the chased object. The camera's animation is controlled by a simple
        /// physical spring attached to the camera and anchored to the desired position.
        /// </summary>
        public override void Update(GameTime time)
        {
            float dt;
            if (time != null)
            {
                dt = (float)time.ElapsedGameTime.TotalSeconds;
            }
            else 
            {
                dt = 0;
            }
            
            UpdateWorldPositions();

            // Calculate spring force
            Vector3 stretch = position - desiredPosition;
            Vector3 force = -stiffness * stretch - damping * velocity;

            // Apply acceleration
            Vector3 acceleration = force / mass;
            velocity += acceleration * dt;

            // Apply velocity
            position += velocity * dt;

            UpdateMatrices();

            if (isProjDirty)
            {
                UpdateProjection();
            }

            //MathEx.QuaternionToMatrix(ref orientation, out Frustum.view);


            Frustum.Update();//ref mViewMatrix, ref mProjMatrix);

            //更新摄像机的Front,Top,Right变量

            //front = new Vector3(0, 0, -1);
            //top = new Vector3(0, 1, 0);
            //right = new Vector3(1, 0, 0);

            //MathEx.MatrixTransformVec(ref Frustum.view, ref front);
            //MathEx.MatrixTransformVec(ref Frustum.view, ref top);
            //MathEx.MatrixTransformVec(ref Frustum.view, ref right);



            //front = MathEx.QuaternionRotate(orientation, new Vector3(0, 0, -1));
            //top = MathEx.QuaternionRotate(orientation, new Vector3(0, 1, 0));
            //right = MathEx.QuaternionRotate(orientation, new Vector3(1, 0, 0));
        }

        #endregion
    }
}
