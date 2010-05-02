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
    class ColorNotify : UIComponent
    {
        bool passed = false;

        Texture tex;

        public ColorNotify(Player player)
        {
            ColorValue color = player.SideColor;
            if (color == ColorValue.Red)
            {
                FileLocation fl = FileSystem.Instance.Locate("ig_nred.tex", GameFileLocs.GUI);
                tex = UITextureManager.Instance.CreateInstance(fl);
            }
            else if (color == ColorValue.Green)
            {
                FileLocation fl = FileSystem.Instance.Locate("ig_ngreen.tex", GameFileLocs.GUI);
                tex = UITextureManager.Instance.CreateInstance(fl);

            }
            else if (color == ColorValue.Yellow )
            {
                FileLocation fl = FileSystem.Instance.Locate("ig_nyellow.tex", GameFileLocs.GUI);
                tex = UITextureManager.Instance.CreateInstance(fl);

            }
            else
            {
                FileLocation fl = FileSystem.Instance.Locate("ig_nblue.tex", GameFileLocs.GUI);
                tex = UITextureManager.Instance.CreateInstance(fl);
            }
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
                return 101;
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
