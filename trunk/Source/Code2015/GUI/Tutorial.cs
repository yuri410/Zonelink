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

        Texture background;
        Texture[] help;

        Texture cursor;
        Point mousePosition;

        int currentPage;

        Button nextButton;
        Button exitButton;

        NormalSoundObject mouseHover;
        NormalSoundObject mouseDown;

        public Tutorial()
        {
            help = new Texture[TotalPages];
            for (int i = 0; i < 14; i++)
            {
                FileLocation fl = FileSystem.Instance.Locate((i + 1).ToString() + ".tex", GameFileLocs.Help);
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

            fl2 = FileSystem.Instance.Locate("tut_bg.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl2);

            fl2 = FileSystem.Instance.Locate("cursor.tex", GameFileLocs.GUI);
            cursor = UITextureManager.Instance.CreateInstance(fl2);
      
            
            mouseHover = (NormalSoundObject)SoundManager.Instance.MakeSoundObjcet("buttonHover", null, 0);
            mouseDown = (NormalSoundObject)SoundManager.Instance.MakeSoundObjcet("buttonDown", null, 0);
        }

        public void Advance()
        {
            currentPage++;
        }

        public bool IsFinished
        {
            get { return currentPage >= TotalPages; }
        }

       

        public override void Render(Sprite sprite)
        {
            sprite.Draw(background, 0, 0, ColorValue.White);

            int idx = currentPage;
            if (idx >= TotalPages)
                idx = TotalPages - 1;

            nextButton.Render(sprite);
            exitButton.Render(sprite);

            Rectangle rect = new Rectangle(62, 186, 916, 465);
            sprite.Draw(help[idx], rect, ColorValue.White);
            sprite.Draw(cursor, mousePosition.X, mousePosition.Y, ColorValue.White);
        }
        public override void Update(GameTime time)
        {
            mousePosition.X = MouseInput.X;
            mousePosition.Y = MouseInput.Y; 
            
            if (MouseInput.IsMouseUpLeft)
                currentPage++;
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
    }
}
