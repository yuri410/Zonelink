using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Code2015.World;
using Zonelink.World;
using Microsoft.Xna.Framework;
using Code2015.EngineEx;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Microsoft.Xna.Framework.Input;
using Zonelink.MathLib;
using CustomModelAnimation;

namespace Zonelink
{
    class GameScene : DrawableGameComponent
    {
        Game1 game;
        TerrainTile[] terrainTiles;
        
        Model CityModel;
        Matrix[] CityModelTransforms;
        BasicEffect basicEffect;


        RtsCamera camera;

        TerrainMaterialLibrary terrainLibrary;
        Effect terrainEffect;
        bool isLoaded;

        #region 骨骼测试
        Model skinnedModel;
        bool playingSkinned;
        RootAnimationPlayer skinnedRootPlayer;
        ModelAnimationClip skinnedRootClip;
        SkinnedAnimationPlayer skinnedPlayer;
        ModelAnimationClip skinnedClip;
        #endregion



        public RtsCamera Camera { get { return camera; } }

        public GameScene(Game1 game)
            : base(game)
        {
            this.game = game;
           
        }

        public override void Initialize()
        {
            terrainTiles = new TerrainTile[PlanetEarth.ColTileCount * PlanetEarth.LatTileCount];

            Viewport vp = game.GraphicsDevice.Viewport;
            float aspectRatio = vp.Width / (float)vp.Height;

            camera = new RtsCamera(45, aspectRatio);
            camera.NearPlane = 20;
            camera.FarPlane = 6000;

            base.Initialize();
        }
        protected override void LoadContent()
        {
            LoadTerrain();
          
            //加载城市资源
            CityModel = game.Content.Load<Model>("Model\\test");
            CityModelTransforms = new Matrix[CityModel.Bones.Count];
            basicEffect = new BasicEffect(game.GraphicsDevice);

            //Iint Battle Field
            BattleField.Instance.Initialize();


            // Load the skinned model
            skinnedModel = game.Content.Load<Model>("Model\\DudeWalk");
            //skinnedWorld = Matrix.CreateScale(.025f, .025f, .025f) * Matrix.CreateRotationY((float)(-Math.PI / 2));
            // Create animation players for the skinned model
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


            isLoaded = true;
        }

        #region
        private void LoadTerrain()
        {
            terrainLibrary = new TerrainMaterialLibrary(game);
            terrainLibrary.LoadTextureSet(Path.Combine(GameFileLocs.Configs, "terrainMaterial.xml"));

            terrainEffect = game.Content.Load<Effect>("Effect\\TerrainEffect");
            terrainEffect.CurrentTechnique = terrainEffect.Techniques[0];

            TerrainData.Initialize();

            for (int i = 1, index = 0; i < PlanetEarth.ColTileCount * 2; i += 2)
            {
                for (int j = 1; j < PlanetEarth.LatTileCount * 2; j += 2)
                {
                    terrainTiles[index++] = new TerrainTile(game, i, j + PlanetEarth.LatTileStart);
                }
            }

        }
        #endregion



        protected override void UnloadContent()
        {
            isLoaded = false;
            for (int i = 0; i < terrainTiles.Length; i++)
            {
                terrainTiles[i].Dispose();
            }
            terrainEffect.Dispose();
        }

        public override void Draw(GameTime time)
        {
            
            if (!isLoaded)
                return;

            Frustum frus = camera.Frustum;  
  
            //渲染地形
            game.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            foreach (EffectPass p in terrainEffect.CurrentTechnique.Passes)
            {
                for (int i = 0; i < terrainTiles.Length; i++)
                {
                    Vector3 center = terrainTiles[i].BoundingSphere.Center;
                    if (frus.IntersectsSphere(ref center, terrainTiles[i].BoundingSphere.Radius))
                    {
                        terrainEffect.Parameters["mvp"].SetValue(terrainTiles[i].Transformation * camera.ViewMatrix * camera.ProjectionMatrix);
                        terrainEffect.Parameters["terrSize"].SetValue(33);
                        terrainEffect.Parameters["world"].SetValue(terrainTiles[i].Transformation);

                        terrainEffect.Parameters["texDet1"].SetValue(terrainLibrary.GetTexture("Snow"));
                        terrainEffect.Parameters["texDet2"].SetValue(terrainLibrary.GetTexture("Grass"));
                        terrainEffect.Parameters["texDet3"].SetValue(terrainLibrary.GetTexture("Sand"));
                        terrainEffect.Parameters["texDet4"].SetValue(terrainLibrary.GetTexture("Rock"));


                        terrainEffect.Parameters["texColor"].SetValue(terrainLibrary.GlobalColorTexture);
                        terrainEffect.Parameters["texIdx"].SetValue(terrainLibrary.GlobalIndexTexture);
                        terrainEffect.Parameters["texNorm"].SetValue(terrainLibrary.GlobalNormalTexture);
                        terrainEffect.Parameters["texCliff"].SetValue(terrainLibrary.CliffColor);

                    }


                    //float4 k_d;
                    //float4 k_a;

                    //float4 i_a;
                    //float4 i_d;

                    p.Apply();

                    terrainTiles[i].Render();
                }

                //渲染战场
                foreach (City city in BattleField.Instance.Cities)
                {                  
                    if (frus.IntersectsSphere(city.Position, city.BoundingSphere.Radius))
                    {
                        DrawSkinnedModel(city.Transformation, skinnedModel, skinnedPlayer, skinnedRootPlayer);

                        //CityModel.CopyAbsoluteBoneTransformsTo(CityModelTransforms);

                        //foreach (ModelMesh m in CityModel.Meshes)
                        //{
                        //    foreach (BasicEffect e in m.Effects)
                        //    {
                        //        e.World = CityModelTransforms[m.ParentBone.Index] * city.Transformation;// *;
                        //        e.View = camera.ViewMatrix;
                        //        e.Projection = camera.ProjectionMatrix;
                        //    }
                        //    m.Draw();
                        //}
                    }
                    
                }
            }


        }

        public override void Update(GameTime time) 
        {
            camera.Update(time);

            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.W))
            {
                camera.MoveFront();
            }
            if (state.IsKeyDown(Keys.S))
            {
                camera.MoveBack();
            }
            if (state.IsKeyDown(Keys.A))
            {
                camera.MoveLeft();
            }
            if (state.IsKeyDown(Keys.D))
            {
                camera.MoveRight();
            }

            //if (skinnedPlayer != null && skinnedClip != null)
            //{
            //    skinnedPlayer.StartClip(skinnedClip, 1, TimeSpan.Zero);
            //    playingSkinned = true;
            //}

            //if (skinnedRootPlayer != null && skinnedRootClip != null)
            //{
            //    skinnedRootPlayer.StartClip(skinnedRootClip, 1, TimeSpan.Zero);
            //    playingSkinned = true;
            //}
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
            // If we are playing skinned animations, update the players
            if (playingSkinned)
            {
                if (skinnedRootPlayer != null)
                    skinnedRootPlayer.Update(time);

                if (skinnedPlayer != null)
                    skinnedPlayer.Update(time);
            }

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
                    effect.Projection = camera.ProjectionMatrix;
                    effect.View = camera.ViewMatrix;
                    if (boneTransforms != null)
                        effect.SetBoneTransforms(boneTransforms);
                    effect.World = rootTransform * transform;
                    effect.SpecularColor = Vector3.Zero;
                }

                mesh.Draw();
            }
        }

        //判断动画是否播放完了
        void skinnedPlayer_Completed(object sender, EventArgs e)
        {
            playingSkinned = false;
        }

    }
}
