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
            if (!IsLoaded)
            {
                IsLoaded = TerrainMeshManager.Instance.IsIdle & ModelManager.Instance.IsIdle & TextureManager.Instance.IsIdle;                
            }
        }
    }

}
