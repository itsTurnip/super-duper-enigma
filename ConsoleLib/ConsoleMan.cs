using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Threading;

namespace ConsoleLib
{
    internal enum StdHandle
    {
        Error = -12,
        Output,
        Input
    }
    public class ConsoleMan
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool FreeConsole();
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AttachConsole(int id);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(StdHandle std);
        public ObservableCollection<object> WindowList { get; }
        public event KeyPressedEventHandler KeyPressed;
        int selectedIndex = -1;
        bool stop = false;
        public object SelectedItem { 
            get
            {
                if(selectedIndex >= 0)
                    return WindowList[selectedIndex];
                return null;
            }
        }
        public ConsoleMan()
        {
            WindowList = new ObservableCollection<object>();
            if (!AllocConsole())
                throw new Exception();
            IntPtr input = GetStdHandle(StdHandle.Input);
            IntPtr output = GetStdHandle(StdHandle.Output);
            Console.SetIn(new StreamReader(new FileStream(new SafeFileHandle(input, true), FileAccess.Read)));
            Console.SetOut(new StreamWriter(new FileStream(new SafeFileHandle(output, true), FileAccess.Write)) { AutoFlush = true });
            Console.CursorVisible = false;
        }
        void WriteLine(object obj, object[] arg = null)
        {
            WriteLine(obj, ConsoleColor.Gray, ConsoleColor.Black, arg);
        }
        void WriteLine(object obj, ConsoleColor foregroundColor, ConsoleColor backgroundColor, object[] arg = null)
        {
            int width = Console.WindowWidth;
            int len = obj.ToString().Length;
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            if (arg != null)
                Console.Write(obj.ToString() + new string(' ', width - len), arg);
            else
                Console.Write(obj.ToString() + new string(' ', width - len));
        }
        void Write(object obj, object[] arg = null)
        {
            Write(obj, ConsoleColor.Gray, ConsoleColor.Black, arg);
        }
        void Write(object obj, ConsoleColor foregroundColor, ConsoleColor backgroundColor, object[] arg = null)
        {
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            if (arg != null)
                Console.Write(obj.ToString(), arg);
            else
                Console.Write(obj);
        }
        void Choose(bool next)
        {
            if (next)
            {
                selectedIndex += 1;
                if (selectedIndex >= WindowList.Count)
                    selectedIndex = 0;
            } else
            {
                selectedIndex -= 1;
                if (selectedIndex < 0)
                    selectedIndex = WindowList.Count - 1;
            }
        }
        public void Render()
        {
            Console.CursorVisible = false;
            int height = Console.WindowHeight;
            int width = Console.WindowWidth;
            // int topBorder = 0;
            int bottomBorder = height - 1;
            Console.SetCursorPosition(0, 0);
            int i = 0;
            while (i < bottomBorder && i < WindowList.Count)
            {
                Console.SetCursorPosition(0, i);
                if (i == selectedIndex)
                    WriteLine(WindowList[i], ConsoleColor.Black, ConsoleColor.Gray);
                else
                    WriteLine(WindowList[i]);
                i++;
            }
            while (i < bottomBorder)
            {
                Console.SetCursorPosition(0, i);
                WriteLine("");
                i++;
            }
            Console.SetCursorPosition(0, i);
            Write(new string(' ', width - 1));
            Console.SetCursorPosition(0, bottomBorder);
            Write("Esc");
            Write("Exit", ConsoleColor.Black, ConsoleColor.Gray);
            Write("\tUp");
            Write("Previous", ConsoleColor.Black, ConsoleColor.Gray);
            Write("\tDown");
            Write("Next", ConsoleColor.Black, ConsoleColor.Gray);
        }
        public void Loop()
        {
            while(!stop)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo cki = Console.ReadKey(true);
                    switch(cki.Key)
                    {
                        case ConsoleKey.Escape:
                            stop = true;
                            break;
                        case ConsoleKey.DownArrow:
                            Choose(true);
                            break;
                        case ConsoleKey.UpArrow:
                            Choose(false);
                            break;
                    }
                    KeyPressed?.Invoke(new KeyPressedEvent(this, cki.Key));
                }
                Render();
                Thread.Sleep(10);
            }
        }
        ~ConsoleMan()
        {
            FreeConsole();
        }
    }
}
