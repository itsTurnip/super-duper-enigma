using System;
using ConsoleLib;
using DispatcherLib;

namespace Terminal
{
    internal class Program
    {
        static Dispatcher dispatcher;
        private static void Main(string[] args)
        {
            ConsoleMan man = new ConsoleMan();
            CheckLoad mmf = new CheckLoad();
            mmf.CheckLoadMMF();
            dispatcher = new Dispatcher();
            man.WindowList = dispatcher.GetList();
            man.ShowMessage("Bullshit happens");
            man.Keys.AddRange(new MenuKeyInfo[]
            {
                new MenuKeyInfo()
                { Key = "S", Description = "Settings"},
                new MenuKeyInfo()
                { Key = "R", Description = "Run file"},
                new MenuKeyInfo()
                { Key = "T", Description = "Report"},
                new MenuKeyInfo()
                { Key = "K", Description = "Kill" }
            });
            const int nameLength = 25;
            man.Columns.AddRange(new string[]
            {
                "PID  ", "Name".PadRight(nameLength), "CPU", "RAM"
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
            else if (e.Key == ConsoleKey.R)
            {
                FileRun.Run();
            }
            else if (e.Key == ConsoleKey.K)
            {
                if (dispatcher.Kill(man.SelectedIndex))
                {
                    man.ShowMessage("Success!", ConsoleColor.Green);
                    man.SelectedIndex = man.SelectedIndex; // пересчет на случай, если оказались в последней позиции и произошло удаление => отсутствие OutOfRangeException
                }
                else
                {
                    man.ShowMessage("Couldn't kill process. Maybe it's system process", ConsoleColor.Red);
                }
            }
            else if(e.Key == ConsoleKey.T)
            {
                Report report = new Report();
                report.DoReport(man.WindowList);
            }
        }
    }
}