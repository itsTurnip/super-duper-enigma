using System;
using ConsoleLib;

namespace Terminal
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleMan man = new ConsoleMan();
            foreach (string el in "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque feugiat ut metus eu feugiat. Suspendisse non tellus in risus posuere dapibus. Suspendisse volutpat in quam non lobortis. Aliquam elementum purus erat, vitae viverra ex semper a. Phasellus vel ullamcorper odio. Donec molestie est non pharetra gravida. Cras iaculis, mauris quis iaculis volutpat, turpis dui sagittis neque, sed hendrerit libero arcu non mauris. Nullam at consectetur tortor. Vivamus dictum dapibus ex, nec posuere orci. Sed consectetur fermentum mollis. Suspendisse commodo nisl eu leo egestas, rhoncus finibus magna sagittis.".Split())
                man.WindowList.Add(el);
            MMF.CheckMMF();
            man.ShowMessage("Bullshit happens");
            man.Loop();
        }
    }
}
