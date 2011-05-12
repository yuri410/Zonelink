using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;


namespace TestCode2015
{
    class TestFeildProperty
    {
        static readonly int _RunTime = 10000000;
        public static void UseField()
        {
            DateTime begin = DateTime.Now;
            int milliSecondBegin = DateTime.Now.Millisecond;
            for (int i = 0; i < _RunTime; i++)
            {
                _TestField = 1;
            }
            DateTime end = DateTime.Now;
            TimeSpan result = end - begin;
            Console.WriteLine(result.ToString());
        }
        public static void UseProperty()
        {
            DateTime begin = DateTime.Now;
            int milliSecondBegin = DateTime.Now.Millisecond;
            for (int i = 0; i < _RunTime; i++)
            {
                TestField = 1;
            }
            DateTime end = DateTime.Now;
            TimeSpan result = end - begin;
            Console.WriteLine(result.ToString());
        }

        static int _TestField;
        static int TestField
        {
            set { _TestField = value; }
            get { return _TestField; }
        }
        

    }

   
}
