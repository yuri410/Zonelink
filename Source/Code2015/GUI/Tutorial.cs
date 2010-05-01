using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Apoc3D;
using Apoc3D.MathLib;
using Apoc3D.GUI.Controls;

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


            nextButton = new Button();
            nextButton.Enabled = true;
            nextButton.IsValid = true;
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

            sprite.Draw(help[currentPage], 0, 0, ColorValue.White);
        }
        public override void Update(GameTime time)
        {

        }
    }
}
