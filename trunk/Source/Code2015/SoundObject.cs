using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Apoc3D;
using Apoc3D.MathLib;
using MXF = Microsoft.Xna.Framework;

namespace Code2015
{

    public enum SoundKind
    {
        city, farm, forest, lightning, oil, rain, river, sea, truck, wind
    }

    public class SoundObject
    {
        /// <summary>
        /// 发声物体的位置，使用物理引擎里的Vector3；
        /// </summary>
        public Vector3 Position
        {
            get;
            set;
        }
        public readonly float MinDistance = 50;
        public readonly float MaxDistance = 100;
        SoundEffectGame soundEffectGame;
        List<SoundEffectGame> activeList = new List<SoundEffectGame>();
        ContentManager contentManager;

        public string Name
        {
            get;
            set;
        }
        bool islooped = true;
        public bool IsLooped
        {
            get { return islooped; }
            set { islooped = value; }
        }

        string[,] SoundsName = new string[10, 10];//存储所有声音的名称
        string[] ObjectName = new string[10];
        public SoundObject(SoundManager sm, string name, Vector3 emitter)
        {
            unsafe
            {
                contentManager = sm.MakeContentManager();
                this.IsLooped = sm.IsLoop;
                this.Name = name;
                GetFlag();
                MXF.Vector3* tempPointer = (MXF.Vector3*)(&emitter);
                this.soundEffectGame.Emitter.Position = *tempPointer;
                this.Position = emitter;//代表发声物体的位置。
                this.SoundsName = sm.SoundsName;
                this.ObjectName = sm.ObjectName;
            }
        }
        public int Flag
        {
            get { return GetFlag(); }
        }
        public int GetFlag()
        {
            int flag = 0;
            for (int i = 0; i < 10; i++)
            {
                if (ObjectName[i].Contains(this.Name))
                    flag = i;
            }
            return flag;
        }

        public void Play(float disance, Vector3 camaraPosition)
        {
            if (IsLooped)
            {
                Random r = new Random();
                int length = 0;
                for (int i = 0; i < 10; i++)
                {
                    if (SoundsName[Flag, i] != null)
                    {
                        length = i;
                    }
                }
                while (activeList.Count < 4)
                {
                    SoundEffectGame temp1 = contentManager.Load<SoundEffectGame>(SoundsName[Flag, 0].ToString());
                    activeList.Add(temp1);
                    int medium = r.Next(1, length);
                    SoundEffectGame temp2 = contentManager.Load<SoundEffectGame>(SoundsName[Flag, medium].ToString());
                    activeList.Add(temp2);
                }
                if (disance < MaxDistance && disance > MinDistance)
                {

                    for (int index = 0; index < activeList.Count; index++)
                    {
                        SoundEffectInstance instance = activeList[index].Play(camaraPosition, this.Position, 0.9f, 0, false);
                        instance.Pause();
                        if (instance.State == SoundState.Stopped)
                        {
                            SoundEffectInstance instance1 = activeList[index].Play(camaraPosition, this.Position, 0.9f, 0, false);
                            instance1.Play();
                        }

                    }

                }
                if (disance > MaxDistance)
                {
                    for (int index = 0; index < activeList.Count; index++)
                    {
                        SoundEffectInstance instance = activeList[index].Play(camaraPosition, this.Position, 0.4f, 0, false);
                        instance.Pause();
                        if (instance.State == SoundState.Stopped)
                        {
                            SoundEffectInstance instance1 = activeList[index].Play(camaraPosition, this.Position, 0.4f, 0, false);
                            instance1.Play();
                        }

                    }
                }

            }
            else
            {
                Random r = new Random();
                int length = 0;
                for (int i = 0; i < 10; i++)
                {
                    if (SoundsName[Flag, i] != null)
                    {
                        length = i;
                    }
                }
                while (activeList.Count < 4)
                {
                    int medium = r.Next(0, length);
                    SoundEffectGame temp = contentManager.Load<SoundEffectGame>(SoundsName[Flag, medium].ToString());
                    activeList.Add(temp);
                }
                if (disance < MaxDistance && disance > MinDistance)
                {

                    for (int index = 0; index < activeList.Count; index++)
                    {
                        SoundEffectInstance instance = activeList[index].Play(camaraPosition, this.Position, 0.9f, 0, false);
                        instance.Pause();
                        if (instance.State == SoundState.Stopped)
                        {
                            SoundEffectInstance instance1 = activeList[index].Play(camaraPosition, this.Position, 0.9f, 0, false);
                            instance1.Play();
                        }

                    }
                }
                if (disance > MaxDistance)
                {

                    for (int index = 0; index < activeList.Count; index++)
                    {
                        SoundEffectInstance instance = activeList[index].Play(camaraPosition, this.Position, 0.4f, 0, false);
                        instance.Pause();
                        if (instance.State == SoundState.Stopped)
                        {
                            SoundEffectInstance instance1 = activeList[index].Play(camaraPosition, this.Position, 0.4f, 0, false);
                            instance1.Play();
                        }

                    }
                }

            }
        }
        public void Update(GameTime time, Vector3 camaraPosition)
        {
            float distance = Vector3.Distance(this.Position, camaraPosition);
            Play(distance, camaraPosition);
        }
    }
}
