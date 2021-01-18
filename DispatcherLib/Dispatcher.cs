using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DispatcherLib
{
    [ComVisible(true)]
    [Guid("8251bda3-9a44-4628-bfeb-d5d6427b37af")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class Dispatcher
    {
        private const int TH32CS_SNAPPROCESS = 0x2;
        [DllImport("tlhelp32.dll", SetLastError = true)]
        private static extern IntPtr CreateToolhelp32Snapshot(int dwFlags, int dwProcessId);
        [DllImport("tlhelp32.dll", SetLastError = true)]
        private static extern bool Process32First(IntPtr hSnapshot, out PROCESSENTRY32 peProcessEntry);
        [DllImport("tlhelp32.dll", SetLastError = true)]
        private static extern bool Process32Next(IntPtr hSnapshot, out PROCESSENTRY32 peProcessEntry);
        [DllImport("tlhelp32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        private ThreadSafeList<object> procs = new ThreadSafeList<object>();
        public Dispatcher()
        {
            IntPtr hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
            try
            {
                if (hSnapshot != null && Process32First(hSnapshot, out PROCESSENTRY32 procInfo))
                {
                    do
                    {
                        Type procT = Type.GetTypeFromProgID("ProcessLib.ProcAgent");
                        dynamic proc = Activator.CreateInstance(procT);
                        if (!proc.SetPID(procInfo.th32ProcessID))
                            continue;
                        procs.Add(proc);
                        proc.Listen();

                    } while (Process32Next(hSnapshot, out procInfo));
                }
            } finally
            {
                if (hSnapshot != null)
                    CloseHandle(hSnapshot);
            }
        }
        public IList<object> GetList()
        {
            return procs;
        }
    }
}
