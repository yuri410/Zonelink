using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Animation;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.BalanceSystem;
using Code2015.EngineEx;
using Code2015.Logic;

namespace Code2015.World
{
    class CityLinkableMark : SceneObject
    {
        Player player;
        CityObject start;
        CityObject[] targets;

        Model[] linkArrow;

        FastList<RenderOperation> opBuffer = new FastList<RenderOperation>();

        public void SetCity(CityObject start, CityObject[] targets)
        {
            this.start = start;
            this.targets = targets;

            if (targets != null)
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    CityObject end = targets[i];

                    Vector3 dir = end.Position - start.Position;

                    dir.Normalize();

                    Vector3 pa = start.Position + dir * CityStyleTable.CityRadius;
                    Vector3 pb = end.Position - dir * CityStyleTable.CityRadius;


                    float dist = Vector3.Distance(pa, pb);

                    float longitude = MathEx.Degree2Radian(MathEx.LinearInterpose(start.Longitude, end.Longitude, 0.3f));
                    float latitude = MathEx.Degree2Radian(MathEx.LinearInterpose(start.Latitude, end.Latitude, 0.3f));

                    Matrix ori = Matrix.Identity;
                    ori.Right = Vector3.Normalize(pa - pb);
                    ori.Up = PlanetEarth.GetNormal(longitude, latitude);
                    ori.Forward = Vector3.Normalize(Vector3.Cross(ori.Up, ori.Right));
                    ori.TranslationValue = MathEx.LinearInterpose(pa, pb, 0.3f);
                    ori.TranslationValue.Normalize();

                    ori.TranslationValue *= (start.Position.Length() + 10);

                    linkArrow[i].CurrentAnimation = new NoAnimation(
                        Matrix.Scaling(Game.ObjectScale * 1.5f, Game.ObjectScale * 1.5f, Game.ObjectScale * 1.5f) * 
                        Matrix.RotationY(-MathEx.PiOver2) * ori);

                }
            }
        }

        public CityLinkableMark(RenderSystem rs, Player player)
            : base(false)
        {
            this.player = player;
            this.linkArrow = new Model[4];

            FileLocation fl = FileSystem.Instance.Locate("linkArrow.mesh", GameFileLocs.Model);
            for (int i = 0; i < linkArrow.Length; i++)
            {
                linkArrow[i] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
            }

            BoundingSphere.Radius = float.MaxValue;
            Transformation = Matrix.Identity;
        }

        #region IRenderable 成员

        public override RenderOperation[] GetRenderOperation()
        {
            if (targets != null)
            {
                opBuffer.FastClear();

                if (start.IsCaptured && start.Owner == player)
                {
                    for (int i = 0; i < targets.Length; i++)
                    {
                        if (!targets[i].IsCaptured)
                        {
                            RenderOperation[] ops = linkArrow[i].GetRenderOperation();
                            if (ops != null)
                            {
                                opBuffer.Add(ops);
                            }
                        }
                    }
                }

                opBuffer.Trim();
                return opBuffer.Elements;
            }
            return null;
        }

        public override RenderOperation[] GetRenderOperation(int level)
        {
            return GetRenderOperation();
        }

        #endregion

        public override void Update(GameTime time)
        {

        }

        public override bool IsSerializable
        {
            get { return false; }
        }
    }
}
