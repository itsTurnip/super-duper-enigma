using System;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Collections.Generic;
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
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AttachConsole(int id);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(StdHandle std);

        private int visibleTop = 0;
        private int visibleBottom = 0;
        public IList<object> WindowList { get; set; } = new List<object>();

        public event KeyPressedEventHandler KeyPressed;

        private int selectedIndex = -1;
        private bool stop = false;
        public bool delete = false;
        public string Title { get; set; } = "Terminal";
        private string message;
        private ConsoleColor messageColor = ConsoleColor.DarkRed;

        public object SelectedItem
        {
            get
            {
                if (selectedIndex >= 0)
                    return WindowList[selectedIndex];
                return null;
            }
        }

        public ConsoleColor SelectionBackground { get; set; } = ConsoleColor.Gray;
        public ConsoleColor SelectionForeground { get; set; } = ConsoleColor.Black;

        public ConsoleColor DefaultBackground { get; set; } = ConsoleColor.Black;

        public ConsoleColor DefaultForeground { get; set; } = ConsoleColor.Gray;
        public List<MenuKeyInfo> Keys { get; } = new List<MenuKeyInfo>();
        public List<string> Columns { get; } = new List<string>();
        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                selectedIndex = value;
                if (selectedIndex >= WindowList.Count)
                    selectedIndex = 0;
                if (selectedIndex < 0)
                    selectedIndex = WindowList.Count - 1;
            }
        }

        public ConsoleMan()
        {
            if (!AllocConsole())
                throw new Exception();
            IntPtr input = GetStdHandle(StdHandle.Input);
            IntPtr output = GetStdHandle(StdHandle.Output);
            Console.SetIn(new StreamReader(new FileStream(new SafeFileHandle(input, true), FileAccess.Read)));
            Console.SetOut(new StreamWriter(new FileStream(new SafeFileHandle(output, true), FileAccess.Write)) { AutoFlush = true });
        }

        private void WriteLine(object obj, int line = -1, object[] arg = null)
        {
            WriteLine(obj, DefaultForeground, DefaultBackground, line, arg);
        }

        private void WriteLine(object obj, ConsoleColor foregroundColor, ConsoleColor backgroundColor, int line = -1, object[] arg = null)
        {
            if (line >= 0)
                Console.SetCursorPosition(0, line);
            int cursor = Console.CursorLeft;
            int width = Console.WindowWidth;
            int len = obj.ToString().Length;
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            int spaces = width - cursor - len;
            if (spaces > 0)
                if (arg != null)
                    Console.Write(obj.ToString() + new string(' ', width - cursor - len), arg);
                else
                    Console.Write(obj.ToString() + new string(' ', width - cursor - len));
        }

        private void Write(object obj, object[] arg = null)
        {
            Write(obj, DefaultForeground, DefaultBackground, arg);
        }

        private void Write(object obj, ConsoleColor foregroundColor, ConsoleColor backgroundColor, object[] arg = null)
        {
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            if (arg != null)
                Console.Write(obj.ToString(), arg);
            else
                Console.Write(obj);
        }

        public void ShowMessage(string message)
        {
            this.message = message;
        }

        public void ShowMessage(string message, ConsoleColor color)
        {
            messageColor = color;
            ShowMessage(message);
        }

        private const int headerHeight = 3;

        private void renderHead()
        {
            Console.SetCursorPosition(0, 0);
            Write($"{Title}    ", SelectionForeground, SelectionBackground);
            if (message != null)
                WriteLine(message, messageColor, SelectionBackground);
            else
                WriteLine("", messageColor, SelectionBackground);
            foreach(var column in Columns)
                Write($"{column}\t");
            WriteLine("");
            WriteLine("", 2);
        }

        private const int footerHeight = headerHeight;

        private void renderFooter()
        {
            const string s = "    ";
            int bottomline = Console.WindowHeight - 1;
            Console.SetCursorPosition(0, bottomline);
            Write("Esc");
            Write("Exit", SelectionForeground, SelectionBackground);
            Write(s + "Up");
            Write("Previous", SelectionForeground, SelectionBackground);
            Write(s + "Down");
            Write("Next", SelectionForeground, SelectionBackground);
            Write(s + "Bot");
            Write($"{visibleBottom}", SelectionForeground, SelectionBackground);
            Write(s + "Top");
            Write($"{visibleTop}", SelectionForeground, SelectionBackground);
            Write(s + "Sel");
            Write($"{selectedIndex}", SelectionForeground, SelectionBackground);
            foreach (MenuKeyInfo keyInfo in Keys)
            {
                Write(s + $"{keyInfo.Key}");
                Write($"{keyInfo.Description}", SelectionForeground, SelectionBackground);
            }
            var pos = Console.CursorLeft;
            var width = Console.WindowWidth;
            if (width > pos)
                Write(new string(' ', width - pos - 1));
        }

        public void renderDelFooter(dynamic process)
        {
            
            const string s = "    ";
            int bottomline = Console.WindowHeight - 1;
            Console.SetCursorPosition(0, bottomline);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, bottomline);
            Write($"Kill {process.Name}?\t");
            Write("Y");
            Write("Yes", SelectionForeground, SelectionBackground);
            Write(s + "N");
            Write("No", SelectionForeground, SelectionBackground);
        }

        private void renderList()
        {
            int height = Console.WindowHeight - 1;
            int visibleArea = visibleBottom - visibleTop;
            
            if (visibleArea != height - footerHeight - 1)
            {
                visibleBottom = visibleTop + height - footerHeight - 1;
                Console.WriteLine("", visibleBottom);
            }
            
            if (selectedIndex >= 0)
            {
                if (selectedIndex < visibleTop)
                {
                    int diff = visibleTop - selectedIndex;
                    visibleTop -= diff;
                    visibleBottom -= diff;
                }
                else if (selectedIndex >= visibleBottom)
                {
                    int diff = selectedIndex - visibleBottom + 1;
                    visibleTop += diff;
                    visibleBottom += diff;
                }
            }
            if (visibleArea <= 0)
                return;
            int line = headerHeight;
            int index = visibleTop;
            while (index < visibleBottom && index < WindowList.Count)
            {
                if (index == selectedIndex)
                    WriteLine(WindowList[index], SelectionForeground, SelectionBackground, line);
                else
                    WriteLine(WindowList[index], line);
                line++;
                index++;
            }
            while (line < height)
            {
                WriteLine("", line);
                line++;
            }
        }

        private void renderFull()
        {
            Console.CursorVisible = false;
            if (Console.WindowHeight - footerHeight <= 0)
                return;
            renderHead();
            renderList();
            if(!delete)
                renderFooter();
        }

        public void Loop()
        {
            visibleBottom = Console.WindowHeight - footerHeight - 1;
            while (!stop)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo cki = Console.ReadKey(true);
                    if (!delete)
                    {
                        switch (cki.Key)
                        {
                            case ConsoleKey.Escape:
                                stop = true;
                                break;

                            case ConsoleKey.DownArrow:
                                SelectedIndex += 1;
                                break;

                            case ConsoleKey.UpArrow:
                                SelectedIndex -= 1;
                                break;
                        }
                        
                    }
                    KeyPressed?.Invoke(new KeyPressedEvent(this, cki.Key));
                }
                renderFull();
                //Thread.Sleep(10);
            }
        }

        ~ConsoleMan()
        {
            FreeConsole();
        }
    }
}