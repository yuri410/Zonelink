﻿using System;
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

namespace Zonelink
{
    /// <summary>
    ///  表示当前正在进行的游戏场景中的状态
    /// </summary>
    class BattleField
    {
        Level level;
        Technology techMgr;
        Player localPlayer;

        //单例
        public static readonly BattleField Instance = new BattleField();  

        List<City>  visibleCityList = new List<City>();
        

        public int VisibleCityCount
        {
            get { return visibleCityList.Count; }
        } 

        public City GetVisibleCity(int i)
        {
            return visibleCityList[i];
        } 
 
        public void Update(GameTime gameTime)
        {

        }
    }
}
