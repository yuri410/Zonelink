using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.GUI.Controls;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
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
            FileLocation fl = FileSystem.Instance.Locate("cd_panel.tex", GameFileLocs.GUI);
            leftPanel = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("cd_item.tex", GameFileLocs.GUI);
            Texture btnNrm = UITextureManager.Instance.CreateInstance(fl);

            btn1 = new Button();
            btn1.Image = btnNrm;
            btn1.Width = 300;
            btn1.Height = 200;

            btn2 = new Button();
            btn2.Image = btnNrm;
            btn2.Width = 300;
            btn2.Height = 200;


            btn3 = new Button();
            btn3.Image = btnNrm;
            btn3.Width = 300;
            btn3.Height = 200;


            btn4 = new Button();
            btn4.Image = btnNrm;
            btn4.Width = 300;
            btn4.Height = 200;
        }


        public override void Render(Sprite sprite)
        {
            sprite.Draw(leftPanel, 0, 0, ColorValue.White);

            btn1.Render(sprite);
            btn2.Render(sprite);
            btn3.Render(sprite);
            btn4.Render(sprite);
        }
        public override void Update(GameTime time)
        {
            btn1.Update(time);
            btn2.Update(time);
            btn3.Update(time);
            btn4.Update(time);
        }
    }
}
