using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Graphics;
using Apoc3D.GUI.Controls;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.BalanceSystem;
using Code2015.EngineEx;
using Code2015.GUI.Controls;
using Code2015.Logic;
using Code2015.World;
using Code2015.World.Screen;

namespace Code2015.GUI
{
    /// <summary>
    ///  用于显示游戏中物体的信息
    /// </summary>
    class InfoUI : UIComponent
    {
        GameScene scene;
        GameState gameLogic;
        RenderSystem renderSys;
        Code2015 game;
        Game parent;
        Font font;
        Player player;


        CityInfoDisplay cityInfoDisplay;
        ResInfoDisplay resInfoDisplay;

        RtsCamera camera;

        CityLinkableMark linkArrow;


        ISelectableObject selected;
        CityObject city;
       

        Point selectedProjPos;

        public ISelectableObject SelectedObject
        {
            get { return selected; }
            set
            {
                if (selected != value)
                {
                    selected = value;

                    if (selected != null)
                    {
                        city = selected as CityObject;

                        if (city != null)
                        {
                            Vector3 ppos = renderSys.Viewport.Project(city.Position, camera.ProjectionMatrix, camera.ViewMatrix, Matrix.Identity);

                            selectedProjPos.X = (int)ppos.X;
                            selectedProjPos.Y = (int)ppos.Y;

                            City cc = city.City;
                            CityObject[] nearby = new CityObject[cc.LinkableCityCount];

                            for (int i = 0; i < cc.LinkableCityCount; i++)
                            {
                                nearby[i] = cc.GetLinkableCity(i).Parent;
                            }

                            linkArrow.SetCity(city, nearby);
                        }
                    }
                    else
                    {
                        city = null;
                    }

                    if (city == null)
                    {
                        linkArrow.SetCity(null, null);
                    }
                }
            }
        }

        public InfoUI(Code2015 game, Game parent, GameScene scene, GameState gamelogic)
        {
            this.parent = parent;
            this.game = game;
            this.renderSys = game.RenderSystem;
            this.scene = scene;
            this.gameLogic = gamelogic;

            this.camera = scene.Camera;
            this.player = parent.HumanPlayer;

            FileLocation fl = FileSystem.Instance.Locate("def.fnt", GameFileLocs.GUI);
            font = FontManager.Instance.CreateInstance(renderSys, fl, "default");

            this.cityInfoDisplay = new CityInfoDisplay(scene, renderSys, player);
            this.resInfoDisplay = new ResInfoDisplay(scene, renderSys);

            linkArrow = new CityLinkableMark(renderSys);
            scene.Scene.AddObjectToScene(linkArrow);
        }

        public CityInfoDisplay CityInfoDisplay
        {
            get { return cityInfoDisplay; }
        }

        public override int Order
        {
            get { return 1; }
        }
        public override bool HitTest(int x, int y)
        {
            return false;
        }
      



        public override void Render(Sprite sprite)
        {
            cityInfoDisplay.Render(sprite);
            resInfoDisplay.Render(sprite);

        }


        public override void Update(GameTime time)
        {
            cityInfoDisplay.Update(time);
            resInfoDisplay.Update(time);

        }
    }
}
