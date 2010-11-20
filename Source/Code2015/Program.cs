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
using Apoc3D.Core;
using Apoc3D.Graphics;
using Apoc3D.Media;
using Apoc3D.Vfs;
using Microsoft.Xna.Framework.Storage;
using X = Microsoft.Xna.Framework;
using XGS = Microsoft.Xna.Framework.GamerServices;
using XN = Microsoft.Xna.Framework.Net;

namespace Code2015
{
    static class Program
    {
        public const int ScreenWidth = 1280;
        public const int ScreenHeight = 720;
        static DeviceContent devContent;

        static RenderWindow CreateRenderWindow()
        {
            FileSystem.Instance.AddWorkingDir(StorageContainer.TitleLocation);
            GraphicsAPIManager.Instance.RegisterGraphicsAPI(new Apoc3D.RenderSystem.Xna.XnaGraphicsAPIFactory());
            devContent = GraphicsAPIManager.Instance.CreateDeviceContent();

            PresentParameters pm = new PresentParameters();
            
            pm.BackBufferFormat = ImagePixelFormat.A8R8G8B8;
            pm.BackBufferWidth = ScreenWidth;
            pm.BackBufferHeight = ScreenHeight;
            pm.IsWindowed = false;
            pm.DepthFormat = DepthFormat.Depth32;
            

            RenderControl ctrl = devContent.Create(pm);

            RenderWindow window = (RenderWindow)ctrl;

            window.EventHandler = new Code2015(devContent.RenderSystem, (X.Game)window.Tag);

            #region hacks
            //X.Game game = (X.Game)window.Tag;
            //XGS.GamerServicesComponent liveComp = new XGS.GamerServicesComponent(game);
            //game.Components.Add(liveComp);
            #endregion

            return window;
        }
        public static RenderWindow Window
        {
            get;
            private set;
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (RenderWindow game = CreateRenderWindow())
            {
                Window = game;
                game.Run();
            }
            //Properties.Settings.Default.Save();
        }
    }
}

