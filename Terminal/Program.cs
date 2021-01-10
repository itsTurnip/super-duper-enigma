using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terminal
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ConsoleMan man = new ConsoleMan();
            MMF mmf = new MMF();
            foreach (string el in "I like this shit but it's really hard I like this shit but it's really hard I like this shit but it's really hard I like this shit but it's really hard I like this shit but it's really hard I like this shit but it's really hard I like this shit but it's really hard I like this shit but it's really hard I like this shit but it's really hard I like this shit but it's really hard".Split())
                man.WindowList.Add(el);
            mmf.CheckMMF();
            man.Loop();
        }
    }
}