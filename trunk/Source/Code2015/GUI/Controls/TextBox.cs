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
