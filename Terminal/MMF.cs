using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;

namespace Terminal
{
    internal class MMF
    {
        private MemoryMappedFile mmFile;

        public void CheckMMF()
        {
            try
            {
                using (var mmCheckFile = MemoryMappedFile.OpenExisting("CheckLoad", MemoryMappedFileRights.FullControl, HandleInheritability.Inheritable))
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
                mmFile = MemoryMappedFile.CreateNew("CheckLoad", 12, MemoryMappedFileAccess.ReadWrite, MemoryMappedFileOptions.DelayAllocatePages, HandleInheritability.None);

                using (var mmfAccessor = mmFile.CreateViewAccessor())
                {
                    var valueToWrite = "Hello world!";
                    mmfAccessor.WriteArray(0, Encoding.ASCII.GetBytes(valueToWrite), 0, valueToWrite.Length);
                }
            }
        }

        private void Tree()
        {
            int[] beeps = new int[] { 247, 417, 417, 370, 417, 329, 247, 247, 247, 417, 417, 370, 417, 497, 497, 277, 277, 440, 440, 417, 370, 329, 247, 417, 417, 370, 417, 329 };
            for (int i = 0; i < beeps.Length; ++i)
            {
                Console.Beep(beeps[i], 500);
            }
        }
    }
}