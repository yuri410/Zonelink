using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D;
using Apoc3D.Graphics;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;
using Code2015.Logic;

namespace Code2015.GUI
{
    class ExitConfirm : UIComponent
    {
        bool passed = true;

        Texture tex;

        public bool IsShown { get { return !passed; } }
        public void Show()
        {
            passed = false;
        }

        public ExitConfirm()
        {
            FileLocation fl = FileSystem.Instance.Locate("ig_exitconfirm.tex", GameFileLocs.GUI);
            tex = UITextureManager.Instance.CreateInstance(fl);

        }
        public override void Render(Sprite sprite)
        {
            if (!passed)
            {
                sprite.Draw(tex, 0, 0, ColorValue.White);
            }
        }
        public override bool HitTest(int x, int y)
        {
            return !passed;
        }
        public override int Order
        {
            get
            {
                return 105;
            }
        }
        public override void Update(GameTime time)
        {
        }
        public override void UpdateInteract(GameTime time)
        {
            if (MouseInput.IsMouseUpLeft)
            {
                passed = true;
            }
        }
    }
}
