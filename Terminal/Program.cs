using System;
using ConsoleLib;

namespace Terminal
{
    class Program
    {
        static void Main(string[] args)
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
