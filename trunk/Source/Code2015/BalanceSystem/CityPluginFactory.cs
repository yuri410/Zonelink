using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Code2015.BalanceSystem
{
    public abstract class CityPluginFactory
    {
        public CityPlugin BookCityPlugin(string type)
        {
            CityPlugin cityplugin;
            cityplugin = CreateCityPlugin(type);
            return cityplugin;
        }

        public abstract CityPlugin CreateCityPlugin(string typr);   

    }
}
