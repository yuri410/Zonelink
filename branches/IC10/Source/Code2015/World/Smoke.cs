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

        private const int MinSmokeCount = 5;
        private const int MaxSmokeCount = 10;

        struct Smoke
        {
            public Vector3 Position;
            public float Life;
            public float Scale;
            public Vector3 Wind;
        };

        Smoke[] emitSmokes;
        FastList<RenderOperation> opBuffer = new FastList<RenderOperation>();

        private Model smokeModel;

        private Vector3 emitPosition;
        private Vector3 emitDirection;
        private Vector3 emitNormal;


        public Smokes(Vector3 start, Vector3 dir, Vector3 normal, RenderSystem rs)
        {
            emitPosition = start;
            emitDirection = dir;
            emitNormal = normal;

            int smokeCount = MinSmokeCount + (int)((MaxSmokeCount - MinSmokeCount) * Randomizer.GetRandomSingle());

            emitSmokes = new Smoke[smokeCount];

            FileLocation fl = FileSystem.Instance.Locate("smoke.mesh", GameFileLocs.Model);
            smokeModel = new Model(ModelManager.Instance.CreateInstance(rs, fl));


            for (int i = 0; i < emitSmokes.Length; i++)
            {
                emitSmokes[i].Position = emitPosition;
                emitSmokes[i].Scale = 1.0f;
                emitSmokes[i].Life = 1.0f + Randomizer.GetRandomSingle();

                Vector3 tanget = Vector3.Cross(emitDirection, emitNormal);

                float randomX = ( (-1) * Randomizer.GetRandomSingle() + 0.5f ) * MathEx.PiOver2;
                Vector4 newDir = Vector3.Transform(emitDirection, Matrix.RotationAxis(normal, randomX));

                float randomY = ( (-1) * Randomizer.GetRandomSingle() + 0.5f) * MathEx.PiOver2;
                newDir = Vector4.Transform(newDir, Matrix.RotationAxis(tanget, randomY));
                
                emitSmokes[i].Wind.X = newDir.X;
                emitSmokes[i].Wind.Y = newDir.Y;
                emitSmokes[i].Wind.Z = newDir.Z;
                emitSmokes[i].Wind.Normalize();   

            }
            

        }

        public void Update(GameTime gameTime)
        { 
            float dt = gameTime.ElapsedGameTimeSeconds;

            for(int i = 0; i < emitSmokes.Length; i++)
            {
                emitSmokes[i].Life -= dt;  

                if (emitSmokes[i].Life < 1)   //释放
                {
                    emitSmokes[i].Position += dt * emitSmokes[i].Wind;
                    emitSmokes[i].Scale += dt;
                }

                if (emitSmokes[i].Life < 0)  //回收
                {
                    emitSmokes[i].Life = 1.0f + Randomizer.GetRandomSingle();
                    emitSmokes[i].Position = emitPosition;
                    emitSmokes[i].Scale = 1.0f;
                }
                    
            }
        }

        #region IRenderable 成员

        public RenderOperation[] GetRenderOperation()
        {
            foreach (Smoke s in emitSmokes)
            {
                if (s.Life < 1)
                {
                    RenderOperation[] ops = null;
                    ops = smokeModel.GetRenderOperation();
                    if (ops != null)
                    {
                        for (int i = 0; i < ops.Length; i++)
                        {
                            RenderOperation op = ops[i];
                            op.Transformation = Matrix.Scaling(new Vector3(s.Scale)) * Matrix.Translation(s.Position);
                            op.Sender = this;
                            opBuffer.Add(ref op);
                        }
                    }

                }        
            }
            opBuffer.Trim();
            return opBuffer.Elements;
          
        }

        public RenderOperation[] GetRenderOperation(int level)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}
