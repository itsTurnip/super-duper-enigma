using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;

namespace Terminal
{
    internal class MMF
    {
        public static void CheckMMF()
        {
            try
            {
                using (var mmCheckFile = MemoryMappedFile.OpenExisting("MMF.mmf"))
                {
                    using (var mmAccessor = mmCheckFile.CreateViewAccessor())
                    {
                        var readOut = new byte[12];
                        mmAccessor.ReadArray(0, readOut, 0, readOut.Length);
                        if (Encoding.ASCII.GetString(readOut) == "Hello world!")
                        {
                            Console.WriteLine("The perfect task manager is already up and running.\nEnjoy it!!!");
                            Tree();
                            Environment.Exit(0);
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                var mmFile = MemoryMappedFile.CreateNew(@"MMF.mmf", 12);
                using (var mmfAccessor = mmFile.CreateViewAccessor())
                {
                    var valueToWrite = "Hello world!";
                    mmfAccessor.WriteArray(0, Encoding.ASCII.GetBytes(valueToWrite), 0, valueToWrite.Length);
                }
            }
        }

        private static void Tree()
        {
            int[] beeps = new int[] { 247, 417, 417, 370, 417, 329, 247, 247, 247, 417, 417, 370, 417, 497, 497, 277, 277, 440, 440, 417, 370, 329, 247, 417, 417, 370, 417, 329 };
            for (int i = 0; i < beeps.Length; ++i)
            {
                Console.Beep(beeps[i], 500);
            }
        }
    }
}