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
using Apoc3D.Config;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Effects;
using Apoc3D.MathLib;
using Apoc3D.Media;
using Apoc3D.Vfs;
using Code2015.Effects;
using Code2015.EngineEx;
using Code2015.GUI;
using Code2015.World;
using X = Microsoft.Xna.Framework;
using XGS = Microsoft.Xna.Framework.GamerServices;
using XN = Microsoft.Xna.Framework.Net;
using XFG = Microsoft.Xna.Framework.Graphics;
using Code2015.Logic;

namespace Code2015
{
    interface IGameComponent
    {
        void Render();
        void Render(Sprite sprite);
        void Update(GameTime time);
    }

    /// <summary>
    ///  表示游戏。处理、分配各种操作例如渲染，逻辑等等。
    /// </summary>
    class Code2015 : IRenderWindowHandler
    {
        RenderSystem renderSys;

        Menu menu;
        Game currentGame;

        Sprite sprite;
        X.Game game;


        public Code2015(RenderSystem rs, X.Game game)
        {
            this.renderSys = rs;
            this.game = game;
        }

        /// <summary>
        ///  表示当前是否在游戏过程中。即不在主菜单都为在游戏过程中。
        /// </summary>
        public bool IsIngame 
        {
            get { return currentGame != null; }
        }

        public Game CurrentGame
        {
            get { return currentGame; }
        }

        public RenderSystem RenderSystem
        {
            get { return renderSys; }
        }

        #region IRenderWindowHandler 成员

        /// <summary>
        ///  处理游戏初始化操作，在游戏初始加载之前
        /// </summary>
        public void Initialize()
        {
            FileLocateRule.Textures = GameFileLocs.Texture;
            FileLocateRule.Effects = GameFileLocs.Effect;

            ConfigurationManager.Initialize();
            ConfigurationManager.Instance.Register(new IniConfigurationFormat());
            ConfigurationManager.Instance.Register(new GameConfigurationFormat());

            EffectManager.Initialize(renderSys);
            EffectManager.Instance.RegisterModelEffectType(TerrainEffect33Factory.Name, new TerrainEffect33Factory(renderSys));
            EffectManager.Instance.RegisterModelEffectType(WaterEffectFactory.Name, new WaterEffectFactory(renderSys));
            EffectManager.Instance.RegisterModelEffectType(StandardEffectFactory.Name, new StandardEffectFactory(renderSys));
            EffectManager.Instance.RegisterModelEffectType(SkinnedStandardEffectFactory.Name, new SkinnedStandardEffectFactory(renderSys));
            EffectManager.Instance.RegisterModelEffectType(EarthBaseEffectFactory.Name, new EarthBaseEffectFactory(renderSys));
            EffectManager.Instance.RegisterModelEffectType(AtmosphereEffectFactory.Name, new AtmosphereEffectFactory(renderSys));
            EffectManager.Instance.RegisterModelEffectType(CityLinkEffectFactory.Name, new CityLinkEffectFactory(renderSys));
            EffectManager.Instance.RegisterModelEffectType(CityRingEffectFactory.Name, new CityRingEffectFactory(renderSys));
            EffectManager.Instance.RegisterModelEffectType(TreeEffectFactory.Name, new TreeEffectFactory(renderSys));
            EffectManager.Instance.RegisterModelEffectType(ParticleRDEffectFactory.Name, new ParticleRDEffectFactory(renderSys));
            EffectManager.Instance.RegisterModelEffectType(CloudEffectFactory.Name, new CloudEffectFactory(renderSys));
            EffectManager.Instance.RegisterModelEffectType(BoltEffectFactory.Name, new BoltEffectFactory(renderSys));
            EffectManager.Instance.RegisterModelEffectType(CitySelEffectFactory.Name, new CitySelEffectFactory(renderSys));
            EffectManager.Instance.RegisterModelEffectType(SmokeRDEffectFactory.Name, new SmokeRDEffectFactory(renderSys));
            EffectManager.Instance.RegisterModelEffectType(GoalEffectFactory.Name, new GoalEffectFactory(renderSys));
            EffectManager.Instance.RegisterModelEffectType(BoardEffectFactory.Name, new BoardEffectFactory(renderSys));

            RulesTable.LoadRules();

            TextureManager.Initialize(1048576 * 200);
            TextureManager.Instance.Factory = renderSys.ObjectFactory;
            MaterialLibrary.Initialize(renderSys);

            ModelManager.Initialize();

            EffectManager.Instance.LoadEffects();
            FileLocation fl = FileSystem.Instance.Locate("terrainMaterial.ini", GameFileLocs.Config);
            MaterialLibrary.Instance.LoadTextureSet(fl);
            TreeModelLibrary.Initialize(renderSys);

            GameFontManager.Initiaize(renderSys);

            TerrainData.Initialize();

            sprite = renderSys.ObjectFactory.CreateSprite();
            SoundManager.Initialize(((X.Game)Program.Window.Tag).Services);
            StaticPlay.Init(((X.Game)Program.Window.Tag).Services);
            StaticPlay.PlayStart();

            menu = new Menu(this, renderSys);
        }

        public void finalize()
        {
            sprite.Dispose();

            TextureManager.Instance.Dispose();
            ModelManager.Instance.Dispose();
            EffectManager.Instance.Dispose();
            TerrainMeshManager.Instance.Dispose();

            FileSystem.Instance.Dispose();

            EngineTimer.Dispose();
        }


        public void StartNewGame(GameCreationParameters gcp)
        {
            currentGame = new Game(this, gcp);
        }
        public void Back() 
        {
            currentGame = null;
            
        }

        public void Exit()
        {
            game.Exit();
        }

        #region unused
        /// <summary>
        ///  处理游戏初始加载资源工作
        /// </summary>
        public void Load()
        {
            Precache.Cache(renderSys);
            // streaming 结构，不在此加载资源
        }

        /// <summary>
        ///  处理游戏关闭时的释放资源工作
        /// </summary>
        public void Unload()
        {

        }
        #endregion

        /// <summary>
        ///  进行游戏逻辑帧中的处理
        /// </summary>
        /// <param name="time"></param>
        public void Update(GameTime time)
        {
            MouseInput.Update(time);

            if (menu != null)
            {
                menu.Update(time);
            }

            if (currentGame != null)
            {
                currentGame.Update(time);
            }
        }

        /// <summary>
        ///  进行游戏图像渲染帧中的渲染操作
        /// </summary>
        public void Draw()
        {
            RenderTarget defRt = renderSys.GetRenderTarget(0);

            if (currentGame == null)
            {
                renderSys.Clear(ClearFlags.Target | ClearFlags.DepthBuffer, ColorValue.Transparent, 1, 0);
            }
            else
            {
                currentGame.Render();
            }

            if (menu != null)
            {
                menu.Render();
            }

            renderSys.SetRenderTarget(0, defRt);


            sprite.Begin();

            if (currentGame != null)
            {
                currentGame.Render(sprite);
            }
            if (menu != null)
            {
                menu.Render(sprite);
            }

            sprite.End();

        }

        #endregion
    }
}
