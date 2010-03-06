using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.MathLib;
using Code2015.World;

namespace Code2015.EngineEx
{
    public class RtsCamera : Camera
    {
        bool isPerspective;
        float height;
        float orthoZoom;

        protected float dMoveSpeed = 0.2f * MathEx.PIf / 180;
        protected float dTurnSpeed = MathEx.PIf / 90;

        float longitude;
        float latitude;

        float rotation;
        float yaw;

        

        public RtsCamera(float fovy, float aspect)
            : base(aspect)
        {
            OrthoZoom = 65;
            isPerspective = fovy < 175 && fovy > 5;
            //FieldOfView = fovy;
            Height = 60;


            ResetView();
        }

        void UpdateView()
        {
            Vector3 target = PlanetEarth.GetPosition(longitude, latitude);

            Vector3 axis = target;
            axis.Normalize();

            //float sign = latitude > 0 ? 1 : -1;
            Vector3 up = Vector3.UnitY;

            Vector3 rotAxis = axis;

            Vector3 yawAxis = Vector3.Cross(axis, up);
            yawAxis.Normalize();

            Quaternion rotTrans = Quaternion.RotationAxis(rotAxis, rotation);
            axis = Vector3.TransformSimple(axis, Quaternion.RotationAxis(yawAxis, yaw) * rotTrans);

            position = target + axis * height * 20;
            Matrix viewTrans = Matrix.LookAtRH(position, target,
                Vector3.TransformSimple(up, rotTrans));

            Frustum.View = viewTrans;
            Frustum.Update();


            front = viewTrans.Forward;
            top = viewTrans.Up;
            right = viewTrans.Right;


            Quaternion.RotationMatrix(ref viewTrans, out orientation);
        }

        public override void ResetView()
        {
            isPerspective = FieldOfView < 175 && FieldOfView > 5;

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
                if (value >= 30 && value < 75)
                {
                    yaw = MathEx.Degree2Radian(height) - MathEx.PiOver2;

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
            longitude -= (float)Math.Sin(rotation) * dMoveSpeed;
            latitude += (float)Math.Cos(rotation) * dMoveSpeed;
            if (latitude > MathEx.PIf * 5 / 18f)
                latitude = MathEx.PIf * 5 / 18f;
            if (latitude < -MathEx.PIf * 5 / 18f)
                latitude = -MathEx.PIf * 5 / 18f;

        }
        public void MoveBack()
        {
            longitude += (float)Math.Sin(rotation) * dMoveSpeed;
            latitude -= (float)Math.Cos(rotation) * dMoveSpeed;
            if (latitude > MathEx.PIf * 5 / 18f)
                latitude = MathEx.PIf * 5 / 18f;
            if (latitude < -MathEx.PIf * 5 / 18f)
                latitude = -MathEx.PIf * 5 / 18f;
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
