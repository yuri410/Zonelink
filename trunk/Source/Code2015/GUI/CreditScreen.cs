using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D;
using Apoc3D.MathLib;
using Apoc3D.Vfs;
using Code2015.EngineEx;

namespace Code2015.GUI
{
    class CreditScreen : UIComponent
    {
        RenderSystem renderSys;
        Menu parent;

        Texture cursor;
        Point mousePosition;

        NormalSoundObject mouseHover;
        NormalSoundObject mouseDown;

        public CreditScreen(RenderSystem rs, Menu parent)
        {
            this.renderSys = rs;
            this.parent = parent;

            FileLocation fl = FileSystem.Instance.Locate("cursor.tex", GameFileLocs.GUI);
            cursor = UITextureManager.Instance.CreateInstance(fl);



            mouseHover = (NormalSoundObject)SoundManager.Instance.MakeSoundObjcet("buttonHover", null, 0);
            mouseDown = (NormalSoundObject)SoundManager.Instance.MakeSoundObjcet("buttonDown", null, 0);
        }

        public override void Render(Sprite sprite)
        {


            sprite.Draw(cursor, mousePosition.X, mousePosition.Y, ColorValue.White);
        }
        public override void Update(GameTime time)
        {
            mousePosition.X = MouseInput.X;
            mousePosition.Y = MouseInput.Y; 
            
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
