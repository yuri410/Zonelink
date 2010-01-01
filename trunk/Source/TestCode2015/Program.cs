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
            City city = new City(UrbanSize.Town);
            
            Console.WriteLine("{0}   {1}", city.ProduceHPSpeed, city.ProduceLPSpeed);
            //-100,-100

            CityPluginFactory f=new CityPluginFactory();
            CityPlugin plugin = f.MakeEducationAgent();
            city.Add(plugin);
            Console.WriteLine("{0}   {1}", city.ProduceHPSpeed, city.ProduceLPSpeed);
            //-110,-110

            city.Remove(plugin);
            Console.WriteLine("{0}   {1}", city.ProduceHPSpeed, city.ProduceLPSpeed);
            //-100,-100

            CityPlugin  plugin1 = f.MakeOilRefinary();
            city.Add(plugin1);
            Console.WriteLine("{0}   {1}", city.ProduceHPSpeed, city.ProduceLPSpeed);
            //0,-100
                Console.ReadLine();
        }
       
    }


}
