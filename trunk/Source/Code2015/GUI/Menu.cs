using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.GUI.Controls;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.Logic;

namespace Code2015.GUI
{

    /// <summary>
    ///  表示游戏菜单
    /// </summary>
    class Menu : UIComponent, IGameComponent
    {
        Code2015 game;
        MainMenu mainMenu;
        SelectScreen sideSelect;


        public UIComponent CurrentScreen
        {
            get;
            set;
        }

        public SelectScreen GetSelectScreen()
        {
            return sideSelect;
        }

        public Menu(Code2015 game, RenderSystem rs)
        {
            this.game = game;
            this.mainMenu = new MainMenu(game, this, rs);
            this.sideSelect = new SelectScreen(game);

            this.CurrentScreen = mainMenu;
        }


        public void Render()
        {

        }

        public override void Render(Sprite sprite)
        {
            if (!game.IsIngame)
            {
                if (CurrentScreen != null)
                {
                    CurrentScreen.Render(sprite);
                }
            }
        }
        public override void Update(GameTime time)
        {
            if (!game.IsIngame)
            {
                if (CurrentScreen != null)
                {
                    CurrentScreen.Update(time);
                }
            }
        }
    }

}
