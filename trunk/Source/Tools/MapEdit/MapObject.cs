using System;
using System.Collections.Generic;
using System.Text;

namespace MapEdit
{

    enum ObjectType
    {
    }


    class MapObject
    {
        public float Longitude
        {
            get;
            set;
        }

        public float Latitude
        {
            get;
            set;
        }


        public ObjectType Type
        {
            get;
            set;
        }


    }
}
