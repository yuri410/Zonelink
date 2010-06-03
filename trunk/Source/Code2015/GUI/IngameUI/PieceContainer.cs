/*
-----------------------------------------------------------------------------
This source file is part of Zonelink

Copyright (c) 2009+ Tao Games

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  if not, write to the Free Software Foundation, 
Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA, or go to
http://www.gnu.org/copyleft/gpl.txt.

-----------------------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.World;
using Code2015.World.Screen;
using Code2015.BalanceSystem;
using Code2015.Logic;

namespace Code2015.GUI
{
    class PieceContainerOverlay : UIComponent
    {
        Texture containers_conver;
        public PieceContainerOverlay(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            FileLocation fl = FileSystem.Instance.Locate("ig_box_cover.tex", GameFileLocs.GUI);
            containers_conver = UITextureManager.Instance.CreateInstance(fl);

        }

        public override int Order
        {
            get
            {
                return 11;
            }
        }

        public override void Render(Sprite sprite)
        {
            //sprite.Draw(ico_exchange, 1075, 521, ColorValue.White);
            sprite.Draw(containers_conver, 702, 626, ColorValue.White);

        }

    }
    class PieceContainer : UIComponent
    {
        GameScene scene;
        GameState gameLogic;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;

        GameFont f14;

        //侧边栏图标
        Texture containers;

        Dictionary<Player, Texture> ico_exchange = new Dictionary<Player, Texture>();
        Texture defaultExchange;

        int count1;
        int count2;
        int count3;
        int count4;

        int exCounter;
        

        MdgResource exInside;
        Player exInsidePlayer;
        Player player;

        GoalIcons icons;
        MdgResourceManager resources;

        static readonly Vector2 PinPosition = new Vector2(1219, 668);

        public static bool IsInRange(Vector2 p)
        {
            return Vector2.Distance(p, PinPosition) < MdgPhysicsParams.BallRadius;
        }

        public PieceContainer(Code2015 game, Game parent, GameScene scene, GameState gamelogic, GoalIcons icons)
        {
            this.parent = parent;
            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;
            this.gameLogic = gamelogic;
            this.icons = icons;
            this.resources = icons.Manager;

            //侧边栏
            FileLocation fl = FileSystem.Instance.Locate("ig_box.tex", GameFileLocs.GUI);
            containers = UITextureManager.Instance.CreateInstance(fl);

            f14 = GameFontManager.Instance.F14;



            player = gamelogic.LocalHumanPlayer;

            for (int i = 0; i < gamelogic.LocalPlayerCount; i++)
            {
                Player cp = gamelogic.GetLocalPlayer(i);

                ColorValue color = cp.SideColor;

                fl = FileSystem.Instance.Locate("ig_exchange.tex", GameFileLocs.GUI);
                defaultExchange = UITextureManager.Instance.CreateInstance(fl);

                if (color == ColorValue.Red)
                {
                    fl = FileSystem.Instance.Locate("ig_exchangered.tex", GameFileLocs.GUI);
                }
                else if (color == ColorValue.Green)
                {
                    fl = FileSystem.Instance.Locate("ig_exchangegreen.tex", GameFileLocs.GUI);
                }
                else if (color == ColorValue.Yellow)
                {
                    fl = FileSystem.Instance.Locate("ig_exchangeyellow.tex", GameFileLocs.GUI);
                }
                else
                {
                    fl = FileSystem.Instance.Locate("ig_exchangeblue.tex", GameFileLocs.GUI);
                }
                ico_exchange.Add(cp, UITextureManager.Instance.CreateInstance(fl));
            }

        }

        public override int Order
        {
            get { return 2; }
        }
        public override bool HitTest(int x, int y)
        {
            return false;
        }
        public override void UpdateInteract(GameTime time)
        {
            
        }

        public override void Render(Sprite sprite)
        {
            sprite.Draw(containers, 702, 626, ColorValue.White);

            if (exInsidePlayer == null)
            {
                sprite.Draw(defaultExchange, 1155, 598, ColorValue.White);
            }
            else
            {
                sprite.Draw(ico_exchange[exInsidePlayer], 1155, 598, ColorValue.White);
            }

            if (exInside != null)
            {
                exInside.Render(sprite);
            }
            sprite.SetTransform(Matrix.Identity);

            f14.DrawString(sprite, count4.ToString(), 775, 704, ColorValue.White);
            f14.DrawString(sprite, count2.ToString(), 882, 704, ColorValue.White);
            f14.DrawString(sprite, count1.ToString(), 984, 704, ColorValue.White);
            f14.DrawString(sprite, count3.ToString(), 1084, 704, ColorValue.White);

            f14.DrawString(sprite, "EXCHANGE", 1172, 605, ColorValue.White);

            if (exInsidePlayer != null)
            {
                GameFontManager.Instance.F18G1.DrawString(sprite, "WITH OTHERS", 1160, 702, exInsidePlayer.SideColor);
            }
            
        }
        
        public MdgResource Exchange(MdgResource res)
        {
            MdgResource result = exInside;
            icons.Manager.Add(result);
            result.ReactivePhysics();
            result.Position = PinPosition;

            icons.Manager.Remove(res);
            
            
            exInside = res;              
            exInsidePlayer = player;

            result.Velocity += new Vector2(-0.1f, -0.9f) * 250;

            exCounter = 0;
            return result;
        }
        public override void Update(GameTime time)
        {
            count1 = 0;
            for (int i = 0; i < resources.GetResourceCount(MdgType.Hunger); i++)
            {
                if (resources.GetResource(MdgType.Hunger, i).IsInBox)
                {
                    count1++;
                }
            }

            count2 = 0;
            for (int i = 0; i < resources.GetResourceCount(MdgType.Education); i++)
            {
                if (resources.GetResource(MdgType.Education, i).IsInBox)
                {
                    count2++;
                }
            }
            for (int i = 0; i < resources.GetResourceCount(MdgType.GenderEquality); i++)
            {
                if (resources.GetResource(MdgType.GenderEquality, i).IsInBox)
                {
                    count2++;
                }
            } 
            
            count3 = 0;
            for (int i = 0; i < resources.GetResourceCount(MdgType.Diseases); i++)
            {
                if (resources.GetResource(MdgType.Diseases, i).IsInBox)
                {
                    count3++;
                }
            }
            for (int i = 0; i < resources.GetResourceCount(MdgType.MaternalHealth); i++)
            {
                if (resources.GetResource(MdgType.MaternalHealth, i).IsInBox)
                {
                    count3++;
                }
            }
            for (int i = 0; i < resources.GetResourceCount(MdgType.ChildMortality); i++)
            {
                if (resources.GetResource(MdgType.ChildMortality, i).IsInBox)
                {
                    count3++;
                }
            }

            count4 = 0;
            for (int i = 0; i < resources.GetResourceCount(MdgType.Environment); i++)
            {
                if (resources.GetResource(MdgType.Environment, i).IsInBox)
                {
                    count4++;
                }
            }


            if (exInside != null) 
            {
                exInside.Position = PinPosition;
            }

            exCounter++;
            if (exCounter > 600)
            {
                //currentEx = Randomizer.GetRandomInt(ico_exchange.Length);
                exCounter = 0;


                exInside = new MdgResource(icons.Manager, icons.PhysicsWorld,
                    (MdgType)Randomizer.GetRandomInt((int)MdgType.Count - 1), PinPosition, 0);


                exInside.NotifyRemoved();
                
                do
                {
                    exInsidePlayer = gameLogic.GetLocalPlayer(Randomizer.GetRandomInt(gameLogic.LocalPlayerCount));
                }
                while (exInsidePlayer == player);
            }
        }
    }
}
