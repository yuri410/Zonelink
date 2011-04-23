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

    class Smokes : IRenderable
    {

        private const int MinSmokeCount = 3;
        private const int MaxSmokeCount = 5;
        private const float MinReleaseDelay = 0.2f;
        private const float MaxReleaseDelay = 0.6f;

        struct Smoke
        {
            public Vector3 Position;
            public float Life;
            public float Scale;
            public Vector3 Wind;

            public float Rotation;
            public float ReleaseTime;
        };

        Smoke[] emitSmokes;
        FastList<RenderOperation> opBuffer = new FastList<RenderOperation>();

        private Model smokeModel;
        City parent;
        Vector3 emitPosition;
        Vector3 emitDirection;
        Vector3 emitNormal;

        float emitCd;
        public bool EmitEnabled { get; set; }
        float NextEmitCD()
        {
            return MinReleaseDelay + Randomizer.GetRandomSingle() * (MaxReleaseDelay - MinReleaseDelay);
        }
        public Smokes(City city, RenderSystem rs)
        {
            this.parent = city;

           
            int smokeCount = MinSmokeCount + (int)((MaxSmokeCount - MinSmokeCount) * Randomizer.GetRandomSingle());

            emitSmokes = new Smoke[smokeCount];

            FileLocation fl = FileSystem.Instance.Locate("smoke.mesh", GameFileLocs.Model);
            smokeModel = new Model(ModelManager.Instance.CreateInstance(rs, fl));

            UpdateDirection();

            

        }
        void UpdateDirection()
        {
            Vector3 dir = new Vector3(0, 0.5f, -1.0f);
            dir.Normalize();

            emitDirection = Vector3.TransformSimple(dir, parent.CurrentFacing);
            emitPosition = Vector3.TransformSimple(new Vector3(0, 380, -125), parent.CurrentFacing);
            Vector3 right = Vector3.TransformSimple(Vector3.UnitX, parent.CurrentFacing);
            emitNormal = Vector3.Cross(right, emitDirection);

        }

        void ResetParticle(int i)
        {
            emitSmokes[i].Position = emitPosition;
            emitSmokes[i].Scale = 0.33f;
            emitSmokes[i].Life = 1.5f + 0.5f * Randomizer.GetRandomSingle();
            emitSmokes[i].ReleaseTime = NextEmitCD();
            emitSmokes[i].Rotation = Randomizer.GetRandomSingle() * MathEx.PIf * 2;

            Vector3 tanget = Vector3.Cross(emitDirection, emitNormal);

            float randomX = ((-1) * Randomizer.GetRandomSingle() + 0.5f) * MathEx.PiOver2;
            Vector3 newDir = Vector3.TransformSimple(emitDirection, Matrix.RotationAxis(emitNormal, randomX));

            float randomY = ((-1) * Randomizer.GetRandomSingle() + 0.5f) * MathEx.PiOver2;
            newDir = Vector3.TransformSimple(newDir, Matrix.RotationAxis(tanget, randomY));

            emitSmokes[i].Wind.X = newDir.X;
            emitSmokes[i].Wind.Y = newDir.Y;
            emitSmokes[i].Wind.Z = newDir.Z;
            emitSmokes[i].Wind.Normalize();
        }

        public void Update(GameTime gameTime)
        {
            float dt = gameTime.ElapsedGameTimeSeconds;
            
            if (EmitEnabled)
                emitCd -= dt;
          

            for (int i = 0; i < emitSmokes.Length; i++)
            {
                emitSmokes[i].Life -= dt;

                if (emitSmokes[i].Life > 0)
                {
                    emitSmokes[i].Position += (dt * 100) * emitSmokes[i].Wind;
                    emitSmokes[i].Scale += dt;
                }
                else if (EmitEnabled)
                {
                    if (emitSmokes[i].ReleaseTime > 0)
                    {
                        emitSmokes[i].ReleaseTime -= dt;
                    }
                    else if (emitCd < 0)
                    {
                        ResetParticle(i);
                        emitCd = 0.5f;
                    }
                }

            }

            UpdateDirection();
        }

            
    

        #region IRenderable 成员

        public RenderOperation[] GetRenderOperation()
        {
            opBuffer.FastClear();
            foreach (Smoke s in emitSmokes)
            {
                if (s.Life >0)
                {
                    RenderOperation[] ops = smokeModel.GetRenderOperation();
                    if (ops != null)
                    {
                        for (int i = 0; i < ops.Length; i++)
                        {
                            ops[i].Transformation = Matrix.RotationZ(s.Rotation) * Matrix.Scaling(new Vector3(s.Scale));
                            ops[i].Transformation.TranslationValue = s.Position;
                        }

                        opBuffer.Add(ops);
                    }
                }        
            }
            opBuffer.Trim();
            return opBuffer.Elements;
          
        }

        public RenderOperation[] GetRenderOperation(int level)
        {
            return GetRenderOperation();
        }

        #endregion
    }

}
