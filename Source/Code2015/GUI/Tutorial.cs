using System;
using System.Collections.Generic;
using System.Text;
using Apoc3D.Graphics;
using Apoc3D.Vfs;
using Code2015.EngineEx;

namespace Code2015.GUI
{
    public class Tutorial:UIComponent
    {
        Texture[] help;

        public Tutorial()
        {
                
        }
        public void LoadHelp()
        {
            help = new Texture[14];
            for (int i = 0; i < 14; i++)
            {
                FileLocation fl = FileSystem.Instance.Locate(i.ToString()+".tex", GameFileLocs.GUI);
                help[i] = UITextureManager.Instance.CreateInstance(fl);
           
            
            }
        }

        public override void UpdateInteract(Apoc3D.GameTime time)
        {
            base.UpdateInteract(time);
        }
        public override void Render(Sprite sprite)
        {
            base.Render(sprite);
        }
        public override void Update(Apoc3D.GameTime time)
        {
            base.Update(time);
        }
    }
}
