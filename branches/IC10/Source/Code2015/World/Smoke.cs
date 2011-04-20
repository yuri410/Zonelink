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
        struct Smoke
        {
            public Vector3 Position;
            public float Life;
            public float Scale;
        };

        Smoke[] emitSmokes;
        FastList<RenderOperation> opBuffer = new FastList<RenderOperation>();

        private Model smokeModel;
        private Vector3 emitPosition;
        private Vector3 emitDirection;


        public Smokes(Vector3 start, Vector3 dir, RenderSystem rs)
        {
            emitPosition = start;
            emitDirection = dir;
            emitSmokes = new Smoke[4];

            FileLocation fl = FileSystem.Instance.Locate("smoke.mesh", GameFileLocs.Model);
            smokeModel = new Model(ModelManager.Instance.CreateInstance(rs, fl));

            emitSmokes[0].Position = emitPosition;
            emitSmokes[0].Life = 1.0f;
            emitSmokes[0].Scale = 1.0f;

            emitSmokes[1].Position = emitPosition;
            emitSmokes[1].Life = 1.2f;
            emitSmokes[1].Scale = 1.0f;

            emitSmokes[2].Position = emitPosition;
            emitSmokes[2].Life = 1.4f;
            emitSmokes[2].Scale = 1.0f;

            emitSmokes[3].Position = emitPosition;
            emitSmokes[3].Life = 1.6f;
            emitSmokes[3].Scale = 1.0f;

        }

        public void Update(GameTime gameTime)
        { 
            float dt = gameTime.ElapsedGameTimeSeconds;

            for(int i = 0; i < emitSmokes.Length; i++)
            {
                emitSmokes[i].Life -= dt;  

                if (emitSmokes[i].Life < 1)   //释放
                {
                    emitSmokes[i].Position += dt * emitDirection;
                    emitSmokes[i].Scale += dt;
                }

                if (emitSmokes[i].Life < 0)  //回收
                {
                    emitSmokes[i].Life = 1.2f;
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
