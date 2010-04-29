using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.GUI.Controls;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.AI;
using Code2015.EngineEx;
using Code2015.Logic;
using Code2015.World;
using Code2015.BalanceSystem;

namespace Code2015.GUI
{
    class SelectScreen : UIComponent
    {
        static readonly ColorValue[] Colors = new ColorValue[] { ColorValue.Red, ColorValue.Yellow, ColorValue.Green, ColorValue.Blue };

        Code2015 game;
        Menu parent;
        RenderSystem renderSys;

        Texture backGround;
        Texture cityRound;

        Button leftArrow;
        Button rightArrow;
        Button btnContinue;

        Texture redcont;
        Texture greencont;
        Texture bluecont;
        Texture yellowcont;
        Texture redconth;
        Texture greenconth;
        Texture blueconth;
        Texture yellowconth;


        //bool selected;
        //bool[] selectedTable = new bool[Colors.Length];
        //ColorValue selectedColor;

        Texture cursor;
        Point mousePosition;

        NormalSoundObject mouseHover;
        NormalSoundObject mouseDown;

        Quaternion prevRot;
        Quaternion rotation;
        Quaternion nextRot;
        float rotprg;

        int selIndex;


        //public bool Selected
        //{
        //    get { return selected; }
        //}

        //public ColorValue SelectedColor
        //{
        //    get { return selectedColor; }
        //}

        public SelectScreen(Code2015 game, Menu parent)
        {
            this.renderSys = game.RenderSystem;
            this.game = game;
            this.parent = parent;

            FileLocation fl = FileSystem.Instance.Locate("ssl_bg.tex", GameFileLocs.GUI);
            backGround = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ssl_all.tex", GameFileLocs.GUI);
            cityRound = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ssl_red2.tex", GameFileLocs.GUI);
            redcont = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ssl_blue2.tex", GameFileLocs.GUI);
            bluecont = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ssl_green2.tex", GameFileLocs.GUI);
            greencont = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ssl_yellow2.tex", GameFileLocs.GUI);
            yellowcont = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("ssl_red1.tex", GameFileLocs.GUI);
            redconth = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ssl_blue1.tex", GameFileLocs.GUI);
            blueconth = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ssl_green1.tex", GameFileLocs.GUI);
            greenconth = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("ssl_yellow1.tex", GameFileLocs.GUI);
            yellowconth = UITextureManager.Instance.CreateInstance(fl);

            btnContinue = new Button();
            btnContinue.X = 471;// 430;
            btnContinue.Y = 52;// 38;
            btnContinue.Width = 245;// 329;
            btnContinue.Height = 51;// 90;
            btnContinue.MouseEnter += Button_MouseIn;
            btnContinue.MouseDown += Button_DownSound;
            btnContinue.MouseClick += Button_Click;
            btnContinue.Enabled = true;
            btnContinue.IsValid = true;

            leftArrow = new Button();
            leftArrow.X = 374;
            leftArrow.Y = 23;
            fl = FileSystem.Instance.Locate("ssl_leftArrow.tex", GameFileLocs.GUI);
            leftArrow.Image = UITextureManager.Instance.CreateInstance(fl);
            leftArrow.ImageMouseDown = leftArrow.Image;
            leftArrow.ImageMouseOver = leftArrow.Image;

            leftArrow.MouseEnter += Button_MouseIn;
            leftArrow.MouseDown += Button_DownSound;
            leftArrow.Width = 120;
            leftArrow.Height = 102;
            leftArrow.MouseEnter += Button_MouseIn;
            leftArrow.MouseDown += Button_DownSound;
            leftArrow.MouseClick += LeftArrow_Click;
            leftArrow.Enabled = true;
            leftArrow.IsValid = true;

            rightArrow = new Button();
            rightArrow.X = 692;
            rightArrow.Y = 23;
            fl = FileSystem.Instance.Locate("ssl_rightArrow.tex", GameFileLocs.GUI);
            rightArrow.Image = UITextureManager.Instance.CreateInstance(fl);

            rightArrow.ImageMouseDown = rightArrow.Image;
            rightArrow.ImageMouseOver = rightArrow.Image;

            rightArrow.MouseEnter += Button_MouseIn;
            rightArrow.MouseDown += Button_DownSound;
            rightArrow.Width = 120;
            rightArrow.Height = 102;
            rightArrow.MouseEnter += Button_MouseIn;
            rightArrow.MouseDown += Button_DownSound;
            rightArrow.MouseClick += RightArrow_Click;
            rightArrow.Enabled = true;
            rightArrow.IsValid = true;

            mouseHover = (NormalSoundObject)SoundManager.Instance.MakeSoundObjcet("buttonHover", null, 0);
            mouseDown = (NormalSoundObject)SoundManager.Instance.MakeSoundObjcet("buttonDown", null, 0);

            fl = FileSystem.Instance.Locate("cursor.tex", GameFileLocs.GUI);
            cursor = UITextureManager.Instance.CreateInstance(fl);
            rotation = Quaternion.Identity;
            nextRot = Quaternion.Identity;
            prevRot = Quaternion.Identity;

            selIndex = Randomizer.GetRandomInt(4);
            UpdateRot();
        }


        void Button_MouseIn(object sender, MouseButtonFlags btn)
        {
            mouseHover.Fire();
        }
        void Button_DownSound(object sender, MouseButtonFlags btn)
        {
            if (btn == MouseButtonFlags.Left)
            {
                mouseDown.Fire();
            }
        }
        GameGoal CreateGoal()
        {
            GameGoal goal = new GameGoal(CityGrade.LargeCityPointThreshold * 10);

            return goal;
        }

        void UpdateRot() 
        {
            prevRot = rotation;
            nextRot = Quaternion.RotationAxis(Vector3.UnitZ, selIndex * MathEx.PiOver2);
            rotprg = 0;
        }
        void RightArrow_Click(object sender, MouseButtonFlags btn)
        {
            if (btn == MouseButtonFlags.Left)
            {
                selIndex++;
                if (selIndex > 3)
                {
                    selIndex = 0;
                }
                UpdateRot();
            }
        }
        void LeftArrow_Click(object sender, MouseButtonFlags btn) 
        {
            if (btn == MouseButtonFlags.Left)
            {
                selIndex--;
                if (selIndex < 0)
                {
                    selIndex = 3;
                }
                UpdateRot();
            }
        }

        ColorValue GetColor(int selIndex) { ColorValue selectedColor = ColorValue.Red;
            switch (selIndex)
            {
                case 1:
                    selectedColor = ColorValue.Red;
                    break;
                case 3:
                    selectedColor = ColorValue.Yellow;
                    break;
                case 2:
                    selectedColor = ColorValue.Green;

                    break;
                case 0:
                    selectedColor = ColorValue.Blue;

                    break;
            
            }return selectedColor;
        }
        void Button_Click(object sender, MouseButtonFlags btn)
        {
           
            
            //if (!selected && btn == MouseButtonFlags.Left)
            //{
            //    if (sender == side1)
            //    {
            //        selectedColor = Colors[0];
            //        selectedTable[0] = true;
            //        selected = true;
            //    }
            //    else if (sender == side2)
            //    {
            //        selectedColor = Colors[1];
            //        selectedTable[1] = true;
            //        selected = true;
            //    }
            //    else if (sender == side3)
            //    {
            //        selectedColor = Colors[2];
            //        selectedTable[2] = true;
            //        selected = true;
            //    }
            //    else if (sender == side4)
            //    {
            //        selectedColor = Colors[3];
            //        selectedTable[3] = true;
            //        selected = true;
            //    }
            //}

            if (btn == MouseButtonFlags.Left)
            {
                GameCreationParameters gcp = new GameCreationParameters();

                gcp.Player1 = new Player("Player", CreateGoal(), 0);
                gcp.Player1.SideColor = GetColor(selIndex);
                gcp.Player2 = new AIPlayer(CreateGoal(), 1);
                gcp.Player3 = new AIPlayer(CreateGoal(), 2);
                gcp.Player4 = new AIPlayer(CreateGoal(), 3);


                int i = Randomizer.GetRandomInt(3);


                while (i == selIndex)
                {
                    i++;
                    i %= Colors.Length;
                }
                gcp.Player2.SideColor = GetColor(i);
                int sel1 = i;

                i = Randomizer.GetRandomInt(2);
                while (i == selIndex || i == sel1)
                {
                    i++;
                    i %= Colors.Length;
                }
                gcp.Player3.SideColor = GetColor(i);
                int sel2 = i;

                while (i == selIndex || i == sel1 || i == sel2)
                {
                    i++;
                    i %= Colors.Length;
                }
                gcp.Player4.SideColor = GetColor(i);


                parent.CurrentScreen = parent.GetMainMenu();
                game.StartNewGame(gcp);
            }
        }

        public override void Render(Sprite sprite)
        {
            sprite.Draw(backGround, 0, 0, ColorValue.White);
            leftArrow.Render(sprite);
            rightArrow.Render(sprite);
            //btnContinue.Render(sprite);

            if (btnContinue.IsMouseOver)
            {
                switch (selIndex)
                {
                    case 1:
                        sprite.Draw(redconth, 430, 38, ColorValue.White);
                        break;
                    case 3:
                        sprite.Draw(yellowconth, 430, 38, ColorValue.White);
                        break;
                    case 2:
                        sprite.Draw(greenconth, 430, 38, ColorValue.White);
                        break;
                    case 0:
                        sprite.Draw(blueconth, 430, 38, ColorValue.White);

                        break;
                }
            }
            else
            {
                switch (selIndex)
                {
                    case 1:
                        sprite.Draw(redcont, 430, 38, ColorValue.White);
                        break;
                    case 3:
                        sprite.Draw(yellowcont, 430, 38, ColorValue.White);
                        break;
                    case 2:
                        sprite.Draw(greencont, 430, 38, ColorValue.White);
                        break;
                    case 0:
                        sprite.Draw(bluecont, 430, 38, ColorValue.White);

                        break;
                }
            }

            sprite.SetTransform(Matrix.Translation(-cityRound.Width / 2, -cityRound.Height / 2, 0) *
                Matrix.RotationQuaternion(rotation) * Matrix.Translation(Program.ScreenWidth * 0.5f, 127 + cityRound.Height / 2, 0));
            sprite.Draw(cityRound, 0, 0, ColorValue.White);

            sprite.SetTransform(Matrix.Identity);

            GameFontManager.Instance.F18.DrawString(sprite, "CONTINUE", 530, 80, ColorValue.White);

            sprite.Draw(cursor, mousePosition.X, mousePosition.Y, ColorValue.White);
              
        }

        public override void Update(GameTime time)
        {
            leftArrow.Update(time);
            rightArrow.Update(time);
            btnContinue.Update(time);

            mousePosition.X = MouseInput.X;
            mousePosition.Y = MouseInput.Y;


            rotprg += time.ElapsedGameTimeSeconds * 1.5f;

            if (rotprg > 1)
            {
                rotprg = 1;
                rotation = nextRot;
            }
            else
            {
                rotation = Quaternion.Lerp(prevRot, nextRot, rotprg);
            }
        }
    }
}
