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
    public class Tutorial : UIComponent
    {
        const int TotalPages = 14;
        Texture[] help;



        int currentPage;

        Button nextButton;
        Button exitButton;

        public Tutorial()
        {
            help = new Texture[TotalPages];
            for (int i = 0; i < 14; i++)
            {
                FileLocation fl = FileSystem.Instance.Locate(i.ToString() + ".tex", GameFileLocs.GUI);
                help[i] = UITextureManager.Instance.CreateInstance(fl);
            }

            FileLocation fl2 = FileSystem.Instance.Locate("tut_ig_cont.tex", GameFileLocs.GUI);
            nextButton = new Button();
            nextButton.Enabled = true;
            nextButton.IsValid = true;
            nextButton.Image = UITextureManager.Instance.CreateInstance(fl2);
            fl2 = FileSystem.Instance.Locate("tut_ig_cont_hover.tex", GameFileLocs.GUI);
            nextButton.ImageMouseOver = UITextureManager.Instance.CreateInstance(fl2);

            exitButton = new Button();
            exitButton.Enabled = true;
            exitButton.IsValid = true;
            fl2 = FileSystem.Instance.Locate("tut_ig_quit.tex", GameFileLocs.GUI);
            exitButton.Image = UITextureManager.Instance.CreateInstance(fl2);
            fl2 = FileSystem.Instance.Locate("tut_ig_quit_hover.tex", GameFileLocs.GUI);
            exitButton.ImageMouseOver = UITextureManager.Instance.CreateInstance(fl2);

        }

        public void Advance()
        {
            currentPage++;
        }

        public bool IsFinished
        {
            get { return currentPage >= TotalPages; }
        }

        public override int Order
        {
            get
            {
                return 100;
            }
        }

        public override void UpdateInteract(GameTime time)
        {
            switch (currentPage) 
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                    break;
            }
        }
        public override void Render(Sprite sprite)
        {
            int idx = currentPage;
            if (idx >= TotalPages)
                idx = TotalPages - 1;

            nextButton.Render(sprite);
            exitButton.Render(sprite);

            sprite.Draw(help[currentPage], 0, 0, ColorValue.White);
        }
        public override void Update(GameTime time)
        {

        }
    }
}
