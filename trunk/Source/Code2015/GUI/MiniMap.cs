using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Code2015.World;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Apoc3D.MathLib;

namespace Code2015.GUI
{
    class MiniMap : UIComponent
    {
        const int PanelX = 0;
        const int PanelY = 530;
        const int PanelWidth = 364;
        const int PanelHeight = 190;
        const float PopBaseSpeed = 0.1f;

        const int MapX = 45;
        const int MapY = 29;
        const int MapWidth = 290;
        const int MapHeight = 125;

        const float RotIn = -MathEx.PiOver2;

        GameScene scene;
        GameState gameLogic;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;

        Texture background;

        AnimState state;

        float rot = RotIn;

        public MiniMap(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.parent = parent;
            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;
            this.gameLogic = gamelogic;


            FileLocation fl = FileSystem.Instance.Locate("ig_minimap.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl);


        }



        public override void Render(Sprite sprite)
        {
            
        }
        public override void Update(GameTime time)
        {
            const float StdRot = 0;
            if (state == AnimState.Out)
            {
                rot += (StdRot - rot + PopBaseSpeed) * time.ElapsedGameTimeSeconds;
                if (rot >= StdRot)
                {
                    rot = StdRot;
                    state = AnimState.Outside;
                }

            }
            else if (state == AnimState.In)
            {
                rot -= (StdRot - rot + PopBaseSpeed) * time.ElapsedGameTimeSeconds;
                if (rot < RotIn)
                {
                    rot = RotIn;
                    state = AnimState.Inside;                    
                }
            }

            if (state == AnimState.Outside)
            {
                //timeCounter -= time.ElapsedGameTimeSeconds;
                //if (timeCounter < 0)
                //{
                //    state = AnimState.In;
                //}
            }
        }
    }
}
