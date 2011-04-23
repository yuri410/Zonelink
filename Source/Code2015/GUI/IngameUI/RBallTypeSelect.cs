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
        enum State
        {
            Close,
            Opened,
            Opening,
            Closing
        }

        GameScene scene;
        GameState gameLogic;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;
        Player player;

        Texture background;

        Texture greenBallBtn;
        Texture healthBallBtn;
        Texture educationBallBtn;

        State state;

        float animProgress;


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

        
        RBallType selectedBallType;

        City targetCity;
        City sourceCity;

        bool isCancelled;

        public bool IsCancelled
        {
            get { return isCancelled; }
        }
        public City SourceCity
        {
            get { return sourceCity; }
        }
        public City TargetCity
        {
            get { return targetCity; }
        }
        public RBallType SelectedBallType
        {
            get { return selectedBallType; }
        }
        public override int Order
        {
            get
            {
                return 80;
            }
        }


        void ChangeState(State ns)
        {
            state = ns;
        }

        public RBallTypeSelect(Code2015 game, Game parent, GameScene scene, GameState logic)
        {
            this.game = game;
            this.parent = parent;
            this.scene = scene;
            this.gameLogic = logic;
            this.player = parent.HumanPlayer;
            this.renderSys = game.RenderSystem;

            //greenBallBtn = new Button();
            //educationBallBtn = new Button();
            //healthBallBtn = new Button();

            FileLocation fl = FileSystem.Instance.Locate("nig_icon_green.tex", GameFileLocs.GUI);
            greenBallBtn = UITextureManager.Instance.CreateInstance(fl);
            //greenBallBtn.Image = UITextureManager.Instance.CreateInstance(fl);
            //greenBallBtn.X = 0;
            //greenBallBtn.Y = 0;
            //greenBallBtn.Width = greenBallBtn.Image.Width;
            //greenBallBtn.Height = greenBallBtn.Image.Height;
            //greenBallBtn.Enabled = true;
            //greenBallBtn.IsValid = true;
            //greenBallBtn.MouseClick += GreenBallButton_MouseClick;

            fl = FileSystem.Instance.Locate("nig_icon_edu.tex", GameFileLocs.GUI);
            educationBallBtn = UITextureManager.Instance.CreateInstance(fl);
            //educationBallBtn.Image = UITextureManager.Instance.CreateInstance(fl);
            //educationBallBtn.X = 0;
            //educationBallBtn.Y = 0;
            //educationBallBtn.Width = educationBallBtn.Image.Width;
            //educationBallBtn.Height = educationBallBtn.Image.Height;
            ////educationBallBtn.Enabled = true;
            //educationBallBtn.IsValid = true;
            //educationBallBtn.MouseClick += EducationBallButton_MouseClick;

            fl = FileSystem.Instance.Locate("nig_icon_hospital.tex", GameFileLocs.GUI);
            healthBallBtn = UITextureManager.Instance.CreateInstance(fl);
            //healthBallBtn.Image = UITextureManager.Instance.CreateInstance(fl);
            //healthBallBtn.X = 0;
            //healthBallBtn.Y = 0;
            //healthBallBtn.Width = healthBallBtn.Image.Width;
            //healthBallBtn.Height = healthBallBtn.Image.Height;
            //healthBallBtn.Enabled = true;
            //healthBallBtn.IsValid = true;
            //healthBallBtn.MouseClick += HealthBallButton_MouseClick;


            fl = FileSystem.Instance.Locate("nig_send_ball.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl);


            resBallsCount[0].Type = RBallType.Green;
            resBallsCount[0].count = 0;

            resBallsCount[1].Type = RBallType.Education;
            resBallsCount[1].count = 0;

            resBallsCount[2].Type = RBallType.Health;
            resBallsCount[2].count = 0;
        }


        public void Open(City souceCity, City targetCity)
        {
            isCancelled = true;
            this.sourceCity = souceCity;
            this.targetCity = targetCity;

            resBallsCount[0].count = 0;
            resBallsCount[1].count = 0;
            resBallsCount[2].count = 0;

            ChangeState(State.Opening);
        }
        public void Close()
        {
            ChangeState(State.Closing);
        }
        public bool IsOpened
        {
            get { return state == State.Opened || state == State.Opening; }
        }

        public override bool HitTest(int x, int y)
        {
            if (state == State.Opened)
            {
                return true;
            }
            return false;
        }


        public override void Render(Sprite sprite)
        {

            RenderSendBall(sprite);


        }



        public override void Update(Apoc3D.GameTime time)
        {
            //greenBallBtn.Update(time);
            //healthBallBtn.Update(time);
            //educationBallBtn.Update(time);
            if (state == State.Opened)
            {
                StatisticRBall();
            }
            if (state == State.Opening)
            {
                animProgress += time.ElapsedGameTimeSeconds * 4;
                if (animProgress > 1)
                {
                    animProgress = 1;
                    ChangeState(State.Opened);
                }
            }
            else if (state == State.Closing)
            {
                animProgress -= time.ElapsedGameTimeSeconds * 4;
                if (animProgress < 0)                
                {
                    animProgress = 0;
                    ChangeState(State.Close);                        
                }
            }            
        }

        public override void UpdateInteract(Apoc3D.GameTime time)
        {
            if (state == State.Opened)
            {
                if (MouseInput.IsMouseUpLeft)
                {
                    Close();
                }
            }
            
        }

        private void StatisticRBall()
        {
            if (targetCity != null)
            {
                resBallsCount[0].count = 0;
                resBallsCount[1].count = 0;
                resBallsCount[2].count = 0;

                for (int i = 0; i < sourceCity.NearbyOwnedBallCount; i++)
                {
                    if (sourceCity.GetNearbyOwnedBall(i).Type == RBallType.Green)
                        resBallsCount[0].count++;


                    if (sourceCity.GetNearbyOwnedBall(i).Type == RBallType.Education)
                        resBallsCount[1].count++;

                    if (sourceCity.GetNearbyOwnedBall(i).Type == RBallType.Health)
                        resBallsCount[2].count++;
                }
            }


        }



        //void GreenBallButton_MouseClick(object sender, MouseButtonFlags btn)
        //{
        //    selectedBallType = RBallType.Green;
        //}

        //void HealthBallButton_MouseClick(object sender, MouseButtonFlags btn)
        //{
        //    selectedBallType = RBallType.Health;
        //}

        //void EducationBallButton_MouseClick(object sender, MouseButtonFlags btn)
        //{
        //    selectedBallType = RBallType.Education;
        //}


         private void RenderSendBall(Sprite sprite)
         {
             if (targetCity != null)
             {
                 float radLng = MathEx.Degree2Radian(targetCity.Longitude);
                 float radLat = MathEx.Degree2Radian(targetCity.Latitude);
              
                 Vector3 tangy = PlanetEarth.GetTangentY(radLng, radLat);
                 Vector3 tangx = PlanetEarth.GetTangentX(radLng, radLat);


                 Vector3 cityNormal = PlanetEarth.GetNormal(radLng, radLat);
                 cityNormal.Normalize();

                 Vector3 hpPos = targetCity.Position + cityNormal * 100;

                 Viewport vp = renderSys.Viewport;
                 Vector3 screenPos = vp.Project(hpPos, scene.Camera.ProjectionMatrix,
                                                scene.Camera.ViewMatrix, Matrix.Identity);

                 Vector3 lp = vp.Project(hpPos + tangx, scene.Camera.ProjectionMatrix,
                                                        scene.Camera.ViewMatrix, Matrix.Identity);

                 Vector3 rp = vp.Project(hpPos - tangx, scene.Camera.ProjectionMatrix,
                                                        scene.Camera.ViewMatrix, Matrix.Identity);


                 float scale0 = MathEx.Saturate(1.5f * Vector3.Distance(lp, rp));
                 float scale = (animProgress * animProgress) * scale0;

                 Matrix trans = Matrix.Translation(0, -background.Height / 2, 0) *
                            Matrix.Scaling(scale, scale, 1) * Matrix.Translation(screenPos.X, screenPos.Y, 0);
                 
                 sprite.SetTransform(trans);
                 
                 sprite.Draw(background, 0, 0, ColorValue.White);


                 if (state == State.Opened)
                 {
                     Matrix trans2 = Matrix.Translation(-background.Width / 2, -background.Height / 2, 0) * 
                         Matrix.Scaling(scale0, scale0, 1) * Matrix.Translation(screenPos.X, screenPos.Y, 0);

                     sprite.SetTransform(trans2);
                
                     RenderBallIcon(sprite);
                 }    

                 sprite.SetTransform(Matrix.Identity);

             }
         }

         private void RenderBallIcon(Sprite sprite)
         {
             if (targetCity != null)
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
                                     int x = 42 - greenBallBtn.Width / 2;
                                     int y = 35 - left * greenBallBtn.Height / 2;
                                     sprite.Draw(greenBallBtn, x, y, ColorValue.White);
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
                                     int x = 42 - educationBallBtn.Width / 2;
                                     int y = 45 - left * educationBallBtn.Height / 2;
                                     sprite.Draw(educationBallBtn, x, y, ColorValue.White);
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
                                     int x = 42 - healthBallBtn.Width / 2;
                                     int y = 45 - left * healthBallBtn.Height / 2;
                                     sprite.Draw(healthBallBtn, x, y, ColorValue.White);
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
