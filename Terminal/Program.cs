using System;
using ConsoleLib;

namespace Terminal
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ConsoleMan man = new ConsoleMan();
            man.KeyPressed += Man_KeyPressed;
            foreach (string el in "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque feugiat ut metus eu feugiat. Suspendisse non tellus in risus posuere dapibus. Suspendisse volutpat in quam non lobortis. Aliquam elementum purus erat, vitae viverra ex semper a. Phasellus vel ullamcorper odio. Donec molestie est non pharetra gravida. Cras iaculis, mauris quis iaculis volutpat, turpis dui sagittis neque, sed hendrerit libero arcu non mauris. Nullam at consectetur tortor. Vivamus dictum dapibus ex, nec posuere orci. Sed consectetur fermentum mollis. Suspendisse commodo nisl eu leo egestas, rhoncus finibus magna sagittis.".Split())
                man.WindowList.Add(el);
            MMF mmf = new MMF();
            mmf.CheckMMF();
            man.ShowMessage("Bullshit happens");
            man.Keys.AddRange(new MenuKeyInfo[] { new MenuKeyInfo() 
            { 
                Key = "P",
                Description = "Setting"
            }});
            man.Loop();
        }

        private static void Man_KeyPressed(KeyPressedEvent e)
        {
            if (e.Key == ConsoleKey.P)
            {
                ConsoleMan man = (ConsoleMan)e.Sender;
                PropertiesConsole properties = new PropertiesConsole();
                if (properties.form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    man.DefaultForeground = properties.DefaultForegroundProperties;
                    man.DefaultBackground = properties.DefaultBackgroundProperties;
                }
            }
        }
    }
}