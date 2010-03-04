using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Config;
using Code2015;
using Code2015.BalanceSystem;
using Code2015.Logic;


namespace TestCode2015
{
    class CityTester
    {
        public static void Test()
        {
            Player player = new Player("");
            SimulationRegion region = new SimulationRegion();

            City city = new City(region);
            City city2 = new City(region);

            IniSection sect = new IniSection("");
            sect.Add("Name", "HUST");
            sect.Add("Longitude", "0");
            sect.Add("Latitude", "0");
            sect.Add("Population", "150");
            sect.Add("Size", UrbanSize.Large.ToString());

            city.Parse(sect);



            sect = new IniSection("");
            sect.Add("Name", "Home");
            sect.Add("Longitude", "1");
            sect.Add("Latitude", "0");
            sect.Add("Population", "1");
            sect.Add("Size", UrbanSize.Small.ToString());

            city2.Parse(sect);

            region.Add(city);
            //region.Add(city2);

            //city.AddNearbyCity(city2);
            //city2.AddNearbyCity(city);



            OilField oilFld = new OilField(region);
            sect = new IniSection("");
            sect.Add("Amount", "100000");
            sect.Add("Latitude", "1");
            sect.Add("Longitude", "0");
            oilFld.Parse(sect);
            region.Add(oilFld);


            CityPluginType plgType = new CityPluginType("test plg");
            sect = new IniSection("");
            sect.Add("Cost", "100");
            sect.Add("HRCSpeed", "100");
            sect.Add("Behaviour", CityPluginBehaviourType.CollectorFactory.ToString());
            plgType.Parse(sect);

            CityPlugin plg = new CityPlugin(plgType);
            city.Add(plg);



            plgType = new CityPluginType("test plg");
            sect = new IniSection("");
            sect.Add("Cost", "100");
            sect.Add("FoodConvRate", "0.50");
            sect.Add("FoodCostSpeed", "100");
            sect.Add("Behaviour", CityPluginBehaviourType.CollectorFactory.ToString());
            plgType.Parse(sect);
            plg = new CityPlugin(plgType);
            city.Add(plg);



            //OilField oil = new OilField(region);
            //Forest forest = new Forest(region);
            float c = 0;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Energy Status:");
                Console.Write(" Current Food Storage   ");
                Console.WriteLine(region.EnergyStatus.CurrentFood);
                Console.Write(" Current HR Storage   ");
                Console.WriteLine(region.EnergyStatus.CurrentHR);
                Console.Write(" Current LR Storage   ");
                Console.WriteLine(region.EnergyStatus.CurrentLR);
                Console.Write(" Current Natural HR   ");
                Console.WriteLine(region.EnergyStatus.RemainingHR);
                Console.Write(" Current Natural LR   ");
                Console.WriteLine(region.EnergyStatus.RemainingLR);



                Console.WriteLine("City :" + city.Name);
                Console.WriteLine("City Status:");
                Console.Write(" Development   ");
                Console.WriteLine(city.Development);
                Console.Write(" Population   ");
                Console.WriteLine(city.Population);
                Console.Write(" Disease   ");
                Console.WriteLine(city.Disease);
                Console.Write(" Size   ");
                Console.WriteLine(city.Size);

                Console.WriteLine(" Local  H[{0}] L[{1}] F[{2}]", city.LocalHR.Current, city.LocalLR.Current, city.LocalFood.Current);
                Console.WriteLine(" Drain  H[{0}] L[{1}] F[{2}]", city.GetSelfHRCSpeed(), city.GetSelfLRCSpeed(), city.GetSelfFoodCostSpeed());
                Console.WriteLine(" Ratio  H[{0:P}] L[{1:P}] F[{2:P}]", city.SelfHRCRatio, city.SelfLRCRatio, city.SelfFoodCostRatio);

                c += city.GetCarbonChange();

                //city.LocalFood.Commit(250);
                //city.LocalHR.Commit(250);
                city.LocalLR.Commit(100);

                //Console.WriteLine("City :" + city2.Name);
                //Console.WriteLine("City Status:");
                //Console.Write(" Development   ");
                //Console.WriteLine(city2.Development);
                //Console.Write(" Population   ");
                //Console.WriteLine(city2.Population);
                //Console.Write(" Disease   ");
                //Console.WriteLine(city2.Disease);
                //Console.Write(" Size   ");
                //Console.WriteLine(city2.Size);

                //Console.WriteLine(" Local  H[{0}] L[{1}] F[{2}]", city2.LocalHR.Current, city2.LocalLR.Current, city2.LocalFood.Current);
                //Console.WriteLine(" Drain  H[{0}] L[{1}] F[{2}]", city2.GetSelfHRCSpeed(), city2.GetSelfLRCSpeed(), city2.GetSelfFoodCostSpeed());
                //Console.WriteLine(" Ratio  H[{0:P}] L[{1:P}] F[{2:P}]", city2.SelfHRCRatio, city2.SelfLRCRatio, city2.SelfFoodCostRatio);


                //c += city2.GetCarbonChange();


                Console.WriteLine("World Carbon {0}", c);

                GameTime gt = new GameTime(0, 0, TimeSpan.FromHours(1), TimeSpan.FromHours(1));
                region.Update(gt);

                Thread.Sleep(5);
            }
        }
    }
    class XmlTester
    {
        public static void Test()
        {
            //ConfigurationManager.Initialize();
            //ConfigurationManager.Instance.Register(new XmlConfigurationFormat());

            //Configuration conf = ConfigurationManager.Instance.CreateInstance(@"E:\Documents\ic10gd\Source\Code2015\Configs\cities.xml");

            //Console.WriteLine(conf.Count);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadLine();

            CityTester.Test();
            Console.ReadLine();
        }

    }
}
