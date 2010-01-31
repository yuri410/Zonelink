using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using System.Threading;
using Apoc3D.Graphics;
using Code2015.EngineEx;

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
    class Game
    {
        object syncHelper = new object();

        GameCreationParameters parameters;

        GameScene scene;
        
        bool isLoaded;
        Thread loadingWorker;

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

        public Game(GameCreationParameters gcp)
        {
            parameters = gcp;

            loadingWorker = new Thread(Load);
            loadingWorker.Name = "Game Loader";
        }

        public void StartLoading()
        {
            loadingWorker.Start();
        }

        void Load()
        {




            // wait sync
            TerrainMeshManager.Instance.WaitForIdle();
            ModelManager.Instance.WaitForIdle();
            TextureManager.Instance.WaitForIdle();
        }
        void Unload()
        {

        }

        public void Render()
        {
            if (IsLoaded)
            {
                scene.RenderScene();
            }
        }
        public void Update(GameTime time)
        {
            
        }
    }

}
