using System;
using ConsoleLib;
using DispatcherLib;

namespace Terminal
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ConsoleMan man = new ConsoleMan();
            CheckLoad mmf = new CheckLoad();
            mmf.CheckLoadMMF();
            Dispatcher dispatcher = new Dispatcher();
            man.WindowList = dispatcher.GetList();
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
            ConsoleMan man = (ConsoleMan)e.Sender;
            if (e.Key == ConsoleKey.S)
            {
                
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
            if (e.Key == ConsoleKey.T)
            {
                Report report = new Report();
                report.DoReport(man.WindowList);
            }
        }
    }
}