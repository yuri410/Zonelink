using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zonelink.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Code2015.EngineEx;
using System.IO;
using Apoc3D;
using Code2015.Logic;
using Zonelink.Graphics;

namespace Zonelink
{
    /// <summary>
    ///  表示当前正在进行的游戏场景中的状态
    /// </summary>
    class BattleField
    {
        const int MaxCities = 120;

        ResourceBallType[] resTypes = new ResourceBallType[4];

        NatureResource[] naturalResource;

        List<RBall> resBalls = new List<RBall>();

        Map map;
        //Technology techMgr;
        Player localPlayer;

        Game1 game;
   
#region CityModel
        //cityXML中读取的城市
        List<City> cityList = new List<City>(MaxCities);
        Dictionary<City, RigidModel> cityModelStopped;
        Dictionary<City, RigidModel> cityModelIdle;
        Dictionary<City, RigidModel> cityModelSend;
        Dictionary<City, RigidModel> cityModelReceive;

        //GameScene渲染需要访问
        public Dictionary<City, RigidModel> CityModelStopped { get { return cityModelStopped; } }
        public Dictionary<City, RigidModel> CityModelIdle { get { return cityModelIdle; } }
        public Dictionary<City, RigidModel> CityModelSend { get { return cityModelSend; } }
        public Dictionary<City, RigidModel> CityModelReceive { get { return cityModelReceive; } }

#endregion

       

        public Map Map { get { return map; } }
        public List<City> Cities
        {
            get { return cityList; }
        }
                       
        public int CityCount
        {
            get { return cityList.Count; }
        } 

        public City GetVisibleCity(int i)
        {
            return cityList[i];
        }

        public BattleField(Game1 game)
        {
            this.game = game;

            map = new Map(this);

            //Init Cities
            LoadCities();

            LoadCityModel();

            //Init Natural Resource
            InitializeNaturalResource();

            //Load City Model

        }

        #region 初始化城市
        private void LoadCities()
        {
            string type;
            GameConfiguration resCon = Utils.LoadConfig("cities.xml");
            GameConfiguration.ValueCollection resVals = resCon.Values;
            foreach (GameConfigurationSection sect in resVals)
            {
                City city;
                type = sect.GetString("Type", string.Empty);
                if (type == "Green" || type == "oil")
                {
                    city = new GatherCity(this, null);
                }
                else
                {
                    city = new City(this, null);
                }
                city.Parse(sect);
                cityList.Add(city);
            }
        }

        private void LoadCityModel()
        {
            this.cityModelSend = new Dictionary<City, RigidModel>(CityCount);
            this.cityModelStopped = new Dictionary<City, RigidModel>(CityCount);
            this.cityModelReceive = new Dictionary<City, RigidModel>(CityCount);
            this.cityModelIdle = new Dictionary<City, RigidModel>(CityCount);

            foreach (City city in this.Cities)
            {
                this.cityModelSend.Add(city, new RigidModel(game, "testSend"));
                this.cityModelStopped.Add(city, new RigidModel(game, "testStopped"));
                this.cityModelIdle.Add(city, new RigidModel(game, "testIdle"));
                this.cityModelReceive.Add(city, new RigidModel(game, "testRecv"));
            }

        }
        #endregion

       

        //初始化自然资源
        void InitializeNaturalResource()
        {
            GameConfiguration resCon = Utils.LoadConfig("resources.xml");    
            GameConfiguration.ValueCollection resVals = resCon.Values;

            List<NatureResource> resources = new List<NatureResource>(MaxCities);

            foreach (GameConfigurationSection sect in resVals)
            {
                NatureResource res = new NatureResource();
                res.Parse(sect);
                res.Reset(100);
                resources.Add(res);            
            }
            naturalResource = resources.ToArray();

            for (int i = 0; i < cityList.Count; i++)
            {
                if ( cityList[i].Type == CityType.Oil || cityList[i].Type == CityType.Green)
                {
                    ((GatherCity)cityList[i]).FindResources(resources);
                }
            }   


        }

        public void CreateResourceBall(City city)
        {
            RBall ball = new RBall(city.Owner, city);
            resBalls.Add(ball);
        }


        public void Update(GameTime gameTime)
        {
            foreach ( City city in cityList)
            {
                city.Update(gameTime);

                if (!this.cityModelIdle[city].IsPlaying)
                {
                    this.cityModelIdle[city].Play();
                }
                else
                {
                    this.cityModelIdle[city].Update(gameTime);
                }

                if (!this.cityModelStopped[city].IsPlaying)
                {
                    this.cityModelStopped[city].Play();
                }
                else
                {
                    this.cityModelStopped[city].Update(gameTime);
                }

                if (!this.cityModelSend[city].IsPlaying)
                {
                    this.cityModelSend[city].Play();
                }
                else
                {
                    this.cityModelSend[city].Update(gameTime);
                }

                if (!this.cityModelReceive[city].IsPlaying)
                {
                    this.cityModelReceive[city].Play();
                }
                else
                {
                    this.cityModelReceive[city].Update(gameTime);
                }

            }
        }


    }
}
