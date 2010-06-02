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
http://www.gnu.org/copyleft/lesser.txt.

-----------------------------------------------------------------------------
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace Apoc3D.GUI.Controls
{

    public class TextBox : TextControl
    {
        //int currentPosition;

        

        public TextBox()
        {
            IsInputControl = true;
            Text = string.Empty;
        }

        //void ProcessKey(Key k)
        //{

        //    switch (k)
        //    {
        //        case Key.Backspace:
        //            if (currentPosition > 0)
        //            {
        //                Text.Remove(currentPosition - 1, 1);
        //            }
        //            break;
        //        case Key.Delete:
        //            if (currentPosition < Text.Length - 1)
        //            {
        //                Text.Remove(Text.Length - 1, 1);
        //            }
        //            break;
        //        case Key.Home:

        //            break;
        //        case Key.End:

        //            break;
        //        case Key.LeftArrow:
        //            if (currentPosition > 0)
        //                currentPosition--;
        //            break;
        //        case Key.RightArrow:
        //            if (currentPosition < Text.Length - 1)
        //                currentPosition++;
        //            break;
        //        default:

        //            break;
        //    }
        //}


        //protected override void OnKeyPressed(System.Windows.Forms.KeyPressEventArgs e)
        //{
        //    base.OnKeyPressed(e);

            
        //}
    }
}
