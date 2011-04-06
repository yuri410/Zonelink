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


        Map map;
        //Technology techMgr;
        Player localPlayer;

        Game1 game;


        //cityXML中读取的城市
        List<City> cityList = new List<City>(MaxCities);

        public Map Map { get { return map; } }
        public List<City> Cities
        {
            get { return cityList; }
        }
                       
        public int VisibleCityCount
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

        //初始化城市
        private void LoadCities()
        {            
            GameConfiguration resCon = Utils.LoadConfig("cities.xml");
            GameConfiguration.ValueCollection resVals = resCon.Values;
            foreach (GameConfigurationSection sect in resVals)
            {
                City city = new City(this, null);
                city.Parse(sect);                
                cityList.Add(city);           
            }
        }

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

        public void Update(GameTime gameTime)
        {
            foreach ( City city in cityList)
            {
                city.fsmMachine.Update(gameTime);
            }
        }


    }
}
