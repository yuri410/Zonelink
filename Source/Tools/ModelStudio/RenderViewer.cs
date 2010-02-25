using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.Scene;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Apoc3D.Config;
using Apoc3D.Graphics.Effects;
using Code2015.EngineEx;
using Code2015.Effects;

namespace ModelStudio
{
    class ModelWrapper : Entity 
    {
        

        public ModelWrapper() 
            : base(false)
        {
            Transformation = Matrix.Identity;
            position = Vector3.Zero;
        }

        public void setmdl(Model mdl) 
        {
            ModelL0 = mdl;
        }

        public override bool IsSerializable
        {
            get { return false; }
        }
    }
    class RenderViewer : IRenderWindowHandler
    {
        RenderSystem renderSys;
        SceneRenderer renderer;
        SceneManager sceneManager;
        ChaseCamera camera;
        ModelWrapper obj;

        public Model CurrentModel
        {
            get { return obj.ModelL0; }
            set { obj.setmdl(value); }
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
        }

        public void finalize()
        {
           
        }

        public void Load()
        {

            camera = new ChaseCamera(.75f);
            camera.ChaseDirection = new Vector3(0, 0, 1);
            camera.ChasePosition = Vector3.Zero;


            sceneManager = new SceneManager();
            SceneRendererParameter sm = new SceneRendererParameter ();
            sm.PostRenderer = new DefaultPostRenderer();
            sm.SceneManager = sceneManager;
            sm.UseShadow = false;


            renderer = new SceneRenderer(renderSys, sm);

            renderer.RegisterCamera(camera);

            obj = new ModelWrapper();

            sceneManager.AddObjectToScene(obj);
        }

        public void Unload()
        {
        }

        public void Update(GameTime time)
        {
            renderer.Update(time);
        }

        public void Draw()
        {
            renderSys.Clear(ClearFlags.Target, ColorValue.Black, 1, 0);
            renderer.RenderScene();
        }

        #endregion
    }
}
