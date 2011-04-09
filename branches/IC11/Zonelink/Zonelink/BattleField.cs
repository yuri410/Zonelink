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
        public const int MaxCities = 120;

        ResourceBallType[] resTypes = new ResourceBallType[4];

        NatureResource[] naturalResource;

        List<RBall> resBalls = new List<RBall>();

        Map map;
        //Technology techMgr;
        Player localPlayer;
        Player[] aiPlayers;

        Game1 game;

        List<City> cityList = new List<City>(MaxCities);

        public Map Map { get { return map; } }
        public List<City> Cities
        {
            get { return cityList; }
        }
        public NatureResource[] NaturalResources 
        {
            get { return naturalResource; }
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
            Dictionary<string, City> resolveTable = new Dictionary<string, City>(MaxCities);
            foreach (GameConfigurationSection sect in resVals)
            {
                City city;
                type = sect.GetString("Type", string.Empty).ToLowerInvariant();
                if (type == "green" || type == "oil")
                {
                    city = new GatherCity(this, null);
                }
                else
                {
                    city = new City(this, null);
                }
                city.Parse(sect);

                resolveTable.Add(sect.Name, city);
                cityList.Add(city);
            }

            for (int i = 0; i < cityList.Count; i++) 
            {
                cityList[i].ResolveCities(resolveTable);
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

        public void CreateResourceBall(Player owner, City city, RBallType type)
        {
            RBall ball = new RBall(owner, city, type);
            resBalls.Add(ball);
        }


        public void Update(GameTime gameTime)
        {
            foreach ( City city in cityList)
            {
                city.Update(gameTime);


            }

            for (int i = 0; i < naturalResource.Length; i++) 
            {
                naturalResource[i].Update(gameTime);
            }
        }


    }
}
