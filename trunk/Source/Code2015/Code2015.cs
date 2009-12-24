using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.Graphics.Effects;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.Effects;
using Code2015.EngineEx;
using Code2015.World;
using X = Microsoft.Xna.Framework;
using XGS = Microsoft.Xna.Framework.GamerServices;
using XN = Microsoft.Xna.Framework.Net;


namespace Code2015
{
    /// <summary>
    ///  表示游戏。处理、分配各种操作例如渲染，逻辑等等。
    /// </summary>
    class Code2015 : IRenderWindowHandler
    {
        RenderSystem renderSys;
        //TerrainTile terrain;
        List<TerrainTile> terrList = new List<TerrainTile>();
        FpsCamera camera;
        SceneRenderer renderer;

        public Code2015(RenderSystem rs)
        {
            this.renderSys = rs;

        }

        #region IRenderWindowHandler 成员

        /// <summary>
        ///  处理游戏初始化操作，在游戏初始加载之前
        /// </summary>
        public void Initialize()
        {
        }

        /// <summary>
        ///  处理游戏初始加载资源工作
        /// </summary>
        public void Load()
        {
            EffectManager.Initialize(renderSys);
            EffectManager.Instance.RegisterModelEffectType(TerrainEffectFactory.Name, new TerrainEffectFactory(renderSys));

            EffectManager.Instance.LoadEffects();

            SceneRendererParameter sm = new SceneRendererParameter();
            sm.SceneManager = new OctplSceneManager(PlanetEarth.PlanetRadius);
            sm.UseShadow = false;
            sm.PostRenderer = new DefaultPostRenderer();

            {
                TerrainTile terrain = new TerrainTile(renderSys, 57, 1);
                sm.SceneManager.AddObjectToScene(terrain);
            }
            {
                TerrainTile terrain = new TerrainTile(renderSys, 57, 3);
                sm.SceneManager.AddObjectToScene(terrain);
            }
            {
                TerrainTile terrain = new TerrainTile(renderSys, 57, 5);
                sm.SceneManager.AddObjectToScene(terrain);
            }
            {
                TerrainTile terrain = new TerrainTile(renderSys, 57, 7);
                sm.SceneManager.AddObjectToScene(terrain);
            }
            {
                TerrainTile terrain = new TerrainTile(renderSys, 57, 9);
                sm.SceneManager.AddObjectToScene(terrain);
            }
            {
                TerrainTile terrain = new TerrainTile(renderSys, 57, 11);
                sm.SceneManager.AddObjectToScene(terrain);
            }
            renderer = new SceneRenderer(renderSys, sm);
            camera = new FpsCamera(1);
            renderer.RegisterCamera(camera);
            //camera.Position = new Vector3(PlanetEarth.PlanetRadius, 0, 0);

            camera.NearPlane = 1;
            camera.FarPlane = 10000;
            camera.MoveSpeed = 50;
        }

        /// <summary>
        ///  处理游戏关闭时的释放资源工作
        /// </summary>
        public void Unload()
        {
        }

        #region 临时测试代码
        [System.Runtime.InteropServices.DllImport("user32")]
        static extern bool GetAsyncKeyState(VKeys vKey);
        [System.Runtime.InteropServices.DllImport("user32")]
        static extern bool GetAsyncKeyState(short vKey);

        enum VKeys : short
        {
            VK_LBUTTON = 1,
            VK_RBUTTON = 2,
            VK_CANCEL = 3,
            VK_MBUTTON = 4,  //   NOT contiguous with L & RBUTTON  
            VK_XBUTTON1 = 5,
            VK_XBUTTON2 = 6,
            VK_BACK = 8,
            VK_TAB = 9,
            VK_CLEAR = 12,
            VK_RETURN = 13,
            VK_SHIFT = 0x10,
            VK_CONTROL = 17,
            VK_MENU = 18,
            VK_PAUSE = 19,
            VK_CAPITAL = 20,
            VK_KANA = 21,
            VK_HANGUL = 21,
            VK_JUNJA = 23,
            VK_FINAL = 24,
            VK_HANJA = 25,
            VK_KANJI = 25,
            VK_CONVERT = 28,
            VK_NONCONVERT = 29,
            VK_ACCEPT = 30,
            VK_MODECHANGE = 31,
            VK_ESCAPE = 27,
            VK_SPACE = 0x20,
            VK_PRIOR = 33,
            VK_NEXT = 34,
            VK_END = 35,
            VK_HOME = 36,
            VK_LEFT = 37,
            VK_UP = 38,
            VK_RIGHT = 39,
            VK_DOWN = 40,
            VK_SELECT = 41,
            VK_PRINT = 42,
            VK_EXECUTE = 43,
            VK_SNAPSHOT = 44,
            VK_INSERT = 45,
            VK_DELETE = 46,
            VK_HELP = 47,
            VK_A = 65,
            VK_D = 68,
            VK_S = 83,
            VK_W = 87,

            //VK_0 thru VK_9 are the same as ASCII '0' thru '9' ($30 - $39)  
            //VK_A thru VK_Z are the same as ASCII 'A' thru 'Z' ($41 - $5A)  
            VK_LWIN = 91,
            VK_RWIN = 92,
            VK_APPS = 93,
            VK_SLEEP = 95,
            VK_NUMPAD0 = 96,
            VK_NUMPAD1 = 97,
            VK_NUMPAD2 = 98,
            VK_NUMPAD3 = 99,
            VK_NUMPAD4 = 100,
            VK_NUMPAD5 = 101,
            VK_NUMPAD6 = 102,
            VK_NUMPAD7 = 103,
            VK_NUMPAD8 = 104,
            VK_NUMPAD9 = 105,
            VK_MULTIPLY = 106,
            VK_ADD = 107,
            VK_SEPARATOR = 108,
            VK_SUBTRACT = 109,
            VK_DECIMAL = 110,
            VK_DIVIDE = 111,
            VK_F1 = 112,
            VK_F2 = 113,
            VK_F3 = 114,
            VK_F4 = 115,
            VK_F5 = 116,
            VK_F6 = 117,
            VK_F7 = 118,
            VK_F8 = 119,
            VK_F9 = 120,
            VK_F10 = 121,
            VK_F11 = 122,
            VK_F12 = 123,
            VK_F13 = 124,
            VK_F14 = 125,
            VK_F15 = 126,
            VK_F16 = 127,
            VK_F17 = 128,
            VK_F18 = 129,
            VK_F19 = 130,
            VK_F20 = 131,
            VK_F21 = 132,
            VK_F22 = 133,
            VK_F23 = 134,
            VK_F24 = 135,
            VK_NUMLOCK = 144,
            VK_SCROLL = 145,
            // VK_L & VK_R - left and right Alt, Ctrl and Shift virtual keys.
            // Used only as parameters to GetAsyncKeyState() and GetKeyState().
            // No other API or message will distinguish left and right keys in this way.  
            VK_LSHIFT = 160,
            VK_RSHIFT = 161,
            VK_LCONTROL = 162,
            VK_RCONTROL = 163,
            VK_LMENU = 164,
            VK_RMENU = 165,

            VK_BROWSER_BACK = 166,
            VK_BROWSER_FORWARD = 167,
            VK_BROWSER_REFRESH = 168,
            VK_BROWSER_STOP = 169,
            VK_BROWSER_SEARCH = 170,
            VK_BROWSER_FAVORITES = 171,
            VK_BROWSER_HOME = 172,
            VK_VOLUME_MUTE = 173,
            VK_VOLUME_DOWN = 174,
            VK_VOLUME_UP = 175,
            VK_MEDIA_NEXT_TRACK = 176,
            VK_MEDIA_PREV_TRACK = 177,
            VK_MEDIA_STOP = 178,
            VK_MEDIA_PLAY_PAUSE = 179,
            VK_LAUNCH_MAIL = 180,
            VK_LAUNCH_MEDIA_SELECT = 181,
            VK_LAUNCH_APP1 = 182,
            VK_LAUNCH_APP2 = 183,

            VK_OEM_1 = 186,
            VK_OEM_PLUS = 187,
            VK_OEM_COMMA = 188,
            VK_OEM_MINUS = 189,
            VK_OEM_PERIOD = 190,
            VK_OEM_2 = 191,
            VK_OEM_3 = 192,
            VK_OEM_4 = 219,
            VK_OEM_5 = 220,
            VK_OEM_6 = 221,
            VK_OEM_7 = 222,
            VK_OEM_8 = 223,
            VK_OEM_102 = 226,
            VK_PACKET = 231,
            VK_PROCESSKEY = 229,
            VK_ATTN = 246,
            VK_CRSEL = 247,
            VK_EXSEL = 248,
            VK_EREOF = 249,
            VK_PLAY = 250,
            VK_ZOOM = 251,
            VK_NONAME = 252,
            VK_PA1 = 253,
            VK_OEM_CLEAR = 254
        }
        #endregion

        /// <summary>
        ///  进行游戏逻辑帧中的处理
        /// </summary>
        /// <param name="time"></param>
        public void Update(GameTime time)
        {
            if (GetAsyncKeyState((short)'W'))
            {
                camera.MoveFront();
            }
            if (GetAsyncKeyState((short)'D'))
            {
                camera.MoveLeft();
            }
            if (GetAsyncKeyState((short)'A'))
            {
                camera.MoveRight();
            }
            if (GetAsyncKeyState((short)'S'))
            {
                camera.MoveBack();
            }
            if (GetAsyncKeyState(VKeys.VK_SPACE))
            {
                camera.MoveUp();
            }
            if (GetAsyncKeyState(VKeys.VK_CONTROL))
            {
                camera.MoveDown();
            }

            if (GetAsyncKeyState(VKeys.VK_LEFT))
            {
                camera.TurnRight();
            }
            if (GetAsyncKeyState(VKeys.VK_RIGHT))
            {
                camera.TurnLeft();
            }
            if (GetAsyncKeyState(VKeys.VK_UP))
            {
                camera.TurnUp();
            }
            if (GetAsyncKeyState(VKeys.VK_DOWN))
            {
                camera.TurnDown();
            }

            camera.Update(time);

            renderer.Update(time);
        }

        /// <summary>
        ///  进行游戏图像渲染帧中的渲染操作
        /// </summary>
        public void Draw()
        {
            renderer.RenderScene();
        }

        #endregion
    }
}
