using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using CustomModelAnimation;
using System.IO;
using Microsoft.Xna.Framework;

namespace Zonelink.Graphics
{
    class SkinnedModel
    {
        Model skinnedModel;
        bool playingSkinned;
        RootAnimationPlayer skinnedRootPlayer;
        ModelAnimationClip skinnedRootClip;
        SkinnedAnimationPlayer skinnedPlayer;
        ModelAnimationClip skinnedClip;
        string clipName = "Take 001";

        public event EventHandler Completed;

        public bool IsPlaying { get { return playingSkinned; } }
        public string GetClipName() { return clipName; }
        public void SetClipName(string val)
        {
            clipName = val;

            // Create animation players for the skinned model
            ModelData modelData = skinnedModel.Tag as ModelData;
            
            if (modelData != null)
            {
                if (modelData.RootAnimationClips != null && modelData.RootAnimationClips.ContainsKey(clipName))
                {
                    skinnedRootClip = modelData.RootAnimationClips[clipName];

                    skinnedRootPlayer = new RootAnimationPlayer();
                    skinnedRootPlayer.Completed += new EventHandler(skinnedPlayer_Completed);
                }
                if (modelData.ModelAnimationClips != null && modelData.ModelAnimationClips.ContainsKey(clipName))
                {
                    skinnedClip = modelData.ModelAnimationClips[clipName];

                    skinnedPlayer = new SkinnedAnimationPlayer(modelData.BindPose, modelData.InverseBindPose, modelData.SkeletonHierarchy);
                    skinnedPlayer.Completed += new EventHandler(skinnedPlayer_Completed);
                }
            }

        }
        public Model Model { get { return skinnedModel; } }
        public SkinnedModel(Game1 game, string assetName)
        {
            // Load the skinned model
            skinnedModel = game.Content.Load<Model>(Path.Combine("Model", assetName));

            SetClipName(clipName);
            
        }

        //判断动画是否播放完了
        void skinnedPlayer_Completed(object sender, EventArgs e)
        {
            playingSkinned = false;
            if (Completed != null)
                Completed(this, EventArgs.Empty);
        }

        public Matrix[] GetBoneTransforms()
        {
            Matrix[] boneTransforms = null;
            if (skinnedPlayer != null)
                boneTransforms = skinnedPlayer.GetSkinTransforms();
            return boneTransforms;
        }
        public Matrix GetRootTransform()
        {
            Matrix rootTransform = Matrix.Identity;
            if (skinnedRootPlayer != null)
                rootTransform = skinnedRootPlayer.GetCurrentTransform();
            return rootTransform;
        }
        public void SetupEffect(SkinnedEffect effect,Matrix transform) 
        {
            Matrix[] boneTransforms = null;
            if (skinnedPlayer != null)
                boneTransforms = skinnedPlayer.GetSkinTransforms();

            Matrix rootTransform = Matrix.Identity;
            if (skinnedRootPlayer != null)
                rootTransform = skinnedRootPlayer.GetCurrentTransform();
            if (boneTransforms != null)
                effect.SetBoneTransforms(boneTransforms);
            effect.World = rootTransform * transform;
            //effect.SpecularColor = Vector3.Zero;
        }

        public void Play()
        {
            if (playingSkinned == false)
            {
                if (skinnedPlayer != null && skinnedClip != null)
                {
                    skinnedPlayer.StartClip(skinnedClip, 1, TimeSpan.Zero);
                    playingSkinned = true;
                }

                if (skinnedRootPlayer != null && skinnedRootClip != null)
                {
                    skinnedRootPlayer.StartClip(skinnedRootClip, 1, TimeSpan.Zero);
                    playingSkinned = true;
                }
            }
        }
        public void Update(GameTime time)
        {
            if (playingSkinned)
            {
                if (skinnedRootPlayer != null)
                    skinnedRootPlayer.Update(time);

                if (skinnedPlayer != null)
                    skinnedPlayer.Update(time);
            }
        }       
    }
}
