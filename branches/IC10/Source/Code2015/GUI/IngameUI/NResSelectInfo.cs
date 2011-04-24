using System;
using System.Collections.Generic;
using System.Text;
using Code2015.World;
using Apoc3D;
using Apoc3D.Graphics;
using Code2015.Logic;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;

namespace Code2015.GUI.IngameUI
{
    class NResSelectInfo : UIComponent
    {
        GameScene scene;
        GameState gameLogic;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;
        Player player;

        Texture oilBackground;
        Texture oilBarTex;
        Texture oilEdge;

        Texture woodBackgrond;
        Texture woodBarTex;
        Texture woodEdge;

        Texture harvBackground;
        Texture harvHPBar;
        Texture harvHPEdge;
        Texture harvSTBar;
        Texture harvSTEdge;
        

        GameFontRuan f8;

        NaturalResource selectNRes;
        public NaturalResource SelectedResource
        {
            get { return selectNRes; }
            set
            {
                if ( selectNRes != value)
                {
                    
                    selectNRes = value;
                }
            }
        }

        Harvester selectHarvester;
        public Harvester SelectHarvester
        {
            get { return selectHarvester; }
            set
            {
                if (selectHarvester != value)
                {

                    selectHarvester = value;
                }
            }
        }


        public override int Order
        {
            get
            {
                return 53;
            }
        }


        public NResSelectInfo(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.scene = scene;
            this.renderSys = game.RenderSystem;
            this.player = parent.HumanPlayer;
            this.gameLogic = gamelogic;
            this.parent = parent;
            this.game = game;

            FileLocation fl = FileSystem.Instance.Locate("nig_status_oil_bk.tex", GameFileLocs.GUI);
            oilBackground = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_status_oil_bar_value.tex", GameFileLocs.GUI);
            oilBarTex = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_status_oil_bar_edge.tex", GameFileLocs.GUI);
            oilEdge = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_status_green_bk.tex", GameFileLocs.GUI);
            woodBackgrond = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_status_green_bar_value.tex", GameFileLocs.GUI);
            woodBarTex = UITextureManager.Instance.CreateInstance(fl);


            fl = FileSystem.Instance.Locate("nig_status_green_bar_edge.tex", GameFileLocs.GUI);
            woodEdge = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_status_harv_bk.tex", GameFileLocs.GUI);
            harvBackground = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_status_harv_hpbar_value.tex", GameFileLocs.GUI);
            harvHPBar = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_status_harv_hpbar_edge.tex", GameFileLocs.GUI);
            harvHPEdge = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_status_harv_stbar_value.tex", GameFileLocs.GUI);
            harvSTBar = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("nig_status_harv_stbar_edge.tex", GameFileLocs.GUI);
            harvSTEdge = UITextureManager.Instance.CreateInstance(fl);


            f8 = GameFontManager.Instance.FRuanEdged8;
        }

        public override bool HitTest(int x, int y)
        {
            return false;
        }


        public override void Render(Sprite sprite)
        {

            RenderHarvester(sprite);

            if (selectNRes is OilFieldObject)
            {
                RenderOil(sprite);
            }else
                if (selectNRes is ForestObject)
                {
                    RenderWood(sprite);
                }
        }

        public override void Update(GameTime time)
        {
            base.Update(time);
        }


        public  void RenderOil(Sprite sprite)
        {
            
            OilFieldObject oil = selectNRes as OilFieldObject;

            
            int oilBarTexWidth = (int)(oilBarTex.Width * oil.CurrentAmount / oil.MaxAmount);

            string hpInfo = ((int)oil.CurrentAmount).ToString() + "/" + oil.MaxAmount.ToString();

            sprite.Draw(oilBackground, 405, 580, ColorValue.White);

            sprite.Draw(oilBarTex, new Rectangle(580, 625, oilBarTexWidth, oilBarTex.Height),
                    new Rectangle(0, 0, oilBarTexWidth, oilBarTex.Height), ColorValue.White);
            sprite.Draw(oilEdge, 566, 625, ColorValue.White);


            Matrix trans = Matrix.Scaling(0.8f, 0.8f, 1) * Matrix.Translation(new Vector3(666, 634, 0));
            sprite.SetTransform(trans);
            f8.DrawString(sprite, hpInfo, 0, 0, ColorValue.White);
            sprite.SetTransform(Matrix.Identity);

        }

        public  void RenderWood(Sprite sprite)
        {
            ForestObject wood = selectNRes as ForestObject;


            int woodBarTexWidth = (int)(woodBarTex.Width * wood.CurrentAmount / wood.MaxAmount);

            string hpInfo = ((int)wood.CurrentAmount).ToString() + "/" + wood.MaxAmount.ToString();

            sprite.Draw(woodBackgrond, 405, 580, ColorValue.White);

            sprite.Draw(woodBarTex, new Rectangle(598, 625, woodBarTexWidth, woodBarTex.Height),
                    new Rectangle(0, 0, woodBarTexWidth, woodBarTex.Height), ColorValue.White);
            sprite.Draw(woodEdge, 580, 610, ColorValue.White);


            Matrix trans = Matrix.Scaling(0.8f, 0.8f, 1) * Matrix.Translation(new Vector3(676, 634, 0));
            sprite.SetTransform(trans);
            f8.DrawString(sprite, hpInfo, 0, 0, ColorValue.White);
            sprite.SetTransform(Matrix.Identity);

        }

        public void RenderHarvester(Sprite sprite)
        {
            if (selectHarvester != null)
            {
                int harvHPBarWidth = (int)(harvHPBar.Width * SelectHarvester.CurrentHP / SelectHarvester.GetProps().HP);
                int harvSTBarWidth = (int)(harvSTBar.Width * SelectHarvester.CurrentStorage / SelectHarvester.GetProps().Storage);

                string hpInfo = ((int)SelectHarvester.CurrentHP).ToString()+ "/" + ((int)SelectHarvester.GetProps().HP).ToString();
                string stInfo = ((int)SelectHarvester.CurrentStorage).ToString() + "/" + ((int)SelectHarvester.GetProps().Storage).ToString();

                sprite.Draw(harvBackground, 405, 580, ColorValue.White);

                sprite.Draw(harvHPBar, new Rectangle(588, 658, harvHPBarWidth, harvHPBar.Height),
                        new Rectangle(0, 0, harvHPBarWidth, harvHPBar.Height), ColorValue.White);
                sprite.Draw(harvHPEdge, 570, 655, ColorValue.White);


                sprite.Draw(harvSTBar, new Rectangle(699, 690, harvSTBarWidth, harvSTBar.Height),
                    new Rectangle(0, 0, harvSTBarWidth, harvSTBar.Height), ColorValue.White);
                sprite.Draw(harvSTEdge, 685, 686, ColorValue.White);

                Matrix trans = Matrix.Scaling(0.8f, 0.8f, 1) * Matrix.Translation(new Vector3(676, 660, 0));
                sprite.SetTransform(trans);
                f8.DrawString(sprite, hpInfo, 0, 0, ColorValue.White);
                sprite.SetTransform(Matrix.Identity);
            }
        }

    }
}
