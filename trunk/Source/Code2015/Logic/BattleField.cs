using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Apoc3D;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.Logic;
using Code2015.World;

namespace Code2015
{

    class ResourceBallEventArg : EventArgs 
    {
        public RBall ResourceBall
        {
            get;
            private set;
        }

        public bool BornOrDie
        {
            get;
            private set;
        }

        public ResourceBallEventArg(bool bornOrDie, RBall ball)
        {
            BornOrDie = bornOrDie;
            ResourceBall = ball;
        }
    }
    delegate void ResourceBallEventHandler(object sender, ResourceBallEventArg e);

    /// <summary>
    ///  表示当前正在进行的游戏场景中的状态
    /// </summary>
    class BattleField
    {
        public const int MaxCities = 120;

        NaturalResource[] naturalResource;

        List<RBall> resBalls = new List<RBall>();
        List<RGatheredBall> gatherBalls = new List<RGatheredBall>();


        Map map;
        BallPathFinderManager ballPathFinderMgr;
        BallPathFinder ballPathFinder;

        FogOfWar fogofwar;

        City[] cities;

        public FogOfWar Fog { get { return fogofwar; } }
        public Map Map { get { return map; } }
        public City[] Cities
        {
            get { return cities; }
        }
        public NaturalResource[] NaturalResources 
        {
            get { return naturalResource; }
        }
        public BallPathFinder BallPathFinder
        {
            get { return ballPathFinder; }
        }
        public int CityCount
        {
            get { return cities.Length; }
        } 

        public City GetVisibleCity(int i)
        {
            return cities[i];
        }

        public BattleField()
        {
            map = new Map(this);
            fogofwar = new FogOfWar();

            //Init Cities
            LoadCities();

            ballPathFinderMgr = new BallPathFinderManager(cities);
            ballPathFinder = ballPathFinderMgr.CreatePathFinder();

            //Init Natural Resource
            InitializeNaturalResource();


            fogofwar = new FogOfWar();

        }

        #region 初始化城市
        /// <summary>
        /// 初始化城市
        /// </summary>
        private void LoadCities()
        {

            List<City> cityList = new List<City>(MaxCities);

            FileLocation fl = FileSystem.Instance.Locate("cities.xml", GameFileLocs.Config);
            GameConfiguration resCon = new GameConfiguration(fl);
            GameConfiguration.ValueCollection resVals = resCon.Values;
            Dictionary<string, City> resolveTable = new Dictionary<string, City>(MaxCities);
            foreach (GameConfigurationSection sect in resVals)
            {
                City city;
                string typestr = sect.GetString("Type", string.Empty).ToLowerInvariant();

                CityType type = City.ParseType(typestr);

                switch (type)
                {
                    case CityType.Neutral:
                        city = new City(this, null, type);
                        break;
                    case CityType.Oil:
                    case CityType.Green:
                        city = new GatherCity(this, null, type);
                        break;
                    case CityType.Disease:
                    case CityType.Education:
                    case CityType.Health:
                    case CityType.Volience:
                        city = new ProductionCity(this, null, type);
                        break;
                    default:
                        city = new City(this, null, type);
                        break;
                }

                city.Parse(sect);

                resolveTable.Add(sect.Name, city);
                cityList.Add(city);
            }

            for (int i = 0; i < cityList.Count; i++) 
            {
                cityList[i].ResolveCities(resolveTable);
            }

            cities = cityList.ToArray();
        }

        #endregion



        /// <summary>
        /// 初始化自然资源
        /// </summary>
        void InitializeNaturalResource()
        {
            FileLocation fl = FileSystem.Instance.Locate("resources.xml", GameFileLocs.Config);
            GameConfiguration resCon = new GameConfiguration(fl);
            GameConfiguration.ValueCollection resVals = resCon.Values;

            List<NaturalResource> resources = new List<NaturalResource>(MaxCities);

            foreach (GameConfigurationSection sect in resVals)
            {
                string type = sect.GetString("Type", string.Empty).ToLowerInvariant();

                if (type == "wood")
                {
                    ForestObject forest = new ForestObject(this);
                    forest.Parse(sect);
                    resources.Add(forest);

                }
                else if (type == "petro")
                {
                    OilFieldObject fld = new OilFieldObject(this);
                    fld.Parse(sect);
                    resources.Add(fld);
                }           
            }
            naturalResource = resources.ToArray();

            for (int i = 0; i < cities.Length; i++)
            {
                if ( cities[i].Type == CityType.Oil || cities[i].Type == CityType.Green)
                {
                    ((GatherCity)cities[i]).FindResources(resources);
                }
            }   


        }


        public event ResourceBallEventHandler ResourceBallChanged;

        public RGatheredBall CreateRGatherBall(List<RBall> balls, City dockCity, List<City> result) 
        {
            RGatheredBall ball = new RGatheredBall(balls, dockCity, result);
            gatherBalls.Add(ball);
            return ball;
        }
        public void CreateResourceBall(Player owner, City city, RBallType type)
        {
            RBall ball = new RBall(owner, city, type);
            resBalls.Add(ball);

            if (ResourceBallChanged != null) 
            {
                ResourceBallChanged(this, new ResourceBallEventArg(true, ball));
            }
        }
        public void DestroyResourceBall(RBall ball)
        {
            resBalls.Remove(ball);
            if (ResourceBallChanged != null)
            {
                ResourceBallChanged(this, new ResourceBallEventArg(false, ball));
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int i = gatherBalls.Count-1; i >=0;i--)
            {
                gatherBalls[i].Update(gameTime);

                if (gatherBalls[i].IsOver)
                {
                    gatherBalls.RemoveAt(i);
                }
            }
            fogofwar.Update(gameTime);
            //foreach ( City city in cities)
            //{
            //    city.Update(gameTime);


            //}

            //for (int i = 0; i < naturalResource.Length; i++) 
            //{
            //    naturalResource[i].Update(gameTime);
            //}
        }


    }
}
