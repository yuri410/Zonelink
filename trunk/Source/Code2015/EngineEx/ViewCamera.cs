using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.MathLib;
using Code2015.World;

namespace Code2015.EngineEx
{
    public class RtsCamera : Camera
    {
        bool isPerspective;
        float height = 60;
        float orthoZoom;

        protected float dMoveSpeed = 2.5f;
        protected float dTurnSpeed = MathEx.PIf / 45;

        float longitude;
        float latitude;

        float rotation;
        float yaw;

        public RtsCamera(float fovy, float aspect)
            : base(aspect)
        {
            OrthoZoom = 65;
            isPerspective = fovy < 175 && fovy > 5;
            FieldOfView = fovy;



            Update(null);
        }

        void UpdateView()
        {
            position = PlanetEarth.GetPosition(longitude, latitude);

            Vector3 axis = position;
            axis.Normalize();

            orientation = Quaternion.RotationAxis(axis, rotation) * Quaternion.RotationAxis(Vector3.UnitX, yaw);

        }

        public override void ResetView()
        {
            isPerspective = FieldOfView < 175 && FieldOfView > 5;

            yaw = MathEx.PIf / 4;

            UpdateView();
        }

        public override void GetSubareaProjection(ref RectangleF rect, out Matrix mat)
        {
            if (isPerspective)
            {
                Matrix.PerspectiveOffCenterRH(rect.Left * NearPlaneWidth, rect.Right * NearPlaneWidth, rect.Top * NearPlaneHeight, rect.Bottom * NearPlaneHeight, NearPlane, FarPlane, out mat);
            }
            else
            {
                Matrix.OrthoOffCenterRH(rect.Left * NearPlaneWidth, rect.Right * NearPlaneWidth, rect.Top * NearPlaneHeight, rect.Bottom * NearPlaneHeight, NearPlane, FarPlane, out mat);
            }
        }

        public override void UpdateProjection()
        {
            isPerspective = FieldOfView < 175 && FieldOfView > 5;

            if (isPerspective)
            {
                base.UpdateProjection();
            }
            else
            {
                NearPlaneWidth = AspectRatio * OrthoZoom;
                NearPlaneHeight = OrthoZoom;
                Frustum.Projection = Matrix.OrthoRH(NearPlaneWidth, NearPlaneHeight, NearPlane, FarPlane);

                isProjDirty = false;
            }
        }

        public override void Update(GameTime time)
        {
            UpdateView();
            if (isProjDirty)
            {
                UpdateProjection();
            }

            Matrix viewTrans;
            Matrix.RotationQuaternion(ref orientation, out viewTrans);

            viewTrans = Matrix.Translation(-position) * viewTrans;
            Frustum.View = viewTrans;
            Frustum.Update();

            front = viewTrans.Forward;
            top = viewTrans.Up;
            right = viewTrans.Right;

        }

        public float OrthoZoom
        {
            get { return orthoZoom; }
            set
            {
                orthoZoom = value;
                isProjDirty = true;
            }
        }

        public float Height
        {
            get
            {
                return height;
            }
            set
            {
                if (value >= 30 && value < 105)
                {
                    yaw = MathEx.Degree2Radian(value - height);

                    height = value;
                }
            }
        }
        public bool IsPerspective
        {
            get { return isPerspective; }
        }

        public void MoveFront()
        {
            longitude += (float)Math.Sin(rotation) * dMoveSpeed;
            latitude += (float)Math.Cos(rotation) * dMoveSpeed;
        }
        public void MoveBack()
        {
            longitude += (float)Math.Sin(rotation) * dMoveSpeed;
            latitude += (float)Math.Cos(rotation) * dMoveSpeed;
        }

        public void MoveLeft()
        {
            longitude -= (float)Math.Cos(rotation) * dMoveSpeed;
            latitude -= (float)Math.Sin(rotation) * dMoveSpeed;
        }
        public void MoveRight()
        {
            longitude += (float)Math.Cos(rotation) * dMoveSpeed;
            latitude += (float)Math.Sin(rotation) * dMoveSpeed;
        }
        public void TurnLeft()
        {
            rotation -= dTurnSpeed;
        }
        public void TurnRight()
        {
            rotation += dTurnSpeed;
        }


    }

}
