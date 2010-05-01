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
using Code2015.BalanceSystem;

namespace Code2015.GUI
{
    class NoticeBar : UIComponent
    {
        const int PanelX = -2;
        const int PanelY = 30;
        const int PanelWidth = 635;
        const int PanelHeight = 70;
        const float PopBaseSpeed = 20;

        const float DisplayDuration = 10;
        const float DisplayInterval = DisplayDuration + 20;
        GameScene scene;
        GameState gameLogic;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;
        Player player;
        MiniMap minimap;

        AnimState state;

        float cx = -PanelWidth;

        Texture background;
        float timeCounter;

        float[] activeStateCd;

        NormalSoundObject nolumber;
        NormalSoundObject nooil;
        NormalSoundObject nofood;
        NormalSoundObject storm;

        string currentMessage = string.Empty;

        void NewMessage(EventEntry e)
        {
            if (e.Object == null)
                return;

            if (activeStateCd[(int)e.Type] < float.Epsilon)
            {
                switch (e.Type)
                {
                    case EventType.Food:
                        currentMessage = "CITIZENS NEED MORE FOOD.\n BUILD MORE MORE FARMS.";
                        nofood.Fire();

                        state = AnimState.Out;
                        break;
                    case EventType.Oil:

                        currentMessage = "OIL CAN HELP CITY DEVELOP FASTER.\nTRY GET SOME OIL FOR THE CITY.";
                        nooil.Fire();

                        state = AnimState.Out; 
                        break;
                    case EventType.Strike:
                        currentMessage = "A HURRICANE STRIKES YOUR CITY.";
                        storm.Fire();

                        state = AnimState.Out;
                        break;
                    case EventType.Wood:
                        currentMessage = "WE NEED MORE LUMBER.";

                        nolumber.Fire();

                        state = AnimState.Out;
                        break;
                }
                City c = e.Object as City;
                if (c != null && c.IsCaptured)
                {
                    minimap.AddNotifyRed(e.Object.Longitude, e.Object.Latitude, c.Owner.SideColor);
                }


                activeStateCd[(int)e.Type] = DisplayInterval; 
            }
          
        }

        public NoticeBar(Code2015 game, Game parent, GameScene scene, GameState gamelogic, MiniMap map)
        {
            this.minimap = map;
            this.parent = parent;
            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;
            this.gameLogic = gamelogic;
            this.player = parent.HumanPlayer;

            FileLocation fl = FileSystem.Instance.Locate("ig_notice.tex", GameFileLocs.GUI);
            background = UITextureManager.Instance.CreateInstance(fl);

            EventLogger.Instance.NewLog += NewMessage;

            activeStateCd = new float[(int)EventType.Count];



            nolumber = (NormalSoundObject)SoundManager.Instance.MakeSoundObjcet("nolumber", null, 0);
            nooil = (NormalSoundObject)SoundManager.Instance.MakeSoundObjcet("nooil", null, 0);
            nofood = (NormalSoundObject)SoundManager.Instance.MakeSoundObjcet("nofood", null, 0);
            storm = (NormalSoundObject)SoundManager.Instance.MakeSoundObjcet("stormy", null, 0);
            
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
            {
                background.Dispose();
                background = null;
                EventLogger.Instance.NewLog -= NewMessage;
            }
        }

        public override int Order
        {
            get { return 6; }
        }
        public override bool HitTest(int x, int y)
        {
            return false;
        }
        public override void Render(Sprite sprite)
        {
            if (state != AnimState.Inside)
            {
                sprite.Draw(background, (int)cx, PanelY, ColorValue.White);

                GameFont f18 = GameFontManager.Instance.F18;
                if (!string.IsNullOrEmpty(currentMessage))
                {
                    f18.DrawString(sprite, currentMessage, (int)cx + 90, PanelY + 15, ColorValue.White);
                }
            }
        }

        public override void Update(GameTime time)
        {
            if (state == AnimState.Outside)
            {
                timeCounter -= time.ElapsedGameTimeSeconds;
                if (timeCounter < 0)
                {
                    state = AnimState.In;
                }
            }

            if (state == AnimState.Out)
            {
                cx += (PanelWidth + PanelX - cx + PopBaseSpeed) * time.ElapsedGameTimeSeconds * 2;
                if (cx >= PanelX)
                {
                    cx = PanelX;
                    state = AnimState.Outside;
                    timeCounter = DisplayDuration;
                }
            }
            else if (state == AnimState.In)
            {
                cx -= (PanelWidth + PanelX - cx + PopBaseSpeed) * time.ElapsedGameTimeSeconds * 2;
                if (cx < -PanelWidth)
                {
                    cx = -PanelWidth;
                    state = AnimState.Inside;
                }

                currentMessage = string.Empty;
            }




            for (int i = 0; i < activeStateCd.Length; i++)
            {
                activeStateCd[i] -= time.ElapsedGameTimeSeconds;
                if (activeStateCd[i] < 0)
                    activeStateCd[i] = 0;
            }
        }
    }
}
