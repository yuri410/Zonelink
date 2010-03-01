using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Config;
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Effects;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.Effects;
using Code2015.EngineEx;
using XI = Microsoft.Xna.Framework.Input;

namespace ModelStudio
{
   
    class RenderViewer : IRenderWindowHandler
    {
        RenderSystem renderSys;
        Sprite sprite;
        Font font;

        public ResourceHandle< Texture> CurrentTexture
        {
            get;
            set;
        }

        public RenderViewer(RenderSystem rs) 
        {
            renderSys = rs;
        }

        #region IRenderWindowHandler 成员

        public void Initialize()
        {
            FileLocateRule.Textures = GameFileLocs.Texture;
            FileLocateRule.Effects = GameFileLocs.Effect;

            ConfigurationManager.Initialize();
            ConfigurationManager.Instance.Register(new IniConfigurationFormat());
            ConfigurationManager.Instance.Register(new GameConfigurationFormat());

            EffectManager.Initialize(renderSys);
            EffectManager.Instance.RegisterModelEffectType(TerrainEffect513Factory.Name, new TerrainEffect513Factory(renderSys));
            EffectManager.Instance.RegisterModelEffectType(TerrainEffect129Factory.Name, new TerrainEffect129Factory(renderSys));
            EffectManager.Instance.RegisterModelEffectType(TerrainEffect33Factory.Name, new TerrainEffect33Factory(renderSys));
            EffectManager.Instance.RegisterModelEffectType(WaterEffectFactory.Name, new WaterEffectFactory(renderSys));
            EffectManager.Instance.RegisterModelEffectType(StandardEffectFactory.Name, new StandardEffectFactory(renderSys));

            TextureManager.Initialize(1048576 * 100);
            TextureManager.Instance.Factory = renderSys.ObjectFactory;

            ModelManager.Initialize();
            EffectManager.Instance.LoadEffects();

        }

        public void finalize()
        {
           
        }

        public void Load()
        {
            FileLocation fl = FileSystem.Instance.Locate("def.fnt", GameFileLocs.GUI);
            font = FontManager.Instance.CreateInstance(renderSys, fl, "default");
            sprite = renderSys.ObjectFactory.CreateSprite();
        }

        public void Unload()
        {
        }

        public void Update(GameTime time)
        {
          
        }

        public void Draw()
        {
            renderSys.Clear(ClearFlags.DepthBuffer | ClearFlags.Target, ColorValue.White, 1, 0);

            if (CurrentTexture != null)
            {
                if (CurrentTexture.State == ResourceState.Loaded)
                {
                    Texture tex = CurrentTexture;

                    sprite.Begin();

                    Size clSize = Program.Window.ClientSize;

                    float aspect = tex.Width / (float)tex.Height;

                    if (tex.Width < clSize.Width)
                        clSize.Width = tex.Width;
                    if (tex.Height < clSize.Height)
                        clSize.Height = tex.Height;

                    
                    sprite.Draw(tex, new Rectangle(0, 0, clSize.Width, clSize.Height), ColorValue.White);

                    string msg = "Format: " + tex.Format.ToString() +
                        "\nSize: " + tex.Width.ToString() + "x" + tex.Height.ToString() +
                        "\nLevels:" + tex.SurfaceCount.ToString();

                    unchecked
                    {
                        font.DrawString(sprite, msg, 11, 11, 15, DrawTextFormat.Left, (int)0xff000000);
                    }

                    font.DrawString(sprite, msg, 10, 10, 15, DrawTextFormat.Left, -1);

                    sprite.End();
                }
                else
                {
                    CurrentTexture.Touch();
                }
            }
            
        }
        #endregion
    }
}
