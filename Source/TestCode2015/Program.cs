using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Apoc3D.Collections;
using Code2015.BalanceSystem;
using Apoc3D;


namespace TestCode2015
{
    class Program
    {
        static void Main(string[] args)
        {
            TestFeildProperty.UseField();
            TestFeildProperty.UseProperty();

            Forest forest = new Forest();
            float amount = 0;
            FastList<AnimalSpecies> animals = forest.InitAnimals();
            for (int i = 0; i < animals.Count; i++)
            {
                amount += animals[i].Amount;
            }
            Console.WriteLine(amount);
            Console.WriteLine(forest.GetTypeofSource());


            OilField oil = new OilField();
            Console.WriteLine(oil.ConsumeSpeed);
            Console.WriteLine(oil.NatureSourceType);

           // Console.WriteLine(oil.OilFieldType);

            Console.WriteLine(oil.SourceAmount);

            CityPluginFactory factory = new CityPluginFactory();
           CityPlugin plugin= factory.MakeCollege();
            Console.WriteLine(plugin.CostMoney);


            City city = new City();
            FastList< CityPlugin> plugins= city.ChoosedCityPlugin();
            city.NotifyAdded(city);
            city.Out();
            city.NotifyRemoved(city);
            GameTime time = new GameTime();
           

           city.Out();
                Console.ReadLine();
        }
       
    }


}
