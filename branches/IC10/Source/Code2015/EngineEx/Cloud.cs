/*
-----------------------------------------------------------------------------
This source file is part of Zonelink

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

        Model[] strike;
        Model model;

        float scale;
        Matrix trans;

        float startTime;
        int strikeFrame;
        float strikeTime;

        const float FadeTime = 1;
        const float StrikeDuration = 2;
        const float Duration = 10;

        Normal3DSoundObject sndObject;
        bool played;

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

            strike = new Model[3];
            for (int i = 0; i < 3; i++)
            {
                fl = FileSystem.Instance.Locate("strike_lgt" + (i + 1).ToString("D2") + ".mesh", GameFileLocs.Model);
                strike[i] = new Model(ModelManager.Instance.CreateInstance(rs, fl));
            }

            strikeTime = -2 * Randomizer.GetRandomSingle();
            UpdateModelAnim();

            sndObject = (Normal3DSoundObject)SoundManager.Instance.MakeSoundObjcet("lightning", null, 1800);
         }
        public void Apply3D()
        {
            sndObject.Position = trans.TranslationValue;
        }

        void UpdateModelAnim()
        {
            if (model != null)
            {
                model.CurrentAnimation = new NoAnimation(
                    Matrix.Scaling(scale, scale, scale) * trans);

                for (int i = 0; i < strike.Length; i++)
                {
                    strike[i].CurrentAnimation = new NoAnimation(Matrix.Scaling(scale, scale, scale) * Matrix.Translation(0, -50, 0) * trans);
                }
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
                strikeFrame++;

                if (strikeFrame >= strike.Length * 2)
                    strikeFrame = 0;
                return strike[strikeFrame / 2].GetRenderOperation();                
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
            {
                played = false;
                strikeTime = 0;
            }
            else if (!played && strikeTime > StrikeDuration - 0.2f)
            {
                sndObject.Fire();
                played = true;
            }


        }

        #endregion
    }
}
