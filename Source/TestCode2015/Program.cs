using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Config;
using Code2015.BalanceSystem;
using System.Threading;


namespace TestCode2015
{
    class CityTester
    {
        public static void Test()
        {
            Player player = new Player();
            SimulateRegion region = new SimulateRegion(player, null);

            City city = new City(region.EnergyStatus);

            IniSection sect = new IniSection("");
            sect.Add("Name", "HUST");
            sect.Add("Longitude", "0");
            sect.Add("Latitude", "0");
            sect.Add("Population", "10");
            sect.Add("Size", UrbanSize.Small.ToString());

            city.Parse(sect);

            while (true)
            {
                GameTime gt = new GameTime(0, 0, TimeSpan.FromHours(1), TimeSpan.FromHours(1));

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
                Console.Write(" Local HP   ");
                Console.WriteLine(city.LocalHP);
                Console.Write(" Local LP   ");
                Console.WriteLine(city.LocalLP);
                Console.Write(" Local Food   ");
                Console.WriteLine(city.LocalFood);

                Console.WriteLine("-------------------------------------------------------");


               

                Thread.Sleep(100);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            GameTime time = new GameTime(100, 1, new TimeSpan(10, 10, 10, 10), new TimeSpan(1, 1, 1, 1));

            Console.ReadLine();
        }

    }
}
