using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;

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

        void Load()
        {

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
