using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Code2015.BalanceSystem;
using Code2015.EngineEx;
using Code2015.GUI;
using Code2015.Logic;
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
        public const float ObjectScale = 3;
        public const float TreeScale = ObjectScale * 1.33f;

        object syncHelper = new object();

        GameCreationParameters parameters;

        Code2015 game;

        #region 界面/呈现
        InGameUI ingameUI;

        RenderSystem renderSys;
        GameScene scene;
        CityStyleTable cityStyles;

        #endregion

        Map map;

        #region 游戏状态
        GameState gameState;
        #endregion


        bool isLoaded;
        int loadingCountDown = 100;
        int maxLoadingOp;

        public GameScene Scene
        {
            get { return scene; }
        }

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
        public float LoadingProgress
        {
            get;
            private set;
        }

        public Player HumanPlayer
        {
            get;
            private set;
        }

        public Map Map 
        {
            get { return map; }
        }

        Player[] GetLocalPlayers(GameCreationParameters gcp)
        {
            List<Player> list = new List<Player>();
            if (gcp.Player1 !=null && gcp.Player1.Type != PlayerType.Remote) 
            {
                list.Add(gcp.Player1);
            }
            if (gcp.Player2 != null && gcp.Player2.Type != PlayerType.Remote)
            {
                list.Add(gcp.Player2);
            }
            if (gcp.Player3 != null && gcp.Player3.Type != PlayerType.Remote)
            {
                list.Add(gcp.Player3);
            }
            if (gcp.Player4 != null && gcp.Player4.Type != PlayerType.Remote) 
            {
                list.Add(gcp.Player4);
            }
            return list.ToArray();
        }

        public Game(Code2015 game, GameCreationParameters gcp)
        {
            this.game = game;
            this.renderSys = game.RenderSystem;
            this.parameters = gcp;


            // 初始化GameState
            GameStateBuilder stateBuilder = new GameStateBuilder();


            gameState = new GameState(stateBuilder, GetLocalPlayers(gcp));
            HumanPlayer = gameState.LocalHumanPlayer;

            // 初始化场景
            this.cityStyles = new CityStyleTable(renderSys);
            this.scene = new GameScene(renderSys);
            {
                City hstart = HumanPlayer.Area.RootCity;
                this.scene.Camera.Longitude = MathEx.Degree2Radian(hstart.Longitude);
                this.scene.Camera.Latitude = MathEx.Degree2Radian(hstart.Latitude);
            }
            SimulationWorld slgSystem = gameState.SLGWorld;


            map = new Map(slgSystem);
            for (int i = 0; i < slgSystem.CityCount; i++)
            {
                City city = slgSystem.GetCity(i);

                CityObject cityObj = new CityObject(renderSys, map, scene.Scene, city, cityStyles);

                cityObj.CityVisible += scene.City_Visible;

                scene.Scene.AddObjectToScene(cityObj);
            }

            for (int i = 0; i < slgSystem.ResourceCount; i++)
            {
                NaturalResource obj = slgSystem.GetResource(i);
                switch (obj.Type)
                {
                    case NaturalResourceType.Petro:
                        OilField oilfld = obj as OilField;
                        if (oilfld != null)
                        {
                            OilFieldObject oilObj = new OilFieldObject(renderSys, oilfld);
                            oilObj.ResVisible += scene.Resource_Visible;
                            scene.Scene.AddObjectToScene(oilObj);
                        }
                        break;
                    case NaturalResourceType.Wood:
                        Forest forest = obj as Forest;
                        if (forest != null)
                        {
                            ForestObject forestObj = new ForestObject(renderSys, forest);
                            forestObj.ResVisible += scene.Resource_Visible;
                            scene.Scene.AddObjectToScene(forestObj);
                        }
                        break;
                }
            }


            slgSystem.EnergyStatus.DisasterArrived += this.DisasterArrived;
            //slgSystem.EnergyStatus.DisasterOver += this.DisasterOver;

            this.ingameUI = new InGameUI(game, this, scene, gameState);
        }

        void DisasterArrived(Disaster d)
        {
            LightningStrom strom = new LightningStrom(renderSys, d);
            scene.Scene.AddObjectToScene(strom);
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
                bool newVal = TerrainMeshManager.Instance.IsIdle &
                    ModelManager.Instance.IsIdle &
                    TextureManager.Instance.IsIdle &
                    TreeBatchModelManager.Instance.IsIdle;

                if (newVal)
                {
                    if (--loadingCountDown < 0)
                    {
                        IsLoaded = true;
                    }
                }
                else
                {
                    if (++loadingCountDown > 100)
                        loadingCountDown = 100;
                }

                int ldop = TerrainMeshManager.Instance.GetCurrentOperationCount();
                ldop += ModelManager.Instance.GetCurrentOperationCount();
                ldop += TextureManager.Instance.GetCurrentOperationCount();
                ldop += TreeBatchModelManager.Instance.GetCurrentOperationCount();

                if (ldop > maxLoadingOp)
                    maxLoadingOp = ldop;

                if (maxLoadingOp > 0)
                {
                    float newPrg = 1 - ldop / (float)maxLoadingOp;
                    if (newPrg > LoadingProgress)
                        LoadingProgress = newPrg;
                }
            }
            else
            {                
                if (gameState.IsTimeUp)
                {
                    if (!ingameUI.IsShowingScore)
                    {
                        ScoreEntry[] entries = gameState.GetScores();
                        ingameUI.ShowScore(entries);
                    }
                }
                else
                {
                    gameState.Update(time);
                }
                ingameUI.Update(time);

            }
        }
    }

}
