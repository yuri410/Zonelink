using System;
using Apoc3D.Graphics;
using Apoc3D.Core;
using Apoc3D.Vfs;
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

