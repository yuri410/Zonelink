using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace meshconv
{
    class Program
    {
        static void Main(string[] args)
        {
            Converter.Initialize();

            string line;
            do
            {
                line = Console.ReadLine();

                if (line == "exit")
                {
                    break;
                }

                if (File.Exists(line))
                {
                    string dest = Path.Combine(Path.GetDirectoryName(line), Path.GetFileNameWithoutExtension(line) + ".mesh");
                    Converter.Convert(line, dest);
                }
                else
                {
                    Console.WriteLine("not exist");
                }
            }
            while (true);
        }
    }
}
