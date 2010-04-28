using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Animation;
using Apoc3D.MathLib;
using Apoc3D.Vfs;

namespace Code2015.EngineEx
{
    class Cloud : IRenderable, IUpdatable
    {
        

        RenderSystem renderSys;

        Model strike;
        Model model;

        float scale;
        Matrix trans;

        float startTime;

        float strikeTime;

        const float FadeTime = 1;
        const float StrikeDuration = 2;
        const float Duration = 10;


        public float Blend 
        {
            get
            {
                if (startTime > -FadeTime)
                {
                    return -startTime;
                }
                else if (startTime < -Duration + FadeTime)
                {
                    return startTime + Duration;
                }
                return 1;
            }
        }
        public float StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        public Cloud(RenderSystem rs, float beginTime)
        {
            this.renderSys = rs;
            this.scale = 7.5f;
            this.startTime = beginTime;

            this.trans = Matrix.Identity;

            FileLocation fl = FileSystem.Instance.Locate("cloud_lgt.mesh", GameFileLocs.Model);
            model = new Model(ModelManager.Instance.CreateInstance(rs, fl));

            int idx = Randomizer.GetRandomInt(3);
            fl = FileSystem.Instance.Locate("strike_lgt" + (idx + 1).ToString("D2") + ".mesh", GameFileLocs.Model);
            strike = new Model(ModelManager.Instance.CreateInstance(rs, fl));

            strikeTime = -2 * Randomizer.GetRandomSingle();
            UpdateModelAnim();
        }

        void UpdateModelAnim()
        {
            if (model != null)
            {
                model.CurrentAnimation = new NoAnimation(
                    Matrix.Scaling(scale, scale, scale) * trans);

                strike.CurrentAnimation = new NoAnimation(Matrix.Scaling(scale, scale, scale) * Matrix.Translation(0, -50, 0) * trans);
            }
        }

        public float Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                UpdateModelAnim();
            }
        }

        public Matrix Transform
        {
            get { return trans; }
            set
            {
                trans = value;
                UpdateModelAnim();
            }
        }

        public bool IsActive
        {
            get { return startTime < 0 && startTime > -Duration; }
        }

        #region IRenderable 成员
        public RenderOperation[] GetRenderOperation2()
        {
            if (strikeTime > StrikeDuration - 0.2f && startTime < 0 && startTime > -Duration)
            {
                return  strike.GetRenderOperation();
                
            }
            return null;
        }

        public RenderOperation[] GetRenderOperation()
        {
            if (startTime < 0 && startTime > -Duration)
            {
                RenderOperation[] ops = model.GetRenderOperation();
                if (ops != null)
                {
                    for (int i = 0; i < ops.Length; i++)
                    {
                        ops[i].Sender = this;
                    }
                }
                return ops;
            }
            return null;
        }

        public RenderOperation[] GetRenderOperation(int level)
        {
            if (startTime < 0 && startTime > -Duration)
            {
                RenderOperation[] ops = model.GetRenderOperation();
                if (ops != null)
                {
                    for (int i = 0; i < ops.Length; i++)
                    {
                        ops[i].Sender = this;
                    }
                }
                return ops;
            }
            return null;
        }

        #endregion

        #region IUpdatable 成员

        public void Update(GameTime dt)
        {
            startTime -= dt.ElapsedGameTimeSeconds;


            strikeTime += dt.ElapsedGameTimeSeconds;
            if (strikeTime > StrikeDuration)
                strikeTime = 0;

        }

        #endregion
    }
}
