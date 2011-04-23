using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.World;
using Code2015.Logic;

namespace Code2015.GUI.IngameUI
{
    class Cursor : UIComponent
    {
        [Flags]
        enum MouseCursor
        {
            Normal = 0,
            LeftArrow = 1,
            RightArrow = 2,
            UpArrow = 4,
            DownArrow = 8,
            UpRightArrow = UpArrow | RightArrow,
            DownRightArrow = DownArrow | RightArrow,
            UpLeftArrow = UpArrow | LeftArrow,
            DownLeftArrow = DownArrow | LeftArrow,
            Selection = 64,
            Move = 65,
            Attack = 66,
            Stop = 67
        }
        GameScene scene;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;
        GameState logic;
        Player player;

        Picker picker;

        SelectionMarker selectionMarker;
        CitySelectInfo citySelectInfo;
        RBallTypeSelect sendBallSelect;
        MiniMap miniMap;

        bool isWaitingRBallSelect;

        Texture cursor;
        Texture[] cursor_sel;
        Texture[] cursor_attack;

        Texture cursor_move;

        Texture cursor_up;
        Texture cursor_down;
        Texture cursor_left;
        Texture cursor_right;
        Texture cursor_ul;
        Texture cursor_ur;
        Texture cursor_dl;
        Texture cursor_dr;
        Texture cursor_stop;

        MouseCursor cursorState;

        int cursorYOffset;
        int selCurIndex;
        int selCurAnimSign = 1;

        Point mouseRightPosition;

        public Cursor(Code2015 game, Game parent, GameScene scene, GameState gamelogic, Picker picker,
             CitySelectInfo citySelectInfo, RBallTypeSelect sendBallSelect, MiniMap map, SelectionMarker marker)
        {
            this.parent = parent;
            this.logic = gamelogic;
            this.player = gamelogic.LocalHumanPlayer;

            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;

            this.picker = picker;

            this.selectionMarker = marker;
            this.citySelectInfo = citySelectInfo;
            this.sendBallSelect = sendBallSelect;
            this.miniMap = map;

            FileLocation fl = FileSystem.Instance.Locate("cursor.tex", GameFileLocs.GUI);
            cursor = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("cursor_u.tex", GameFileLocs.GUI);
            cursor_up = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("cursor_l.tex", GameFileLocs.GUI);
            cursor_left = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("cursor_d.tex", GameFileLocs.GUI);
            cursor_down = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("cursor_r.tex", GameFileLocs.GUI);
            cursor_right = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("cursor_lu.tex", GameFileLocs.GUI);
            cursor_ul = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("cursor_ru.tex", GameFileLocs.GUI);
            cursor_ur = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("cursor_ld.tex", GameFileLocs.GUI);
            cursor_dl = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("cursor_rd.tex", GameFileLocs.GUI);
            cursor_dr = UITextureManager.Instance.CreateInstance(fl);
            fl = FileSystem.Instance.Locate("cursor_move.tex", GameFileLocs.GUI);
            cursor_move = UITextureManager.Instance.CreateInstance(fl);

            fl = FileSystem.Instance.Locate("cursor_stop.tex", GameFileLocs.GUI);
            cursor_stop = UITextureManager.Instance.CreateInstance(fl);


            cursor_sel = new Texture[11];

            for (int i = 0; i < cursor_sel.Length; i++)
            {
                fl = FileSystem.Instance.Locate("selcursor" + (i + 13).ToString("D2") + ".tex", GameFileLocs.GUI);
                cursor_sel[i] = UITextureManager.Instance.CreateInstance(fl);
            }

            cursor_attack = new Texture[11];
            for (int i = 0; i < cursor_attack.Length; i++)
            {
                fl = FileSystem.Instance.Locate("selcursor" + (i + 1).ToString("D2") + ".tex", GameFileLocs.GUI);
                cursor_attack[i] = UITextureManager.Instance.CreateInstance(fl);
            }

            cursorState = MouseCursor.Normal;

        }




        void UpdateScroll(GameTime time)
        {
            #region 屏幕边缘滚动视野
            const int ScrollPadding = 3;
            RtsCamera camera = parent.Scene.Camera;

            camera.Height += MouseInput.DScrollWheelValue * 0.05f;
            cursorState = MouseCursor.Normal;
            if (MouseInput.X <= ScrollPadding)
            {
                camera.MoveLeft();
                cursorState |= MouseCursor.LeftArrow;
            }
            if (MouseInput.X >= Program.Window.ClientSize.Width - ScrollPadding)
            {
                camera.MoveRight();
                cursorState |= MouseCursor.RightArrow;
            }
            if (MouseInput.Y <= ScrollPadding)
            {
                camera.MoveFront();
                cursorState |= MouseCursor.UpArrow;
            }
            if (MouseInput.Y >= Program.Window.ClientSize.Height - ScrollPadding)
            {
                camera.MoveBack();
                cursorState |= MouseCursor.DownArrow;
            }

            #endregion
            if (MouseInput.IsMouseDownRight)
            {
                mouseRightPosition.X = MouseInput.X;
                mouseRightPosition.Y = MouseInput.Y;
            }
            if (MouseInput.IsRightPressed)
            {
                if (MouseInput.X != mouseRightPosition.X && MouseInput.Y != mouseRightPosition.Y)
                {
                    int dx = MouseInput.X - mouseRightPosition.X;
                    int dy = MouseInput.Y - mouseRightPosition.Y;

                    if (dx > 10) dx = 20;
                    if (dx < -10) dx = -20;
                    if (dy > 10) dy = 20;
                    if (dy < -10) dy = -20;

                    camera.Move(dx * -0.05f, dy * -0.05f);
                }
            }
        }

        void UpdateMouseCursorScene(GameTime time) 
        {
            if (MouseInput.IsMouseUpLeft)
            {
                picker.SelectCurrentObject();
            }
            if (selectionMarker.SelectedObject != null)
            {
                City selCity = selectionMarker.SelectedObject as City;
                City hoverCity = selectionMarker.MouseHoverObject as City;

                bool passed = false;
                if (selCity != null && hoverCity != null)
                {
                    if (selCity != hoverCity)
                    {
                        if (selCity.IsCaptured && selCity.Owner == player)
                        {
                            #region 出击
                            if (selCity.CanHandleCommand())
                            {
                                // attack
                                if (MouseInput.IsMouseUpRight && selectionMarker.HasPath)
                                {
                                    if (selCity.NearbyOwnedBallCount > 0)
                                    {
                                        if (selCity.HasMultipleTypeRBalls())
                                        {
                                            isWaitingRBallSelect = true;
                                            sendBallSelect.Open(selCity, hoverCity);
                                        }
                                        else
                                        {
                                            selCity.Throw(hoverCity);
                                        }
                                    }
                                }
                                if (selectionMarker.HasPath && selCity.NearbyOwnedBallCount > 0)
                                {
                                    if (hoverCity.Owner != selCity.Owner)
                                    {
                                        cursorState = MouseCursor.Attack;
                                    }
                                    else
                                    {
                                        cursorState = MouseCursor.Move;
                                    }
                                }
                                else
                                {
                                    cursorState = MouseCursor.Stop;
                                }

                                passed = true;
                            }
                            #endregion
                        }
                        else
                        {
                            #region 漂流
                            if (selCity.HasOwnedBalls(player) && selCity.Owner != player && hoverCity.Owner == player)
                            {
                                if (selectionMarker.PathNodeLength == 2)
                                {
                                    if (MouseInput.IsMouseUpRight)
                                    {
                                        for (int i = 0; i < selCity.NearbyEnemyBallCount; i++)
                                        {
                                            selCity.GetNearbyEnemyBall(i).Float(hoverCity);
                                        }

                                    }
                                    cursorState = MouseCursor.Move;
                                    passed = true;
                                }
                            }
                            #endregion
                        }
                    }
                }

                if (!passed)
                {
                    if (selectionMarker.MouseHoverObject != null)
                    {
                        if (selectionMarker.SelectedObject != selectionMarker.MouseHoverObject)
                        {
                            cursorState = MouseCursor.Selection;
                        }
                        else
                        {
                            cursorState = MouseCursor.Normal;
                        }
                    }
                    else
                    {
                        // command harv
                        cursorState = MouseCursor.Normal;
                    }
                }
            }
            else
            {
                if (selectionMarker.MouseHoverObject != null)
                {
                    cursorState = MouseCursor.Selection;
                }
                else
                {
                    cursorState = MouseCursor.Normal;
                }
            }
        }
        void UpdateMouseCursor(GameTime time) 
        {
            bool isOnHUD = isWaitingRBallSelect;

            if (miniMap.HitTest(MouseInput.X, MouseInput.Y))
                isOnHUD = true;

            if (isOnHUD)
            {
                cursorState = MouseCursor.Normal;
            }
            else
            {
                UpdateMouseCursorScene(time);
            }
            
        }
        public override void Update(GameTime time)
        {
            base.Update(time);

            UpdateScroll(time);

            selectionMarker.SelectedObject = picker.SelectedObject;
            selectionMarker.MouseHoverObject = picker.MouseHoverObject;

            UpdateMouseCursor(time);

            citySelectInfo.SelectedCity = picker.SelectedCity;


            if (isWaitingRBallSelect)
            {
                if (!sendBallSelect.IsOpened)
                {
                    if (!sendBallSelect.IsCancelled)
                        sendBallSelect.SourceCity.Throw(sendBallSelect.TargetCity, sendBallSelect.SelectedBallType);

                    isWaitingRBallSelect = false;
                }
            }
        }
            
    
        public override void Render(Sprite sprite)
        {
            cursorYOffset = 0;

            Point hsp = new Point();
            Texture ctex = cursor;
            switch (cursorState)
            {
                case MouseCursor.Normal:
                    hsp = new Point(15, 19);
                    ctex = cursor;
                    break;
                case MouseCursor.LeftArrow:
                    hsp = new Point(5, 24);
                    ctex = cursor_left;
                    break;
                case MouseCursor.DownArrow:
                    hsp = new Point(24, 26);
                    ctex = cursor_down;
                    break;
                case MouseCursor.RightArrow:
                    hsp = new Point(31, 24);
                    ctex = cursor_right;
                    break;
                case MouseCursor.UpArrow:
                    hsp = new Point(25, 4);
                    ctex = cursor_up;
                    break;
                case MouseCursor.DownLeftArrow:
                    hsp = new Point(8, 34);
                    ctex = cursor_dl;
                    break;
                case MouseCursor.DownRightArrow:
                    hsp = new Point(34, 34);
                    ctex = cursor_dr;
                    break;
                case MouseCursor.UpLeftArrow:
                    hsp = new Point(7, 7);
                    ctex = cursor_ul;
                    break;
                case MouseCursor.UpRightArrow:
                    hsp = new Point(35, 8);
                    ctex = cursor_ur;
                    break;
                case MouseCursor.Stop:
                    hsp = new Point(33, 33);
                    ctex = cursor_stop;
                    break;
                case MouseCursor.Move:
                    hsp = new Point(65, 86);
                    ctex = cursor_move;


                    if (cursorYOffset > 15)
                    {
                        selCurAnimSign = -1;
                        cursorYOffset = 15;
                    }
                    else if (cursorYOffset < 0)
                    {
                        selCurAnimSign = 1;
                        cursorYOffset = 0;
                    }
                    cursorYOffset += selCurAnimSign;
                    break;
                        
                case MouseCursor.Attack:
                    hsp = new Point(66, 62);

                    ctex = cursor_attack[selCurIndex];

                    if (selCurIndex >= cursor_sel.Length - 1)
                    {
                        selCurAnimSign = -1;
                        selCurIndex = cursor_sel.Length - 1;
                    }
                    else if (selCurIndex <= 0)
                    {
                        selCurAnimSign = 1;
                        selCurIndex = 0;
                    }

                    selCurIndex += selCurAnimSign;
                    break;
                case MouseCursor.Selection:
                    hsp = new Point(33, 33);

                    ctex = cursor_sel[selCurIndex];

                    if (selCurIndex >= cursor_sel.Length - 1)
                    {
                        selCurAnimSign = -1;
                        selCurIndex = cursor_sel.Length - 1;
                    }
                    else if (selCurIndex <= 0)
                    {
                        selCurAnimSign = 1;
                        selCurIndex = 0;
                    }

                    selCurIndex += selCurAnimSign;
                    break;
            }

            
            sprite.Draw(ctex, MathEx.Clamp(0, Program.ScreenWidth, MouseInput.X) - hsp.X,
                MathEx.Clamp(0, Program.ScreenHeight, MouseInput.Y) - hsp.Y - cursorYOffset, ColorValue.White);
        }

        public override bool HitTest(int x, int y)
        {
            return false;
        }

        public override int Order
        {
            get
            {
                return 100;
            }
        }
    }
}
