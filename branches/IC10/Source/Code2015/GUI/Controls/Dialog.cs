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
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Apoc3D.Config;

namespace Apoc3D.GUI.Controls
{
    public class DialogSettings
    {
        public class DialogStyle
        {
            //string text;
            string name;

            bool useButtonA;
            bool useButtonB;

            ConfigurationSection section;
            ConfigurationSection btnASect;
            ConfigurationSection btnBSect;



            public DialogStyle(Configuration uiIni, string sectName)
            {
                name = sectName;
                section = uiIni[sectName];
                //text = IniFile.GetUIString(section, "Text", st);
                useButtonA = section.GetBool("UseButtonA", true);
                useButtonB = section.GetBool("UseButtonB", true);

                if (useButtonA)
                    btnASect = uiIni[section["ButtonA"]];
                if (useButtonB)
                    btnBSect = uiIni[section["ButtonB"]];
            }

            public string Name
            {
                get { return name; }
            }

            public ConfigurationSection Section
            {
                get { return section; }
            }
            public ConfigurationSection ButtonASection
            {
                get { return btnASect; }
            }
            public ConfigurationSection ButtonBSection
            {
                get { return btnBSect; }
            }
            public bool UseButtonA
            {
                get { return useButtonA; }
            }
            public bool UseButtonB
            {
                get { return useButtonB; }
            }
        }

        Dictionary<string, DialogStyle> data;


        public DialogStyle this[string name]
        {
            get
            {
                return data[name];
            }
        }

        public DialogSettings(GameUI gui)
        {
            ConfigModel.Configuration uiIni = gui.UIConfig;

            ConfigurationSection sect = uiIni["DialogSettings"];
            data = new Dictionary<string, DialogStyle>(sect.Count, CaseInsensitiveStringComparer.Instance);

            for (int i = 0; i < sect.Count; i++)
            {
                string name = sect[i.ToString()];

                DialogStyle ds = new DialogStyle(uiIni, name);

                data.Add(name, ds);
            }

        }
    }

    public class Dialog : Control, IDisposable
    {
        public const int DWidth = 451;
        public const int DHeight = 326;

        const int Border = 50;

        Texture bgImg;
        Label lbl;

        Button btnA;
        Button btnB;
        UITexAnim btnABg;
        UITexAnim btnBBg;
        DialogSettings.DialogStyle style;


        bool isShown;

        CloseHandler close;

        [ScriptEvent("close")]
        public event CloseHandler Close
        {
            add { close += value; }
            remove { close -= value; }
        }

        public void OnClose(object sender)
        {
            if (close != null)
                close(this);
        }

        public Dialog(GameUI gameUI, DialogSettings.DialogStyle style)
            : this(gameUI, style, null)
        {
        }


        public Dialog(GameUI gameUI, DialogSettings.DialogStyle style, string name)
            : base(gameUI, name)
        {
            this.style = style;

            //lbl = new Label(gameUI, name + Type.Delimiter + "ContentLabel");
            //lbl.AutoSize = false;

            Width = DWidth;
            Height = DHeight;
            X = (GameUI.UIWidth - DWidth) / 2;
            Y = (GameUI.UIHeight - DHeight) / 2;
        }


        public Button ButtonA
        {
            get { return btnA; }
        }
        public Button ButtonB
        {
            get { return btnB; }
        }
        public bool IsShown
        {
            get { return isShown; }
        }
        public void ShowDialog()
        {
            GameUI.CurrentDialog = this;
            isShown = true;
        }
        public void CloseDialog()
        {
            GameUI.CurrentDialog = null;
            isShown = false;
            OnClose(this);
        }

        public System.Drawing.Font Font
        {
            get { return lbl.Font; }
            set { lbl.Font = value; }
        }
        public string Text
        {
            get { return lbl.Text; }
            set { lbl.Text = value; }
        }

        public override void PaintControl()
        {
            Sprite.Transform = Matrix.Translation(X, Y, 0);
            base.PaintControl();
        }
 
        void PaintImpl(Sprite spr)
        {


            if (bgImg != null && !bgImg.Disposed)
            {
                Sprite.Draw(bgImg, modColor);
            }
            //lbl.PaintControl();
            //if (btnA != null)
            //{
            //    btnA.PaintControl();
            //}
            //if (btnB != null)
            //{
            //    btnB.PaintControl();
            //}
        }

        public override void Parse(ConfigurationSection sect)
        {
        }

        void UpdatingImpl(object sender, float dt)
        {
            //if (btnA != null)
            //{
            //    btnA.Update(dt);
            //}
            //if (btnB != null)
            //{
            //    btnB.Update(dt);
            //}
            //lbl.Update(dt);
            if (lbl != null)
            {
                lbl.Text = Text;
                lbl.Font = Font;
            }
        }
        void ObjectLoadImpl(object sender)
        {          
            ConfigurationSection sect = style.Section;


            lbl = Label.FromConfigSection(GameUI, sect);
            lbl.AutoSize = false;
            lbl.Size = new Size(DWidth - Border * 2, DHeight - Border * 2);
            lbl.X = Border; lbl.Y = Border;
          
            modColor = style.Section.GetColorRGBA("ModualteColor", Color.White);

            if (style.UseButtonA)
            {
                sect = style.ButtonASection;
                btnA = Button.FromConfigSection(GameUI, sect);
                btnABg = new UITexAnim(style.ButtonASection, FileSystem.LocalMix, string.Empty);
                btnA.Size = new Size(btnABg.Width, btnABg.Height);
                btnA.Image = btnABg[0];
                btnA.ImageMouseOver = btnABg[1];
                btnA.ImageMouseDown = btnABg[2];

                btnA.X += X;
                btnA.Y += Y;

                AddControl(btnA);
            }
            if (style.UseButtonB)
            {
                sect = style.ButtonBSection;
                btnB = Button.FromConfigSection(GameUI, sect);

                btnBBg = new UITexAnim(style.ButtonBSection, FileSystem.LocalMix, string.Empty);
                btnB.Size = new Size(btnBBg.Width, btnBBg.Height);
                btnB.Image = btnBBg[0];
                btnB.ImageMouseOver = btnBBg[1];
                btnB.ImageMouseDown = btnBBg[2];
                btnB.X += X;
                btnB.Y += Y;

                AddControl(btnB);
            }


            //ImageBase bg = style.Section.GetImage(FileSystem.GameResLR);
            // new UIImage(style.Section, FileSystem.LocalMix, string.Empty, FileSystem.GameResLR);
            UIImageInformation bgInfo = style.Section.GetImage(FileSystem.GameResLR);

            bgImg = UITextureManager.Instance.CreateInstance(bgInfo);// bg.GetTexture(Game.Instance.Device, Usage.None, Pool.Managed);
            //bg.Dispose();

            lbl.X += X;
            lbl.Y += Y;
            AddControl(lbl);
        }


        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (bgImg != null && !bgImg.Disposed)
                {
                    UITextureManager.Instance.DestoryInstance(bgImg);
                    //bgImg.Dispose();//.DisposeHandler(null, EventArgs.Empty);
                    //bgImg = null;
                }
                if (lbl != null)
                {
                    lbl.Dispose();
                }

                if (btnA != null)
                {
                    btnA.Dispose();
                    btnA = null;
                }
                if (btnB != null)
                {
                    btnB.Dispose();
                    btnB = null;
                }
                if (btnABg != null)
                {
                    btnABg.Dispose();
                }
                if (btnBBg != null)
                {
                    btnBBg.Dispose();
                }
            }
        }
       


    }
}
