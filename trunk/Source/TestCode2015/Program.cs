﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Apoc3D;
using Apoc3D.Collections;
using Apoc3D.Config;
using Code2015.BalanceSystem;


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

            region.Add(city);

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

                Console.WriteLine(" Local  H[{0}] L[{1}] F[{2}]", city.LocalHP, city.LocalLP, city.LocalFood);
                Console.WriteLine(" Drain  H[{0}] L[{1}] F[{2}]", city.ProduceHPSpeed, city.ProduceLPSpeed, city.FoodCostSpeed);

                c += city.GetCarbonChange();

                Console.WriteLine("World Carbon {0}", c);

                GameTime gt = new GameTime(0, 0, TimeSpan.FromHours(1), TimeSpan.FromHours(1));
                region.Update(gt);

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

            CityTester.Test();
            Console.ReadLine();
        }

    }
}
