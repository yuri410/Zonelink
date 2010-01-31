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


namespace TestCode2015
{
    class CityTester
    {
        public static void Test()
        {
            Player player = new Player("");
            SimulateRegion region = new SimulateRegion(player, null);

            City city = new City(region.EnergyStatus);

            IniSection sect = new IniSection("");
            sect.Add("Name", "HUST");
            sect.Add("Longitude", "0");
            sect.Add("Latitude", "0");
            sect.Add("Population", "1000");
            sect.Add("Size", UrbanSize.Large.ToString());

            city.Parse(sect);

            region.Add(city);

            //OilField oil = new OilField(region);
            //Forest forest = new Forest(region);
            float c = 0;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Energy Status:");
                Console.Write(" Current Food Storage   ");
                Console.WriteLine(region.EnergyStatus.CurrentFood);
                Console.Write(" Current HP Storage   ");
                Console.WriteLine(region.EnergyStatus.CurrentHPEnergy);
                Console.Write(" Current LP Storage   ");
                Console.WriteLine(region.EnergyStatus.CurrentLPEnergy);

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

                Console.WriteLine("World Carbon {0}", c);

                GameTime gt = new GameTime(0, 0, TimeSpan.FromHours(1), TimeSpan.FromHours(1));
                region.Update(gt);

                Thread.Sleep(100);
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

            XmlTester.Test();
            Console.ReadLine();
        }

    }
}
