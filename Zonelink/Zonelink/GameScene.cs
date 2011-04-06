using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Code2015.EngineEx;
using Code2015.World;
using CustomModelAnimation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Zonelink.Graphics;
using Zonelink.MathLib;
using Zonelink.World;

namespace Zonelink
{
    class GameScene : DrawableGameComponent
    {
        Game1 game;
        GameState state;
        
#region  Terrain
        TerrainTile[] terrainTiles;
        TerrainMaterialLibrary terrainLibrary;
        Effect terrainEffect;
#endregion
      
        


        Model CityModel;
        Matrix[] CityModelTransforms;
        BasicEffect basicEffect;
    

        RtsCamera camera;

        
        bool isLoaded;

        SkinnedModel model;


        

        RigidModel modelSend;
        RigidModel modelIdle;
        RigidModel modelStopped;
        RigidModel modelReceive;


        public RtsCamera Camera { get { return camera; } }

        public GameScene(Game1 game, GameState state)
            : base(game)
        {
            this.game = game;
            this.state = state;
           
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

            
            model = new SkinnedModel(game, "DudeWalk");

            modelSend = new RigidModel(game, "testSend");
            modelReceive = new RigidModel(game, "testRecv");
            modelStopped = new RigidModel(game, "testStopped");
            modelIdle = new RigidModel(game, "testIdle");         

            isLoaded = true;
        }

       

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

        protected override void UnloadContent()
        {
            isLoaded = false;
            for (int i = 0; i < terrainTiles.Length; i++)
            {
                terrainTiles[i].Dispose();
            }
            terrainEffect.Dispose();
        }


        private void DrawTerrain(GameTime time)
        {
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
            }
        }
        private void DrawBattleField(GameTime time)
        {
            Frustum frus = camera.Frustum;
            
            BattleField btfld = state.Field;


            //渲染战场
            foreach (City city in btfld.Cities)
            {
                if (frus.IntersectsSphere(city.Position, city.BoundingSphere.Radius))
                {
                    switch (city.AnimationType)
                    {
                        case CityAnimationType.Stopped:
                            DrawRigidModel(city.Transformation, this.state.Field.CityModelStopped[city]);
                            
                            break;

                        case CityAnimationType.SendBall:
                            DrawRigidModel(city.Transformation, this.state.Field.CityModelSend[city]);
                            if (modelStopped.IsPlaying == false)
                            {
                                city.animationPlayOver = true;
                            }
                            break;

                        case CityAnimationType.ReceiveBall:
                            DrawRigidModel(city.Transformation, this.state.Field.CityModelReceive[city]);
                            break;;
                    }
                                      
                }

            }
        }

        public override void Draw(GameTime time)
        {
            if (!isLoaded)
                return;

            DrawTerrain(time);
            DrawBattleField(time);
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

            BattleField btfld = this.state.Field;
            btfld.Update(time);

            //if (!model.IsPlaying)
            //{
            //    model.Play();
            //}
            //else
            //{
            //    model.Update(time);
            //}

            if (!modelStopped.IsPlaying)
            {
                modelStopped.Play();
            }
            else
            {
                modelStopped.Update(time);
            }

            if (!modelSend.IsPlaying)
            {
                modelSend.Play();
            }
            else
            {
                modelSend.Update(time);
            }



            //if (!modelReceive.IsPlaying)
            //{
            //    modelReceive.Play();
            //}
            //else
            //{
            //    modelReceive.Update(time);
            //}


        }

        private void DrawSkinnedModel(Matrix transform, SkinnedModel model)
        {
            
            Model m = model.Model;
            
            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.Projection = camera.ProjectionMatrix;
                    effect.View = camera.ViewMatrix;
                    model.SetupEffect(effect, transform);
                             
                    effect.SpecularColor = Vector3.Zero;
                }

                mesh.Draw();
            }
        }

        private void DrawRigidModel(Matrix transform, RigidModel model)
        {
            Model m = model.Model;

            foreach (ModelMesh mesh in m.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.Projection = camera.ProjectionMatrix;
                    effect.View = camera.ViewMatrix;
                    model.SetupEffect(effect, transform, mesh);

                    effect.SpecularColor = Vector3.Zero;
                }

                mesh.Draw();
            }
        }


    }
}
