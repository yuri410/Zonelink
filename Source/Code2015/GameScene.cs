using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Code2015.EngineEx;
using Code2015.World;

namespace Code2015
{
    class GameScene
    {
        RenderSystem renderSys;

        List<TerrainTile> terrList = new List<TerrainTile>();
        FpsCamera camera;
        ReflectionCamera reflectionCamera;
        SceneRenderer renderer;
        RenderTarget reflectionRt;

        public GameScene(RenderSystem rs)
        {
            renderSys = rs;
        }

        public void Update(GameTime time)
        {

        }

        public void RenderScene()
        {

        }

    }
}
