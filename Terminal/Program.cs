using ConsoleLib;
using DispatcherLib;

namespace Terminal
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleMan man = new ConsoleMan();
            Dispatcher dispatcher = new Dispatcher();
            man.WindowList = dispatcher.GetList();
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
    }
}
