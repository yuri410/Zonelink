using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    ///  所有有游戏逻辑状态
    /// </summary>
    class GameState
    {
        BattleField field;

        public void StartNewGame(Level level)
        {

        }
        
        public void Update(GameTime gameTime)
        {
            field.Update(gameTime);
        }
    }
}
