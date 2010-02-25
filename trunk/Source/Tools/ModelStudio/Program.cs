using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Apoc3D.Graphics;
using Apoc3D.Media;
using Apoc3D.Core;
using Apoc3D.Vfs;

namespace ModelStudio
{
    static class Program
    {
        public static RenderViewer Viewer
        {
            get;
            private set;
        }

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            FileSystem.Instance.AddWorkingDir(@"E:\Documents\ic10gd\Source\Code2015\bin\x86\Debug");

            GraphicsAPIManager.Instance.RegisterGraphicsAPI(new Apoc3D.RenderSystem.Xna.XnaGraphicsAPIFactory());

            DeviceContent dc = GraphicsAPIManager.Instance.CreateDeviceContent();

            PresentParameters pm2 = new PresentParameters();
            pm2.IsWindowed = true;
            pm2.BackBufferFormat = ImagePixelFormat.A8R8G8B8;
            pm2.BackBufferHeight = 600;
            pm2.BackBufferWidth = 800;

            RenderWindow wnd = (RenderWindow)dc.Create(pm2);
            Viewer = new RenderViewer(dc.RenderSystem);
            wnd.EventHandler = Viewer;

            MainForm frm = new MainForm(dc.RenderSystem);
            frm.Show();

            wnd.Run();
        }
    }
}
