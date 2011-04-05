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

namespace Zonelink
{
    /// <summary>
    ///  表示当前正在进行的游戏场景中的状态
    /// </summary>
    class BattleField
    {
        const int MaxCities = 120;

        ResourceBallType[] resTypes = new ResourceBallType[4];

        Level level;
        Technology techMgr;
        Player localPlayer;

        //单例
        public static readonly BattleField Instance = new BattleField();

        //cityXML中读取的城市
        List<City> CityList = new List<City>(MaxCities);
                       
        public int VisibleCityCount
        {
            get { return CityList.Count; }
        } 

        public City GetVisibleCity(int i)
        {
            return CityList[i];
        }

        public void Initialize()
        {
            //Init Cities
            LoadCities();
        }

        //初始化城市
        private void LoadCities()
        {
            string path = Path.Combine(GameFileLocs.Configs, "cities.xml");
            GameConfiguration resCon = new GameConfiguration(path);
            GameConfiguration.ValueCollection resVals = resCon.Values;

            foreach (GameConfigurationSection sect in resVals)
            {
                City city = new City(null);
                city.Parse(sect);
                city.UpdateLocation();

                CityList.Add(city);           
            }
        }

        public void Update(GameTime gameTime)
        {

        }
    }
}
