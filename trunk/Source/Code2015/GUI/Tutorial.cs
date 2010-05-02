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
    class Tutorial : UIComponent
    {
        const int TotalPages = 14;
        Menu parent;
        Texture background;
        Texture[] help;
        Texture[] helpText;

        Texture contquit;
        Texture contHover1;
        Texture contHover2;
        Texture helpBtnDown;

        Texture cursor;
        Point mousePosition;

        int currentPage;

        Button nextButton;
        Button exitButton;

        NormalSoundObject mouseHover;
        NormalSoundObject mouseDown;

        public Tutorial(Menu parent)
        {
            this.parent = parent;
            help = new Texture[TotalPages];
            helpText = new Texture[TotalPages];

            for (int i = 0; i < 14; i++)
            {
                FileLocation fl = FileSystem.Instance.Locate("sh" + (i + 1).ToString() + ".tex", GameFileLocs.Help);
                help[i] = UITextureManager.Instance.CreateInstance(fl);
                fl = FileSystem.Instance.Locate("sh" + (i + 1).ToString() + "t.tex", GameFileLocs.Help);
                helpText[i] = UITextureManager.Instance.CreateInstance(fl);
            }


            nextButton = new Button();
            nextButton.Enabled = true;
            nextButton.IsValid = true;
            nextButton.X = 1071;
            nextButton.Y = 354;
            nextButton.Width = 90;
            nextButton.Height = 204;
            nextButton.MouseClick += Continue_Click;

            exitButton = new Button();
            exitButton.Enabled = true;
            exitButton.IsValid = true;
            exitButton.X = 1071;
            exitButton.Y = 569;
            exitButton.Width = 90;
            exitButton.Height = 112;
            exitButton.MouseClick += Exit_Click;

            FileLocation fl2 = FileSystem.Instance.Locate("tut_bg.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl2);

            fl2 = FileSystem.Instance.Locate("cursor.tex", GameFileLocs.GUI);
            cursor = UITextureManager.Instance.CreateInstance(fl2);

            fl2 = FileSystem.Instance.Locate("tut_helpcont_hover.tex", GameFileLocs.GUI);
            contHover1 = UITextureManager.Instance.CreateInstance(fl2);
            fl2 = FileSystem.Instance.Locate("tut_helpcont_hover2.tex", GameFileLocs.GUI);
            contHover2 = UITextureManager.Instance.CreateInstance(fl2);
            fl2 = FileSystem.Instance.Locate("tut_helpcont.tex", GameFileLocs.GUI);
            contquit = UITextureManager.Instance.CreateInstance(fl2);

            fl2 = FileSystem.Instance.Locate("mm_btn_help_down.tex", GameFileLocs.GUI);
            helpBtnDown = UITextureManager.Instance.CreateInstance(fl2);

            mouseHover = (NormalSoundObject)SoundManager.Instance.MakeSoundObjcet("buttonHover", null, 0);
            mouseDown = (NormalSoundObject)SoundManager.Instance.MakeSoundObjcet("buttonDown", null, 0);
        }

        public bool Advance()
        {
            currentPage++;
            return currentPage < TotalPages;
        }

        public bool IsFinished
        {
            get { return currentPage >= TotalPages; }
        }

        void Continue_Click(object sender,MouseButtonFlags btn)
        {
            if (btn == MouseButtonFlags.Left) 
            {
                if (!Advance())
                {
                    parent.CurrentScreen = null;
                }
            }
        }
        void Exit_Click(object sender, MouseButtonFlags btn)
        {
            if (btn == MouseButtonFlags.Left)
            {
                parent.CurrentScreen = null;
            }
        }

        public override void Render(Sprite sprite)
        {
            sprite.Draw(background, 0, 0, ColorValue.White);

            int idx = currentPage;
            if (idx >= TotalPages)
                idx = TotalPages - 1;

            if (nextButton.IsMouseOver && !nextButton.IsPressed)
            {
                sprite.Draw(contHover1, 1051, 345, ColorValue.White);
            }
            else if (exitButton.IsMouseOver && !exitButton.IsPressed)
            {
                sprite.Draw(contHover2, 1051, 345, ColorValue.White);
            }
            else
            {
                sprite.Draw(contquit, 1051, 345, ColorValue.White);
            }

            Rectangle rect = new Rectangle(62, 186, 916, 465);
            sprite.Draw(help[idx], rect, ColorValue.White);
            sprite.Draw(helpText[idx], 84, 15, ColorValue.White);


            sprite.Draw(helpBtnDown, 1107 - 192 / 2, 241 - 195 / 2, ColorValue.White);
            sprite.Draw(cursor, mousePosition.X, mousePosition.Y, ColorValue.White);
        }
        public override void Update(GameTime time)
        {
            mousePosition.X = MouseInput.X;
            mousePosition.Y = MouseInput.Y;

            nextButton.Update(time);
            exitButton.Update(time);
        }
    }
}
