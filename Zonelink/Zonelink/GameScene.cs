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

namespace Zonelink
{
    class GameScene : DrawableGameComponent
    {
        Game1 game;
        TerrainTile[] terrainTiles;
        RtsCamera camera;

        TerrainMaterialLibrary terrainLibrary;
        Effect terrainEffect;
        bool isLoaded;


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
            //terrain
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
            

            //Iint Battle Field
            BattleField.Instance.Initialize();

            isLoaded = true;
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

        public override void Draw(GameTime time)
        {
            
            if (!isLoaded)
                return;

            Frustum frus = camera.Frustum;    
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
        }
    }
}
