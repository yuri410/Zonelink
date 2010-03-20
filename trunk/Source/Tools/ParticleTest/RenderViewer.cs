using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Config;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Effects;
using Apoc3D.MathLib;
using Apoc3D.Scene;
using Apoc3D.Vfs;
using Code2015.Effects;
using Code2015.EngineEx;
using Code2015.ParticleSystem;
using XI = Microsoft.Xna.Framework.Input;

namespace ModelStudio
{
    class RenderViewer : IRenderWindowHandler
    {
        RenderSystem renderSys;
        SceneRenderer renderer;
        SceneManager sceneManager;
        ChaseCamera camera;

        ParticleEffect peff;

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
            EffectManager.Instance.RegisterModelEffectType(AtmosphereEffectFactory.Name, new AtmosphereEffectFactory(renderSys));
            EffectManager.Instance.RegisterModelEffectType(CityLinkEffectFactory.Name, new CityLinkEffectFactory(renderSys));
            EffectManager.Instance.RegisterModelEffectType(CityRingEffectFactory.Name, new CityRingEffectFactory(renderSys));
            EffectManager.Instance.RegisterModelEffectType(TreeEffectFactory.Name, new TreeEffectFactory(renderSys));
            EffectManager.Instance.RegisterModelEffectType(ParticleRDEffectFactory.Name, new ParticleRDEffectFactory(renderSys));

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
            Size clSize = Program.Window.ClientSize;
            camera = new ChaseCamera(clSize.Width / (float)clSize.Height);
            camera.ChaseDirection = new Vector3(0, 0, 1);
            camera.ChasePosition = new Vector3(0, 0, 0);
            camera.DesiredPositionOffset = new Vector3(0, 0, 40);  
            camera.Mode = RenderMode.Final;
            camera.FarPlane = 1000;
            camera.NearPlane = 0.5f;

            distance = 40;
            yang = MathEx.Degree2Radian(30);
            xang = MathEx.Degree2Radian(45);

            sceneManager = new SceneManager();
            SceneRendererParameter sm = new SceneRendererParameter ();
            sm.PostRenderer = new DefaultPostRenderer();
            sm.SceneManager = sceneManager;
            sm.UseShadow = false;


            renderer = new SceneRenderer(renderSys, sm);

            renderer.RegisterCamera(camera);

            
            peff = new ParticleEffect(renderSys, 320);
            peff.Emitter = new ParticleEmitter(1);
            peff.Modifier = new ParticleModifier();


            sceneManager.AddObjectToScene(peff);
        }

        public void Unload()
        {
        }

        XI.MouseState lastState;
        float xang;
        float yang;
        float distance;

        public void Update(GameTime time)
        {
            renderer.Update(time);

            XI.MouseState mstate = XI.Mouse.GetState();

            camera.ChaseDirection = new Vector3((float)Math.Cos(xang), -(float)Math.Sin(yang), (float)Math.Sin(xang));
            camera.DesiredPositionOffset = new Vector3(0, 0, distance);

            distance -= 0.05f * (mstate.ScrollWheelValue - lastState.ScrollWheelValue);
            if (mstate.RightButton == XI.ButtonState.Pressed)
            {
                xang += MathEx.Degree2Radian(mstate.X - lastState.X) * 0.5f;
                yang += MathEx.Degree2Radian(mstate.Y - lastState.Y) * 0.5f;
            }
            else if (mstate.LeftButton == XI.ButtonState.Pressed) 
            {
                //obj.Orientation *= Matrix.RotationY(MathEx.Degree2Radian(mstate.X - lastState.X) * 0.5f);
            }

            lastState = mstate;
        }

        public void Draw()
        {
            renderer.RenderScene();
        }

        #endregion
    }
}
