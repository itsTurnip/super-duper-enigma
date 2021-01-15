using System;
using System.Runtime.InteropServices;
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
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AttachConsole(int id);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(StdHandle std);

        private int visibleTop = 0;
        private int visibleBottom = 0;
        public ObservableCollection<object> WindowList { get; }

        public event KeyPressedEventHandler KeyPressed;

        private int selectedIndex = -1;
        private bool stop = false;
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

        private void Choose(bool next)
        {
            if (next)
            {
                selectedIndex += 1;
                if (selectedIndex >= WindowList.Count)
                    selectedIndex = 0;
            }
            else
            {
                selectedIndex -= 1;
                if (selectedIndex < 0)
                    selectedIndex = WindowList.Count - 1;
            }
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

        private const int headerHeight = 2;

        private void renderHead()
        {
            Console.SetCursorPosition(0, 0);
            Write($"{Title}\t", SelectionForeground, SelectionBackground);
            if (message != null)
                WriteLine(message, messageColor, SelectionBackground);
            else
                WriteLine("", messageColor, SelectionBackground);
            WriteLine("");
        }

        private const int footerHeight = 2;

        private void renderFooter()
        {
            int bottomline = Console.WindowHeight - 1;
            Console.SetCursorPosition(0, bottomline);
            Write("Esc");
            Write("Exit", SelectionForeground, SelectionBackground);
            Write("\tUp");
            Write("Previous", SelectionForeground, SelectionBackground);
            Write("\tDown");
            Write("Next", SelectionForeground, SelectionBackground);
            Write("\tBot");
            Write($"{visibleBottom}", SelectionForeground, SelectionBackground);
            Write("\tTop");
            Write($"{visibleTop}", SelectionForeground, SelectionBackground);
            Write("\tSel");
            Write($"{selectedIndex}", SelectionForeground, SelectionBackground);
            var pos = Console.CursorLeft;
            var width = Console.WindowWidth;
            if (width > pos)
            {
                Write(new string(' ', width - pos - 1));
            }
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
            renderHead();
            renderList();
            renderFooter();
        }

        public void Loop()
        {
            visibleBottom = Console.WindowHeight - 3;
            while (!stop)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo cki = Console.ReadKey(true);
                    switch (cki.Key)
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