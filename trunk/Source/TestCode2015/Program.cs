using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;


namespace TestCode2015
{
    class Program
    {
        static void Main(string[] args)
        {
            TestFeildProperty.UseField();
            TestFeildProperty.UseProperty();
            test t = new test();
            ArrayList list = new ArrayList();
            list.Add("no");
            list.Add("ok");
            Console.WriteLine(list.Capacity);
            int i=t.Kind(list);
            Console.WriteLine(i);
            Console.ReadLine();
        }
       
    }


}
