using ConsoleLib;
using DispatcherLib;

namespace Terminal
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ConsoleMan man = new ConsoleMan();
            Dispatcher dispatcher = new Dispatcher();
            man.WindowList = dispatcher.GetList();
            CheckLoad mmf = new CheckLoad();
            mmf.CheckLoadMMF();
            man.ShowMessage("Bullshit happens");
            man.Keys.AddRange(new MenuKeyInfo[]
            {
                new MenuKeyInfo()
                { Key = "S", Description = "Settings"},
                new MenuKeyInfo()
                { Key = "R", Description = "Run file"},
                new MenuKeyInfo()
                { Key = "T", Description = "Report"}
            });
            man.KeyPressed += Man_KeyPressed;
			man.Loop();
        }

        private static void Man_KeyPressed(KeyPressedEvent e)
        {
            if (e.Key == ConsoleKey.S)
            {
                ConsoleMan man = (ConsoleMan)e.Sender;
                PropertiesConsole properties = new PropertiesConsole();
                if (properties.form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    man.DefaultForeground = properties.DefaultForegroundProperties;
                    man.DefaultBackground = properties.DefaultBackgroundProperties;
                }
            }
            if (e.Key == ConsoleKey.R)
            {
                FileRun.Run();
            }
        }
    }
}