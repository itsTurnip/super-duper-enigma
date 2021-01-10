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

        int visibleTop = 0;
        int visibleBottom = 0;
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
        public ConsoleColor SelectionBackground { get => selectionBackground; set => selectionBackground = value; }
        ConsoleColor selectionBackground = ConsoleColor.Gray;
        public ConsoleColor SelectionForeground { get => selectionForeground; set => selectionForeground = value; }
        ConsoleColor selectionForeground = ConsoleColor.Black;
        
        public ConsoleColor DefaultBackground { get => defaultBackground; set => defaultBackground = value; }
        ConsoleColor defaultBackground = ConsoleColor.Black;
        
        public ConsoleColor DefaultForeground { get => defaultForeground; set => defaultForeground = value; }

        ConsoleColor defaultForeground = ConsoleColor.Gray;

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
        void WriteLine(object obj, int line = -1, object[] arg = null)
        {
            WriteLine(obj, defaultForeground, defaultBackground, line, arg);
        }
        void WriteLine(object obj, ConsoleColor foregroundColor, ConsoleColor backgroundColor, int line = -1, object[] arg = null)
        {
            if(line >= 0)
                Console.SetCursorPosition(0, line);
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
            int bottomBorder = height - 1;
            int i = 0;

            int visibleArea = visibleBottom - visibleTop;
            if (visibleArea != bottomBorder - 1)
                visibleBottom = visibleTop + bottomBorder - 1;

            if (selectedIndex >= 0)
            {
                if (selectedIndex < visibleTop)
                {
                    int diff = visibleTop - selectedIndex;
                    visibleTop -= diff;
                    visibleBottom -= diff;
                } else if (selectedIndex > visibleBottom)
                {
                    int diff = selectedIndex - visibleBottom;
                    visibleTop += diff;
                    visibleBottom += diff;
                }
            }

            for (int index = i + visibleTop; index < visibleBottom && i < WindowList.Count; i++)
            {
                index = i + visibleTop;
                if (index == selectedIndex)
                    WriteLine(WindowList[index], selectionForeground, selectionBackground, i);
                else
                    WriteLine(WindowList[index], i);
                
            }
            while (i < bottomBorder)
            {
                WriteLine("", i);
                i++;
            }
            Console.SetCursorPosition(0, i);
            Write(new string(' ', width - 1));
            Console.SetCursorPosition(0, bottomBorder);
            Write("Esc");
            Write("Exit", selectionForeground, selectionBackground);
            Write("\tUp");
            Write("Previous", selectionForeground, selectionBackground);
            Write("\tDown");
            Write("Next", selectionForeground, selectionBackground);
            Write("\tBot");
            Write($"{visibleBottom}", selectionForeground, selectionBackground);
            Write("\tTop");
            Write($"{visibleTop}", selectionForeground, selectionBackground);
            Write("\tSel");
            Write($"{selectedIndex}", selectionForeground, selectionBackground);
        }
        public void Loop()
        {
            visibleBottom = Console.WindowHeight - 2;
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
                //Thread.Sleep(10);
            }
        }
        ~ConsoleMan()
        {
            FreeConsole();
        }
    }
}
