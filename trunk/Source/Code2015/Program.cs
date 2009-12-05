using System;
using Apoc3D.Graphics;

namespace Code2015
{
    static class Program
    {
        static RenderWindow CreateRenderWindow()
        {
            DeviceContent devContent = GraphicsAPIManager.Instance.CreateDeviceContent();

            PresentParameters pm = new PresentParameters();

            RenderControl ctrl = devContent.Create(pm);

            RenderWindow wnd = (RenderWindow)ctrl;



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

