using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.Logic;
using Code2015.World;
using Apoc3D.GUI.Controls;

namespace Code2015.GUI.IngameUI
{
    class CitySelectInfo : UIComponent
    {

        GameScene scene;
        GameState gameLogic;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;
        Player player;


        #region 城市状态
        Texture statusEnemyBackground;  
        Texture statusBackground;  
        Texture statusHPBackground;
        Texture statusHPTex;
        Texture statusExpBackground;
        Texture statusExpTex;
        Texture statusProducePrgTex;


        Texture statusExpBuffTex;
        bool isExpBuffer;
        Texture statusHPBufferTex;
        bool isHPBuffer;
        Texture statusExpdownBuff;
        bool isDownShow;
        #endregion
        
        #region 血条
        Texture onCityHPBackground;
        Texture onCityHPTex;
        Texture onCityHPBase;
        #endregion


        Texture greenBallTex;
        Texture educationBallTex;
        Texture healthBallTex;

        GameFontRuan f8;
        GameFontRuan f6;

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



        public CitySelectInfo(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.scene = scene;
            this.renderSys = game.RenderSystem;
            this.player = parent.HumanPlayer;
            this.gameLogic = gamelogic;
            this.parent = parent;
            this.game = game;

            FileLocation fl = FileSystem.Instance.Locate("nig_status_bk.tex", GameFileLocs.GUI);
            statusBackground = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_status_enemyCity.tex", GameFileLocs.GUI);
            statusEnemyBackground = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_status_hp_group.tex", GameFileLocs.GUI);
            statusHPBackground = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_status_exp_bk.tex", GameFileLocs.GUI);
            statusExpBackground = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_status_hp.tex", GameFileLocs.GUI);
            statusHPTex = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_status_exp.tex", GameFileLocs.GUI);
            statusExpTex = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_exp_buff.tex", GameFileLocs.GUI);
            statusExpBuffTex = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_hp_buff.tex", GameFileLocs.GUI);
            statusHPBufferTex = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_expdown_buff.tex", GameFileLocs.GUI);
            statusExpdownBuff = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_city_hp_group.tex", GameFileLocs.GUI);
            onCityHPBackground = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_city_hp.tex", GameFileLocs.GUI);
            onCityHPTex = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_city_hp_bk.tex", GameFileLocs.GUI);
            onCityHPBase = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_icon_green.tex", GameFileLocs.GUI);
            greenBallTex = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_icon_edu.tex", GameFileLocs.GUI);
            educationBallTex = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_icon_hospital.tex", GameFileLocs.GUI);
            healthBallTex = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_status_city_produceprg.tex", GameFileLocs.GUI);
            statusProducePrgTex = UITextureManager.Instance.CreateInstance(fl);

            resBallsCount[0].Type = RBallType.Green;
            resBallsCount[0].count = 0;

            resBallsCount[1].Type = RBallType.Education;
            resBallsCount[1].count = 0;

            resBallsCount[2].Type = RBallType.Health;
            resBallsCount[2].count = 0;

            f8 = GameFontManager.Instance.FRuanEdged8;
            f6 = GameFontManager.Instance.FRuanEdged6;

        }

        public override int Order
        {
            get
            {
                return 52;
            }
        }


        public override bool HitTest(int x, int y)
        {
            if (selectCity != null)
            {
                Rectangle rect = new Rectangle(417, 586, 498, 134);
                return Control.IsInBounds(x, y, ref rect);
            }
            return false;
        }


        public override void Render(Sprite sprite)
        {

            RenderSelectedCityHP(sprite);
            RenderCityStatus(sprite);
            RenderBallIcon(sprite);
        }


        private void StatisticRBall()
        {
            if (selectCity != null)
            {
                resBallsCount[0].count = 0;
                resBallsCount[1].count = 0;
                resBallsCount[2].count = 0;
                isExpBuffer = false;
                isHPBuffer = false;
                isDownShow = false;

                for (int i = 0; i < selectCity.NearbyOwnedBallCount; i++)
                {
                    if (selectCity.GetNearbyOwnedBall(i).Type == RBallType.Green)
                        resBallsCount[0].count++;


                    if (selectCity.GetNearbyOwnedBall(i).Type == RBallType.Education)
                        resBallsCount[1].count++;

                    if (selectCity.GetNearbyOwnedBall(i).Type == RBallType.Health)
                        resBallsCount[2].count++;
                }

                for (int i = 0; i < selectCity.NearbyEnemyBallCount; i++)
                {
                    if (selectCity.GetNearbyEnemyBall(i).Type == RBallType.Volience)
                    {
                        isDownShow = true;
                        break;
                    }
                }

                if (resBallsCount[1].count != 0)
                {
                    isExpBuffer = true;
                 }
                if (resBallsCount[2].count != 0)
                {
                    isHPBuffer = true;
                }

            }

            

        }


        public override void Update(GameTime time)
        {
            base.Update(time);
            StatisticRBall();
        }


        private void RenderSelectedCityHP(Sprite sprite)
        {
            if (SelectedCity != null)
            {
                float radLng = MathEx.Degree2Radian(SelectedCity.Longitude);
                float radLat = MathEx.Degree2Radian(SelectedCity.Latitude);

                Vector3 tangy = PlanetEarth.GetTangentY(radLng, radLat);
                Vector3 tangx = PlanetEarth.GetTangentX(radLng, radLat);

                Vector3 cityNormal = PlanetEarth.GetNormal(radLng, radLat);
                cityNormal.Normalize();

                Vector3 hpPos = SelectedCity.Position + tangy * 150 + cityNormal * 400;

                Viewport vp = renderSys.Viewport;
                Vector3 screenPos = vp.Project(hpPos, scene.Camera.ProjectionMatrix,
                                               scene.Camera.ViewMatrix, Matrix.Identity);

                Vector3 lp = vp.Project(hpPos + tangx, scene.Camera.ProjectionMatrix,
                                                       scene.Camera.ViewMatrix, Matrix.Identity);

                Vector3 rp = vp.Project(hpPos - tangx, scene.Camera.ProjectionMatrix,
                                                       scene.Camera.ViewMatrix, Matrix.Identity);


                float scale = 1.75f * Vector3.Distance(lp, rp) / 2;

                Matrix trans = Matrix.Translation(-onCityHPBackground.Width / 2, -onCityHPBackground.Height / 2, 0) *
                           Matrix.Scaling(scale, scale, 1) * Matrix.Translation(screenPos.X, screenPos.Y, 0);



                int hpTexWidth = (int)(onCityHPTex.Width * SelectedCity.HPRate);

                sprite.SetTransform(trans);
                sprite.Draw(onCityHPBase, 0, 0, ColorValue.White);
                sprite.Draw(onCityHPTex, new Rectangle(0, 0, hpTexWidth, onCityHPTex.Height),
                    new Rectangle(0, 0, hpTexWidth, onCityHPTex.Height), ColorValue.White);
                sprite.Draw(onCityHPBackground, 0, 0, ColorValue.White);
                f6.DrawString(sprite, SelectedCity.Level.ToString(), 25, -16, ColorValue.White);

                sprite.SetTransform(Matrix.Identity);

                

            }
        }

        private void RenderCityStatus(Sprite sprite)
        {
            if (selectCity != null)
            {

                int hp = (int)selectCity.HealthValue;
                int hpFull = (int)(selectCity.HealthValue / selectCity.HPRate);
                int level = selectCity.Level;
                string name = selectCity.Name;


                if (selectCity.Owner == player)
                {
                    sprite.Draw(statusBackground, 405, 580, ColorValue.White);
                }
                else
                {
                    sprite.Draw(statusEnemyBackground, 405, 580, ColorValue.White);
                }

                if (isHPBuffer)
                    sprite.Draw(statusHPBufferTex, 776, 624, ColorValue.White);
                if (isExpBuffer)
                    sprite.Draw(statusExpBuffTex, 802, 624, ColorValue.White);
                if (isDownShow)
                    sprite.Draw(statusExpdownBuff, 839, 622, ColorValue.White);

                f6.DrawString(sprite, selectCity.Name.ToUpperInvariant(), 456, 572, ColorValue.White);
                f6.DrawString(sprite, level.ToString().ToUpperInvariant(), 775, 570, ColorValue.White);

                //画资源球图标                                
                int hpTexWidth = (int)(statusHPTex.Width * SelectedCity.HPRate);
                string hpInfo = hp.ToString() + "/" + hpFull.ToString();

                int expTexWidth = (int)(statusExpTex.Width * selectCity.LevelEP);
                string expInfo = "EXP " + ((int)(selectCity.LevelEP * 100)).ToString() + "/100";

                //HP
                sprite.Draw(statusHPTex, new Rectangle(505, 638, hpTexWidth, statusHPTex.Height),
                    new Rectangle(0, 0, hpTexWidth, statusHPTex.Height), ColorValue.White);
                sprite.Draw(statusHPBackground, 506, 640, ColorValue.White);

                if (selectCity.Owner == player)
                {
                    Matrix trans = Matrix.Scaling(0.8f, 0.8f, 1) * Matrix.Translation(new Vector3(579, 638, 0));
                    sprite.SetTransform(trans);
                    f8.DrawString(sprite, hpInfo, 0, 0, ColorValue.White);
                    sprite.SetTransform(Matrix.Identity);

                    //EXP
                    sprite.Draw(statusExpTex, new Rectangle(578, 624, expTexWidth, statusExpTex.Height),
                        new Rectangle(0, 0, expTexWidth, statusExpTex.Height), ColorValue.White);
                    sprite.Draw(statusExpBackground, 579, 624, ColorValue.White);


                    trans = Matrix.Scaling(0.8f, 0.8f, 1) * Matrix.Translation(new Vector3(635, 620, 0));
                    sprite.SetTransform(trans);
                    f8.DrawString(sprite, expInfo, 0, 0, ColorValue.White);
                    sprite.SetTransform(Matrix.Identity);
                }
            }
        }
 
        private void RenderBallIcon(Sprite sprite)
        {
            if (selectCity != null && selectCity.Owner == player)
            {

                //Array.Sort(resBallsCount, BallRecordCompare);

                int left = 0;
                for (int i = 0; i < resBallsCount.Length; i++)
                {
                    RBallType ballType = resBallsCount[i].Type;
                    bool overrideDraw = selectCity.CanProduceProduction() ? (ballType == selectCity.GetProductionType()) : false;
                    //resBallsCount[i].Type == selectCity.GetProductionType()
                    switch (ballType)
                    {
                        case RBallType.Green:
                            {
                                if (resBallsCount[i].count != 0 || overrideDraw)
                                {
                                    if (selectCity.CanProduceProduction() && selectCity.GetProductionType() == RBallType.Green)
                                    {

                                        int height = (int)(selectCity.GetProductionProgress() * statusProducePrgTex.Height);


                                        sprite.Draw(statusProducePrgTex, new Rectangle(651 + 69 * left, 723 - height, statusProducePrgTex.Width, height),
                                             new Rectangle(0, statusProducePrgTex.Height - height, statusProducePrgTex.Width, height), ColorValue.White);

                                    }

                                    int x = 687 + 69 * left - greenBallTex.Width / 2;
                                    int y = 692 - greenBallTex.Height / 2;
                                    sprite.Draw(greenBallTex, x, y, ColorValue.White);
                                    Matrix trans = Matrix.Scaling(0.8f, 0.8f, 1) *
                                            Matrix.Translation(new Vector3(x + 45 - f8.MeasureString(resBallsCount[i].count.ToString()).Width, y + 18, 0));
                                    sprite.SetTransform(trans);
                                    f8.DrawString(sprite, resBallsCount[i].count.ToString(), 0, 0, ColorValue.White);
                                    sprite.SetTransform(Matrix.Identity);
                                    left++;
                                }

                                

                            }
                            break;
                        case RBallType.Education:
                            {
                                if (resBallsCount[i].count != 0 || overrideDraw)
                                {
                                    if (selectCity.CanProduceProduction() && selectCity.GetProductionType() == RBallType.Education)
                                    {

                                        int height = (int)(selectCity.GetProductionProgress() * statusProducePrgTex.Height);


                                        sprite.Draw(statusProducePrgTex, new Rectangle(651 + 69 * left, 723 - height, statusProducePrgTex.Width, height),
                                             new Rectangle(0, statusProducePrgTex.Height - height, statusProducePrgTex.Width, height), ColorValue.White);

                                    }

                                    int x = 687 + 69 * left - educationBallTex.Width / 2;
                                    int y = 692 - educationBallTex.Height /2 ;
                                    sprite.Draw(educationBallTex, x, y, ColorValue.White);
                                    Matrix trans = Matrix.Scaling(0.8f, 0.8f, 1) * 
                                            Matrix.Translation( new Vector3(x + 45 - f8.MeasureString(resBallsCount[i].count.ToString()).Width, y + 18, 0));
                                    sprite.SetTransform(trans);
                                    f8.DrawString(sprite, resBallsCount[i].count.ToString(), 0, 0, ColorValue.White);
                                    sprite.SetTransform(Matrix.Identity);
                                  

                                    left++;
                                }
                               
                            }
                            break;
                        case RBallType.Health:
                            {
                                if (resBallsCount[i].count != 0 || overrideDraw)
                                {
                                    if (selectCity.CanProduceProduction() && selectCity.GetProductionType() == RBallType.Health)
                                    {

                                        int height = (int)(selectCity.GetProductionProgress() * statusProducePrgTex.Height);


                                        sprite.Draw(statusProducePrgTex, new Rectangle(651 + 69 * left, 723 - height, statusProducePrgTex.Width, height),
                                             new Rectangle(0, statusProducePrgTex.Height - height, statusProducePrgTex.Width, height), ColorValue.White);

                                    }

                                    int x = 687 + 69 * left - healthBallTex.Width / 2;
                                    int y = 692 - healthBallTex.Height / 2;
                                    sprite.Draw(healthBallTex, x, y, ColorValue.White);
                                    Matrix trans = Matrix.Scaling(0.8f, 0.8f, 1) *
                                             Matrix.Translation(new Vector3(x + 45 - f8.MeasureString(resBallsCount[i].count.ToString()).Width, y + 18, 0));
                                    sprite.SetTransform(trans);
                                    f8.DrawString(sprite, resBallsCount[i].count.ToString(), 0, 0, ColorValue.White);
                                    sprite.SetTransform(Matrix.Identity);
                                    left++;
                                }
                             
                            }
                            break;
                    }
                }

                
            }
        }


    }
}
