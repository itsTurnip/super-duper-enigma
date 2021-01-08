using System;

namespace ConsoleLib
{
    public delegate void KeyPressedEventHandler(KeyPressedEvent e);
    public class KeyPressedEvent
    {
        public object Sender { get; }
        public ConsoleKey Key { get; }
        internal KeyPressedEvent(object sender, ConsoleKey key) 
        {
            Key = key;
            Sender = sender;
        }
    }
}