using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.GUI.Controls;
using Code2015.World;

namespace Code2015.GUI
{
    class CityDesigner : UIComponent
    {

        Texture leftPanel;

        Button btn1;
        Button btn2;
        Button btn3;
        Button btn4;


        CityObject currentCity;


        public CityObject CurrentCity
        {
            get { return currentCity; }
            set { currentCity = value; }
        }

        public CityDesigner(Game parent, GameScene scene)
        {

        }


        public override void Render(Sprite sprite)
        {
            base.Render(sprite);
        }
        public override void Update(GameTime time)
        {
            base.Update(time);
        }
    }
}
