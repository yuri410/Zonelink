using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace EffectLibrary
{

    class TimeStamp
    {
        class Comparer : IEqualityComparer<string>
        {

            #region IEqualityComparer<string> 成员

            public bool Equals(string x, string y)
            {
                return string.Compare(x, y, StringComparison.OrdinalIgnoreCase) == 0;
            }

            public int GetHashCode(string obj)
            {
                return obj.ToLowerInvariant().GetHashCode();
            }

            #endregion
        }
        Dictionary<string, DateTime> times = new Dictionary<string, DateTime>(new Comparer());

        public TimeStamp()
        {
            if (File.Exists("ts.dat"))
            {
                BinaryReader br = new BinaryReader(File.Open("ts.dat", FileMode.Open), Encoding.UTF8);

                int count = br.ReadInt32();

                for (int i = 0; i < count; i++)
                {
                    string fn = br.ReadString().ToLowerInvariant();
                    times.Add(fn, DateTime.FromBinary(br.ReadInt64()));
                }
                br.Close();
            }
        }

        public bool IsOutDated(string file, string target)
        {
            if (!File.Exists(target))
                return true;
            if (OutdateOverride)
                return true;

            FileInfo fi = new FileInfo(file);
            FileInfo ft = new FileInfo(target);
            
            return (fi.LastWriteTime > ft.LastWriteTime);
        }

        public bool IsHeaderOutDated(string file)
        {

            FileInfo fi = new FileInfo(file);
            DateTime time;
            if (times.TryGetValue(file, out time))
            {
                return fi.LastWriteTime > time;
            }
            return true;
        }
        public void Save(string src)
        {
            BinaryWriter bw = new BinaryWriter(File.Open("ts.dat", FileMode.OpenOrCreate), Encoding.UTF8);

            string[] files = Directory.GetFiles(src, "*.*", SearchOption.AllDirectories);
            bw.Write(files.Length);
            for (int i = 0; i < files.Length; i++)
            {
                bw.Write(files[i].ToLowerInvariant());
                bw.Write(new FileInfo(files[i]).LastWriteTime.ToBinary());
            }

            bw.Close();
        }

        public bool OutdateOverride { get; set; }
    }

    static class Program
    {
        static bool Compile(string start, string output, string aarg, bool isVS)
        {
            ProcessStartInfo info = new ProcessStartInfo("fxc.exe",
                "\"" + start + "\" /T " + (isVS ? "vs_3_0" : "ps_3_0") + " /E main /O2 /Zpr /WX /nologo /Fo \"" + output + "\"" + aarg);

            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.UseShellExecute = false;
            //info.StandardOutputEncoding = Encoding.Default;

            Process p = Process.Start(info);

            p.WaitForExit();

            string res = p.StandardOutput.ReadToEnd();
            Console.WriteLine(res);

            res = p.StandardError.ReadToEnd();
            if (!string.IsNullOrEmpty(res))
            {
                Console.Error.Write(res);
            }

            return p.ExitCode == 0;
        }
        static void Main(string[] args)
        {
            //string src = @"E:\Documents\ic10gd\Source\EffectLibrary\";// args[0];
            //string taget = @"E:\Desktop\ssssss";// args[1];
            string src = args[0];
            string taget = args[1];

            TimeStamp ts = new TimeStamp();

            string[] hfiles = Directory.GetFiles(src, "*.*sh", SearchOption.AllDirectories);

            bool passed = false;

            for (int i = 0; i < hfiles.Length; i++)
            {
                if (ts.IsHeaderOutDated(hfiles[i]))
                {
                    passed = true;
                    break;
                }
            }

            ts.OutdateOverride = passed;


            

            string[] files = Directory.GetFiles(src, "*.vs", SearchOption.AllDirectories);
            string[] dirs = Directory.GetDirectories(src, "*", SearchOption.AllDirectories);

            string argi;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < dirs.Length; i++)
            {
                sb.Append(" /I ");
                sb.Append(dirs[i]);
            }
            argi = sb.ToString();


            for (int i = 0; i < files.Length; i++)
            {
                string output = Path.Combine(taget, Path.GetFileNameWithoutExtension(files[i]) + ".cvs");

                if (ts.IsOutDated(files[i], output))
                {

                    
                    if (!Compile(files[i], output, argi, true))
                    {
                        Environment.ExitCode = 1;
                        return;
                    }
                }
            }

            files = Directory.GetFiles(src, "*.ps", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++)
            {
                string output = Path.Combine(taget, Path.GetFileNameWithoutExtension(files[i]) + ".cps");

                if (ts.IsOutDated(files[i], output))
                {

                    if (!Compile(files[i], output, argi, false))
                    {
                        Environment.ExitCode = 1;
                        return;
                    }
                }
            }

            ts.Save(src);

        }

    }
}
