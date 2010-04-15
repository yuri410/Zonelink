using System;
using System.Collections.Generic;
using System.Text;

namespace MapEdit
{

    enum ObjectType
    {
        City,
        ResWood,
        ResOil,
        Sound,
        Scene
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
