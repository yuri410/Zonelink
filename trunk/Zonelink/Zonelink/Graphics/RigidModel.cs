using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using CustomModelAnimation;
using Microsoft.Xna.Framework;
using System.IO;

namespace Zonelink.Graphics
{
    class RigidModel
    {
        Model rigidModel;

        bool playingRigid;
        RootAnimationPlayer rigidRootPlayer;
        ModelAnimationClip rigidRootClip;
        RigidAnimationPlayer rigidPlayer;
        ModelAnimationClip rigidClip;
        string clipName = "Take 001";


        public event EventHandler Completed;

        public bool IsPlaying { get { return playingRigid; } }
        public string GetClipName() { return clipName; }
        public void SetClipName(string val)
        {
            clipName = val;
            

            // Create animation players/clips for the rigid model
            ModelData modelData = rigidModel.Tag as ModelData;
            if (modelData != null)
            {
                if (modelData.RootAnimationClips != null && modelData.RootAnimationClips.ContainsKey(clipName))
                {
                    rigidRootClip = modelData.RootAnimationClips[clipName];

                    rigidRootPlayer = new RootAnimationPlayer();
                    rigidRootPlayer.Completed += new EventHandler(rigidPlayer_Completed);
                    rigidRootPlayer.StartClip(rigidRootClip, 1, TimeSpan.Zero);
                }
                if (modelData.ModelAnimationClips != null && modelData.ModelAnimationClips.ContainsKey(clipName))
                {
                    rigidClip = modelData.ModelAnimationClips[clipName];

                    rigidPlayer = new RigidAnimationPlayer(rigidModel.Bones.Count);
                    rigidPlayer.Completed += new EventHandler(rigidPlayer_Completed);
                    rigidPlayer.StartClip(rigidClip, 1, TimeSpan.Zero);
                }
            }

        }
        public Model Model { get { return rigidModel; } }

        public RigidModel(Game1 game, string assetName)
        {
            // Load the skinned model
            rigidModel = game.Content.Load<Model>(assetName);
            
            SetClipName(clipName);
            
        }

        //判断动画是否播放完了
        void rigidPlayer_Completed(object sender, EventArgs e)
        {
            playingRigid = false;
            if (Completed != null)
                Completed(this, EventArgs.Empty);
        }

        public void SetupEffect(SkinnedEffect effect, Matrix transform)
        {
            Matrix[] boneTransforms = null;
            if (rigidPlayer != null)
                boneTransforms = rigidPlayer.GetBoneTransforms();

            Matrix rootTransform = Matrix.Identity;
            if (rigidRootPlayer != null)
                rootTransform = rigidRootPlayer.GetCurrentTransform();

            if (boneTransforms != null)
                effect.SetBoneTransforms(boneTransforms);
            effect.World = rootTransform * transform;
            //effect.SpecularColor = Vector3.Zero;
        }

        public void Play()
        {
            if (!playingRigid)
            {
                if (rigidPlayer != null && rigidClip != null)
                {
                    rigidPlayer.StartClip(rigidClip, 1, TimeSpan.Zero);
                    playingRigid = true;
                }

                if (rigidRootPlayer != null && rigidRootClip != null)
                {
                    rigidRootPlayer.StartClip(rigidRootClip, 1, TimeSpan.Zero);
                    playingRigid = true;
                }
            }
        }
        public void Update(GameTime time)
        {
            if (playingRigid)
            {
                if (rigidRootPlayer != null)
                    rigidRootPlayer.Update(time);

                if (rigidPlayer != null)
                    rigidPlayer.Update(time);
            }
        }       
    }
}
