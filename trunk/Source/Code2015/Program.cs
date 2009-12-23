using System;
using Apoc3D.Graphics;
using Apoc3D.Core;
using Apoc3D.Vfs;
using Apoc3D.Media;
using Microsoft.Xna.Framework.Storage;
using X = Microsoft.Xna.Framework;
using XN = Microsoft.Xna.Framework.Net;
using XGS = Microsoft.Xna.Framework.GamerServices;

namespace Code2015
{
    static class Program
    {
        static DeviceContent devContent;

        static RenderWindow CreateRenderWindow()
        {
            FileSystem.Instance.AddWorkingDir(StorageContainer.TitleLocation);
            PluginManager.Initiailze(null, null);

            devContent = GraphicsAPIManager.Instance.CreateDeviceContent();

            PresentParameters pm = new PresentParameters();
            
            pm.BackBufferFormat = ImagePixelFormat.X8R8G8B8;
            pm.BackBufferWidth = Properties.Settings.Default.ScreenWidth;
            pm.BackBufferHeight = Properties.Settings.Default.ScreenHeight;
            pm.IsWindowed = Properties.Settings.Default.IsWindowed;
            pm.DepthFormat = DepthFormat.Depth24Stencil8;


            RenderControl ctrl = devContent.Create(pm);

            RenderWindow window = (RenderWindow)ctrl;

            window.EventHandler = new Code2015(devContent.RenderSystem);

            #region hacks
            X.Game game = (X.Game)window.Tag;

            //XGS.GamerServicesComponent liveComp = new XGS.GamerServicesComponent(game);
            //game.Components.Add(liveComp);
            #endregion

            return window;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (RenderWindow game = CreateRenderWindow())
            {
                game.Run();
            }
            Properties.Settings.Default.Save();
        }
    }
}

