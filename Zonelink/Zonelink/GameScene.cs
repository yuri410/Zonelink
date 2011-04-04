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

namespace Zonelink
{
    class GameScene : DrawableGameComponent
    {
        Game1 game;
        TerrainTile[] terrainTiles;
        RtsCamera camera;

        TerrainMaterialLibrary terrainLibrary;

        public RtsCamera Camera { get { return camera; } }

        public GameScene(Game1 game)
            : base(game)
        {
            this.game = game;
            terrainTiles = new TerrainTile[PlanetEarth.ColTileCount * PlanetEarth.LatTileCount];

            Viewport vp = game.GraphicsDevice.Viewport;
            float aspectRatio = vp.Width / (float)vp.Height;

            camera = new RtsCamera(45, aspectRatio);
            camera.NearPlane = 20;
            camera.FarPlane = 6000;
        }


        protected override void LoadContent()
        {
            terrainLibrary = new TerrainMaterialLibrary(game);
            terrainLibrary.LoadTextureSet(Path.Combine(GameFileLocs.Configs, "terrainMaterial.xml"));

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
            for (int i = 0; i < terrainTiles.Length; i++)
            {
                terrainTiles[i].Dispose();
            }
        }

        public override void Draw(GameTime time)
        {
            for (int i = 0; i < terrainTiles.Length; i++)
            {
                terrainTiles[i].Render();
            }
        }

        public override void Update(GameTime time) 
        {
            camera.Update(time);
        }
    }
}
