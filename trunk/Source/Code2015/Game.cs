using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Apoc3D;
using Apoc3D.Graphics;
using Code2015.EngineEx;
using Code2015.GUI;

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

        RenderSystem renderSys;
        GameScene scene;
        

        bool isLoaded;

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
            this.scene = new GameScene(renderSys);
            this.ingameUI = new InGameUI(game, this, scene);
        }

        public void Render(Sprite sprite)
        {
            ingameUI.Render(sprite);
        }
        public void Render()
        {
            if (IsLoaded)
            {
                scene.RenderScene();
            }
            else
            {
                scene.ActivateVisibles();
            }
        }
        public void Update(GameTime time)
        {
            if (!IsLoaded)
            {
                IsLoaded = TerrainMeshManager.Instance.IsIdle & ModelManager.Instance.IsIdle & TextureManager.Instance.IsIdle;
            }
            else
            {
                scene.Update(time);
                //throw new Exception();
            }
        }
    }

}
