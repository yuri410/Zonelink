using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.MathLib;

namespace Code2015.EngineEx
{
    public class RtsCamera : Camera
    {
        bool isPerspective;
        float height = 60;
        float orthoZoom;

        protected float dMoveSpeed = 2.5f;
        protected float dTurnSpeed = MathEx.PIf / 45;

        public RtsCamera(float fovy, float aspect)
            : base(aspect)
        {
            OrthoZoom = 65;
            isPerspective = fovy < 175 && fovy > 5;
            FieldOfView = fovy;
            ResetView();

            Update();
        }

        public override void ResetView()
        {
            isPerspective = FieldOfView < 175 && FieldOfView > 5;
            if (isPerspective)
            {
                orientation = Quaternion.RotationAxis(new Vector3(0, 1, 0), -MathEx.PIf / 4);
                orientation *= Quaternion.RotationAxis(new Vector3(1, 0, 0), MathEx.PIf / 4);
            }
            else
            {
                orientation = Quaternion.RotationAxis(new Vector3(0, 1, 0), -MathEx.PIf / 4);
                orientation *= Quaternion.RotationAxis(new Vector3(1, 0, 0), MathEx.PIf / 6);
            }
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
                //fFrustum.proj = Matrix.PerspectiveFovRH(MathEx.Angle2Radian(FieldOfView), AspectRatio, NearPlane, FarPlane);
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

        public override void Update()
        {
            position.Y = height;
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
                    orientation *= Quaternion.RotationAxis(new Vector3(1, 0, 0), MathEx.Degree2Radian(value - height));

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
            Vector3 f = front;
            f.Y = 0;
            f.Normalize();
            position += f * dMoveSpeed;
        }
        public void MoveBack()
        {
            Vector3 f = front;
            f.Y = 0;
            f.Normalize();
            position -= f * dMoveSpeed;
        }

        public void TurnLeft()
        {
            Quaternion iq = orientation;
            iq.Conjugate();
            Vector3 top = Vector3.TransformSimple(Vector3.UnitY, iq);

            orientation *= Quaternion.RotationAxis(top, -dTurnSpeed);

            orientation.Normalize();
        }
        public void TurnRight()
        {
            Quaternion iq = orientation;
            iq.Conjugate();

            Vector3 top = Vector3.TransformSimple(Vector3.UnitY, iq);

            orientation *= Quaternion.RotationAxis(top, dTurnSpeed);
            orientation.Normalize();
        }
        public void MoveLeft()
        {
            position -= right * dMoveSpeed;
        }
        public void MoveRight()
        {
            position += right * dMoveSpeed;
        }

    }

}
