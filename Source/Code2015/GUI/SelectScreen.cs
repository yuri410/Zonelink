using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.GUI.Controls;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;

namespace Code2015.GUI
{
    class SelectScreen : UIComponent
    {
        RenderSystem renderSys;

        Texture backGround;

        Button side1;
        Button side2;
        Button side3;
        Button side4;

        bool selected;
        ColorValue selectedColor;

        public bool Selected
        {
            get { return selected; }
        }

        public ColorValue SelectedColor
        {
            get { return selectedColor; }
        }

        public SelectScreen(Code2015 game)
        {
            this.renderSys = game.RenderSystem;

            FileLocation fl = FileSystem.Instance.Locate("selectBg.tex", GameFileLocs.GUI);
            backGround = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ssl_redBtn.tex", GameFileLocs.GUI);
            Texture btnNrm = UITextureManager.Instance.CreateInstance(fl);

            side1 = new Button();
            side1.Width = btnNrm.Width;
            side1.Height = btnNrm.Height;
            side1.X = 100;
            side1.Y = 100;
            side1.Image = btnNrm;

            side2 = new Button();
            side2.Width = btnNrm.Width;
            side2.Height = btnNrm.Height;
            side2.X = 300;
            side2.Y = 100;
            side2.Image = btnNrm;

            side3 = new Button();
            side3.Width = btnNrm.Width;
            side3.Height = btnNrm.Height;
            side3.X = 500;
            side3.Y = 100;
            side3.Image = btnNrm;

            side4 = new Button();
            side4.Width = btnNrm.Width;
            side4.Height = btnNrm.Height;
            side4.X = 700;
            side4.Y = 100;
            side4.Image = btnNrm;

            side1.MouseClick += Button_Click;
            side2.MouseClick += Button_Click;
            side3.MouseClick += Button_Click;
            side4.MouseClick += Button_Click;
        }

        void Button_Click(object sender, MouseButtonFlags btn)
        {
            if (!selected && btn == MouseButtonFlags.Left)
            {
                if (sender == side1)
                {
                    selectedColor = ColorValue.Red;
                    selected = true;
                }
                else if (sender == side2)
                {
                    selectedColor = ColorValue.Yellow;
                    selected = true;
                }
                else if (sender == side3)
                {
                    selectedColor = ColorValue.Green;
                    selected = true;
                }
                else if (sender == side4)
                {
                    selectedColor = ColorValue.Blue;
                    selected = true;
                }
            }
        }

        public override void Render(Sprite sprite)
        {
            sprite.Draw(backGround, 0, 0, ColorValue.White);

            side1.Render(sprite);
            side2.Render(sprite);
            side3.Render(sprite);
            side4.Render(sprite);
        }

        public override void Update(GameTime time)
        {
            side1.Update(time);
            side2.Update(time);
            side3.Update(time);
            side4.Update(time);
        }
    }
}
