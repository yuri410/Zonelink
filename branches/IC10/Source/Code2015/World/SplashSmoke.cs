using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.Collections;
using Apoc3D.MathLib;
using Apoc3D;
using Apoc3D.Vfs;
using Code2015.EngineEx;

namespace Code2015.World
{

    class SplashSmokes : IRenderable
    {
        const int SmokeCount = 8;
        const float SmokeStepTime = 0.05f;
        const float SmokeStepHeight = 400.0f / SmokeCount;

        struct Smoke
        {
            public Vector3 Position;
            public float Life;
            public float Scale;
            //public Vector3 Wind;

            public float Rotation;
            public float ReleaseTime;
        };

        Smoke[] emitSmokes;
        FastList<RenderOperation> opBuffer = new FastList<RenderOperation>();

        private Model smokeModel;
        City parent;

        public SplashSmokes(City city, RenderSystem rs)
        {
            this.parent = city;



            emitSmokes = new Smoke[SmokeCount];

            FileLocation fl = FileSystem.Instance.Locate("smoke_light.mesh", GameFileLocs.Model);
            smokeModel = new Model(ModelManager.Instance.CreateInstance(rs, fl));
        }

        public void Fire()
        {
            for (int i = 0; i < emitSmokes.Length; i++)
            {
                emitSmokes[i].Position = new Vector3(0, SmokeStepHeight * i, 0);
                emitSmokes[i].Scale = 2.5f;
                emitSmokes[i].Life = 0.4f;
                emitSmokes[i].ReleaseTime = i * SmokeStepTime;
                emitSmokes[i].Rotation = Randomizer.GetRandomSingle() * MathEx.PIf * 2;

            }
        }

        public void Update(GameTime gameTime)
        {
            float dt = gameTime.ElapsedGameTimeSeconds;


            for (int i = 0; i < emitSmokes.Length; i++)
            {
                if (emitSmokes[i].ReleaseTime > 0)
                {
                    emitSmokes[i].ReleaseTime -= dt;
                }
                else if (emitSmokes[i].Life > 0)
                {
                    emitSmokes[i].Position.Y += (dt * 25);
                    emitSmokes[i].Scale += 1.5f * dt;
                    emitSmokes[i].Life -= dt;
                }
            }


        }

            
    

        #region IRenderable 成员

        public RenderOperation[] GetRenderOperation()
        {
            bool passed = false;
            opBuffer.FastClear();
            foreach (Smoke s in emitSmokes)
            {
                if (s.ReleaseTime <= 0 && s.Life > 0)
                {
                    RenderOperation[] ops = smokeModel.GetRenderOperation();
                    if (ops != null)
                    {
                        for (int i = 0; i < ops.Length; i++)
                        {
                            ops[i].Transformation = Matrix.RotationZ(s.Rotation) * Matrix.Scaling(s.Scale * 2, s.Scale * 2, s.Scale * 2);
                            ops[i].Transformation.TranslationValue = s.Position;
                        }

                        opBuffer.Add(ops);

                        passed = true;
                    }
                }
            }

            if (passed)
            {
                opBuffer.Trim();
                return opBuffer.Elements;
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
