using System;
using System.Collections.Generic;
using System.Text;
using Code2015.World;
using Apoc3D;
using Apoc3D.Graphics;
using Code2015.Logic;
using Code2015.EngineEx;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Apoc3D.GUI.Controls;


namespace Code2015.GUI.IngameUI
{
    class RBallTypeSelect : UIComponent
    {
        GameScene scene;
        GameState gameLogic;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;
        Player player;

        Texture background;

        Button greenBallBtn;
        Button healthBallBtn;
        Button educationBallBtn;


        struct BallRecord
        {
            public RBallType Type;
            public int count;
        }
        private static int BallRecordCompare(BallRecord a, BallRecord b)
        {
            return b.count.CompareTo(a.count);
        }

        BallRecord[] resBallsCount = new BallRecord[3];

        
        RBallType selectBallType;


        City sendCity;

        City selectCity;
        public City SelectedCity
        {
            get { return selectCity; }
            set
            {
                if (selectCity != value)
                {
                    selectCity = value;

                    resBallsCount[0].count = 0;
                    resBallsCount[1].count = 0;
                    resBallsCount[2].count = 0;
                    //if (selectCity != null)
                    //{
                    //    if (selectCity.Owner != player)
                    //    {
                    //        selectCity = null;
                    //    }
                    //}
                }
            }
        }

        bool isSending;

        public override int Order
        {
            get
            {
                return 99;
            }
        }


        public RBallTypeSelect(Code2015 game, Game parent, GameScene scene, GameState logic)
        {
            this.game = game;
            this.parent = parent;
            this.scene = scene;
            this.gameLogic = logic;
            this.player = parent.HumanPlayer;
            this.renderSys = game.RenderSystem;

            greenBallBtn = new Button();
            educationBallBtn = new Button();
            healthBallBtn = new Button();

            FileLocation fl = FileSystem.Instance.Locate("nig_icon_green.tex", GameFileLocs.GUI);
            greenBallBtn.Image = UITextureManager.Instance.CreateInstance(fl);
            greenBallBtn.X = 0;
            greenBallBtn.Y = 0;
            greenBallBtn.Width = greenBallBtn.Image.Width;
            greenBallBtn.Height = greenBallBtn.Image.Height;
            greenBallBtn.Enabled = true;
            greenBallBtn.IsValid = true;
            greenBallBtn.MouseClick += GreenBallButton_MouseClick;

            fl = FileSystem.Instance.Locate("nig_icon_edu.tex", GameFileLocs.GUI);
            educationBallBtn.Image = UITextureManager.Instance.CreateInstance(fl);
            educationBallBtn.X = 0;
            educationBallBtn.Y = 0;
            educationBallBtn.Width = educationBallBtn.Image.Width;
            educationBallBtn.Height = educationBallBtn.Image.Height;
            educationBallBtn.Enabled = true;
            educationBallBtn.IsValid = true;
            educationBallBtn.MouseClick += EducationBallButton_MouseClick;

            fl = FileSystem.Instance.Locate("nig_icon_hospital.tex", GameFileLocs.GUI);
            healthBallBtn.Image = UITextureManager.Instance.CreateInstance(fl);
            healthBallBtn.X = 0;
            healthBallBtn.Y = 0;
            healthBallBtn.Width = healthBallBtn.Image.Width;
            healthBallBtn.Height = healthBallBtn.Image.Height;
            healthBallBtn.Enabled = true;
            healthBallBtn.IsValid = true;
            healthBallBtn.MouseClick += HealthBallButton_MouseClick;


            fl = FileSystem.Instance.Locate("nig_send_ball.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl);


            resBallsCount[0].Type = RBallType.Green;
            resBallsCount[0].count = 0;

            resBallsCount[1].Type = RBallType.Education;
            resBallsCount[1].count = 0;

            resBallsCount[2].Type = RBallType.Health;
            resBallsCount[2].count = 0;
        }


        public override bool HitTest(int x, int y)
        {
            return base.HitTest(x, y);
        }


        public override void Render(Sprite sprite)
        {
            RenderSendBall(sprite);
        }



        public override void Update(Apoc3D.GameTime time)
        {
            greenBallBtn.Update(time);
            healthBallBtn.Update(time);
            educationBallBtn.Update(time);

            StatisticRBall();

        }

        public override void UpdateInteract(Apoc3D.GameTime time)
        {
            greenBallBtn.UpdateInteract(time);
            healthBallBtn.UpdateInteract(time);
            educationBallBtn.UpdateInteract(time);
        }

        private void StatisticRBall()
        {
            if (selectCity != null)
            {
                resBallsCount[0].count = 0;
                resBallsCount[1].count = 0;
                resBallsCount[2].count = 0;

                for (int i = 0; i < selectCity.NearbyOwnedBallCount; i++)
                {
                    if (selectCity.GetNearbyOwnedBall(i).Type == RBallType.Green)
                        resBallsCount[0].count++;


                    if (selectCity.GetNearbyOwnedBall(i).Type == RBallType.Education)
                        resBallsCount[1].count++;

                    if (selectCity.GetNearbyOwnedBall(i).Type == RBallType.Health)
                        resBallsCount[2].count++;
                }
            }


        }



        void GreenBallButton_MouseClick(object sender, MouseButtonFlags btn)
        {
            selectBallType = RBallType.Green;
        }

        void HealthBallButton_MouseClick(object sender, MouseButtonFlags btn)
        {
            selectBallType = RBallType.Health;
        }

         void EducationBallButton_MouseClick(object sender, MouseButtonFlags btn)
        {
            selectBallType = RBallType.Education;
        }


         private void RenderSendBall(Sprite sprite)
         {
             if (SelectedCity != null)
             {
                 float radLng = MathEx.Degree2Radian(SelectedCity.Longitude);
                 float radLat = MathEx.Degree2Radian(SelectedCity.Latitude);

                 Vector3 tangy = PlanetEarth.GetTangentY(radLng, radLat);
                 Vector3 tangx = PlanetEarth.GetTangentX(radLng, radLat);

                 Vector3 cityNormal = PlanetEarth.GetNormal(radLng, radLat);
                 cityNormal.Normalize();

                 Vector3 hpPos = SelectedCity.Position  + cityNormal * 100;

                 Viewport vp = renderSys.Viewport;
                 Vector3 screenPos = vp.Project(hpPos, scene.Camera.ProjectionMatrix,
                                                scene.Camera.ViewMatrix, Matrix.Identity);

                 Vector3 lp = vp.Project(hpPos + tangx, scene.Camera.ProjectionMatrix,
                                                        scene.Camera.ViewMatrix, Matrix.Identity);

                 Vector3 rp = vp.Project(hpPos - tangx, scene.Camera.ProjectionMatrix,
                                                        scene.Camera.ViewMatrix, Matrix.Identity);


                 float scale = 2.5f * Vector3.Distance(lp, rp) / 2;

                 Matrix trans = Matrix.Translation(-background.Width / 2, -background.Height / 2, 0) *
                            Matrix.Scaling(scale, scale, 1) * Matrix.Translation(screenPos.X, screenPos.Y, 0);

                 sprite.SetTransform(trans);
                 sprite.Draw(background, 0, 0, ColorValue.White);

                 RenderBallIcon(sprite);
                 

                 sprite.SetTransform(Matrix.Identity);

             }
         }

         private void RenderBallIcon(Sprite sprite)
         {
             if (selectCity != null)
             {
                 //Array.Sort(resBallsCount, BallRecordCompare);

                 int left = 0;
                 for (int i = 0; i < resBallsCount.Length; i++)
                 {
                     switch (resBallsCount[i].Type)
                     {
                         case RBallType.Green:
                             {
                                 if (resBallsCount[i].count != 0)
                                 {
                                     int x = 42 - greenBallBtn.Image.Width / 2;
                                     int y = 35 - left * greenBallBtn.Image.Height / 2;
                                     sprite.Draw(greenBallBtn.Image, x, y, ColorValue.White);
                                     left++;
                                 }
                                 else
                                 {
                                     break;
                                 }

                             }
                             break;
                         case RBallType.Education:
                             {
                                 if (resBallsCount[i].count != 0)
                                 {
                                     int x = 42 - educationBallBtn.Image.Width / 2;
                                     int y = 45 - left * educationBallBtn.Image.Height / 2;
                                     sprite.Draw(educationBallBtn.Image, x, y, ColorValue.White);
                                     left++;
                                 }
                                 else
                                 {
                                     break;
                                 }
                             }
                             break;
                         case RBallType.Health:
                             {
                                 if (resBallsCount[i].count != 0)
                                 {
                                     int x = 42 - healthBallBtn.Image.Width / 2;
                                     int y = 45 - left * healthBallBtn.Image.Height / 2;
                                     sprite.Draw(healthBallBtn.Image, x, y, ColorValue.White);
                                     left++;
                                 }
                                 else
                                 {
                                     break;
                                 }
                             }
                             break;
                     }
                 }
             }
         }
    
    }



}
