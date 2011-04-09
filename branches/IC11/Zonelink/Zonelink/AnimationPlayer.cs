using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using CustomModelAnimation;
using Microsoft.Xna.Framework;

namespace Zonelink
{
    class AnimationPlayer
    {
        // Skinned model, animation players, clips
        public Model skinnedModel { get; set; }

        Matrix skinnedWorld;
        RootAnimationPlayer skinnedRootPlayer;
        ModelAnimationClip skinnedRootClip;
        SkinnedAnimationPlayer skinnedPlayer;
        ModelAnimationClip skinnedClip;

        public Matrix ProjectionMatrix { get; set; }
        public Matrix ViewMatrix { get; set; }


        public bool PlayingSkinned { get; set;}

        public AnimationPlayer()
        {
            
        }


        public void UpdateModel()
        {
            PlayingSkinned = true;

            ModelData modelData = skinnedModel.Tag as ModelData;
            if (modelData != null)
            {
                if (modelData.RootAnimationClips != null && modelData.RootAnimationClips.ContainsKey("Take 001"))
                {
                    skinnedRootClip = modelData.RootAnimationClips["Take 001"];

                    skinnedRootPlayer = new RootAnimationPlayer();
                    skinnedRootPlayer.Completed += new EventHandler(skinnedPlayer_Completed);
                }
                if (modelData.ModelAnimationClips != null && modelData.ModelAnimationClips.ContainsKey("Take 001"))
                {
                    skinnedClip = modelData.ModelAnimationClips["Take 001"];

                    skinnedPlayer = new SkinnedAnimationPlayer(modelData.BindPose, modelData.InverseBindPose, modelData.SkeletonHierarchy);
                    skinnedPlayer.Completed += new EventHandler(skinnedPlayer_Completed);
                }

            }
        }

        public void Update( Matrix view, Matrix proj, GameTime gameTime)
        {
            this.ViewMatrix = view;
            this.ProjectionMatrix = proj;     

            if (skinnedPlayer != null && skinnedClip != null)
            {
                skinnedPlayer.StartClip(skinnedClip, 1, TimeSpan.Zero);
                PlayingSkinned = true;
            }

            if (skinnedRootPlayer != null && skinnedRootClip != null)
            {
                skinnedRootPlayer.StartClip(skinnedRootClip, 1, TimeSpan.Zero);
                PlayingSkinned = true;
            }               

            // If we are playing skinned animations, update the players
            if (PlayingSkinned)
            {
                if (skinnedRootPlayer != null)
                    skinnedRootPlayer.Update(gameTime);

                if (skinnedPlayer != null)
                    skinnedPlayer.Update(gameTime);
            }
        }

        public void Draw(Matrix world, GameTime gameTime)
        {
            DrawSkinnedModel(world, skinnedModel, skinnedPlayer, skinnedRootPlayer);
        }


        private void DrawSkinnedModel(Matrix transform, Model model, SkinnedAnimationPlayer skinnedAnimationPlayer, RootAnimationPlayer rootAnimationPlayer)
        {
            Matrix[] boneTransforms = null;
            if (skinnedAnimationPlayer != null)
                boneTransforms = skinnedAnimationPlayer.GetSkinTransforms();

            Matrix rootTransform = Matrix.Identity;
            if (rootAnimationPlayer != null)
                rootTransform = rootAnimationPlayer.GetCurrentTransform();

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.Projection = ProjectionMatrix;
                    effect.View = ViewMatrix;
                    if (boneTransforms != null)
                        effect.SetBoneTransforms(boneTransforms);
                    effect.World = rootTransform * skinnedWorld * transform;
                    effect.SpecularColor = Vector3.Zero;
                }

                mesh.Draw();
            }
        }

        //判断动画是否播放完了
        void skinnedPlayer_Completed(object sender, EventArgs e)
        {
            PlayingSkinned = false;
        }

    }
}
