using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Apoc3D;
using Apoc3D.Graphics;
using Code2015.EngineEx;
using Code2015.GUI;
using Code2015.World;

namespace Code2015
{
    class GameCreationParameters
    {
        public Player Player1
        {
            get;
            set;
        }

        public Player Player2
        {
            get;
            set;
        }

        public Player Player3
        {
            get;
            set;
        }

        public Player Player4
        {
            get;
            set;
        }
    }

    /// <summary>
    ///  表示一场游戏
    /// </summary>
    class Game : IGameComponent
    {
        object syncHelper = new object();

        GameCreationParameters parameters;

        Code2015 game;
        InGameUI ingameUI;
        GameState gameState;

        RenderSystem renderSys;
        GameScene scene;


        bool isLoaded;
        int loadingCount = 100;

        public bool IsLoaded
        {
            get
            {
                lock (syncHelper)
                {
                    return isLoaded;
                }
            }
            private set
            {
                lock (syncHelper)
                {
                    isLoaded = value;
                }
            }
        }

        public Game(Code2015 game, GameCreationParameters gcp)
        {
            this.game = game;
            this.renderSys = game.RenderSystem;
            this.parameters = gcp;


            // 初始化GameState
            GameStateBuilder stateBuilder = new GameStateBuilder();
            gameState = new GameState(stateBuilder);


            // 初始化场景
            this.scene = new GameScene(renderSys);
            this.ingameUI = new InGameUI(game, this, scene);
        }

        public void Render(Sprite sprite)
        {
            ingameUI.Render(sprite);
        }
        public void Render()
        {
            scene.RenderScene();
        }
        public void Update(GameTime time)
        {
            scene.Update(time);

            if (!IsLoaded)
            {
                bool newVal = TerrainMeshManager.Instance.IsIdle & ModelManager.Instance.IsIdle & TextureManager.Instance.IsIdle;
                if (newVal)
                {
                    if (--loadingCount < 0)
                    {
                        IsLoaded = true;
                    }
                }
            }
            else
            {
                gameState.Update(time);
            }
        }
    }

}
