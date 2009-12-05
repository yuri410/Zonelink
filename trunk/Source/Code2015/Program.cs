using System;
using Apoc3D.Graphics;
using Apoc3D.Core;
using Apoc3D.Vfs;
using Apoc3D.Media;
using Microsoft.Xna.Framework.Storage;

namespace Code2015
{
    static class Program
    {
        static RenderWindow CreateRenderWindow()
        {
            FileSystem.Instance.AddWorkingDir(StorageContainer.TitleLocation);
            PluginManager.Initiailze(null, null);

            DeviceContent devContent = GraphicsAPIManager.Instance.CreateDeviceContent();

            PresentParameters pm = new PresentParameters();
            
#warning test code
            pm.BackBufferFormat = ImagePixelFormat.X8R8G8B8;
            pm.BackBufferWidth = 1024;
            pm.BackBufferHeight = 768;
            pm.IsWindowed = true;
            pm.DepthFormat = DepthFormat.Depth24Stencil8;

            RenderControl ctrl = devContent.Create(pm);

            RenderWindow wnd = (RenderWindow)ctrl;

            wnd.EventHandler = new Code2015();

            return wnd;
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
        }
    }
}

